namespace YandexSandbox.Bll.Exceptions;

public class InvalidCarYearException : BllException
{
    public int Year { get; }
    public int MinYear { get; }
    public int MaxYear { get; }

    public InvalidCarYearException(int year, int minYear, int maxYear)
        : base()
    {
        Year = year;
        MinYear = minYear;
        MaxYear = maxYear;
    }
}
