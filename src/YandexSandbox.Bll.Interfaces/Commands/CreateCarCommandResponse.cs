using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Bll.Interfaces.Commands;

public class CreateCarCommandResponse
{
    public required CarModel Car { get; set; }
}
