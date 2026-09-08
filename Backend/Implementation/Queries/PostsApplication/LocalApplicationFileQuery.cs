using Application;
using Application.DTO.PostApplication;
using Application.Exceptions;
using Application.Queries.Posts;
using Data.Access;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries.PostsApplication;

public class LocalApplicationFileQuery(ApplicationDbContext context, IApplicationUser user, string uploadDirectory) : IApplicationFileQuery
{
    public string Id => "get-application-file";
    public string Name => "download an applicant CV for an owned post";

    public ApplicationFileDTO Execute(PostApplicationFilterDTO dto)
    {
        // Ownership is checked in the database before any filesystem access.
        var storedPath = context.PostApplications.AsNoTracking()
            .Where(application => application.PostId == dto.PostId && application.UserId == dto.UserId && application.Post.UserId == user.Id)
            .Select(application => application.FilePath).SingleOrDefault();
        if (storedPath == null) throw new EntityNotFoundException("This application is unavailable.");

        var name = Path.GetFileName(storedPath);
        if (storedPath != "Applications/" + name)
            throw new EntityNotFoundException("This CV is no longer available.");
        var root = Path.GetFullPath(uploadDirectory);
        var path = Path.GetFullPath(Path.Combine(root, name));
        if (!path.StartsWith(Path.TrimEndingDirectorySeparator(root) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new EntityNotFoundException("This CV is no longer available.");

        var contentType = Path.GetExtension(name).ToLowerInvariant() switch {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => null
        };
        if (contentType == null) throw new EntityNotFoundException("This CV is no longer available.");
        try
        {
            return new ApplicationFileDTO {
                Content = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read),
                ContentType = contentType, FileName = "application-" + dto.UserId + Path.GetExtension(name)
            };
        }
        catch (FileNotFoundException) { throw new EntityNotFoundException("This CV is no longer available."); }
        catch (DirectoryNotFoundException) { throw new EntityNotFoundException("This CV is no longer available."); }
    }
}

