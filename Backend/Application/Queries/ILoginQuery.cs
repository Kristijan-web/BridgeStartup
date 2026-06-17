using Application.DTO;

namespace Application.Queries
{
    // response su korisnikovi podaci, to jest objekat
    public interface ILoginQuery : IQuery<LoginDTO, LoginResponseDTO>
    {

    }
}
