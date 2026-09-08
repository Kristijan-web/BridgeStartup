using Application.DTO.Post;
using Application.Queries.Posts;
using Data.Access;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries.Posts
{
    public class EfPostsQuery : IPostsQuery
    {
        public string Id { get; set; } = "get-all-posts";
        public string Name { get; set; } = "getting all posts";

        private ApplicationDbContext _context;

        public EfPostsQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<PostsResponseDTO> Execute(PostsFilterDTO dto)
        {
            IQueryable<Post> posts = _context.Posts
                .Include(x => x.BadgePosts)
                .ThenInclude(x => x.Badge);


            // FILTER PO TITLE-U
            if (!String.IsNullOrEmpty(dto.Title))
            {
                posts = posts.Where(x => x.Title.Contains(dto.Title));
            }


            // FILTER PO BADGE-U
            if (dto.Badge.Count > 0)
            {
                posts = posts.Where(x =>
                    x.BadgePosts.Any(bp =>
                        dto.Badge.Contains(bp.Badge.Name)));
            }


            // SORTIRANJE
            if (!dto.SortBy.Any())
            {
                posts = posts.OrderBy(x => x.Title);
            }
            else
            {
                bool sortByTitle = dto.SortBy.Any(x =>
                    x.Equals("title", StringComparison.OrdinalIgnoreCase));

                if (sortByTitle)
                {
                    string? sortDirection = dto.SortOrder.FirstOrDefault();

                    if (sortDirection?.Equals(
                        "desc",
                        StringComparison.OrdinalIgnoreCase) == true)
                    {
                        posts = posts.OrderByDescending(x => x.Title);
                    }
                    else
                    {
                        posts = posts.OrderBy(x => x.Title);
                    }
                }
            }


            // PAGINACIJA
            int currentPage = dto.Page ?? 1;
            int pageSize = 5;

            int skipPosts = (currentPage - 1) * pageSize;

            posts = posts
                .Skip(skipPosts)
                .Take(pageSize);


            // PROJEKCIJA
            List<PostsResponseDTO> postsDTO = posts
                .Select(x => new PostsResponseDTO
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
                        .Where(bp => bp.Badge.DeletedAt == null)
                        .Select(bp => bp.Badge.Name)

                })
                .ToList();


            return postsDTO;
        }
    }
}