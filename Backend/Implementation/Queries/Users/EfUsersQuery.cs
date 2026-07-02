using Application.DTO.User;
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

            IQueryable<User> query = _context.Users.Include(x => x.Role).AsQueryable();

            // Mogu da napravim extension metodu nad tipom IQueryable

            // U kom sloju cu praviti extension metodu?
            // - pa treba da se da jasna implementacija i time bi isao u Implementation sloj




            int curPage = dto.Page != null ? dto.Page.Value : 1;
            int pageSize = 5;
            int skipPages = (curPage - 1) * pageSize;


            query = query.Skip(skipPages).Take(pageSize);


            if (!String.IsNullOrEmpty(dto.Email))
            {

                query = query.Where(x => x.Email.Contains(dto.Email));
            }



            IEnumerable<User> users = query.ToList();

            return users;
        }


    }


}

