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
                new UseCases { UseCaseId = "account-activate" },
                new UseCases { UseCaseId = "login" },
                new UseCases { UseCaseId = "get-post" },
                new UseCases { UseCaseId = "get-all-posts" },
                new UseCases { UseCaseId = "update-post" },
                new UseCases { UseCaseId = "create-post" },
                new UseCases { UseCaseId = "delete-post" },
                new UseCases { UseCaseId = "get-all-users" },
                new UseCases { UseCaseId = "get-user" },
                new UseCases { UseCaseId = "delete-user" },
                new UseCases { UseCaseId = "update-user" },
                new UseCases { UseCaseId = "apply-to-post-locally" },
                new UseCases { UseCaseId = "get-all-posts-applications" },
                new UseCases { UseCaseId = "get-post-application" },
                new UseCases { UseCaseId = "delete-post-application" },

                // nazivi novih funkcionalnosti
                 new UseCases { UseCaseId = "create-contact" },
                 new UseCases { UseCaseId = "get-contacts" },
                 new UseCases { UseCaseId = "get-contact" },
                 new UseCases { UseCaseId = "delete-contact" },

              

             
          

            // mora ovo da se posalje ka bazi i napravi .cs fajl za RoleUseCases
            };

            _context.AddRange(useCases);
            _context.SaveChanges();

        }
    }

}
