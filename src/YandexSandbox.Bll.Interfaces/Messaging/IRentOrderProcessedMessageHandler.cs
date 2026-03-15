namespace YandexSandbox.Bll.Interfaces.Messaging;

public interface IRentOrderProcessedMessageHandler
{
    Task HandleAsync(RentOrderProcessedMessage message, CancellationToken cancellationToken = default);
}
