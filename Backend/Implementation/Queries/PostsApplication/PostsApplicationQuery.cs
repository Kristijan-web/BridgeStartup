using Application.DTO.PostApplication;
using Application.Queries.PostApplications;
using Data.Access;

namespace Implementation.Queries.PostsApplication
{
    public class PostsApplicationQuery : IPostsApplicationQuery
    {

        public ApplicationDbContext _context;
        public PostsApplicationQuery(ApplicationDbContext context)
        {
            _context = context;

        }
        public string Id => "get-all-posts-applications";

        public string Name => "getting all posts applications";

        public IEnumerable<PostsApplicationDbDTO> Execute(PostsApplicationFilterDTO dto)
        {

            IEnumerable<PostsApplicationDbDTO> postsApplications = _context.PostApplications.Select(x => new PostsApplicationDbDTO
            {
                FilePath = x.FilePath,
                UserId = x.UserId,
                PostId = x.PostId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,

            }).ToList();


            return postsApplications;


        }
    }
}
