using Application.Commands.Contacts;
using Application.DTO.Contact;
using Data.Access;
using Domain;

namespace Implementation.Commands.Contacts
{
    public class CreateContactCommand : ICreateContactCommand
    {
        public string Id => "create-contact";

        public string Name => "create contact";

        private ApplicationDbContext _context;

        public CreateContactCommand(ApplicationDbContext context)
        {
            _context = context;

        }

        public void Execute(CreateContactDTO dto)
        {


            Contact contact = new Contact
            {
                Subject = dto.Subject,
                Message = dto.Message,
                UserId = dto.UserId
            };

            _context.Contacts.Add(contact);
            _context.SaveChanges();


        }
    }
}
