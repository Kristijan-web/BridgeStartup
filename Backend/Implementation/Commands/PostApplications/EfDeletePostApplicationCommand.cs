using Application.Commands.PostApplications;
using Application.DTO.PostApplication;
using Application.Exceptions;
using Data.Access;

namespace Implementation.Commands.PostApplications
{
    public class EfDeletePostApplicationCommand : IDeletePostApplicationCommand
    {
        private readonly ApplicationDbContext _context;

        public EfDeletePostApplicationCommand(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Id => "delete-post-application";

        public string Name => "delete post application";

        public void Execute(DeletePostApplicationDTO dto)
        {
            var postApplication = _context.PostApplications
                .FirstOrDefault(x =>
                    x.UserId == dto.UserId &&
                    x.PostId == dto.PostId &&
                    x.DeletedAt == null);

            if (postApplication == null)
            {
                throw new EntityNotFoundException(
                    $"Post application for user {dto.UserId} and post {dto.PostId} not found."
                );
            }

            postApplication.DeletedAt = DateTime.UtcNow;

            _context.SaveChanges();
        }
    }
}