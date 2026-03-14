using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Queries;

namespace YandexSandbox.Bll.Interfaces.Services;

public interface IRentService
{
    Task<PlaceOrderCommandResponse> PlaceOrderAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default);
    Task<GetOrderByIdQueryResponse?> CheckOrderAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default);
    Task<ProcessOrderCommandResponse> ProcessOrderAsync(ProcessOrderCommand command, CancellationToken cancellationToken = default);
}
