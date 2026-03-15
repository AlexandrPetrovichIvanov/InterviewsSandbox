using YandexSandbox.Bll.Interfaces.Messaging;

namespace YandexSandbox.Bll.Interfaces.Messaging;

public interface IRentOrderPlacedMessageProducer
{
    Task ProduceAsync(RentOrderPlacedMessage message, CancellationToken cancellationToken = default);
}
