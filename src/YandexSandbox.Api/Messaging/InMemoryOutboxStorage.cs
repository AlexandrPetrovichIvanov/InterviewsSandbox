using System.Collections.Concurrent;

namespace YandexSandbox.Api.Messaging;

public class InMemoryOutboxStorage
{
    private readonly ConcurrentQueue<OutboxEntry> _entries = new();

    public void Add(OutboxEntry entry) => _entries.Enqueue(entry);

    public bool TryTake(out OutboxEntry? entry) => _entries.TryDequeue(out entry);
}
