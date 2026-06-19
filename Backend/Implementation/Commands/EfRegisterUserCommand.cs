using Application.Commands;
using Application.DTO.Auth;
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


            _validation.ValidateAndThrow(dto);



            string hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            User user = new User
            {
                Username = dto.Username,
                Password = hash,
                Email = dto.Email,
            };




            _context.Add(user);
            _context.SaveChanges();

        }


    }
}


