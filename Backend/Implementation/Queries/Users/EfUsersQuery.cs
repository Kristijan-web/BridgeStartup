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
            IQueryable<User> query = _context.Users
                                              .Include(x => x.Role);

            // FILTER
            if (!String.IsNullOrEmpty(dto.Username))
            {
                query = query.Where(x => x.Username.Contains(dto.Username));
            }


            // SORTIRANJE
            if (!String.IsNullOrEmpty(dto.sortBy))
            {
                if (dto.sortBy.ToLower() == "createdat")
                {
                    if (dto.sortOrder?.ToLower() == "desc")
                    {
                        query = query.OrderByDescending(x => x.CreatedAt);
                    }
                    else
                    {
                        query = query.OrderBy(x => x.CreatedAt);
                    }
                }
            }


            // PAGINACIJA
            int curPage = dto.Page ?? 1;
            int pageSize = 5;

            int skipUsers = (curPage - 1) * pageSize;

            query = query.Skip(skipUsers)
                         .Take(pageSize);


            return query.ToList();
        }


    }


}

