using Domain;

namespace Data.Access.Seeders
{
    public class RoleSeeder
    {
        public RoleSeeder(ApplicationDbContext _context)
        {



            List<Role> roles = new List<Role> {
                new Role { Name = "user" },
                new Role {Name = "admin"}
            };


            _context.AddRange(roles);
            _context.SaveChanges();


        }
    }
}
