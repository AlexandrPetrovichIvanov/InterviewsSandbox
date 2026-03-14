namespace YandexSandbox.Bll.Exceptions;

public class InvalidVinException : BllException
{
    public string Vin { get; }

    public InvalidVinException(string vin)
        : base()
    {
        Vin = vin;
    }
}
