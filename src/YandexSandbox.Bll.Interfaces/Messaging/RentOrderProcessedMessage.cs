namespace YandexSandbox.Bll.Interfaces.Messaging;

public class RentOrderProcessedMessage
{
    public required int OrderId { get; init; }
    public DateTime ProcessedAt { get; init; }
}
