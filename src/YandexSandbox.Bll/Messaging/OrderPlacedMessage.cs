namespace YandexSandbox.Bll.Messaging;

public class OrderPlacedMessage
{
    public required int OrderId { get; init; }
    public required int CarId { get; init; }
    public DateTime CreatedAt { get; init; }
}
