using Application.DTO.PostApplication;
using Application.Exceptions;
using Application.Queries.PostApplications;
using Data.Access;

namespace Implementation.Queries.PostsApplication
{
    public class EfPostApplicationQuery : IPostApplicationQuery
    {
        public string Id => "get-post-application";

        public string Name => "get post application";

        public ApplicationDbContext _context;

        public EfPostApplicationQuery(ApplicationDbContext context)
        {

            _context = context;
        }

        public PostsApplicationDbDTO Execute(PostApplicationFilterDTO dto)
        {




            PostsApplicationDbDTO postApplication = _context.PostApplications.Where(x => x.UserId == dto.UserId && x.PostId == dto.PostId).Select(x => new PostsApplicationDbDTO
            {

                FilePath = x.FilePath,
                UserId = x.UserId,
                PostId = x.PostId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt

            }).FirstOrDefault() ?? throw new EntityNotFoundException("This application does not exist.");

            return postApplication;


        }
    }
}
