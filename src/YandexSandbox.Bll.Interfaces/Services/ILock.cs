namespace YandexSandbox.Bll.Interfaces.Services;

public interface ILock
{
    Task<IAsyncDisposable> AcquireAsync(string key, CancellationToken cancellationToken = default);
}
