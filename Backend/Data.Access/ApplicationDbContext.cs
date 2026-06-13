using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Access
{
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


        // ovde navodim tabele

        public DbSet<Badge> Badges { get; set; }
        public DbSet<Badge_Post> BadgePosts { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
