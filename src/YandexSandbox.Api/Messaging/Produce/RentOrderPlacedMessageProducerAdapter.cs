using Microsoft.Extensions.Options;
using YandexSandbox.Bll.Interfaces.Messaging;

namespace YandexSandbox.Api.Messaging.Produce;

public class RentOrderPlacedMessageProducerAdapter : IRentOrderPlacedMessageProducer
{
    private readonly InMemoryOutboxStorage _storage;
    private readonly string _topic;

    public RentOrderPlacedMessageProducerAdapter(
        InMemoryOutboxStorage storage,
        IOptions<TopicSettings> topicSettings)
    {
        _storage = storage;
        _topic = topicSettings.Value.TopicMap.GetValueOrDefault("RentOrderPlacedMessage", "RentOrderPlacedMessage");
    }

    public Task ProduceAsync(RentOrderPlacedMessage message, CancellationToken cancellationToken = default)
    {
        _storage.Add(new OutboxEntry { Topic = _topic, Message = message });
        return Task.CompletedTask;
    }
}
