namespace YandexSandbox.Api.Messaging;

public class OutboxEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Topic { get; init; }
    public required object Message { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
