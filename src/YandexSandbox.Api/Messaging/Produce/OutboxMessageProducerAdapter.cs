using Microsoft.Extensions.Options;
using YandexSandbox.Bll.Interfaces.Messaging.Producers;

namespace YandexSandbox.Api.Messaging.Produce;

public class OutboxMessageProducerAdapter<TMessage> : IMessageProducer<TMessage>
{
    private readonly InMemoryOutboxStorage _storage;
    private readonly string _topic;

    public OutboxMessageProducerAdapter(
        InMemoryOutboxStorage storage,
        IOptions<TopicSettings> topicSettings)
    {
        _storage = storage;
        var typeName = typeof(TMessage).Name;
        _topic = topicSettings.Value.TopicMap.GetValueOrDefault(typeName, typeName);
    }

    public Task ProduceAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        _storage.Add(new OutboxEntry { Topic = _topic, Message = message! });
        return Task.CompletedTask;
    }
}
