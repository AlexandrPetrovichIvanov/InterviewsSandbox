using YandexSandbox.Bll.Interfaces.Models;

namespace YandexSandbox.Bll.Interfaces.Repositories;

public interface ICarRepository
{
    Task<CarModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CarModel> CreateAsync(CarModel car, CancellationToken cancellationToken = default);
}
