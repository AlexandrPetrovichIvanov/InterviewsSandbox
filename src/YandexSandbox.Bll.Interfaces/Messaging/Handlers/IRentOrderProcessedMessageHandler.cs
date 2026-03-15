using YandexSandbox.Bll.Interfaces.Messaging.Messages;

namespace YandexSandbox.Bll.Interfaces.Messaging.Handlers;

public interface IRentOrderProcessedMessageHandler
{
    Task HandleAsync(RentOrderProcessedMessage message, CancellationToken cancellationToken = default);
}
