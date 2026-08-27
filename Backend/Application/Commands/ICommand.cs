namespace Application.Commands
{
    public interface ICommand<TData> : IUseCase
    {
        // Zasto me kod ne forsira da definisem potpise iz interfejsa IUseCase?
        // - Zato sto interfejs poziva drugi interfejs, a interfejs sluzi da definise potpise!!!!
        public void Execute(TData dto);


    }
}
