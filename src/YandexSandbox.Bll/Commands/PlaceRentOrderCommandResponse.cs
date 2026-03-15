using YandexSandbox.Bll.Models;

namespace YandexSandbox.Bll.Commands;

public class PlaceRentOrderCommandResponse
{
    public required RentOrderModel Order { get; set; }
}
