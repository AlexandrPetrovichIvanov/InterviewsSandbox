using YandexSandbox.Bll.Models;

namespace YandexSandbox.Bll.Interfaces;

public interface ICarService
{
    Task<CarDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CarDto> CreateAsync(CreateCarRequest request, CancellationToken cancellationToken = default);
}
