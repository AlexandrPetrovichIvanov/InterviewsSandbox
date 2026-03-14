using Microsoft.Extensions.Options;
using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Interfaces.Services;
using YandexSandbox.Bll.Messaging;

namespace YandexSandbox.Api.Messaging;

public class OrderConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly InMemoryMessageBus _bus;
    private readonly string _topic;
    private readonly ILogger<OrderConsumerService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(1);

    public OrderConsumerService(
        IServiceScopeFactory scopeFactory,
        InMemoryMessageBus bus,
        IOptions<TopicSettings> topicSettings,
        ILogger<OrderConsumerService> logger)
    {
        _scopeFactory = scopeFactory;
        _bus = bus;
        _topic = topicSettings.Value.TopicMap.GetValueOrDefault("OrderPlacedMessage", "OrderPlacedMessage");
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = _bus.Consume(_topic);
            if (message is OrderPlacedMessage orderMsg)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var rentService = scope.ServiceProvider.GetRequiredService<IRentService>();
                    await rentService.ProcessOrderAsync(
                        new ProcessOrderCommand { OrderId = orderMsg.OrderId },
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process order {OrderId}", orderMsg.OrderId);
                }
            }
            else
            {
                await Task.Delay(_pollingInterval, stoppingToken);
            }
        }
    }
}
