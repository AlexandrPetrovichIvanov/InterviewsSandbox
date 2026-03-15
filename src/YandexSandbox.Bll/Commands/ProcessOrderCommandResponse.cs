using YandexSandbox.Bll.Models;

namespace YandexSandbox.Bll.Commands;

public class ProcessOrderCommandResponse
{
    public required RentOrderModel Order { get; set; }
}
