namespace YandexSandbox.Bll.Exceptions;

public class CarNotFoundException : BllException
{
    public int CarId { get; }

    public CarNotFoundException(int carId) : base()
    {
        CarId = carId;
    }
}
