namespace YandexSandbox.Bll.Exceptions;

public class RentOrderNotFoundException : BllException
{
    public int OrderId { get; }

    public RentOrderNotFoundException(int orderId) : base()
    {
        OrderId = orderId;
    }
}
