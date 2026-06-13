using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Access.Seeders
{
    public class BadgeSeeder
    {
        public BadgeSeeder(ApplicationDbContext _context)
        {

            // Rucno popuniti

            List<Badge> badges = new List<Badge> {
                    new Badge{Name = "C#"},
                    new Badge{Name = "C"},
                    new Badge{Name = "JavaScript"},
                    new Badge{Name = "Typescript"}
                };

            _context.AddRange(badges);
            _context.SaveChanges();



        }
    }
}
