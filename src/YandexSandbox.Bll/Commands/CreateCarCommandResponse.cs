using YandexSandbox.Bll.Models;

namespace YandexSandbox.Bll.Commands;

public class CreateCarCommandResponse
{
    public required CarModel Car { get; set; }
}
