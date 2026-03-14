namespace YandexSandbox.Bll.Exceptions;

public class OrderNotFoundException : BllException
{
    public int OrderId { get; }

    public OrderNotFoundException(int orderId) : base()
    {
        OrderId = orderId;
    }
}
