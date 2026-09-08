using Application.DTO.User;
using Application.Exceptions;
using Application.Queries;
using Data.Access;

namespace Implementation.Queries.Users
{
    public class EfUserQuery : IUserQuery
    {
        public string Id { get; } = "get-user";
        public string Name { get; } = "get user";

        ApplicationDbContext _context;

        public EfUserQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public UserDbDTO Execute(int id)
        {
            UserDbDTO? user = _context.Users
                .Where(x => x.Id == id)
                .Select(x => new UserDbDTO
                {
                    Id = x.Id,
                    Username = x.Username,
                    Email = x.Email,
                    Password = x.Password,
                    Role = x.Role.Name,
                    AllowedUseCases = x.Role.RoleUseCases
                        .Select(ru => ru.UseCases.UseCaseId)
                })
                .FirstOrDefault();

            if (user == null)
            {
                throw new EntityNotFoundException(
                    $"User with the id of {id} not found"
                );
            }

            return user;
        }

    }
}
