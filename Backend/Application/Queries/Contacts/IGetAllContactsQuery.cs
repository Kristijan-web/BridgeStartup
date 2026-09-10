using Application.DTO.Contact;

namespace Application.Queries.Contacts
{
    // Koji tip podatka dto-a primam
    // / -  Za filtraciju
    // Koji tip podatka vracam
    // - GetContactsDbDTO

    // Koliko tipova podataka ocekuje?

    public interface IGetAllContactsQuery : IQuery<FilterContactsDTO, IEnumerable<GetAllContactsDbDTO>>
    {
    }
}
