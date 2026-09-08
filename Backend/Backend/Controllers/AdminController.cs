using Backend.Authorization;
using Data.Access;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Backend.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize]
[ServiceFilter(typeof(AdminAccessFilter))]
public class AdminController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet("roles")]
    public async Task<IActionResult> Roles() =>
        Ok(await context.Roles.AsNoTracking().Where(x => x.DeletedAt == null)
            .OrderBy(x => x.Name).Select(x => new { x.Id, x.Name }).ToListAsync());

    [HttpGet("users")]
    public async Task<IActionResult> Users() =>
        Ok((await context.Users.AsNoTracking().Include(x => x.Role).OrderBy(x => x.Username)
            .ToListAsync()).Select(UserResponse));

    [HttpGet("users/{id:long}")]
    public async Task<IActionResult> GetUser(long id)
    {
        var user = await context.Users.AsNoTracking().Include(x => x.Role).SingleOrDefaultAsync(x => x.Id == id);
        return user == null ? NotFound() : Ok(UserResponse(user));
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(AdminUserInput input)
    {
        var validation = await ValidateUser(input, null);
        if (validation != null) return validation;
        if (string.IsNullOrEmpty(input.Password)) return BadRequest(new { message = "Password is required." });
        var user = new User
        {
            Username = input.Username.Trim(),
            Email = input.Email.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(input.Password),
            RoleId = input.RoleId,
            ActivatedAt = input.IsActive ? DateTime.UtcNow : null,
            RegisteredAt = DateTime.UtcNow
        };
        context.Users.Add(user);
        var failure = await Save();
        if (failure != null) return failure;
        await context.Entry(user).Reference(x => x.Role).LoadAsync();
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, UserResponse(user));
    }

    [HttpPut("users/{id:long}")]
    public async Task<IActionResult> UpdateUser(long id, AdminUserInput input)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Id == id);
        if (user == null) return NotFound();
        if (IsSelf(id) && (input.RoleId != user.RoleId || !input.IsActive))
            return Conflict(new { message = "You cannot change your own role or deactivate your own account." });
        var validation = await ValidateUser(input, id);
        if (validation != null) return validation;
        user.Username = input.Username.Trim();
        user.Email = input.Email.Trim();
        user.RoleId = input.RoleId;
        user.ActivatedAt = input.IsActive ? user.ActivatedAt ?? DateTime.UtcNow : null;
        // An old email activation link must not reactivate an administratively disabled account.
        user.ActivationCode = null;
        user.UpdatedAt = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(input.Password)) user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
        return await Save() ?? (IActionResult)NoContent();
    }

    [HttpDelete("users/{id:long}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        if (IsSelf(id)) return Conflict(new { message = "You cannot delete your own admin account." });
        var user = await context.Users.SingleOrDefaultAsync(x => x.Id == id);
        if (user == null) return NotFound();
        var now = DateTime.UtcNow;
        user.DeletedAt = now;
        user.ActivationCode = null;
        foreach (var post in await context.Posts.Where(x => x.UserId == id).ToListAsync()) post.DeletedAt = now;
        foreach (var application in await context.PostApplications
            .Where(x => x.UserId == id || x.Post.UserId == id).ToListAsync()) application.DeletedAt = now;
        return await Save() ?? (IActionResult)NoContent();
    }

    [HttpGet("posts")]
    public async Task<IActionResult> Posts() =>
        Ok((await PostQuery().OrderBy(x => x.Title).ToListAsync()).Select(PostResponse));

    [HttpGet("posts/{id:long}")]
    public async Task<IActionResult> GetPost(long id)
    {
        var post = await PostQuery().SingleOrDefaultAsync(x => x.Id == id);
        return post == null ? NotFound() : Ok(PostResponse(post));
    }

    [HttpPost("posts")]
    public async Task<IActionResult> CreatePost(AdminPostInput input)
    {
        var validation = await ValidatePost(input);
        if (validation != null) return validation;
        var post = new Post { BadgePosts = new HashSet<Badge_Post>() };
        await ApplyPost(post, input);
        context.Posts.Add(post);
        var failure = await Save();
        if (failure != null) return failure;
        await context.Entry(post).Reference(x => x.User).LoadAsync();
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, PostResponse(post));
    }

    [HttpPut("posts/{id:long}")]
    public async Task<IActionResult> UpdatePost(long id, AdminPostInput input)
    {
        var post = await context.Posts.Include(x => x.BadgePosts).ThenInclude(x => x.Badge)
            .SingleOrDefaultAsync(x => x.Id == id);
        if (post == null) return NotFound();
        var validation = await ValidatePost(input);
        if (validation != null) return validation;
        await ApplyPost(post, input);
        post.UpdatedAt = DateTime.UtcNow;
        return await Save() ?? (IActionResult)NoContent();
    }

    [HttpDelete("posts/{id:long}")]
    public async Task<IActionResult> DeletePost(long id)
    {
        var post = await context.Posts.SingleOrDefaultAsync(x => x.Id == id);
        if (post == null) return NotFound();
        var now = DateTime.UtcNow;
        post.DeletedAt = now;
        foreach (var application in await context.PostApplications.Where(x => x.PostId == id).ToListAsync())
            application.DeletedAt = now;
        return await Save() ?? (IActionResult)NoContent();
    }

    private bool IsSelf(long id) => User.FindFirst("Id")?.Value == id.ToString();

    private async Task<IActionResult?> ValidateUser(AdminUserInput input, long? id)
    {
        if (!await context.Roles.AnyAsync(x => x.Id == input.RoleId && x.DeletedAt == null))
            return BadRequest(new { message = "Select an existing role." });
        // Deleted accounts still reserve their email because the database has a unique index.
        if (await context.Users.IgnoreQueryFilters().AnyAsync(x => x.Id != id && x.Email == input.Email.Trim()))
            return Conflict(new { message = "This email is already registered." });
        if (!string.IsNullOrEmpty(input.Password) && (input.Password.Length < 8 ||
            !input.Password.Any(char.IsUpper) || !input.Password.Any(char.IsDigit) ||
            System.Text.Encoding.UTF8.GetByteCount(input.Password) > 72))
            return BadRequest(new { message = "Password must have 8 or more characters, an uppercase letter and a number, and at most 72 UTF-8 bytes." });
        return null;
    }

    private async Task<IActionResult?> ValidatePost(AdminPostInput input)
    {
        if (!await context.Users.AnyAsync(x => x.Id == input.UserId))
            return BadRequest(new { message = "Select an existing founder." });
        if (input.Badges == null || input.Badges.Count > 30 ||
            input.Badges.Any(x => string.IsNullOrWhiteSpace(x) || x.Trim().Length > 60))
            return BadRequest(new { message = "Use at most 30 badge names, each between 1 and 60 characters." });
        return null;
    }

    private async Task ApplyPost(Post post, AdminPostInput input)
    {
        post.Title = input.Title.Trim(); post.Description = input.Description.Trim();
        post.Email = string.IsNullOrWhiteSpace(input.Email) ? null : input.Email.Trim();
        post.Phone = string.IsNullOrWhiteSpace(input.Phone) ? null : input.Phone.Trim();
        post.UserId = input.UserId;
        var names = input.Badges.Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var existing = await context.Badges.Where(x => x.DeletedAt == null).ToListAsync();
        var wanted = new List<Badge>();
        foreach (var name in names)
        {
            var badge = existing.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            if (badge == null) { badge = new Badge { Name = name }; context.Badges.Add(badge); existing.Add(badge); }
            wanted.Add(badge);
        }
        foreach (var link in post.BadgePosts.ToList())
        {
            if (!wanted.Contains(link.Badge)) { context.BadgePosts.Remove(link); post.BadgePosts.Remove(link); }
        }
        foreach (var badge in wanted)
        {
            if (!post.BadgePosts.Any(x => x.Badge == badge))
                post.BadgePosts.Add(new Badge_Post { Post = post, Badge = badge });
        }
    }

    private IQueryable<Post> PostQuery() => context.Posts.AsNoTracking()
        .Include(x => x.User).Include(x => x.BadgePosts).ThenInclude(x => x.Badge);

    private static object UserResponse(User user) => new
    {
        user.Id,
        user.Username,
        user.Email,
        user.RoleId,
        Role = user.Role.Name,
        IsActive = user.ActivatedAt != null
    };
    private static object PostResponse(Post post) => new
    {
        post.Id,
        post.Title,
        post.Description,
        post.Email,
        post.Phone,
        post.UserId,
        User = new { post.User.Username, post.User.Email },
        Badges = post.BadgePosts.Where(x => x.Badge.DeletedAt == null).Select(x => x.Badge.Name).ToArray()
    };
    private async Task<IActionResult?> Save()
    {
        try { await context.SaveChangesAsync(); return null; }
        catch (DbUpdateException) { return Conflict(new { message = "The record conflicts with existing data. Refresh and try again." }); }
    }
}

public sealed class AdminUserInput
{
    [Required, MinLength(3), MaxLength(100), RegularExpression(@"^[^\d].{2,}$")]
    public string Username { get; set; } = "";
    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = "";
    public string? Password { get; set; }
    [Range(1, long.MaxValue)]
    public long RoleId { get; set; }
    public bool IsActive { get; set; }
}
public sealed class AdminPostInput
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = "";
    [Required, MaxLength(20000)]
    public string Description { get; set; } = "";
    [EmailAddress, MaxLength(254)]
    public string? Email { get; set; }
    [MaxLength(50)]
    public string? Phone { get; set; }
    [Range(1, long.MaxValue)]
    public long UserId { get; set; }
    public List<string> Badges { get; set; } = new();
}
