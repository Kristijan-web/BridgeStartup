using Application.Exceptions;
using Application.Queries;
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

        public Post Execute(int id)
        {


            Post? post = _context.Posts.Include(x => x.User).FirstOrDefault(x => x.Id == id);

            if (post == null)
            {
                throw new EntityNotFoundException($"Post with the id {id} does not exist");
            }


            return post;
        }
    }
}
