namespace Application.Commands
{
    public interface ICommand<TData> : IUseCase
    {

        public void Execute(TData dto);


    }
}
