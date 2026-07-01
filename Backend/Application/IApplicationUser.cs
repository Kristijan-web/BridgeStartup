namespace Application
{
    public interface IApplicationUser
    {
        // Cemu sluzi ovaj interfejs
        // - Da definise koje podatke ce sadrzati user i njegove funkcionalnosti

        // Ko ce koristiti ovaj interfejs?
        // - UseCaseHandler

        //  Zasto ga koristi bas UseCaseHandler?
        // -  zbog DI, kada on zatrazi ovaj interfejs ima da se proslede podaci korisnika
        public long Id { get; }
        public string Username { get; }
        public string Email { get; }
        public IEnumerable<string> AllowedUseCases { get; }
    }
}
