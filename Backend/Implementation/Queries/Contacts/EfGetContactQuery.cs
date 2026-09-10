using Application.DTO.Contact;
using Application.Exceptions;
using Application.Queries.Contacts;
using Data.Access;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries.Contacts
{
    public class EfGetContactQuery : IGetContactQuery
    {
        public string Id => "get-contact";

        public string Name => "get-contacts";

        private ApplicationDbContext _context;

        public EfGetContactQuery(ApplicationDbContext context)
        {
            _context = context;

        }

        public GetContactDbDTO Execute(long id)
        {

            // Dovhati contact i user-a koji ga je objavio
            GetContactDbDTO? contact = _context.Contacts.Where(x => x.Id == id).Include(x => x.User).ThenInclude(x => x.Role).Select(x => new GetContactDbDTO
            {
                Subject = x.Subject,
                Message = x.Message,
                User = new Application.DTO.User.UserDbDTO
                {
                    Id = x.User.Id,
                    Username = x.User.Username,
                    Email = x.User.Email,
                    Role = x.User.Role.Name
                }
            }).FirstOrDefault();

            if (contact == null)
            {
                throw new EntityNotFoundException("Id for provided contact does not exist");
            }

            return contact;

        }
    }
}
