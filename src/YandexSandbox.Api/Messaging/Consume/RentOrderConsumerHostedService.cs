using YandexSandbox.Bll.Interfaces.Messaging;

namespace YandexSandbox.Api.Messaging.Consume;

public class RentOrderConsumerHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRentOrderProcessingConsumer _consumer;
    private readonly ILogger<RentOrderConsumerHostedService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(1);

    public RentOrderConsumerHostedService(
        IServiceScopeFactory scopeFactory,
        IRentOrderProcessingConsumer consumer,
        ILogger<RentOrderConsumerHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await _consumer.ConsumeAsync(stoppingToken);
            if (message is not null)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IRentOrderProcessedMessageHandler>();
                    await handler.HandleAsync(message, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to handle rent order message {OrderId}", message.OrderId);
                }
            }
            else
            {
                await Task.Delay(_pollingInterval, stoppingToken);
            }
        }
    }
}
