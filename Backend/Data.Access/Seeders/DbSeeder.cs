using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Access.Seeders
{
    public class DbSeeder
    {

        public DbSeeder(ApplicationDbContext _context)
        {

            // prvo ne zavisni seederi

            RoleSeeder roleSeeder = new RoleSeeder(_context);
            BadgeSeeder badgeSeeder = new BadgeSeeder(_context);
            UserSeeder userSeeder = new UserSeeder(_context);
            PostSeeder postSeeder = new PostSeeder(_context);
            BadgePostSeeder badgePostSeeder = new BadgePostSeeder(_context);


        }
    }
}
