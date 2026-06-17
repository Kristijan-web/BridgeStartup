using Application.DTO;
using Application.Exceptions;
using Application.Queries;
using ASPLAB2.API.JWT;
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
        JwtHandler _jwtHandler;

        // Da li implementation uopste treba da vidi API?

        // Sta mi je sve potrebno za sve sto radim u klasi od DI container-a?
        // - ApplicationDbContext
        // - JWTHandler
        public EfLoginQuery(ApplicationDbContext context, JwtHandler jwtHandler)
        {
            _context = context;
            _jwtHandler = jwtHandler;
        }

        public LoginResponseDTO Execute(LoginDTO dto)
        {


            // - FirstOrDefault ce biti null ako rezultat ne postoji
            Boolean doesUserExist = _context.Users.Any(x => x.Email == dto.Email);

            if (!doesUserExist)
            {

                throw new LoginException();

            }

            // Sta radim ako user postoji?

            // - Dohvatam njegove podatke 
            // - Treba da popunim jwt podacima
            // - Treba da vratim podatke kroz dto client-u

            User user = _context.Users.Include(x => x.Role).First(y => y.Email == dto.Email);

            // Kako da popunim JWT korisnikovim podacima?
            // - Treba mi klasa JWT handler, ona ce ovde da se prosledi kroz DI container





            return new LoginResponseDTO { };
        }
    }
}
