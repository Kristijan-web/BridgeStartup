using Application;
using Application.DTO.PostApplication;
using Application.Exceptions;
using Application.Queries.Posts;
using Data.Access;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries.PostsApplication;

public class EfPostApplicantsQuery(ApplicationDbContext context, IApplicationUser user) : IPostApplicantsQuery
{
    public string Id => "get-post-applicants";
    public string Name => "review applicants for an owned post";

    public IEnumerable<PostApplicantDTO> Execute(long postId)
    {
        if (!context.Posts.Any(post => post.Id == postId && post.UserId == user.Id))
            throw new EntityNotFoundException("This post is unavailable or does not belong to you.");

        return context.PostApplications.AsNoTracking().Where(application => application.PostId == postId)
            .OrderByDescending(application => application.CreatedAt).ThenBy(application => application.UserId)
            .Select(application => new { application.UserId, application.User.Username, application.CreatedAt, application.FilePath })
            .ToList().Select(application => new PostApplicantDTO {
                UserId = application.UserId, Username = application.Username, CreatedAt = application.CreatedAt,
                FileName = "application-" + application.UserId + Path.GetExtension(application.FilePath)
            }).ToList();
    }
}

