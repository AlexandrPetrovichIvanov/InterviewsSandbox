using YandexSandbox.Bll.CommonModels;

namespace YandexSandbox.Bll.Queries;

public class GetAllCarsQueryResponse
{
    public required IReadOnlyList<CarModel> Cars { get; set; }
}
