namespace Application.Commands;

public interface IAsyncCommand<TData> : IUseCase
{
    Task ExecuteAsync(TData dto, CancellationToken cancellationToken = default);
}
