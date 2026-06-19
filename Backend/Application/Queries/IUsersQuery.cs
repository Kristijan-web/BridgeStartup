using Application.DTO;
using Domain;

namespace Application.Queries
{
    public interface IUsersQuery : IQuery<SearchUsersDTO, IEnumerable<User>>
    {
    }
}
