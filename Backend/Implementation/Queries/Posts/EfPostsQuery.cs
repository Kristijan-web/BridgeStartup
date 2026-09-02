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

            // ovde mora da se uradi filtracija
            // Po kojim parametrima radim filtraciju post-ova?
            // - Po nazivu
            // - Po tehnologijama

            // Mora da napravim queryable objekat


            // Sta je problem?
            // - Problem je sto hocu da upisem tip podatka Post u PostsResponseDTO

            // Hocu da napravim queryable objekat na koji mogu da chainujem upite

            IQueryable<Post> posts = _context.Posts.Include(x => x.BadgePosts).ThenInclude(x => x.Badge);

            // ==========================================
            // Mogu li direktno da iz Posts->BadgePosts udjem u Badges? Za svaki post dohvatice njihove post badgeve, a post badgeva moze imati 5 

            // Koju relaciju imaju Posts i PostBadges?
            // 1 : M  1 Post -> vise PostBadgeva

            // Koju relaciju imaju PostBadges i Badges?
            // 1 : M 1 Badge -> vise PostBadge-eva

            // Da li pokusavam da pristupim tabeli Badges sa strane 1 ili M?
            // - Ako je M, onda je verovatno M niz redova i logicno ne mogu preko niza da dodjem do jednog reda u tabeli Badges, morao bi da se spustim na nivo jednog zapisa i za njega dohvati njegov red u tabeli Badges (preko select ili any, ako je u where upitu onda any obavezno (razlog zasto vidi dole u komentarima))

            // =========END================


            if (!String.IsNullOrEmpty(dto.Title))
            {

                // Da li trebam da dodelim rezultat promenljivoj posts ili ce se ovo chainovati na upit?
                // - Pitanje je da li ce metoda nad kojoj pozivam promenljivu promeniti vrednost, to jest da li je metoda mutable?                 

                posts = posts.Where(x => x.Title.Contains(dto.Title));


            }



            if (dto.Badge.Count > 0)
            {

                // Imam jedan post mora da dohvatim njegove badge-ve
                // U kojoj tabeli se nalaze badge-evi za odredjeni post?
                // - PostBadges
                // Preko kojih tabela dolazim do tabele PostBadges?
                // - Posts -> BadgePosts -> Badges
                // Mora eager loading

                // Fora je sto se isto moze proslediti niz dto.Badge, da li onda trebam da koristim neku for petlju?


                // Sta je problem?
                // Ako je IEnumerable<bool> onda je rezultat niz boolean-a, a ocekuje se jedan bool
                // Ko ocekuje jedan bool?
                // - Where
                // Da li mogu da koristim where in 

                // Koji tip podatka vraca Select? 
                // - Select je LINQ upit koji se primenjuje nad nizom i sluzi da pristupi jednom elementu niza
                // Da li onda Select upit vraca niz?


                // Problem nastaje jer u bool pokusava da se upise IEnumerable<bool>

                // Ko vraca IEnumerable<bool>
                // - Select metoda vraca IEnumerable<bool>

                // Ko ocekuje bool?
                // Where

                // Kako da izvucem bool-ove iz IENumerable?
                // - Mogu neki for da koristim

                // RESENJE:

                // Zasto je Any resio ovaj problem?
                // - Select je vratio niz bool-ova

                // Koja je razlika izmedju Select i Any u ovom kontextu?
                // - Select ce vratiti IEnumerable<bool> a Where ocekuje jedan bool, zato any je ovde bolji jer ce vratiti bool za svaku proverenu vrednost pojedinacno






                posts = posts.Where(x => x.BadgePosts.Any(x => dto.Badge.Contains(x.Badge.Name)));


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
