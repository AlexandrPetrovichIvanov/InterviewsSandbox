using Microsoft.Extensions.Options;
using YandexSandbox.Bll.Messaging;

namespace YandexSandbox.Api.Messaging;

public class OutboxMessageProducerDecorator : IMessageProducer
{
    private readonly InMemoryOutboxStorage _storage;
    private readonly TopicSettings _topicSettings;

    public OutboxMessageProducerDecorator(
        InMemoryOutboxStorage storage,
        IOptions<TopicSettings> topicSettings)
    {
        _storage = storage;
        _topicSettings = topicSettings.Value;
    }

    public Task ProduceAsync(object message, CancellationToken cancellationToken = default)
    {
        var messageType = message.GetType().Name;
        var topic = _topicSettings.TopicMap.GetValueOrDefault(messageType, messageType);

        _storage.Add(new OutboxEntry { Topic = topic, Message = message });
        return Task.CompletedTask;
    }
}
