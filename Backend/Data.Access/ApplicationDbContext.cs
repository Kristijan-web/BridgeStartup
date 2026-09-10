using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data.Access
{
    // Cemu sluzi ovaj Data access sloj?
    // - On zna kako se podaci čuvaju i čitaju iz baze preko konkretnog ORM-a.
    public class ApplicationDbContext : DbContext
    {

        // mogu da se povezem preko konsturktora a mogu i preko OnCofiguring ili OnModelCreating
        // ja bi preko konstruktora
        // sada treba da se uradi DI injection iz AddDbContext klase

        // Di container ce da vidi da treba da inject-uje DbContextOptions i napravice njegovu instancu i proslediti ApplicationDbContext

        // Genericke klase mi uopste nisu jasne

        // Kada se pravi interfejs, da li je to klasa?
        // - Rekao bih da jeste ali klasa koja sadrzi I
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        // koja metoda se koristi za konfiguraciju?

        // Kada ce metoda ispod da se trigeruje?
        // - Kada se uradi Add-Migration
        protected override void OnModelCreating(ModelBuilder builder)
        {


            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            builder.Entity<User>().HasQueryFilter(x => x.DeletedAt == null);
            builder.Entity<Post>().HasQueryFilter(x => x.DeletedAt == null && x.User.DeletedAt == null);
            builder.Entity<PostApplication>().HasQueryFilter(x => x.DeletedAt == null &&
                x.User.DeletedAt == null && x.Post.DeletedAt == null && x.Post.User.DeletedAt == null);



            base.OnModelCreating(builder);

        }

        // ovde navodim tabele

        public DbSet<Badge> Badges { get; set; }
        public DbSet<Badge_Post> BadgePosts { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RoleUseCases> RoleUseCases { get; set; }
        public DbSet<UseCases> UseCases { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<PostApplication> PostApplications { get; set; }

    }
}
