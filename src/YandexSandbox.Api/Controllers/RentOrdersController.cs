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
public class RentOrdersController : ControllerBase
{
    private readonly IRentService _rentService;
    private readonly IMapper _mapper;

    public RentOrdersController(IRentService rentService, IMapper mapper)
    {
        _rentService = rentService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<RentOrderApiResponse>> PlaceRentOrder(
        [FromBody] PlaceRentOrderApiRequest request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<PlaceRentOrderCommand>(request);
        var response = await _rentService.PlaceRentOrderAsync(command, cancellationToken);
        var apiResponse = _mapper.Map<RentOrderApiResponse>(response.Order);
        return CreatedAtAction(nameof(CheckRentOrder), new { id = apiResponse.Id }, apiResponse);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RentOrderApiResponse>> CheckRentOrder(int id, CancellationToken cancellationToken)
    {
        var response = await _rentService.CheckRentOrderAsync(new GetRentOrderByIdQuery { Id = id }, cancellationToken);
        if (response is null)
            return NotFound();

        return Ok(_mapper.Map<RentOrderApiResponse>(response.Order));
    }
}
