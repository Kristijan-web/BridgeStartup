using Application.DTO.Auth;
using Domain;

namespace Application.Queries
{
    // response su korisnikovi podaci, to jest objekat
    public interface ILoginQuery : IQuery<LoginDTO, User>
    {

    }
}
