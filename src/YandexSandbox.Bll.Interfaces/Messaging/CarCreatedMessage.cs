namespace YandexSandbox.Bll.Interfaces.Messaging;

public class CarCreatedMessage
{
    public required int Id { get; init; }
    public required string Make { get; init; }
    public required string Model { get; init; }
    public required int Year { get; init; }
    public DateTime CreatedAt { get; init; }
}
