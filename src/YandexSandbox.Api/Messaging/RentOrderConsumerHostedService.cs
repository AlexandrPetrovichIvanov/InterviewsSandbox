using Microsoft.Extensions.Options;
using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Interfaces.Services;
using YandexSandbox.Bll.Messaging;

namespace YandexSandbox.Api.Messaging;

public class RentOrderConsumerHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly InMemoryMessageBus _bus;
    private readonly string _topic;
    private readonly ILogger<RentOrderConsumerHostedService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(1);

    public RentOrderConsumerHostedService(
        IServiceScopeFactory scopeFactory,
        InMemoryMessageBus bus,
        IOptions<TopicSettings> topicSettings,
        ILogger<RentOrderConsumerHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _bus = bus;
        _topic = topicSettings.Value.TopicMap.GetValueOrDefault("RentOrderPlacedMessage", "RentOrderPlacedMessage");
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = _bus.Consume(_topic);
            if (message is RentOrderPlacedMessage orderMsg)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var rentService = scope.ServiceProvider.GetRequiredService<IRentOrdersService>();
                    await rentService.ProcessRentOrderAsync(
                        new ProcessRentOrderCommand { OrderId = orderMsg.OrderId },
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process rent order {OrderId}", orderMsg.OrderId);
                }
            }
            else
            {
                await Task.Delay(_pollingInterval, stoppingToken);
            }
        }
    }
}
