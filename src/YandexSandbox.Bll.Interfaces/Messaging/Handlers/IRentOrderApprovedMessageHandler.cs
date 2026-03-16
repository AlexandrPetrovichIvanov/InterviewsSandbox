using YandexSandbox.Bll.Interfaces.Messaging.Messages;

namespace YandexSandbox.Bll.Interfaces.Messaging.Handlers;

public interface IRentOrderApprovedMessageHandler
{
    Task HandleAsync(RentOrderApprovedMessage message, CancellationToken cancellationToken = default);
}
