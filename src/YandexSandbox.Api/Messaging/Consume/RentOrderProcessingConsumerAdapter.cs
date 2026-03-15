using YandexSandbox.Bll.Interfaces.Messaging;

namespace YandexSandbox.Api.Messaging.Consume;

public class RentOrderProcessingConsumerAdapter : IRentOrderProcessingConsumer
{
    public Task<RentOrderProcessedMessage?> ConsumeAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<RentOrderProcessedMessage?>(null);
    }
}
