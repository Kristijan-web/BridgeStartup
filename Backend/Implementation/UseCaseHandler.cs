using Application.Commands;
using Application.Queries;

namespace Implementation
{
    public class UseCaseHandler
    {
        // Ova klasa treba da izvrsava i query i command metode

        // Zasto ova klasa uopste postoje?
        // - Da bi radila autorizaciju na osnovu funkcionalnosti 
        // - Ako odredjena komanda/query ne pripada u niz funkcionalnosti koje ima korisnik onda nece moci da izvrsi tu komandu/query

        // Sta mi je potrebno?
        // - IApplicationUser interfejs
        // - po 1 metoda za query i command
        // - 1 metoda za autorizaciju

        // ajde prvo da napisem metodu za komandu

        // Koje parametre prima ExecuteCommand?
        // - Komandu
        // - Podatke za komandu
        public void ExecuteCommand<T>(ICommand<T> cmd, T dto)
        {


            cmd.Execute(dto);

        }


        public Tresponse ExecuteQuery<Tdata, Tresponse>(IQuery<Tdata, Tresponse> query, Tdata dto)
        {



            return query.Execute(dto);

        }


    }
}
