using YandexSandbox.Dal.Models;

namespace YandexSandbox.Dal.Interfaces;

public interface ICarRepository
{
    Task<Car?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Car>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Car> CreateAsync(Car car, CancellationToken cancellationToken = default);
}
