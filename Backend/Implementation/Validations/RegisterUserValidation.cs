using Application.DTO.Auth;
using Data.Access;
using FluentValidation;

namespace Implementation.Validations
{
    public class RegisterUserValidation : AbstractValidator<RegisterUserDTO>
    {


        // RegisuterUserValidation gde se poziva?
        // Poziva se u klasi EfRegisterUSerCommand
        // Ova klasa je registrovana u DI container?

        // Gde se trazi instanca RegisterUserValidation-a u kojoj klasi?
        // - U konstruktoru EfRegisterUserCommand 

        // Kako se prosledjuje Execute metodi EfRegisterUSerCommand kada ona prima samo DTO?
        // Definise se u konstruktor


        public RegisterUserValidation(ApplicationDbContext _context)
        {


            RuleLevelCascadeMode = CascadeMode.Stop;


            RuleFor(x => x.Username).NotEmpty().WithMessage("Username must exist").Matches(@"^[^\d].{2,}$").WithMessage("Username must be longer than 2 characters and must not start with a number");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password must exist").MinimumLength(8).WithMessage("Password must contain at least 8 characters").Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter").Matches(@"\d").WithMessage("Password must contain at least one number");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email must exist").EmailAddress().WithMessage("Email format is not valid").Must(x => !_context.Users.Any(y => y.Email == x)).WithMessage("Email already exists");

        }



    }
}
