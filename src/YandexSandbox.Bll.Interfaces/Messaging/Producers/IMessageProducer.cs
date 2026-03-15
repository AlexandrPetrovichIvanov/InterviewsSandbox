namespace YandexSandbox.Bll.Interfaces.Messaging.Producers;

public interface IMessageProducer<in TMessage>
{
    Task ProduceAsync(TMessage message, CancellationToken cancellationToken = default);
}
