namespace YandexSandbox.Bll.Interfaces.Messaging.Messages;

public class RentOrderApprovedMessage
{
    public required int OrderId { get; init; }
    public DateTime ApprovedAt { get; init; }
}
