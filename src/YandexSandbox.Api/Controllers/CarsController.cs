using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YandexSandbox.Api.Models;
using YandexSandbox.Bll.Interfaces;
using YandexSandbox.Bll.Models;

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
        var cars = await _carService.GetAllAsync(cancellationToken);
        return Ok(_mapper.Map<List<CarApiResponse>>(cars));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarApiResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var car = await _carService.GetByIdAsync(id, cancellationToken);
        if (car is null)
            return NotFound();

        return Ok(_mapper.Map<CarApiResponse>(car));
    }

    [HttpPost]
    public async Task<ActionResult<CarApiResponse>> Create(
        [FromBody] CreateCarApiRequest request,
        CancellationToken cancellationToken)
    {
        var bllRequest = _mapper.Map<CreateCarRequest>(request);
        var created = await _carService.CreateAsync(bllRequest, cancellationToken);
        var response = _mapper.Map<CarApiResponse>(created);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}
