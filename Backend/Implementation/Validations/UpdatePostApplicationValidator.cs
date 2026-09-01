using Application.DTO.PostApplication.Command;
using Data.Access;
using FluentValidation;

namespace Implementation.Validations
{

    // dto verifikujem
    public class UpdatePostApplicationValidator : AbstractValidator<PostApplicationUpdateDTO>
    {

        public UpdatePostApplicationValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Fi)

        }


    }
}
