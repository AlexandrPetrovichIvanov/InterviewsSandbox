namespace YandexSandbox.Bll.Messaging;

public interface IMessageProducer
{
    Task ProduceAsync(object message, CancellationToken cancellationToken = default);
}
