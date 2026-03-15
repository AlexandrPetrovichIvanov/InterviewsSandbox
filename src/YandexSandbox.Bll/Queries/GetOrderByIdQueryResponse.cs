using YandexSandbox.Bll.Models;

namespace YandexSandbox.Bll.Queries;

public class GetOrderByIdQueryResponse
{
    public required RentOrderModel Order { get; set; }
}
