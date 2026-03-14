namespace YandexSandbox.Api.Messaging;

public class OutboxDispatcherService : BackgroundService
{
    private readonly InMemoryOutboxStorage _storage;
    private readonly InMemoryMessageProducer _producer;
    private readonly ILogger<OutboxDispatcherService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(1);

    public OutboxDispatcherService(
        InMemoryOutboxStorage storage,
        InMemoryMessageProducer producer,
        ILogger<OutboxDispatcherService> logger)
    {
        _storage = storage;
        _producer = producer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            while (_storage.TryTake(out var entry))
            {
                try
                {
                    await _producer.ProduceAsync(entry!.Topic, entry.Message, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to dispatch outbox entry {EntryId}", entry!.Id);
                }
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }
    }
}
