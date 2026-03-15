using YandexSandbox.Bll.Models;

namespace YandexSandbox.Bll.Commands;

public class PlaceOrderCommandResponse
{
    public required RentOrderModel Order { get; set; }
}
