using YandexSandbox.Bll.CommonModels;

namespace YandexSandbox.Bll.Queries;

public class GetCarByIdQueryResponse
{
    public required CarModel Car { get; set; }
}
