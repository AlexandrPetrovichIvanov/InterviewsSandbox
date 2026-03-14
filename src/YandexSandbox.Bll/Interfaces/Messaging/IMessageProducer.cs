namespace YandexSandbox.Bll.Interfaces.Messaging;

public interface IMessageProducer
{
    Task ProduceAsync(object message, CancellationToken cancellationToken = default);
}
