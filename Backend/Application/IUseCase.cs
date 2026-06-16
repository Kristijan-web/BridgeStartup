namespace Application
{
    public interface IUseCase
    {

        // Funkcionalnost treba da ima ime da bi kasnije mogli da korisnicima u aplikaciji dodajemo odredjene funkcionalnosti, al cemu sluzi onda Id?
        public string Id { get; }
        public string Name { get; }
    }
}
