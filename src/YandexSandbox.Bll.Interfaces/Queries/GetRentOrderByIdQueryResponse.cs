using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Bll.Interfaces.Queries;

public class GetRentOrderByIdQueryResponse
{
    public required RentOrderModel Order { get; set; }
}
