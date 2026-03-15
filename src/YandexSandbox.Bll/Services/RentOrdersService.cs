using YandexSandbox.Bll.Exceptions;
using YandexSandbox.Bll.Interfaces.Commands;
using YandexSandbox.Bll.Interfaces.Messaging.Messages;
using YandexSandbox.Bll.Interfaces.Messaging.Producers;
using YandexSandbox.Bll.Interfaces.Models;
using YandexSandbox.Bll.Interfaces.Queries;
using YandexSandbox.Bll.Interfaces.Repositories;
using YandexSandbox.Bll.Interfaces.Services;

namespace YandexSandbox.Bll.Services;

public class RentOrdersService : IRentOrdersService
{
    private readonly IRentOrderRepository _orderRepository;
    private readonly ICarRepository _carRepository;
    private readonly IMessageProducer<RentOrderPlacedMessage> _messageProducer;
    private readonly ILock _lock;

    public RentOrdersService(
        IRentOrderRepository orderRepository,
        ICarRepository carRepository,
        IMessageProducer<RentOrderPlacedMessage> messageProducer,
        ILock @lock)
    {
        _orderRepository = orderRepository;
        _carRepository = carRepository;
        _messageProducer = messageProducer;
        _lock = @lock;
    }

    public async Task<PlaceRentOrderCommandResponse> PlaceRentOrderAsync(PlaceRentOrderCommand command, CancellationToken cancellationToken = default)
    {
        await using var _ = await _lock.AcquireAsync($"car-{command.CarId}", cancellationToken);

        var car = await _carRepository.GetByIdAsync(command.CarId, cancellationToken);
        if (car is null)
            throw new CarNotFoundException(command.CarId);

        var activeOrder = await _orderRepository.GetActiveByCarIdAsync(command.CarId, cancellationToken);
        if (activeOrder is not null)
            throw new CarNotAvailableException(command.CarId);

        var model = new RentOrderModel { CarId = command.CarId };
        var created = await _orderRepository.CreateAsync(model, cancellationToken);

        await _messageProducer.ProduceAsync(new RentOrderPlacedMessage
        {
            OrderId = created.Id,
            CarId = created.CarId,
            CreatedAt = created.CreatedAt
        }, cancellationToken);

        return new PlaceRentOrderCommandResponse { Order = created };
    }

    public async Task<GetRentOrderByIdQueryResponse?> CheckRentOrderAsync(GetRentOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(query.Id, cancellationToken);
        return order is null ? null : new GetRentOrderByIdQueryResponse { Order = order };
    }

    public async Task<ProcessRentOrderCommandResponse> ProcessRentOrderAsync(ProcessRentOrderCommand command, CancellationToken cancellationToken = default)
    {
        await using var _ = await _lock.AcquireAsync($"rent-order-{command.OrderId}", cancellationToken);

        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order is null)
            throw new RentOrderNotFoundException(command.OrderId);

        var updated = new RentOrderModel
        {
            Id = order.Id,
            CarId = order.CarId,
            Processed = true,
            CreatedAt = order.CreatedAt
        };
        await _orderRepository.UpdateAsync(updated, cancellationToken);

        return new ProcessRentOrderCommandResponse { Order = updated };
    }
}
