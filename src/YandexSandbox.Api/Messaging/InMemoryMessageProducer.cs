namespace YandexSandbox.Api.Messaging;

public class InMemoryMessageProducer
{
    private readonly ILogger<InMemoryMessageProducer> _logger;
    private readonly InMemoryMessageBus _bus;

    public InMemoryMessageProducer(ILogger<InMemoryMessageProducer> logger, InMemoryMessageBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public Task ProduceAsync(string topic, object message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Message produced to topic '{Topic}': {@Message}", topic, message);
        _bus.Publish(topic, message);
        return Task.CompletedTask;
    }
}
