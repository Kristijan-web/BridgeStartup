using Application.Commands;
using Application.Email;
using Application.Exceptions;
using Data.Access;
using Domain;
using Implementation.Emails;

namespace Implementation.UseCases.Commands
{
    public class EfActivateAccountCommand : IActivateAccountCommand
    {
        private IEmailSender _sender;
        private EmailTemplateComposer _composer;
        public ApplicationDbContext _context;
        public EfActivateAccountCommand(ApplicationDbContext context, IEmailSender sender, EmailTemplateComposer composer)
        {
            _sender = sender;
            _composer = composer;
            _context = context;
        }

        public string Name => "Account activation";

        public string Id => "account-activate";

        // Logika se trigeruje nakon sto je korisnik kliknuo na aktivacioni link u mail-u
        public void Execute(string data)
        {

            var user = _context.Users.FirstOrDefault(x => x.ActivationCode == data);

            if (user == null)
            {
                throw new EntityNotFoundException(nameof(User));
            }

            if (user.ActivatedAt.HasValue)
            {
                throw new EntityNotFoundException(nameof(User));
            }

            // Ako je proslo vise od 5 minuta od kako je user registrovan onda baca gresku
            //if ((DateTime.UtcNow - user.RegisteredAt.Value).TotalMinutes > 5)
            //{
            //    throw new EntityNotFoundException("Proslo je vreme za registraciju");
            //}

            user.ActivatedAt = DateTime.UtcNow;
            user.ActivationCode = null;

            _context.SaveChanges();

            var html = _composer.GetTemplateContent(EmailTemplate.Activation, user);
            _sender.SendEmail(user.Email, "Account activated", html);


        }
    }
}
