using Application.DTO;
using Application.Queries;
using Domain;

namespace Implementation.Queries
{
    public class EfPostsQuery : IPostsQuery
    {
        // Koji potpis forsira IPostsQUery?
        // - Execute metodu koja vraca IEnumerable<Post> i prima PostsDTO

        public string Id { get; set; } = "get-all-posts";
        public string Name { get; set; } = "getting all posts";

        public IEnumerable<Post> Execute(PostsDTO dto)
        {



            IEnumerable<Post> posts = new List<Post>
                {
                    new Post()
                };

            return posts;
        }
    }
}
