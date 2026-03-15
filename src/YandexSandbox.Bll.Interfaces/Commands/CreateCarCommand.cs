namespace YandexSandbox.Bll.Interfaces.Commands;

public class CreateCarCommand
{
    public required string Make { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public required string Color { get; set; }
    public int Mileage { get; set; }
    public string? Vin { get; set; }
}
