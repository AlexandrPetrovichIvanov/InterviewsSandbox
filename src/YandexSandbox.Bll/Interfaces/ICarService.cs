using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Queries;

namespace YandexSandbox.Bll.Interfaces;

public interface ICarService
{
    Task<GetCarByIdQueryResponse?> GetByIdAsync(GetCarByIdQuery query, CancellationToken cancellationToken = default);
    Task<GetAllCarsQueryResponse> GetAllAsync(GetAllCarsQuery query, CancellationToken cancellationToken = default);
    Task<CreateCarCommandResponse> CreateAsync(CreateCarCommand command, CancellationToken cancellationToken = default);
}
