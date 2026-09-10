using Application.DTO.Contact;

namespace Application.Commands.Contacts
{

    // Koje podaci su mi potrebni
    // - Prvo mora tabela u bazi
    // - Contacts koja sadrzi id usera koji je submitovo formu

    // Koji podaci su mi potrebni da bih napravio novi zapis contact-a u bazi?
    // - 
    public interface ICreateContactCommand : ICommand<CreateContactDTO>
    {
    }
}
