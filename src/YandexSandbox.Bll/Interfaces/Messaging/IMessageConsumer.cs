namespace YandexSandbox.Bll.Interfaces.Messaging;

public interface IMessageConsumer
{
    Task<object?> ConsumeAsync(string topic, CancellationToken cancellationToken = default);
}
