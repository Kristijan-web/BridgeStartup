using Application.Commands;
using Application.DTO.Post;

namespace Implementation.Commands.Posts
{
    public class EfUploadPostFileToLocal : IUploadPostFileToCommand
    {
        public string Id => "apply-to-post-locally";

        public string Name => "Uploading post data locally";

        public async void Execute(ApplyToPostDTO dto)
        {

            //Koncept rada sa fajlovima
            // Kada korisnik posalje fajl na server, pitanje je u kom formatu je poslao sadrzaj fajla, 
            // Kada korisnik posalje fajl na server u preko form-data, kao niz bajtova, onda ti bajtovi treba da se prebace lokalno
            // Onda se postavlja pitanje "na koji nacin zelimo da procitamo sadrzaj upload-ovanog fajla"?
            // - Jedan od nacina je da koristimo stream
            // - Drugi nacin je da direktno odjednom prebacimo sve fajlove u memoriju

            Guid guid = Guid.NewGuid();
            string fileExtension = dto.FileName.Split('.')[1];
            string fileName = $"{dto.FileName.Split('.')[0].ToLower()}-{guid}-{fileExtension}";



            string filePath = Path.Combine("wwwroot", "CVs", fileName);



            using FileStream stream = new FileStream(filePath, FileMode.Create);

            // CopyToAsync napravi sam buffer u memorji i definise bajtove koje ce uzimati chunk i sam pravi petlju koja loop-a kroz fajl.
            await dto.FileStream.CopyToAsync(stream); // fajl ubacuje u definisanu putanju



        }
    }
}
