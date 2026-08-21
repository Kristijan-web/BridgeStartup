using Application.DTO.Auth;
using Application.DTO.Post;
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


            // - FirstOrDefault ce biti null ako rezultat ne postoji
            Boolean doesUserExist = _context.Users.Any(x => x.Email == dto.Email);

            if (!doesUserExist)
            {

                throw new LoginException();

            }



            // Bug je u bazi, nisu dodati usecase-vi za role-u admin
            UserDbDTO user = _context.Users
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

            Console.WriteLine("EHEEEJ");
            Console.WriteLine(string.Join(", ", user.AllowedUseCases));


            return user;



        }
    }
}
