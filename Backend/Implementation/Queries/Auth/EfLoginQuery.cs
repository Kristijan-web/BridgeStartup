using Application.DTO.Auth;
using Application.DTO.User;
using Application.Exceptions;
using Application.Queries;
using Data.Access;

namespace Implementation.Queries.Auth
{
    public class EfLoginQuery : ILoginQuery
    {

        public string Id { get; } = "login";

        public string Name { get; set; } = "login";

        ApplicationDbContext _context;


        public EfLoginQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserDbDTO Execute(LoginDTO dto)
        {

            // NE SME DA RADI LOGIN AKO KORISNIK NIJE verifikaovao nalog
            // Ako je ActivatedAt null ne sme da se izvrsi ova operacija



            // - FirstOrDefault ce biti null ako rezultat ne postoji
            // samo ispod proveri dal je activatedAt rezlitit od null 
            var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

            if (user == null)
            {

                throw new LoginException();

            }

            if (user.ActivatedAt == null)
            {
                // Korisnik postoji i nalog je već aktiviran
                throw new AccountNotVerified("Please activate your account.");
            }





            // Bug je u bazi, nisu dodati usecase-vi za role-u admin
            UserDbDTO userDTO = _context.Users
                .Where(x => x.Email == dto.Email)
                .Select(x => new UserDbDTO
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    Password = x.Password,
                    Role = x.Role.Name,


                    // Meni trebaju nazivi use-case-a za specificnu role-u
                    // Zasto ne mogu da napisem x.Role.RoleUseCases.UseCases.UseCaseId?
                    // - User ima jednu role-u, recimo admin
                    // - Ta role-a ima svoje funkcionalnosti
                    // admin -> add-to-cart, 01-01-2026
                    // admin -> add-user, 01-01-2026
                    // admin -> delete-user 01-01-2026

                    // Ako bih hteo x.Role.RoleUISeCases.UseCaseId ovo ne bi moglo, zasto?
                    // - Jer ima vise useCaseId-eva i ne zna koji da uzme, resenje je da kazem da ce to biti list-a
                    // - Select ce proci kroz svaki element 

                    AllowedUseCases = x.Role.RoleUseCases.Select(y => y.UseCases.UseCaseId).ToList()
                }).First();

            Console.WriteLine(string.Join(", ", userDTO.AllowedUseCases));


            return userDTO;



        }
    }
}
