using Domain;

namespace Data.Access.Seeders
{
    public class UseCaseSeeder
    {



        // Sada pravim seedere

        public UseCaseSeeder(ApplicationDbContext _context)
        {

            // sada podaci za useCase-ove

            // Sta sad pravim 
            // Treba da imam niz objekata UseCases
            List<UseCases> useCases = new List<UseCases> {
                new UseCases { UseCaseId = "register-user" },
                new UseCases { UseCaseId = "login" },
                new UseCases { UseCaseId = "get-post" },
                new UseCases { UseCaseId = "get-all-posts" },
                new UseCases { UseCaseId = "get-user" },
                new UseCases { UseCaseId = "get-all-users" }

            // mora ovo da se posalje ka bazi i napravi .cs fajl za RoleUseCases
            };

            _context.AddRange(useCases);
            _context.SaveChanges();

        }
    }

}
