using Application;

namespace Implementation
{
    public class UnauthorizedUser : IApplicationUser
    {

        // Koji ce biti id ne autorizovanog user-a?
        // Mogu li da stavim bilo koji id koji zelim?
        // - Pa da, iskreno ne bitno je, jedina bitna stvar je je AllowedUseCases
        // Da li cu ikada Id ne autorizovanog korisnika koristiti za neku logiku?
        // - Nema sta da dohvatim iz baze jer je koristinik ne autorizovan, to je njegova role-a

        // Ako je korisnik ne autentifikovan da li to isto znaci da je i ne autorizovan?
        // - Ne autenfikovan znaci da njegov identitet nije pronadjen
        // - Ne autorizovan znaci da on ne sme da izvrsi odredjenu funkcionalnost, znaci da nema permisije

        // Koje role korisnika postoje u ovoj aplikaciji?
        // Koji je naziv role za korisnike koji nisu jos registrovani na aplikaciju? 
        // Ako je ne autentifikovani, to onda znaci da nisu registrovani u bazi
        // Ako je ne autorizovani, to onda da nemaju mogucnost da izvrse odredjenu funkcionalnost
        // Cak i ne autorizvan user moze biti autentifikovan a da ne sme da izvrsi odredjenu funkcionalnost
        // A ako je ne autentifikovan onda smo sigurni da korisnik nije ulogovan na aplikaciju
        // - user
        // - admin
        public long Id => 0;

        public string Username => "unauthorized user";

        public string Email => "unauthorizeduser@gmail.com";

        // treba mi niz stringova
        public IEnumerable<string> AllowedUseCases => new List<string> { };
    }

}
