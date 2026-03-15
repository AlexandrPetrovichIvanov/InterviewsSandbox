using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Bll.Interfaces.Repositories;

public interface IRentOrderRepository
{
    Task<RentOrderModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RentOrderModel?> GetActiveByCarIdAsync(int carId, CancellationToken cancellationToken = default);
    Task<RentOrderModel> CreateAsync(RentOrderModel order, CancellationToken cancellationToken = default);
    Task UpdateAsync(RentOrderModel order, CancellationToken cancellationToken = default);
}
