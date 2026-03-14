namespace YandexSandbox.Api.Messaging;

public class InMemoryMessageProducer
{
    private readonly ILogger<InMemoryMessageProducer> _logger;

    public InMemoryMessageProducer(ILogger<InMemoryMessageProducer> logger)
    {
        _logger = logger;
    }

    public Task ProduceAsync(string topic, object message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Message produced to topic '{Topic}': {@Message}", topic, message);
        return Task.CompletedTask;
    }
}
