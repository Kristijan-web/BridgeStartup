using Application.Commands.Contacts;
using Application.Exceptions;
using Data.Access;
using Domain;

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

        public void Execute(long id)
        {
            Contact? contact = _context.Contacts
                .FirstOrDefault(x => x.Id == id);

            if (contact == null)
            {
                throw new EntityNotFoundException(
                    "Contact for provided id does not exist"
                );
            }

            _context.Contacts.Remove(contact);

            _context.SaveChanges();
        }
    }
}
