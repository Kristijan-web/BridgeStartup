using Application.Commands;
using Application.DTO.Post;
using System;
using System.Collections.Generic;
using System.Text;

namespace Implementation.Commands.Posts
{
    public class EfUploadPostFileToLocal : IUploadPostFileToCommand
    {
        public string Id => "apply-to-post-locally";

        public string Name => "Uploading post data locally";

        public async void Execute(ApplyToPostDTO dto)
        {
            // Ovde ide logika za cuvanje podaka localno

            // Kada mi korisnik posalje fajl koje sve podatke cu imati o tom fajlu iz ApplyToPostDTO-a?
            // public string FileName { get; set; }
            // public string ContentType { get; set; }
            // public long FileLength { get; set; }
            // public Stream FileStream { get; set; } // sadrzaj fajla

            // Koraci:
            // - Prvo trebam da definisem naziv fajla, to mogu sa guid, mogu ono sto je korisnik poslao + guid

            Guid guid = Guid.NewGuid();
            string fileExtension = dto.FileName.Split('.')[1];
            string fileName = $"{dto.FileName.ToLower()}-{guid}-{fileExtension}";

            // Sta je sledeci korak?
            // - Da se fajl upload-uje lokalno
            // - Fajlovi se lokalno upload-uju u wwwroot folder u API sloju

            // Kako da upload-ujem fajl lokalno u wwwroot folder??????
            // - Verovatno postoji neki objekat na koji pozovem metodu i navedem putanju gde zelim da sacuvam fajl

            // Zasto je bolje da koristim Path.Combine umesto da putanju do foldera rucno definisem?
            // - Kako bi definisao putanju do foldera? -> "Home\User\Kris" -> Ovo bi bacilo compile error jer ne postoji escape sekvenca \U ni \K u C#-u. Mogao bi sa "Home\\User\\Kris" I ovo bi radilo, medjutim postavlja se pitanje "Sta ako se kod izvrsava na Linux masini?" -> Onda ovo nece raditi, i fajlovi se uopste nece cuvati, Onda bih morao da pisem kod koji proverava da li se kod izvsava na windwows-u ili linuxu. Da to ne bih radio koristim Path.Combine
            // Sada treba da definisem putanju do foldera gde ce se cuvati fajl

            string filePath = Path.Combine("wwwroot", "CVs");

            // Jos nesto fali...
            // Sada treba da pozovem liniju koda koja ce da cuva fajl u definisanu putanju -> pitanje je da li ce ta linija koda da ocekuje "Apsolutnu ili relativnu putanju"

            // Ako ocekuje relativnu putanju pitanje je u odnosu na koji folder relativno?
            // Ako ocekuje apsolutnu putanju pitanje je od kog root foldera krece?

            // Gde ovde navodim naziv fajla koji ce biti sacuvan u wwwroot/cv folderu?

            // Ovde upload-ujem
            using FileStream stream = new FileStream(filePath, FileMode.Create);

            await dto.FileStream.CopyToAsync(stream); // fajl ubacuje u definisanu putanju



        }
    }
}
