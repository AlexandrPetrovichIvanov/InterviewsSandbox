using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;
using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Interfaces;
using YandexSandbox.Bll.Queries;

namespace YandexSandbox.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly IMapper _mapper;

    public CarsController(ICarService carService, IMapper mapper)
    {
        _carService = carService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CarApiResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _carService.GetAllAsync(new GetAllCarsQuery(), cancellationToken);
        return Ok(_mapper.Map<List<CarApiResponse>>(response.Cars));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarApiResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var response = await _carService.GetByIdAsync(new GetCarByIdQuery { Id = id }, cancellationToken);
        if (response is null)
            return NotFound();

        return Ok(_mapper.Map<CarApiResponse>(response.Car));
    }

    [HttpPost]
    public async Task<ActionResult<CarApiResponse>> Create(
        [FromBody] CreateCarApiRequest request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateCarCommand>(request);
        var response = await _carService.CreateAsync(command, cancellationToken);
        var apiResponse = _mapper.Map<CarApiResponse>(response.Car);
        return CreatedAtAction(nameof(GetById), new { id = apiResponse.Id }, apiResponse);
    }
}
