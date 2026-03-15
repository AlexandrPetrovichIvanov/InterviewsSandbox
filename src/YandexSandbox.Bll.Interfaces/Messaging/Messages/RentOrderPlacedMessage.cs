namespace YandexSandbox.Bll.Interfaces.Messaging.Messages;

public class RentOrderPlacedMessage
{
    public required int OrderId { get; init; }
    public required int CarId { get; init; }
    public DateTime CreatedAt { get; init; }
}
