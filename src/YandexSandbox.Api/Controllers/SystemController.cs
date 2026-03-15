using Microsoft.AspNetCore.Mvc;
using YandexSandbox.Bll.Interfaces.Messaging.Handlers;
using YandexSandbox.Bll.Interfaces.Messaging.Messages;

namespace YandexSandbox.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly IRentOrderProcessedMessageHandler _handler;

    public SystemController(IRentOrderProcessedMessageHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("produce-rent-order-processed")]
    public async Task<IActionResult> ProduceRentOrderProcessedMessage(
        [FromBody] RentOrderProcessedMessage message,
        CancellationToken cancellationToken)
    {
        await _handler.HandleAsync(message, cancellationToken);
        return Ok();
    }
}
