using Application.DTO.Post;

namespace Application.Commands
{
    // komanda samo prima DTO

    // Meni se nalazi DTO u drugom sloju (Backend)
    // Da li Application treba da vidi backend?
    // - Princip aplicationa je taj da je on odgovoran za potpise, on nema veze sa Backend

    // Kako onda ja da koristim DTO iz drugog sloja
    // Ne mogu u ovom sloju da deifnisem DTO jer 
    // Sto u ovom sloju ne mogu da definisem DTO?
    // - Nesto ima veze sa FilePath, tip podatka FilePath je vezan samo za Backend sloj

    // Kako ja sada da definisem DTO koji je u drugom sloju kada aplication ne bih trebao da zna za API?

    // Kako da definisem IFormFile 

    // Sta je IFormFile?
    // U kom formatu se podatak ocekuje kada je definisan IFormFile?
    // - Oznacava fajl
    // Kada definisem u controlleru tip podatka IFormFile someFile, da li ce se fajl prebaciti u tu promenljivu implicitno?

    // Mogu li ovaj interfejs bolje da nazovem?
    // Cilj mi je da imam interfejs koji ce forsirati logiku za razlicite nacine uload-a fajla-, npr cloud, localno itd...
    // IUploadPostFileTo
    public interface IUploadPostFileToCommand : IAsyncCommand<ApplyToPostDTO>
    {
       // zar ne bih trebao da definisem id i name polja od use-case interfejsa?
    }
}
