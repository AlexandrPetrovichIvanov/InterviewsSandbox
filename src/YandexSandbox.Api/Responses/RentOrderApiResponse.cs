namespace YandexSandbox.Api.Responses;

public class RentOrderApiResponse
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public bool Approved { get; set; }
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; }
}
