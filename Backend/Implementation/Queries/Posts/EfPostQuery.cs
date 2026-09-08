using Application.DTO.Post;
using Application.Exceptions;
using Application.Queries.Posts;
using Data.Access;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries.Posts
{
    public class EfPostQuery : IPostQuery
    {


        public string Id { get; } = "get-post";
        public string Name { get; } = "get one post";

        ApplicationDbContext _context;

        public EfPostQuery(ApplicationDbContext context)
        {

            _context = context;
        }

        public PostsResponseDTO Execute(int id)
        {

            PostsResponseDTO? post = _context.Posts.Where(x => x.Id == id).Include(x => x.User).Select(x => new PostsResponseDTO
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Email = x.Email,
                Phone = x.Phone,
                Badges = x.BadgePosts.Where(b => b.Badge.DeletedAt == null).Select(b => b.Badge.Name).ToList(),
                User = new UserDTO
                {
                    Username = x.User.Username,
                    Email = x.User.Email
                }

            }).FirstOrDefault();

            if (post == null)
            {
                throw new EntityNotFoundException($"Post with the id {id} does not exist");
            }


            return post;
        }
    }
}
