using YandexSandbox.Dal.Entities;

namespace YandexSandbox.Dal.Interfaces;

public interface ICarRepository
{
    Task<CarEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CarEntity> CreateAsync(CarEntity car, CancellationToken cancellationToken = default);
}
