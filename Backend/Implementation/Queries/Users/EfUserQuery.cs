using Application.Exceptions;
using Application.Queries;
using Data.Access;
using Domain;
using Microsoft.EntityFrameworkCore;

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

        public User Execute(int id)
        {

            // Sada da dohvatim user-a

            User user = _context.Users.Include(x => x.Role).FirstOrDefault(x => x.Id == id);

            if (user == null)
            {

                throw new EntityNotFoundException($"User with the id of {id} not found");

            }

            // Sada ako se pronadje zeljeni user

            return user;
        }

    }
}
