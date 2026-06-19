using Application.DTO.Post;
using Application.Queries;
using Data.Access;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries
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

        public IEnumerable<Post> Execute(PostsDTO dto)
        {

            // Sta mi je potrebno da bih dohvatio sve post-ove?
            // - context

            // Kako ide logika za dohvatanje svih proizvoda
            // - Samo se pozove context objekat i dohvate se svi post-ovi

            // treba da se eager load-uju korisnikovi podaci

            List<Post> posts = _context.Posts.Include(x => x.User).ToList();



            //IEnumerable<Post> posts = new List<Post>
            //    {
            //        new Post()
            //    };

            return posts;
        }
    }
}
