using YandexSandbox.Bll.CommonModels;

namespace YandexSandbox.Bll.Commands;

public class ProcessOrderCommandResponse
{
    public required RentOrderModel Order { get; set; }
}
