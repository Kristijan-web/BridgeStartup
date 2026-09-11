using Application.DTO.User;
using Application.Queries;
using Data.Access;
using Domain;

namespace Implementation.Queries.Users
{
    public class EfUsersQuery : IUsersQuery
    {
        public string Id { get; } = "get-all-users";
        public string Name { get; } = "getting all users";

        private ApplicationDbContext _context;

        public EfUsersQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<UserDbDTO> Execute(SearchUsersDTO dto)
        {
            IQueryable<User> query = _context.Users.Where(x => x.DeletedAt == null);

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

            query = query
                .Skip(skipUsers)
                .Take(pageSize);


            IQueryable<UserDbDTO> users = query.Select(x => new UserDbDTO
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
                Password = x.Password,

                Role = x.Role.Name,

                AllowedUseCases = x.Role.RoleUseCases
                    .Select(ru => ru.UseCases.UseCaseId)
            });


            return users.ToList();
        }
    }
}