using Application.DTO.Contact;
using Application.Queries.Contacts;
using Data.Access;
using Microsoft.EntityFrameworkCore;

namespace Implementation.Queries.Contacts
{
    public class EfGetAllContactsQuery : IGetAllContactsQuery
    {
        public string Id => "get-contacts";

        public string Name => "get contacts";

        public ApplicationDbContext _context;
        public EfGetAllContactsQuery(ApplicationDbContext context)
        {
            _context = context;
        }


        public IEnumerable<GetContactDbDTO> Execute(FilterContactsDTO dto)
        {

            IEnumerable<GetContactDbDTO> contacts = _context.Contacts.Include(x => x.User).ThenInclude(x => x.Role).Select(x => new GetContactDbDTO
            {
                Id = x.Id,
                Subject = x.Subject,
                Message = x.Message,
                User = new Application.DTO.User.UserDbDTO
                {
                    Id = x.User.Id,
                    Username = x.User.Username,
                    Email = x.User.Email,
                    // Treba mi role-a user-a
                    Role = x.User.Role.Name
                }
            }
           );


            return contacts;


        }
    }
}
