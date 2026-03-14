using YandexSandbox.Bll.CommonModels;

namespace YandexSandbox.Bll.Commands;

public class PlaceOrderCommandResponse
{
    public required RentOrderModel Order { get; set; }
}
