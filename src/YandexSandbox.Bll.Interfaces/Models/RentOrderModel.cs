namespace YandexSandbox.Bll.Interfaces.Models;

public class RentOrderModel
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public bool Approved { get; set; }
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; }
}
