using System.IO.Compression;
using Application.Commands;
using Application.DTO.Post;
using Application.Exceptions;
using Data.Access;
using Domain;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Commands.Posts;

public class EfUploadPostFileToLocal(ApplicationDbContext context, string uploadDirectory) : IUploadPostFileToCommand
{
    public string Id => "apply-to-post-locally";
    public string Name => "apply to a post with a CV";
    public const int MaxFileBytes = 5 * 1024 * 1024;

    public async Task ExecuteAsync(ApplyToPostDTO dto, CancellationToken cancellationToken = default)
    {
        if (!await context.Posts.AnyAsync(x => x.Id == dto.PostId, cancellationToken))
            throw new EntityNotFoundException("This post no longer exists.");
        if (!await context.Users.AnyAsync(x => x.Id == dto.UserId && x.ActivatedAt != null, cancellationToken))
            throw new EntityNotFoundException("This account is no longer active.");
        if (await context.PostApplications.IgnoreQueryFilters().AnyAsync(x => x.UserId == dto.UserId && x.PostId == dto.PostId, cancellationToken))
            throw new ConflictException("You have already applied to this post.");

        var extension = Path.GetExtension(dto.FileName).ToLowerInvariant();
        if (dto.FileLength <= 0 || dto.FileLength > MaxFileBytes)
            throw InvalidFile("Choose a non-empty CV up to 5 MB.");
        if (extension is not (".pdf" or ".doc" or ".docx"))
            throw InvalidFile("Upload a PDF, DOC, or DOCX file.");

        using var buffer = new MemoryStream();
        await dto.FileStream.CopyToAsync(buffer, cancellationToken);
        if (buffer.Length != dto.FileLength || buffer.Length > MaxFileBytes)
            throw InvalidFile("The uploaded file length is invalid.");
        if (!HasValidSignature(buffer, extension))
            throw InvalidFile("The file content does not match its PDF, DOC, or DOCX extension.");

        Directory.CreateDirectory(uploadDirectory);
        var fileName = Guid.NewGuid().ToString("N") + extension;
        var filePath = Path.Combine(Path.GetFullPath(uploadDirectory), fileName);
        var saved = false;
        try
        {
            buffer.Position = 0;
            await using (var destination = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true))
                await buffer.CopyToAsync(destination, cancellationToken);
            context.PostApplications.Add(new PostApplication {
                UserId = dto.UserId, PostId = dto.PostId, FilePath = "Applications/" + fileName, CreatedAt = DateTime.UtcNow
            });
            try { await context.SaveChangesAsync(cancellationToken); }
            catch (DbUpdateException)
            {
                if (await context.PostApplications.IgnoreQueryFilters().AsNoTracking()
                    .AnyAsync(x => x.UserId == dto.UserId && x.PostId == dto.PostId, cancellationToken))
                    throw new ConflictException("You have already applied to this post.");
                throw;
            }
            saved = true;
        }
        finally
        {
            // Only remove the server-generated file for this unsuccessful submission.
            if (!saved && File.Exists(filePath)) File.Delete(filePath);
        }
    }

    private static ValidationException InvalidFile(string message) =>
        new(new[] { new ValidationFailure("userFile", message) });

    private static bool HasValidSignature(MemoryStream buffer, string extension)
    {
        var bytes = buffer.GetBuffer().AsSpan(0, (int)buffer.Length);
        if (extension == ".pdf") return bytes.StartsWith("%PDF-"u8);
        if (extension == ".doc") return bytes.StartsWith(new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 });
        try
        {
            buffer.Position = 0;
            using var archive = new ZipArchive(buffer, ZipArchiveMode.Read, leaveOpen: true);
            return archive.GetEntry("[Content_Types].xml") != null && archive.GetEntry("word/document.xml") != null;
        }
        catch (InvalidDataException) { return false; }
    }
}
