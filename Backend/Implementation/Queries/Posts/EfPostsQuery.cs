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


            IQueryable<Post> posts = _context.Posts.Include(x => x.BadgePosts).ThenInclude(x => x.Badge);



            if (!String.IsNullOrEmpty(dto.Title))
            {

                // Da li trebam da dodelim rezultat promenljivoj posts ili ce se ovo chainovati na upit?
                // - Pitanje je da li ce metoda nad kojoj pozivam promenljivu promeniti vrednost, to jest da li je metoda mutable?                 

                posts = posts.Where(x => x.Title.Contains(dto.Title));


            }



            if (dto.Badge.Count > 0)
            {







                posts = posts.Where(x => x.BadgePosts.Any(x => dto.Badge.Contains(x.Badge.Name)));


            }






            // Alloweed sorting fields
            string allowedSortingFields = "title";

            // Ako nije prosleđeno sortiranje radim defaultno sortiranje po title-u u ascending orderu
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

                    if (sortDirection?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        posts = posts.OrderByDescending(x => x.Title);
                    }
                    else
                    {
                        posts = posts.OrderBy(x => x.Title);
                    }
                }
            }



            List<PostsResponseDTO> postsDTO = posts.Select(x => new PostsResponseDTO
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
                Badges = x.BadgePosts.Select(x => x.Badge.Name)

            }).ToList();




            return postsDTO;
        }
    }
}
