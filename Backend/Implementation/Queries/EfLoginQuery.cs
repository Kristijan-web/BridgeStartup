using Application.DTO.Auth;
using Application.Exceptions;
using Application.Queries;
using Data.Access;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries
{
    public class EfLoginQuery : ILoginQuery
    {
        //  Id je za bazu podataka
        // Name je za korisnike 


        public string Id { get; } = "login";

        public string Name { get; set; } = "login";

        ApplicationDbContext _context;


        public EfLoginQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public User Execute(LoginDTO dto)
        {


            // - FirstOrDefault ce biti null ako rezultat ne postoji
            Boolean doesUserExist = _context.Users.Any(x => x.Email == dto.Email);

            if (!doesUserExist)
            {

                throw new LoginException();

            }


            User user = _context.Users.Include(x => x.Role).First(y => y.Email == dto.Email);



            return user;



        }
    }
}
