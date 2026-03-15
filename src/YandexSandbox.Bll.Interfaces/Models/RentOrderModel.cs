namespace YandexSandbox.Bll.Interfaces.Models;

public class RentOrderModel
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public bool Processed { get; set; }
    public DateTime CreatedAt { get; set; }
}
