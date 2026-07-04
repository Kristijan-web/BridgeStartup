using Domain;

namespace Data.Access.Seeders
{
    public class PostApplicationSeeder
    {
        public PostApplicationSeeder(ApplicationDbContext _context)
        {


            User user = _context.Users.First();
            Post post = _context.Posts.First();


            PostApplication postApplication = new PostApplication { FilePath = "/CVs/kristijanstojanovicCV.pdf", PostId = post.Id, UserId = user.Id };

            _context.Add(postApplication);
            _context.SaveChanges();
        }
    }
}
