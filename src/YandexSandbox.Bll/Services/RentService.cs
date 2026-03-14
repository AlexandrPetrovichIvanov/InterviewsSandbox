using YandexSandbox.Bll.Commands;
using YandexSandbox.Bll.CommonModels;
using YandexSandbox.Bll.Exceptions;
using YandexSandbox.Bll.Interfaces.Messaging;
using YandexSandbox.Bll.Interfaces.Repositories;
using YandexSandbox.Bll.Interfaces.Services;
using YandexSandbox.Bll.Messaging;
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

    public async Task<PlaceOrderCommandResponse> PlaceOrderAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default)
    {
        var car = await _carRepository.GetByIdAsync(command.CarId, cancellationToken);
        if (car is null)
            throw new CarNotFoundException(command.CarId);

        var activeOrder = await _orderRepository.GetActiveByCarIdAsync(command.CarId, cancellationToken);
        if (activeOrder is not null)
            throw new CarNotAvailableException(command.CarId);

        var model = new RentOrderModel { CarId = command.CarId };
        var created = await _orderRepository.CreateAsync(model, cancellationToken);

        await _messageProducer.ProduceAsync(new OrderPlacedMessage
        {
            OrderId = created.Id,
            CarId = created.CarId,
            CreatedAt = created.CreatedAt
        }, cancellationToken);

        return new PlaceOrderCommandResponse { Order = created };
    }

    public async Task<GetOrderByIdQueryResponse?> CheckOrderAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(query.Id, cancellationToken);
        return order is null ? null : new GetOrderByIdQueryResponse { Order = order };
    }

    public async Task<ProcessOrderCommandResponse> ProcessOrderAsync(ProcessOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(command.OrderId, cancellationToken);
        if (order is null)
            throw new OrderNotFoundException(command.OrderId);

        order.Processed = true;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return new ProcessOrderCommandResponse { Order = order };
    }
}
