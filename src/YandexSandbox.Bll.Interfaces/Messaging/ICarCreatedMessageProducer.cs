using YandexSandbox.Bll.Interfaces.Messaging;

namespace YandexSandbox.Bll.Interfaces.Messaging;

public interface ICarCreatedMessageProducer
{
    Task ProduceAsync(CarCreatedMessage message, CancellationToken cancellationToken = default);
}
