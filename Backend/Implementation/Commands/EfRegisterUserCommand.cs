using Application.Commands;
using Application.DTO;
using Data.Access;
using Domain;
using FluentValidation;
using Implementation.Validations;

namespace Implementation.Commands
{
    public class EfRegisterUserCommand : IRegisterUserCommand
    {
        // Ne razumem sta je problem 
        //public string Name => "Create new repertoire record";

        //public string Id => "add-repertoire";

        // Sta je problem?
        // Kako sam definisao polja u IUseCase interfejsu?
        // Kao get i set
        // Sta ako hocu samo get 

        public string Id { get; } = "Register-user";
        public string Name { get; } = "Register user";

        private ApplicationDbContext _context;

        private RegisterUserValidation _validation;

        public EfRegisterUserCommand(ApplicationDbContext context, RegisterUserValidation validation)
        {

            _context = context;
            _validation = validation;
        }




        public void Execute(RegisterUserDTO dto)
        {


            _validation.ValidateAndThrow(dto); // ako se desi greska onda je greska tipa ValidationException?

            // Sta je problem?
            // - Dto uspesno doalzi do ove metode i prolazi 42. liniju za validaciju
            // - The entity type 'RegisterUserDTO' was not found. 
            // - Greska je u tome sto ORM ne zna za koju tabelu da stage-uje podatke, klasa RegisterUserDTO ne postoji kao klasa u DbSet<> 

            // Treba da se doda defaultni role

            // Sta treba da se uradi
            // Treba da se napravi konfiguracija za User tabelu i da se za foreign kolonu stavi defaultna vrednost id-a

            User user = new User
            {
                Username = dto.Username,
                Password = dto.Password,
                Email = dto.Email,
            };

            // Nije prosledjen roleId usera
            // - Moja hipoteza je da EF core po defaultu assign-uje RoleId = 0 a on ne postoji po referencijalnom integritetu i zato baza 


            _context.Add(user);
            _context.SaveChanges();

        }


    }
}


