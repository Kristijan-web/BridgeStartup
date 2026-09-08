using Application.DTO.Post;
using Application.Queries.Posts;
using Data.Access;

namespace Implementation.Queries.Posts
{
    public class EfGetPostApplicationForOwnerQuery : IGetPostApplicationsForOwnerQuery
    {
        public string Id => "get-owner-post-applications";

        public string Name => "get-owner-post-applications";

        private readonly ApplicationDbContext _context;

        public EfGetPostApplicationForOwnerQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public GetPostApplicationsForOwnerDbDTO Execute(
            GetPostApplicationsForOwnerQueryDTO dto)
        {
            var post = _context.Posts
                .Where(x =>
                    x.Id == dto.PostId &&
                    x.UserId == dto.UserId)
                .Select(x => new GetPostApplicationsForOwnerDbDTO
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Email = x.Email,
                    Phone = x.Phone,

                    User = new UserDTO
                    {
                        Username = x.User.Username,
                        Email = x.User.Email
                    },

                    Badges = x.BadgePosts
                        .Select(bp => bp.Badge.Name),

                    Applications = x.PostApplications
                        .Select(pa => new PostApplicationDTO
                        {
                            UserId = pa.UserId,
                            FilePath = pa.FilePath
                        })
                })
                .FirstOrDefault();

            return post;
        }
    }
}