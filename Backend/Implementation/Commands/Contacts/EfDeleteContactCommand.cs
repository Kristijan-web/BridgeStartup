using Application.Commands.Contacts;
using Application.DTO.Contact;
using Application.Exceptions;
using Data.Access;

namespace Implementation.Commands.Contacts
{
    public class EfDeleteContactCommand : IDeleteContactCommand
    {
        public string Id => "delete-contact";

        public string Name => "delete contact";

        private ApplicationDbContext _context;
        public EfDeleteContactCommand(ApplicationDbContext context)
        {

            _context = context;
        }

        public void Execute(long dto)
        {

            // KAko se radi brisanje
            // Ne postoji delete 
            // Mora da dohvatim zapis iz baze i da mu promenim entity state

            GetContactDbDTO? contact = _context.Contacts.Select(x => new GetContactDbDTO
            {
                Subject = x.Subject,
                Message = x.Message
            }).FirstOrDefault();

            if (contact == null)
            {

                throw new EntityNotFoundException("Contact for provided id does not exist");

            }


        }
    }
}
