using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.Exceptions;
using YandexSandbox.Bll.Interfaces.Messaging;
using YandexSandbox.Bll.Interfaces.Repositories;
using YandexSandbox.Bll.Interfaces.Services;
using YandexSandbox.Bll.Messaging;
using YandexSandbox.Bll.Models;
using YandexSandbox.Bll.Queries;

namespace YandexSandbox.Bll.Services;

public class RentService : IRentService
{
    private readonly IRentOrderRepository _orderRepository;
    private readonly ICarRepository _carRepository;
    private readonly IMessageProducer _messageProducer;

    public RentService(
        IRentOrderRepository orderRepository,
        ICarRepository carRepository,
        IMessageProducer messageProducer)
    {
        _orderRepository = orderRepository;
        _carRepository = carRepository;
        _messageProducer = messageProducer;
    }

    public async Task<PlaceRentOrderCommandResponse> PlaceRentOrderAsync(PlaceRentOrderCommand command, CancellationToken cancellationToken = default)
    {
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
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order is null)
            throw new RentOrderNotFoundException(command.OrderId);

        order.Processed = true;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new ProcessRentOrderCommandResponse { Order = order };
    }
}
