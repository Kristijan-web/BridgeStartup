using Application.Commands;
using Application.DTO;
using Data.Access;
using FluentValidation.Results;
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
            try
            {

                ValidationResult isRegisterDTOValid = _validation.Validate(dto);
                if (!isRegisterDTOValid.IsValid)
                {
                    // Sta radim ako nije validno, vratim nazad response objekat sa greskom
                    // Kako da ovo delegiram u catch blok i tamo da handlujem?
                    // Kako je mislim da ce izgledati proces?
                    // - Pa pozvace se neka metoda samo i to je to, verovatno nad nekom klasom.

                    return UnprocessableEntity(isRegisterDTOValid.Errors);

                }

            }
            catch (Exception ex)
            {
                // Ocekuje da se prosledi objekat 

                // Sada treba da se radi logika za upis u bazu

                // treba da se napise validacija podataka

                // Kako ce ovde ici logika za izvrsavanje komande?
                // - Prosledice se data podaci validatoru
                // - Ako prodje onda se pise query logika ka bazi i to je to

                // Kako pozivam validator

                // kako da prosledim objekat po kojem se radi validacija?
                // - Jedino konstruktork

                // Fora je sto ja ne instanciram ovde validator nego ga dobijam od DI container-a



                // Ako pukne validacija u .Validate() metodi, hoce li odmah da se vrati nazad response?
                // Ako hoce koji response kod ce da vrati 


            }
        }
    }
}
