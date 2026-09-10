using Domain;

namespace Data.Access.Seeders
{
    public class RoleUseCaseSeeder
    {
        public RoleUseCaseSeeder(ApplicationDbContext _context)
        {
            // Funkcionalnosti definisane ispod moraju da se zovu tacnu kao i u bazi
            List<string> userUseCases = new List<string> { "register-user", "login", "get-post", "get-all-posts", "apply-to-post-locally", "get-post-application", "update-post", "account-activate", "create-post", "create-contact" };
            // mora da se napravi za admin role-u
            List<string> adminUseCases = new List<string> { "register-user", "login", "get-post", "get-all-posts", "get-user", "get-all-users", "apply-to-post-locally", "get-all-posts-applications", "get-post-application", "update-post", "account-activate", "create-post", "delete-post", "delete-user", "update-user", "delete-post-application", "create-contact", "get-contacts", "get-contact", "delete-contact" };


            // Zasto samo ne bih izvukao sve role iz baze
            List<Role> roles = _context.Roles.ToList();

            Role userRole = roles.First(x => x.Name == "user");
            Role adminRole = roles.First(x => x.Name == "admin");


            // Sada moram da izvucem sve funkcionalnosti
            List<UseCases> useCases = _context.UseCases.ToList();

            // Sta pokusavam da uradim ispod?
            // - Da povezem role-u sa njenim funkcionalnostima
            // Kako to radim?
            // - Uzimam sve funkcionalnosti i uzimam samo za user-a
            List<RoleUseCases> userRoleUseCases = useCases
               .Where(x => userUseCases.Contains(x.UseCaseId)) // filtiram useCase-ove, 
               .Select(x => new RoleUseCases
               {
                   RoleId = userRole.Id,
                   UseCasesId = x.Id
               })
               .ToList();



            List<RoleUseCases> adminRoleUseCases = useCases
              .Where(x => adminUseCases.Contains(x.UseCaseId)) // filtiram useCase-ove, 
              .Select(x => new RoleUseCases
              {
                  RoleId = adminRole.Id,
                  UseCasesId = x.Id
              })
              .ToList();




            _context.AddRange(userRoleUseCases);
            _context.AddRange(adminRoleUseCases);

            _context.SaveChanges();

        }
    }
}
