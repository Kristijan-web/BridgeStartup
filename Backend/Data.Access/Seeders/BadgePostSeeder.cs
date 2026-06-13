using Bogus;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Access.Seeders
{
    public class BadgePostSeeder
    {
        public BadgePostSeeder(ApplicationDbContext _context)
        {
            // Koje tabele mi trebaju za Badge_Post tabelu?
            // - Badge
            // - Post

            List<Badge> badges = _context.Badges.ToList();
            List<Post> posts = _context.Posts.ToList();

            Faker<Badge_Post> badgePostFaker = new Faker<Badge_Post>();

            badgePostFaker.RuleFor(x => x.Badge, f => f.PickRandom(badges));
            badgePostFaker.RuleFor(x => x.Post, f => f.PickRandom(posts));

            List<Badge_Post> badgePosts = badgePostFaker.Generate(4);

            _context.AddRange(badgePosts);
            _context.SaveChanges();

            
        }
    }
}
