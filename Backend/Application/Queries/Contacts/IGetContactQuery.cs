using Application.DTO.Contact;

namespace Application.Queries.Contacts
{
    public interface IGetContactQuery : IQuery<long, GetContactDbDTO>
    {
    }
}
