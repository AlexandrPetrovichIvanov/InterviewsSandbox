using System.Collections.Concurrent;

namespace YandexSandbox.Api.Messaging;

public class InMemoryMessageBus
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<object>> _topics = new();

    public void Publish(string topic, object message)
    {
        var queue = _topics.GetOrAdd(topic, _ => new ConcurrentQueue<object>());
        queue.Enqueue(message);
    }

    public object? Consume(string topic)
    {
        if (_topics.TryGetValue(topic, out var queue) && queue.TryDequeue(out var message))
            return message;
        return null;
    }
}
