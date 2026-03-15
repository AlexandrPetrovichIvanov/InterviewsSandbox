using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Bll.Interfaces.Queries;

public class GetCarByIdQueryResponse
{
    public required CarModel Car { get; set; }
}
