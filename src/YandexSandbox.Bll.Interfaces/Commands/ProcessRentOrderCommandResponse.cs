using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Bll.Interfaces.Commands;

public class ProcessRentOrderCommandResponse
{
    public required RentOrderModel Order { get; set; }
}
