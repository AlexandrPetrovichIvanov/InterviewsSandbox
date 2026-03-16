using YandexSandbox.Bll.Interfaces.Commands;
using YandexSandbox.Bll.Interfaces.Messaging.Handlers;
using YandexSandbox.Bll.Interfaces.Messaging.Messages;
using YandexSandbox.Bll.Interfaces.Services;

namespace YandexSandbox.Bll.Handlers;

public class RentOrderApprovedMessageHandler : IRentOrderApprovedMessageHandler
{
    private readonly IRentOrdersService _rentOrdersService;

    public RentOrderApprovedMessageHandler(IRentOrdersService rentOrdersService)
    {
        _rentOrdersService = rentOrdersService;
    }

    public async Task HandleAsync(RentOrderApprovedMessage message, CancellationToken cancellationToken = default)
    {
        await _rentOrdersService.ApproveRentOrderAsync(
            new ApproveRentOrderCommand { OrderId = message.OrderId },
            cancellationToken);
    }
}
