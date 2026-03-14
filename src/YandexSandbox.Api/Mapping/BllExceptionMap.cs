using YandexSandbox.Bll.Exceptions;

namespace YandexSandbox.Api.Mapping;

public static class BllExceptionMap
{
    private static readonly Dictionary<Type, int> HttpStatusCodes = new()
    {
        { typeof(InvalidCarYearException), StatusCodes.Status422UnprocessableEntity },
        { typeof(InvalidVinException), StatusCodes.Status422UnprocessableEntity }
    };

    private static readonly Dictionary<Type, Func<BllException, string>> MessageFormatters = new()
    {
        {
            typeof(InvalidCarYearException), ex =>
            {
                var e = (InvalidCarYearException)ex;
                return $"Year {e.Year} is out of valid range [{e.MinYear}–{e.MaxYear}].";
            }
        },
        {
            typeof(InvalidVinException), ex =>
            {
                var e = (InvalidVinException)ex;
                return $"VIN must be exactly 17 characters, got {e.Vin.Length}.";
            }
        }
    };

    public static bool TryMap(BllException ex, out int statusCode, out string message)
    {
        var type = ex.GetType();
        if (HttpStatusCodes.TryGetValue(type, out statusCode) &&
            MessageFormatters.TryGetValue(type, out var formatter))
        {
            message = formatter(ex);
            return true;
        }

        statusCode = default;
        message = string.Empty;
        return false;
    }
}
