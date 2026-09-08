using Application.Commands;
using Application.DTO.Auth;
using Application.Email;
using Data.Access;
using Domain;
using FluentValidation;
using Implementation.Emails;
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

        public string Id { get; } = "register-user";
        public string Name { get; } = "register user";

        private ApplicationDbContext _context;

        private RegisterUserValidation _validation;

        private readonly IEmailSender _emailSender;
        private readonly EmailTemplateComposer _composer;

        public EfRegisterUserCommand(ApplicationDbContext context, RegisterUserValidation validation, IEmailSender emailSender, EmailTemplateComposer composer)
        {

            _context = context;
            _validation = validation;
            _emailSender = emailSender;
            _composer = composer;
        }




        public void Execute(RegisterUserDTO dto)
        {


            _validation.ValidateAndThrow(dto);



            string hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            string activationCode = Guid.NewGuid().ToString();

            User user = new User
            {
                Username = dto.Username,
                Password = hash,
                Email = dto.Email,
                ActivationCode = activationCode,
                RegisteredAt = DateTime.UtcNow

            };

            // Link dodajem na dto, kako pravinm link 
            // https://localhost:7086/api/activate/code

            string link = $"https://localhost:7086/api/activate/{activationCode}";

            dto.Link = link;

            var html = _composer.GetTemplateContent(EmailTemplate.Register, dto);

            _emailSender.SendEmail(user.Email, "User registration", html);

            _context.Add(user);
            _context.SaveChanges();

        }


    }
}


