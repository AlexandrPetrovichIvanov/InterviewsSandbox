namespace YandexSandbox.Bll.CommonModels;

public class CarModel
{
    public int Id { get; set; }
    public required string Make { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public required string Color { get; set; }
    public int Mileage { get; set; }
    public string? Vin { get; set; }
    public DateTime CreatedAt { get; set; }
}
