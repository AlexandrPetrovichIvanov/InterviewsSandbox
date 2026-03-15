using System.Collections.Concurrent;
using YandexSandbox.Bll.Interfaces.Services;

namespace YandexSandbox.Api.Services;

public class InMemoryLock : ILock
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

    public async Task<IAsyncDisposable> AcquireAsync(string key, CancellationToken cancellationToken = default)
    {
        var semaphore = _semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        return new LockRelease(semaphore);
    }

    private sealed class LockRelease(SemaphoreSlim semaphore) : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            semaphore.Release();
            return ValueTask.CompletedTask;
        }
    }
}
