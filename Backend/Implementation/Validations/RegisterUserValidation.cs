using Application.DTO;
using FluentValidation;

namespace Implementation.Validations
{
    public class RegisterUserValidation : AbstractValidator<RegisterUserDTO>
    {



        public RegisterUserValidation()
        {


            RuleLevelCascadeMode = CascadeMode.Stop;


            RuleFor(x => x.Username).NotNull().WithMessage("Username must exist").Matches(@"^[^\d].{2,}$").WithMessage("Username must be longer than 2 characters and must not start with a number");

            RuleFor(x => x.Password).NotNull().WithMessage("Password must exist").MinimumLength(8).WithMessage("Password must contain at least 8 characters").Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter").Matches(@"\d").WithMessage("Password must contain at least one number");

            RuleFor(x => x.Email).NotNull().WithMessage("Email must exist").EmailAddress().WithMessage("Email format is not valid");





        }



    }
}
