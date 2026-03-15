namespace YandexSandbox.Bll.Interfaces.Messaging;

public interface IRentOrderProcessingConsumer
{
    Task<RentOrderProcessedMessage?> ConsumeAsync(CancellationToken cancellationToken = default);
}
