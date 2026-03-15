using Microsoft.AspNetCore.Mvc;
using YandexSandbox.Bll.Interfaces.Messaging.Handlers;
using YandexSandbox.Bll.Interfaces.Messaging.Messages;

namespace YandexSandbox.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly IRentOrderProcessedMessageHandler _handler;
    private readonly IWebHostEnvironment _env;

    public SystemController(IRentOrderProcessedMessageHandler handler, IWebHostEnvironment env)
    {
        _handler = handler;
        _env = env;
    }

    [HttpPost("produce-rent-order-processed")]
    public async Task<IActionResult> ProduceRentOrderProcessedMessage(
        [FromBody] RentOrderProcessedMessage message,
        CancellationToken cancellationToken)
    {
        if (!_env.IsDevelopment())
            return NotFound();

        await _handler.HandleAsync(message, cancellationToken);
        return Ok();
    }
}
