using Application.DTO.User;

namespace Application.Queries
{
    public interface IUsersQuery : IQuery<SearchUsersDTO, IEnumerable<UserDbDTO>>
    {
    }
}
