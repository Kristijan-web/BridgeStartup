using Application.DTO.Auth;
using Application.DTO.User;

namespace Application.Queries
{
    // response su korisnikovi podaci, to jest objekat
    public interface ILoginQuery : IQuery<LoginDTO, UserDbDTO>
    {

    }
}
