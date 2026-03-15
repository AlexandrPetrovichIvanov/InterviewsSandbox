using YandexSandbox.Bll.Models;

namespace YandexSandbox.Bll.Queries;

public class GetRentOrderByIdQueryResponse
{
    public required RentOrderModel Order { get; set; }
}
