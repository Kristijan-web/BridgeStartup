using Application.DTO.User;
using Domain;

namespace Application.Queries
{
    public interface IUsersQuery : IQuery<SearchUsersDTO, IEnumerable<User>>
    {
    }
}
