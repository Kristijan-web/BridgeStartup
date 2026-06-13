using Bogus;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Access.Seeders
{
    public class PostSeeder
    {
        public PostSeeder(ApplicationDbContext _context)
        {

            // Sta mi treba od drugih tabela da bih napravio Post seeder?
            // - User

            List<User> users = _context.Users.ToList();

            Faker<Post> postFaker = new Faker<Post>();

            postFaker.RuleFor(x => x.Title, f => f.Commerce.ProductName());
            postFaker.RuleFor(x => x.Description, f => f.Commerce.ProductDescription());
            postFaker.RuleFor(x => x.Email, f => f.Internet.Email());
            postFaker.RuleFor(x => x.Phone, f => f.Phone.PhoneNumber());
            postFaker.RuleFor(x => x.User, f => f.PickRandom(users));

            List<Post> posts = postFaker.Generate(10);

            _context.AddRange(posts);
            _context.SaveChanges();

        }
    }
}
