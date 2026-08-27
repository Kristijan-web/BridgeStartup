using Application.DTO.Post;
using Application.Queries.Posts;
using Data.Access;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries.Posts
{
    public class EfPostsQuery : IPostsQuery
    {
        // Koji potpis forsira IPostsQUery?
        // - Execute metodu koja vraca IEnumerable<Post> i prima PostsDTO

        public string Id { get; set; } = "get-all-posts";
        public string Name { get; set; } = "getting all posts";

        ApplicationDbContext _context;

        public EfPostsQuery(ApplicationDbContext context)
        {
            _context = context;

        }

        public IEnumerable<PostsResponseDTO> Execute(PostsFilterDTO dto)
        {

      

            List<PostsResponseDTO> posts = _context.Posts.Include(x => x.User).Select(x => new PostsResponseDTO {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Email = x.Email,
            Phone = x.Phone,
            User = new UserDTO
            {
                Username = x.User.Username,
                Email = x.User.Email
            }

            }).ToList();




            return posts;
        }
    }
}
