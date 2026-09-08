using Application.Commands.Posts;
using Application.Exceptions;
using Data.Access;

namespace Implementation.Commands.Posts
{
    public class EfDeletePostCommand : IDeletePostCommand
    {
        public string Id => "delete-post";

        public string Name => "delete post";

        private ApplicationDbContext _context;
        public EfDeletePostCommand(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Execute(int id)
        {
            var post = _context.Posts.FirstOrDefault(x => x.Id == id);

            if (post == null)
            {
                throw new EntityNotFoundException("Post not found.");
            }


            _context.Posts.Remove(post);
            _context.SaveChanges();



        }
    }
}
