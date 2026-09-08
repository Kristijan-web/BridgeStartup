using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Commands;
using Application.DTO.Post;
using Backend.Controllers;
using Data.Access;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        long adminId, founderId, badgeId, userRoleId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await db.Database.EnsureCreatedAsync();
            var adminRole = new Role { Name = "admin" };
            var userRole = new Role { Name = "user" };
            var admin = new User { Username = "Admin", Email = "admin@example.test", Password = BCrypt.Net.BCrypt.HashPassword("AdminPass1"), Role = adminRole, ActivatedAt = DateTime.UtcNow };
            var founder = new User { Username = "Founder", Email = "founder@example.test", Password = BCrypt.Net.BCrypt.HashPassword("FounderPass1"), Role = userRole, ActivatedAt = DateTime.UtcNow };
            var badge = new Badge { Name = "TypeScript" };
            db.Users.AddRange(admin, founder);
            db.Badges.Add(badge);
            foreach (var name in new[] { "get-user", "get-all-users", "create-post", "update-post", "delete-post", "delete-user", "update-user", "get-post-application", "apply-to-post-locally" })
            {
                var useCase = new UseCases { UseCaseId = name };
                db.RoleUseCases.Add(new RoleUseCases { Role = adminRole, UseCases = useCase });
                if (name == "apply-to-post-locally") db.RoleUseCases.Add(new RoleUseCases { Role = userRole, UseCases = useCase });
            }
            for (var i = 1; i <= 7; i++)
                db.Posts.Add(new Post { Title = $"Idea {i}", Description = "A useful startup project", User = founder,
                    Email = founder.Email, Phone = "+381123", BadgePosts = new HashSet<Badge_Post> { new() { Badge = badge } } });
            await db.SaveChangesAsync();
            adminId = admin.Id; founderId = founder.Id; badgeId = badge.Id; userRoleId = userRole.Id;
        }

        var firstPage = await client.GetFromJsonAsync<JsonElement[]>("/api/Posts?SortBy=title&SortOrder=asc&Page=1");
        Check(firstPage!.Length == 5 && firstPage[0].GetProperty("title").GetString() == "Idea 1", "public post API returns five sorted records");
        var secondPage = await client.GetFromJsonAsync<JsonElement[]>("/api/Posts?SortBy=title&SortOrder=asc&Page=2");
        Check(secondPage!.Length == 2 && secondPage[0].GetProperty("title").GetString() == "Idea 6", "Page=2 returns remaining posts");
        var descending = await client.GetFromJsonAsync<JsonElement[]>("/api/Posts?SortBy=title&SortOrder=desc&Page=1");
        Check(descending![0].GetProperty("title").GetString() == "Idea 7", "descending sorting is applied by the backend");
        var filtered = await client.GetFromJsonAsync<JsonElement[]>("/api/Posts?Title=Idea%206&SortBy=title&SortOrder=asc&Page=1");
        Check(filtered!.Length == 1, "filtering and pagination work together");
        var postId = firstPage[0].GetProperty("id").GetInt64();
        var details = await client.GetFromJsonAsync<JsonElement>("/api/Posts/" + postId);
        Check(details.GetProperty("user").GetProperty("email").GetString() == "founder@example.test" &&
            details.GetProperty("badges")[0].GetString() == "TypeScript", "post detail DTO contains nested founder and badge names");
        await Expect(client.GetAsync("/api/admin/users"), HttpStatusCode.Unauthorized, "admin list requires authentication");

        var adminToken = await Login(client, "admin@example.test", "AdminPass1");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var users = await client.GetFromJsonAsync<JsonElement[]>("/api/admin/users");
        Check(users!.All(x => x.TryGetProperty("roleId", out _) && x.TryGetProperty("isActive", out _) && !x.TryGetProperty("password", out _)),
            "admin user DTO has editable fields and excludes password hashes");
        var adminPosts = await client.GetFromJsonAsync<JsonElement[]>("/api/admin/posts");
        Check(adminPosts!.Length == 7 && adminPosts.All(x => x.TryGetProperty("userId", out _)), "admin post metadata supplies all founder IDs");
        await Expect(client.GetAsync("/api/admin/roles"), HttpStatusCode.OK, "existing role catalog is available");

        var createdUser = await client.PostAsJsonAsync("/api/admin/users", new { username = "NewFounder", email = "new@example.test", password = "FounderPass1", roleId = userRoleId, isActive = true });
        await Expect(Task.FromResult(createdUser), HttpStatusCode.Created, "working admin route creates users");
        var userId = (await createdUser.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();
        await Expect(client.PatchAsJsonAsync("/api/Users/" + userId, new { userId = adminId, username = "EditedFounder", email = "new@example.test" }),
            HttpStatusCode.NoContent, "resource PATCH updates users through the existing command");
        var updatedUser = await client.GetFromJsonAsync<JsonElement>("/api/admin/users/" + userId);
        Check(updatedUser.GetProperty("username").GetString() == "EditedFounder", "PATCH persists changes to the route user, ignoring a forged body ID");
        var unchangedAdmin = await client.GetFromJsonAsync<JsonElement>("/api/admin/users/" + adminId);
        Check(unchangedAdmin.GetProperty("username").GetString() == "Admin", "PATCH does not update the forged body user");
        Check(!string.IsNullOrWhiteSpace(await Login(client, "new@example.test", "FounderPass1")), "blank edit password preserves login");
        await Expect(client.PatchAsJsonAsync("/api/Users/" + userId, new { password = "ChangedPass1" }), HttpStatusCode.NoContent, "PATCH accepts password changes");
        Check(!string.IsNullOrWhiteSpace(await Login(client, "new@example.test", "ChangedPass1")), "PATCH password is BCrypt hashed and usable for login");
        await Expect(client.PatchAsJsonAsync("/api/Users/999999", new { username = "Missing" }), HttpStatusCode.NotFound, "PATCH reports missing users");
        await Expect(client.DeleteAsync("/api/Users/" + userId), HttpStatusCode.NoContent, "resource DELETE Users removes a user");
        await Expect(client.GetAsync("/api/admin/users/" + userId), HttpStatusCode.NotFound, "deleted user disappears from admin reads");

        var createPost = await client.PostAsJsonAsync("/api/Posts", new { title = "Resource creation", description = "A valid resource-created startup",
            userId = founderId, email = "founder@example.test", phone = "+381123", badges = new[] { badgeId } });
        await Expect(Task.FromResult(createPost), HttpStatusCode.Created, "new resource creation contract accepts numeric badge IDs");
        var created = await client.GetFromJsonAsync<JsonElement[]>("/api/Posts?Title=Resource%20creation");
        Check(created!.Length == 1 && created[0].GetProperty("badges")[0].GetString() == "TypeScript", "numeric badges map to names in post responses");

        // The full admin editor still needs the routes that support badge names and founder changes.
        var adminCreated = await client.PostAsJsonAsync("/api/admin/posts", new { title = "Admin creation", description = "A full editor-created startup",
            userId = founderId, email = (string?)null, phone = (string?)null, badges = new[] { "New skill" } });
        await Expect(Task.FromResult(adminCreated), HttpStatusCode.Created, "admin creation supports badge names without a badge catalog");
        var newPostId = (await adminCreated.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt64();
        await Expect(client.PutAsJsonAsync("/api/admin/posts/" + newPostId, new { title = "Edited post", description = "Updated startup",
            userId = adminId, email = (string?)null, phone = (string?)null, badges = Array.Empty<string>() }),
            HttpStatusCode.NoContent, "full editor can clear badges and change founder");
        var edited = await client.GetFromJsonAsync<JsonElement>("/api/admin/posts/" + newPostId);
        Check(edited.GetProperty("userId").GetInt64() == adminId && edited.GetProperty("badges").GetArrayLength() == 0,
            "full edit persists founder and empty badges");
        await Expect(client.DeleteAsync("/api/Posts/" + newPostId), HttpStatusCode.NoContent, "resource DELETE Posts removes a post");
        await Expect(client.GetAsync("/api/Posts/" + newPostId), HttpStatusCode.NotFound, "deleted post disappears");

        var userToken = await Login(client, "founder@example.test", "FounderPass1");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        await Expect(client.PatchAsJsonAsync("/api/Users/" + adminId, new { username = "Forbidden" }), HttpStatusCode.Unauthorized, "regular user cannot call admin update use case");
        using var form = ApplicationForm(postId);
        await Expect(client.PostAsync("/api/Posts/apply", form), HttpStatusCode.NoContent, "new /Posts/apply route binds multipart CV requests");
        string filePath;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var application = await db.PostApplications.SingleAsync();
            Check(application.PostId == postId && application.UserId == founderId, "application saves the selected post and JWT user, ignoring submitted UserId");
            filePath = Path.Combine(factory.UploadDirectory, Path.GetFileName(application.FilePath));
            Check(File.Exists(filePath) && await File.ReadAllTextAsync(filePath) == "%PDF-1.4 resume", "successful response waits for the CV file and database record");
            Check(!Path.GetFileName(filePath).Contains("resume"), "stored CV filename is generated by the server");
        }
        using var duplicate = ApplicationForm(postId);
        await Expect(client.PostAsync("/api/Posts/apply", duplicate), HttpStatusCode.Conflict, "duplicate application returns 409");
        var otherPostId = firstPage[1].GetProperty("id").GetInt64();
        using var invalid = ApplicationForm(otherPostId, "resume.pdf", "not a PDF"u8.ToArray());
        await Expect(client.PostAsync("/api/Posts/apply", invalid), HttpStatusCode.UnprocessableEntity, "invalid CV signature is rejected");
        using var wrongExtension = ApplicationForm(otherPostId, "resume.exe");
        await Expect(client.PostAsync("/api/Posts/apply", wrongExtension), HttpStatusCode.UnprocessableEntity, "unsupported CV extension is rejected");
        using var oversized = ApplicationForm(otherPostId, "large.pdf", new byte[5 * 1024 * 1024 + 1]);
        await Expect(client.PostAsync("/api/Posts/apply", oversized), HttpStatusCode.UnprocessableEntity, "CV larger than five MB is rejected");
        using var empty = ApplicationForm(otherPostId, "empty.pdf", Array.Empty<byte>());
        await Expect(client.PostAsync("/api/Posts/apply", empty), HttpStatusCode.UnprocessableEntity, "empty CV is rejected");
        using var missingPost = ApplicationForm(999999);
        await Expect(client.PostAsync("/api/Posts/apply", missingPost), HttpStatusCode.NotFound, "application requires an existing post");
        using var missingFile = new MultipartFormDataContent { { new StringContent(otherPostId.ToString()), "PostId" } };
        await Expect(client.PostAsync("/api/Posts/apply", missingFile), HttpStatusCode.BadRequest, "missing multipart file returns 400");
        Check(Directory.GetFiles(factory.UploadDirectory).Length == 1, "rejected applications leave no extra CV files");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var applicationDto = await client.GetFromJsonAsync<JsonElement>($"/api/PostApplications/{postId}/{founderId}");
        Check(applicationDto.GetProperty("postId").GetInt64() == postId && applicationDto.GetProperty("userId").GetInt64() == founderId,
            "saved application can be retrieved with serialized DTO properties");
        await Expect(client.GetAsync($"/api/Posts/{postId}/applications"), HttpStatusCode.NotFound, "even an admin cannot review another owner's applicants through the owner route");
        await Expect(client.GetAsync($"/api/PostApplications/{postId}/{founderId}/file"), HttpStatusCode.NotFound, "another user cannot download an applicant CV");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        var ownPosts = await client.GetFromJsonAsync<JsonElement[]>("/api/Posts/mine");
        var badgeChoices = await client.GetFromJsonAsync<JsonElement[]>("/api/Badges");
        Check(badgeChoices!.Any(badge => badge.GetProperty("id").GetInt64() == badgeId && badge.GetProperty("name").GetString() == "TypeScript"), "regular user can load numeric badge IDs for the post form");
        Check(ownPosts!.Length == 7 && ownPosts.All(post => post.GetProperty("title").GetString()!.StartsWith("Idea ")), "my posts includes only posts owned by the signed-in user");
        Check(ownPosts.Single(post => post.GetProperty("id").GetInt64() == postId).GetProperty("applicationCount").GetInt32() == 1, "my posts includes applicant counts");
        var applicants = await client.GetFromJsonAsync<JsonElement[]>($"/api/Posts/{postId}/applications");
        Check(applicants!.Length == 1 && applicants[0].GetProperty("username").GetString() == "Founder" && applicants[0].GetProperty("userId").GetInt64() == founderId,
            "owner can review applicant usernames and IDs");
        Check(!applicants[0].TryGetProperty("filePath", out _) && applicants[0].GetProperty("fileName").GetString()!.EndsWith(".pdf"), "applicant list has a download name without exposing storage paths");
        using var download = await client.GetAsync($"/api/PostApplications/{postId}/{founderId}/file");
        Check(download.IsSuccessStatusCode && await download.Content.ReadAsStringAsync() == "%PDF-1.4 resume", "owner downloads the exact saved CV");
        Check(download.Content.Headers.ContentType?.MediaType == "application/pdf" && download.Content.Headers.ContentDisposition?.DispositionType == "attachment", "CV has the correct type and download disposition");
        Check(download.Headers.CacheControl?.NoStore == true, "CV responses are not cached");
        await Expect(client.GetAsync($"/api/PostApplications/{postId}/999999/file"), HttpStatusCode.NotFound, "missing applicant file returns 404");
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var app = await db.PostApplications.SingleAsync();
            app.FilePath = "Applications/../../outside.pdf";
            await db.SaveChangesAsync();
        }
        await Expect(client.GetAsync($"/api/PostApplications/{postId}/{founderId}/file"), HttpStatusCode.NotFound, "stored traversal paths cannot escape the private upload directory");
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var app = await db.PostApplications.SingleAsync();
            app.FilePath = "Applications/" + Path.GetFileName(filePath);
            await db.SaveChangesAsync();
        }
        await Expect(client.PostAsJsonAsync("/api/Posts", new { title = "My published idea", description = "Published by a regular user", userId = adminId,
            email = "founder@example.test", phone = "+381123", badges = new[] { badgeId } }), HttpStatusCode.Created, "regular active user can publish a post");
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Check((await db.Posts.SingleAsync(post => post.Title == "My published idea")).UserId == founderId, "post ownership comes from the authenticated user, ignoring a forged owner ID");
        }
        await Expect(client.PostAsJsonAsync("/api/Posts", new { title = "x", description = "short", email = "invalid", phone = "", badges = new[] { badgeId } }),
            HttpStatusCode.UnprocessableEntity, "create post now enforces the existing validator");
        await Expect(client.PostAsJsonAsync("/api/Posts", new { title = "Unknown skill", description = "A valid description", email = "founder@example.test", phone = "123", badges = new[] { 999999 } }),
            HttpStatusCode.UnprocessableEntity, "nonexistent badge IDs are rejected");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        await Expect(client.GetAsync($"/api/PostApplications/999999/{founderId}"), HttpStatusCode.NotFound, "missing application returns 404");
        client.DefaultRequestHeaders.Authorization = null;
        await Expect(client.GetAsync("/api/Posts/mine"), HttpStatusCode.Unauthorized, "guest cannot list owned posts");
        await Expect(client.GetAsync($"/api/Posts/{postId}/applications"), HttpStatusCode.Unauthorized, "guest cannot list applicants");
        await Expect(client.GetAsync($"/api/PostApplications/{postId}/{founderId}/file"), HttpStatusCode.Unauthorized, "guest cannot download CVs");
        await Expect(client.PostAsJsonAsync("/api/Posts", new { title = "Guest idea", description = "No authenticated owner", email = "guest@example.test", phone = "123", badges = new[] { badgeId } }), HttpStatusCode.Unauthorized, "guest cannot publish a post");
        using var guestForm = ApplicationForm(otherPostId);
        await Expect(client.PostAsync("/api/Posts/apply", guestForm), HttpStatusCode.Unauthorized, "guest application requires login");
        await Expect(client.PatchAsJsonAsync("/api/Users/" + adminId, new { username = "Guest" }), HttpStatusCode.Unauthorized, "guest user update requires login");
        Console.WriteLine($"Passed {checks} backend integration checks against an isolated database and upload directory.");
    }

    private static MultipartFormDataContent ApplicationForm(long postId, string fileName = "my.resume.pdf", byte[]? contents = null) => new() {
        { new StringContent(postId.ToString()), "PostId" },
        { new StringContent("99999"), "UserId" },
        { new ByteArrayContent(contents ?? "%PDF-1.4 resume"u8.ToArray()), "userFile", fileName }
    };

    private static async Task<string> Login(HttpClient client, string email, string password)
    {
        using var response = await client.PostAsJsonAsync("/api/Auth/login", new { email, password });
        if (!response.IsSuccessStatusCode) throw new Exception("Test login failed: " + response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("token").GetString()!;
    }
    private static async Task Expect(Task<HttpResponseMessage> task, HttpStatusCode status, string label)
    {
        var response = await task;
        Check(response.StatusCode == status, $"{label}: expected {status}, got {response.StatusCode}");
    }
    private static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception(label);
        checks++; Console.WriteLine("PASS " + label);
    }
}

public sealed class ApiFactory : WebApplicationFactory<PostsController>
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    public string UploadDirectory { get; } = Path.Combine(AppContext.BaseDirectory, "test-uploads", Guid.NewGuid().ToString("N"));
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        connection.Open();
        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging => logging.ClearProviders());
        builder.UseSetting("ConnectionStringSQL", "unused-in-tests");
        builder.UseSetting("JwtSettings:SecretKey", "isolated-integration-test-signing-key-at-least-32-bytes");
        builder.UseSetting("JwtSettings:Issuer", "integration-tests");
        builder.UseSetting("JwtSettings:DurationSeconds", "300");
        builder.UseSetting("EmailSettings:FromEmail", "test@example.test");
        builder.UseSetting("EmailSettings:AppPassword", "unused");
        builder.UseSetting("Uploads:ApplicationsPath", UploadDirectory);
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
