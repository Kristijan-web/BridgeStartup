using Application.DTO.PostApplication.Command;
using FluentValidation;

namespace Implementation.Validations
{

    // dto verifikujem
    public class UpdatePostApplicationValidator : AbstractValidator<PostApplicationUpdateDTO>
    {

        public UpdatePostApplicationValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            // mora da postoji Putanja fajla, Al kako to 



        }


    }
}
