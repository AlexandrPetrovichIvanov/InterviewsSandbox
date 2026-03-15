using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Bll.Interfaces.Queries;

public class GetAllCarsQueryResponse
{
    public required IReadOnlyList<CarModel> Cars { get; set; }
}
