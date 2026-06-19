using Application.DTO;
using Application.Queries;
using Data.Access;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries.Users
{
    public class EfUsersQuery : IUsersQuery
    {

        // koja je povratna vrednost metode Execute?

        public string Id { get; } = "get-all-users";
        public string Name { get; } = "getting all users";

        ApplicationDbContext _context;

        public EfUsersQuery(ApplicationDbContext context)
        {

            _context = context;
        }

        public IEnumerable<User> Execute(SearchUsersDTO dto)
        {



            IEnumerable<User> users = _context.Users.Include(x => x.Role).ToList();

            return users;
        }


    }


}

