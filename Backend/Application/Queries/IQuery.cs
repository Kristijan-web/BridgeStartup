namespace Application.Queries
{
    // query i prima i vraca podatke
    public interface IQuery<Tdata, Tresponse> : IUseCase
    {
        public Tresponse Execute(Tdata dto);

    }
}
