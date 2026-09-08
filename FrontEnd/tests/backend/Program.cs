using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Backend.Controllers;
using Data.Access;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

public static class IntegrationRunner
{
    private static int checks;
    public static async Task Main()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
        await using var factory = new ApiFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions {
            BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false
        });
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();
            var adminRole = new Role { Name = "admin" };
            var userRole = new Role { Name = "user" };
            db.Roles.AddRange(adminRole, userRole);
            var apply = new UseCases { UseCaseId = "apply-to-post-locally" };
            db.RoleUseCases.AddRange(new RoleUseCases { Role = adminRole, UseCases = apply },
                new RoleUseCases { Role = userRole, UseCases = apply });
            db.Users.AddRange(
                new User { Username = "Admin", Email = "admin@example.test", Password = BCrypt.Net.BCrypt.HashPassword("AdminPass1"), Role = adminRole, ActivatedAt = DateTime.UtcNow },
                new User { Username = "Founder", Email = "founder@example.test", Password = BCrypt.Net.BCrypt.HashPassword("FounderPass1"), Role = userRole, ActivatedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();
        }
        await Expect(client.GetAsync("/api/Users"), HttpStatusCode.Unauthorized, "guests cannot read admin users");
        await Expect(client.DeleteAsync("/api/Seeder"), HttpStatusCode.Unauthorized, "guests cannot bypass admin protection through the database seeder");
        await Expect(client.PostAsJsonAsync("/api/Posts", new { }), HttpStatusCode.Unauthorized, "guests cannot mutate posts");
        await Expect(client.GetAsync("/api/Posts"), HttpStatusCode.OK, "public posts are available without signing in");
        await Expect(client.PostAsJsonAsync("/api/Auth/login", new { email = "admin@example.test", password = "wrong" }), HttpStatusCode.Unauthorized, "incorrect passwords are rejected");

        var regular = await Login(client, "founder@example.test", "FounderPass1");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", regular);
        await Expect(client.GetAsync("/api/Users"), HttpStatusCode.Forbidden, "regular users cannot read admin data");
        await Expect(client.DeleteAsync("/api/Seeder"), HttpStatusCode.Forbidden, "regular users cannot clear the database through the seeder");
        await Expect(client.DeleteAsync("/api/Users/1"), HttpStatusCode.Forbidden, "regular users cannot delete users");
        var token = await Login(client, "admin@example.test", "AdminPass1");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var roles = await client.GetFromJsonAsync<JsonElement[]>("/api/admin/roles");
        var userRoleId = roles!.Single(x => x.GetProperty("name").GetString() == "user").GetProperty("id").GetInt64();
        var adminRoleId = roles!.Single(x => x.GetProperty("name").GetString() == "admin").GetProperty("id").GetInt64();
        var adminUsers = await client.GetFromJsonAsync<JsonElement[]>("/api/Users");
        Check(adminUsers!.All(x => x.TryGetProperty("id", out _) && !x.TryGetProperty("password", out _)), "user DTOs have IDs and never expose password hashes");
        var adminId = adminUsers!.Single(x => x.GetProperty("email").GetString() == "admin@example.test").GetProperty("id").GetInt64();
        await Expect(client.DeleteAsync("/api/Users/" + adminId), HttpStatusCode.Conflict, "admins cannot delete themselves");
        await Expect(client.PutAsJsonAsync("/api/Users/" + adminId, new { username = "Admin", email = "admin@example.test", roleId = userRoleId, isActive = true }),
            HttpStatusCode.Conflict, "admins cannot demote themselves");

        await Expect(client.PostAsJsonAsync("/api/Users", new { username = "Bad", email = "invalid", password = "short", roleId = userRoleId, isActive = true }),
            HttpStatusCode.BadRequest, "invalid user input is rejected");
        var createUser = await client.PostAsJsonAsync("/api/Users", new { username = "NewFounder", email = "new@example.test", password = "FounderPass1", roleId = userRoleId, isActive = true });
        await Expect(Task.FromResult(createUser), HttpStatusCode.Created, "admins can create users");
        var newUser = await createUser.Content.ReadFromJsonAsync<JsonElement>();
        var id = newUser.GetProperty("id").GetInt64();
        await Expect(client.PostAsJsonAsync("/api/Users", new { username = "Duplicate", email = "new@example.test", password = "FounderPass1", roleId = userRoleId, isActive = true }),
            HttpStatusCode.Conflict, "duplicate email is rejected");
        await Expect(client.PutAsJsonAsync("/api/Users/" + id, new { username = "EditedFounder", email = "new@example.test", roleId = userRoleId, isActive = true }),
            HttpStatusCode.NoContent, "admins can edit a user without resetting their password");
        var newToken = await Login(client, "new@example.test", "FounderPass1");
        Check(!string.IsNullOrEmpty(newToken), "created account can log in after edit");

        var createdPost = await client.PostAsJsonAsync("/api/Posts", new { title = "Test idea", description = "A useful startup", userId = id, email = "new@example.test", phone = "+381 123", badges = new[] { "C#", "TypeScript", "c#" } });
        await Expect(Task.FromResult(createdPost), HttpStatusCode.Created, "admins can create posts");
        var postId = (await createdPost.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();
        var post = await client.GetFromJsonAsync<JsonElement>("/api/Posts/" + postId);
        Check(post.GetProperty("badges").GetArrayLength() == 2 && post.GetProperty("user").GetProperty("username").GetString() == "EditedFounder",
            "public post details include founder and deduplicated badges");
        await Expect(client.PatchAsJsonAsync("/api/Posts/" + postId, new { title = "Edited idea", description = "Updated", userId = id, email = (string?)null, phone = (string?)null, badges = Array.Empty<string>() }),
            HttpStatusCode.NoContent, "admins can edit posts");
        post = await client.GetFromJsonAsync<JsonElement>("/api/Posts/" + postId);
        Check(post.GetProperty("badges").GetArrayLength() == 0 && post.GetProperty("email").ValueKind == JsonValueKind.Null,
            "editing can remove all badges and clear contact fields");
        await Expect(client.GetAsync("/api/Posts?Title=Edited"), HttpStatusCode.OK, "post title search is accepted");
        await Expect(client.PatchAsJsonAsync("/api/Posts/" + postId, new { title = " ", description = "Updated", userId = id, badges = Array.Empty<string>() }),
            HttpStatusCode.UnprocessableEntity, "blank post titles are rejected");

        // Partial PATCH keeps omitted fields and badges, and never trusts a body PostId.
        await Expect(client.PatchAsJsonAsync("/api/Posts/" + postId, new { title = "Zebra idea", postId = 99999 }),
            HttpStatusCode.NoContent, "existing PATCH updates the route-selected post");
        post = await client.GetFromJsonAsync<JsonElement>("/api/Posts/" + postId);
        Check(post.GetProperty("description").GetString() == "Updated" && post.GetProperty("userId").GetInt64() == id,
            "partial PATCH preserves description and founder");
        var sortPost = await client.PostAsJsonAsync("/api/Posts", new { title = "Alpha idea", description = "Sort fixture", userId = id, badges = new[] { "C#" } });
        var sortId = (await sortPost.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();
        var sorted = await client.GetFromJsonAsync<JsonElement[]>("/api/Posts?SortBy=title&SortOrder=desc");
        Check(sorted![0].GetProperty("title").GetString() == "Zebra idea", "backend sorts titles descending");
        sorted = await client.GetFromJsonAsync<JsonElement[]>("/api/Posts?SortBy=title&SortOrder=asc");
        Check(sorted![0].GetProperty("title").GetString() == "Alpha idea", "backend sorts titles ascending");
        sorted = await client.GetFromJsonAsync<JsonElement[]>("/api/Posts?Title=Alpha&SortBy=title&SortOrder=desc");
        Check(sorted!.Length == 1, "backend combines title filtering and sorting");
        await Expect(client.PatchAsJsonAsync("/api/Posts/" + sortId, new { title = "Alpha updated" }), HttpStatusCode.NoContent, "PATCH accepts an omitted badge list");
        var partial = await client.GetFromJsonAsync<JsonElement>("/api/Posts/" + sortId);
        Check(partial.GetProperty("badges").GetArrayLength() == 1, "omitted badges are preserved");

        client.DefaultRequestHeaders.Authorization = null;
        await Expect(Apply(client, postId), HttpStatusCode.Unauthorized, "anonymous application is rejected");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", regular);
        await Expect(client.PostAsync("/api/Posts", new MultipartFormDataContent {
            { new StringContent(postId.ToString()), "PostId" }
        }), HttpStatusCode.BadRequest, "application requires a CV");
        await Expect(Apply(client, 99999), HttpStatusCode.NotFound, "applications require an existing post");
        await Expect(Apply(client, postId, "resume.exe"), HttpStatusCode.UnprocessableEntity, "unsupported CV extensions are rejected");
        await Expect(Apply(client, postId, "resume.pdf", "not a PDF"), HttpStatusCode.UnprocessableEntity, "misleading file extensions are rejected");
        await Expect(Apply(client, postId, "resume.pdf", ""), HttpStatusCode.UnprocessableEntity, "empty files are rejected");
        await Expect(Apply(client, postId, "resume.pdf", "%PDF-" + new string('x', 5 * 1024 * 1024)),
            HttpStatusCode.UnprocessableEntity, "oversized CVs are rejected");
        await Expect(Apply(client, postId), HttpStatusCode.NoContent, "authenticated users can apply with multipart POST Posts");
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var application = await db.PostApplications.SingleAsync(x => x.PostId == postId);
            var applicant = await db.Users.SingleAsync(x => x.Email == "founder@example.test");
            Check(application.UserId == applicant.Id, "application uses JWT identity instead of a submitted UserId");
            Check(File.Exists(Path.Combine(factory.UploadDirectory, Path.GetFileName(application.FilePath))),
                "application response waits for the CV to finish writing");
        }
        await Expect(Apply(client, postId), HttpStatusCode.Conflict, "duplicate applications are rejected");
        Check(Directory.GetFiles(factory.UploadDirectory).Length == 1, "duplicate application does not leave another file");
        await Expect(Apply(client, sortId, route: "/api/Posts/apply"), HttpStatusCode.NoContent, "explicit apply alias also accepts multipart applications");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await Expect(client.GetAsync("/api/admin/posts"), HttpStatusCode.NotFound, "old admin post CRUD routes are removed");
        await Expect(client.GetAsync("/api/admin/users"), HttpStatusCode.NotFound, "old admin user CRUD routes are removed");

        // The active token must stop working immediately after an admin disables the user.
        await Expect(client.PutAsJsonAsync("/api/Users/" + id, new { username = "EditedFounder", email = "new@example.test", roleId = userRoleId, isActive = false }),
            HttpStatusCode.NoContent, "admins can disable accounts");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
        await Expect(client.GetAsync("/api/Users"), HttpStatusCode.Unauthorized, "disabled accounts cannot use an old JWT");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await Expect(client.PutAsJsonAsync("/api/Users/" + id, new { username = "EditedFounder", email = "new@example.test", roleId = adminRoleId, isActive = true }),
            HttpStatusCode.NoContent, "admins can update roles");
        var promotedToken = await Login(client, "new@example.test", "FounderPass1");
        await Expect(client.PutAsJsonAsync("/api/Users/" + id, new { username = "EditedFounder", email = "new@example.test", roleId = userRoleId, isActive = true }),
            HttpStatusCode.NoContent, "admins can demote another admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", promotedToken);
        await Expect(client.DeleteAsync("/api/Posts/" + postId), HttpStatusCode.Forbidden, "demotion takes effect even for an older admin JWT");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var secondPost = await client.PostAsJsonAsync("/api/Posts", new { title = "Delete me", description = "A second idea", userId = id, badges = Array.Empty<string>() });
        var secondId = (await secondPost.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();
        await Expect(client.DeleteAsync("/api/Posts/" + secondId), HttpStatusCode.NoContent, "admins can delete posts");
        await Expect(client.GetAsync("/api/Posts/" + secondId), HttpStatusCode.NotFound, "deleted posts disappear from public details");
        await Expect(client.DeleteAsync("/api/Users/" + id), HttpStatusCode.NoContent, "admins can delete users");
        await Expect(client.GetAsync("/api/Users/" + id), HttpStatusCode.NotFound, "deleted users disappear from admin reads");
        await Expect(client.GetAsync("/api/Posts/" + postId), HttpStatusCode.NotFound, "user deletion hides their posts");
        await Expect(client.PostAsJsonAsync("/api/Users", new { username = "Duplicate", email = "new@example.test", password = "FounderPass1", roleId = userRoleId, isActive = true }),
            HttpStatusCode.Conflict, "deleted email remains reserved by the unique database index");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
        await Expect(client.GetAsync("/api/Users"), HttpStatusCode.Unauthorized, "deleted users cannot use existing tokens");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token[..^5] + "AAAAA");
        await Expect(client.GetAsync("/api/Users"), HttpStatusCode.Unauthorized, "forged JWT signatures are rejected");
        Console.WriteLine($"Passed {checks} backend integration checks.");
    }

    private static Task<HttpResponseMessage> Apply(HttpClient client, long postId, string filename = "resume.pdf",
        string content = "%PDF-1.4 test CV", string route = "/api/Posts")
    {
        var form = new MultipartFormDataContent {
            { new StringContent(postId.ToString()), "PostId" },
            { new StringContent("99999"), "UserId" },
            { new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(content)), "userFile", filename }
        };
        return client.PostAsync(route, form);
    }

    private static async Task<string> Login(HttpClient client, string email, string password)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/login") { Content = JsonContent.Create(new { email, password }) };
        request.Headers.Authorization = null;
        // Login is anonymous even if the shared client currently carries an admin token.
        using var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new Exception($"Login failed: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("token").GetString()!;
    }
    private static async Task Expect(Task<HttpResponseMessage> task, HttpStatusCode status, string label)
    {
        var response = await task;
        Check(response.StatusCode == status, $"{label}: expected {status}, got {response.StatusCode}. {(!response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "")}");
    }
    private static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception(label);
        checks++; Console.WriteLine("PASS " + label.Split(':')[0]);
    }
}

public sealed class ApiFactory : WebApplicationFactory<AdminController>
{
    public string UploadDirectory { get; } = Path.Combine(AppContext.BaseDirectory, "test-uploads", Guid.NewGuid().ToString("N"));
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        connection.Open();
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.UseSetting("Uploads:ApplicationsPath", UploadDirectory);
        builder.UseSetting("ConnectionStringSQL", "unused-in-tests");
        builder.UseSetting("JwtSettings:SecretKey", "isolated-integration-test-signing-key-at-least-32-bytes");
        builder.UseSetting("JwtSettings:Issuer", "integration-tests");
        builder.UseSetting("JwtSettings:DurationSeconds", "300");
        builder.UseSetting("EmailSettings:FromEmail", "test@example.test");
        builder.UseSetting("EmailSettings:AppPassword", "unused");
        builder.ConfigureTestServices(services => {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection).UseLazyLoadingProxies());
        });
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) connection.Dispose();
    }
}
