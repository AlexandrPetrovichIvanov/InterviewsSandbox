using YandexSandbox.Bll.Interfaces.Commands;
using YandexSandbox.Bll.Interfaces.Queries;

namespace YandexSandbox.Bll.Interfaces.Services;

public interface IRentOrdersService
{
    Task<PlaceRentOrderCommandResponse> PlaceRentOrderAsync(PlaceRentOrderCommand command, CancellationToken cancellationToken = default);
    Task<GetRentOrderByIdQueryResponse?> CheckRentOrderAsync(GetRentOrderByIdQuery query, CancellationToken cancellationToken = default);
    Task<ApproveRentOrderCommandResponse> ApproveRentOrderAsync(ApproveRentOrderCommand command, CancellationToken cancellationToken = default);
}
