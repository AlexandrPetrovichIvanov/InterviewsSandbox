using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Bll.Interfaces.Commands;

public class ApproveRentOrderCommandResponse
{
    public required RentOrderModel Order { get; set; }
}
