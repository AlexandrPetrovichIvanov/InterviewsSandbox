using YandexSandbox.Bll.Interfaces.Messaging;

namespace YandexSandbox.Api.Messaging;

public class InMemoryMessageConsumer : IMessageConsumer
{
    private readonly InMemoryMessageBus _bus;

    public InMemoryMessageConsumer(InMemoryMessageBus bus)
    {
        _bus = bus;
    }

    public Task<object?> ConsumeAsync(string topic, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_bus.Consume(topic));
    }
}
