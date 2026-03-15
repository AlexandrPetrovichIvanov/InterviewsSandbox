using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Bll.Interfaces.Commands;

public class PlaceRentOrderCommandResponse
{
    public required RentOrderModel Order { get; set; }
}
