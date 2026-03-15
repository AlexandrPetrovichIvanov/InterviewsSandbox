using YandexSandbox.Bll.Interfaces.Commands;
using YandexSandbox.Bll.Interfaces.Messaging.Handlers;
using YandexSandbox.Bll.Interfaces.Messaging.Messages;
using YandexSandbox.Bll.Interfaces.Services;

namespace YandexSandbox.Bll.Handlers;

public class RentOrderProcessedMessageHandler : IRentOrderProcessedMessageHandler
{
    private readonly IRentOrdersService _rentOrdersService;

    public RentOrderProcessedMessageHandler(IRentOrdersService rentOrdersService)
    {
        _rentOrdersService = rentOrdersService;
    }

    public async Task HandleAsync(RentOrderProcessedMessage message, CancellationToken cancellationToken = default)
    {
        await _rentOrdersService.ProcessRentOrderAsync(
            new ProcessRentOrderCommand { OrderId = message.OrderId },
            cancellationToken);
    }
}
