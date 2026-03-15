using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Queries;

namespace YandexSandbox.Bll.Interfaces.Services;

public interface IRentOrdersService
{
    Task<PlaceRentOrderCommandResponse> PlaceRentOrderAsync(PlaceRentOrderCommand command, CancellationToken cancellationToken = default);
    Task<GetRentOrderByIdQueryResponse?> CheckRentOrderAsync(GetRentOrderByIdQuery query, CancellationToken cancellationToken = default);
    Task<ProcessRentOrderCommandResponse> ProcessRentOrderAsync(ProcessRentOrderCommand command, CancellationToken cancellationToken = default);
}
