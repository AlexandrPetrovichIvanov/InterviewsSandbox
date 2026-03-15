using Microsoft.Extensions.Options;
using YandexSandbox.Bll.Interfaces.Messaging;

namespace YandexSandbox.Api.Messaging.Produce;

public class CarCreatedMessageProducerAdapter : ICarCreatedMessageProducer
{
    private readonly InMemoryOutboxStorage _storage;
    private readonly string _topic;

    public CarCreatedMessageProducerAdapter(
        InMemoryOutboxStorage storage,
        IOptions<TopicSettings> topicSettings)
    {
        _storage = storage;
        _topic = topicSettings.Value.TopicMap.GetValueOrDefault("CarCreatedMessage", "CarCreatedMessage");
    }

    public Task ProduceAsync(CarCreatedMessage message, CancellationToken cancellationToken = default)
    {
        _storage.Add(new OutboxEntry { Topic = _topic, Message = message });
        return Task.CompletedTask;
    }
}
