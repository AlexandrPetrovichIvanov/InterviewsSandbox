using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;
using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Interfaces.Services;
using YandexSandbox.Bll.Queries;

namespace YandexSandbox.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentController : ControllerBase
{
    private readonly IRentService _rentService;
    private readonly IMapper _mapper;

    public RentController(IRentService rentService, IMapper mapper)
    {
        _rentService = rentService;
        _mapper = mapper;
    }

    [HttpPost("orders")]
    public async Task<ActionResult<RentOrderApiResponse>> PlaceOrder(
        [FromBody] PlaceOrderApiRequest request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<PlaceOrderCommand>(request);
        var response = await _rentService.PlaceOrderAsync(command, cancellationToken);
        var apiResponse = _mapper.Map<RentOrderApiResponse>(response.Order);
        return CreatedAtAction(nameof(CheckOrder), new { id = apiResponse.Id }, apiResponse);
    }

    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<RentOrderApiResponse>> CheckOrder(int id, CancellationToken cancellationToken)
    {
        var response = await _rentService.CheckOrderAsync(new GetOrderByIdQuery { Id = id }, cancellationToken);
        if (response is null)
            return NotFound();

        return Ok(_mapper.Map<RentOrderApiResponse>(response.Order));
    }
}
