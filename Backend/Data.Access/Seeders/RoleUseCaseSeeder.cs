using Domain;

namespace Data.Access.Seeders
{
    public class RoleUseCaseSeeder
    {
        public RoleUseCaseSeeder(ApplicationDbContext _context)
        {




            List<string> userUseCases = new List<string> { "register-user", "login", "get-post", "get-all-posts", "get-user", "get-all-users" };
            // mora da se napravi za admin role-u


            // Zasto samo ne bih izvukao sve role iz baze
            List<Role> roles = _context.Roles.ToList();
            Role userRole = roles.First(x => x.Name == "user");


            // Sada moram da izvucem sve funkcionalnosti

            List<UseCases> useCases = _context.UseCases.ToList();


            List<RoleUseCases> roleUseCases = useCases
               .Where(x => userUseCases.Contains(x.UseCaseId))
               .Select(x => new RoleUseCases
               {
                   RoleId = userRole.Id,
                   UseCasesId = x.Id
               })
               .ToList();

            _context.AddRange(roleUseCases);
            _context.SaveChanges();

        }
    }
}
