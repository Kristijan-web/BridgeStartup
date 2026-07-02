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


            // - FirstOrDefault ce biti null ako rezultat ne postoji
            Boolean doesUserExist = _context.Users.Any(x => x.Email == dto.Email);

            if (!doesUserExist)
            {

                throw new LoginException();

            }



            UserDbDTO user = _context.Users
                .Where(x => x.Email == dto.Email)
                .Select(x => new UserDbDTO
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    Password = x.Password,
                    Role = x.Role.Name,

                    AllowedUseCases = x.Role.RoleUseCases
                     .Select(y => y.UseCases.UseCaseId)
                    .ToList()
                }).First();



            return user;



        }
    }
}
