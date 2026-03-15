using YandexSandbox.Bll.Interfaces.Commands;
using YandexSandbox.Bll.Interfaces.Queries;

namespace YandexSandbox.Bll.Interfaces.Services;

public interface ICarsService
{
    Task<GetCarByIdQueryResponse?> GetByIdAsync(GetCarByIdQuery query, CancellationToken cancellationToken = default);
    Task<GetAllCarsQueryResponse> GetAllAsync(GetAllCarsQuery query, CancellationToken cancellationToken = default);
    Task<CreateCarCommandResponse> CreateAsync(CreateCarCommand command, CancellationToken cancellationToken = default);
}
