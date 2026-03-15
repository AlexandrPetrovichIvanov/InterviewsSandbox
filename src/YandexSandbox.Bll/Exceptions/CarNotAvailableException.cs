namespace YandexSandbox.Bll.Exceptions;

public class CarNotAvailableException : BllException
{
    public int CarId { get; }

    public CarNotAvailableException(int carId) : base()
    {
        CarId = carId;
    }
}
