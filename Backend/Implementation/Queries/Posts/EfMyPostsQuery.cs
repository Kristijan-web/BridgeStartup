using Application;
using Application.DTO.Post;
using Application.Queries.Posts;
using Data.Access;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries.Posts;

public class EfMyPostsQuery(ApplicationDbContext context, IApplicationUser user) : IMyPostsQuery
{
    public string Id => "get-my-posts";
    public string Name => "get the current user's posts";

    public IEnumerable<OwnedPostDTO> Execute(int page) => context.Posts.AsNoTracking()
        .Where(post => post.UserId == user.Id)
        .OrderByDescending(post => post.Id)
        .Skip((Math.Max(1, page) - 1) * 10).Take(10)
        .Select(post => new OwnedPostDTO {
            Id = post.Id, Title = post.Title, ApplicationCount = post.PostApplications.Count()
        }).ToList();
}

