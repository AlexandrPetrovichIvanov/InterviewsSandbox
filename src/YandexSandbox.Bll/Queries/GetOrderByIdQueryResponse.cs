using YandexSandbox.Bll.CommonModels;

namespace YandexSandbox.Bll.Queries;

public class GetOrderByIdQueryResponse
{
    public required RentOrderModel Order { get; set; }
}
