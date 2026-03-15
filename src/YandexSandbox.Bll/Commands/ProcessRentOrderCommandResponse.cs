using YandexSandbox.Bll.Models;

namespace YandexSandbox.Bll.Commands;

public class ProcessRentOrderCommandResponse
{
    public required RentOrderModel Order { get; set; }
}
