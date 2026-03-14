namespace YandexSandbox.Api.Requests;

public class CreateCarApiRequest
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public string? Color { get; set; }
    public int Mileage { get; set; }
    public string? Vin { get; set; }
}
