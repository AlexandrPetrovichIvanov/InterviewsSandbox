using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using YandexSandbox.Api.Messaging;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;
using YandexSandbox.Bll.Interfaces.Messaging.Messages;

namespace YandexSandbox.Tests.Integration.Messaging;

public class OutboxTests
{
    [Fact]
    public async Task CreateCar_ProducesMessageToOutbox()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IHostedService>();
                });
            });

        var client = factory.CreateClient();
        var outbox = factory.Services.GetRequiredService<InMemoryOutboxStorage>();

        var request = new CreateCarApiRequest
        {
            Make = "Mercedes", Model = "S-Class", Year = 2025, Color = "Black"
        };

        await client.PostAsJsonAsync("/api/cars", request);

        outbox.TryTake(out var entry).Should().BeTrue();
        entry!.Topic.Should().Be("car-created");
        var message = entry.Message.Should().BeOfType<CarCreatedMessage>().Subject;
        message.Make.Should().Be("Mercedes");
        message.Model.Should().Be("S-Class");
        message.Year.Should().Be(2025);
        message.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task PlaceRentOrder_ProducesRentOrderPlacedMessageToOutbox()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IHostedService>();
                });
            });

        var client = factory.CreateClient();
        var outbox = factory.Services.GetRequiredService<InMemoryOutboxStorage>();

        var carRequest = new CreateCarApiRequest
        {
            Make = "BMW", Model = "X5", Year = 2025, Color = "Black"
        };
        var carResponse = await client.PostAsJsonAsync("/api/cars", carRequest);
        var car = await carResponse.Content.ReadFromJsonAsync<CarApiResponse>();

        // Drain the car-created message
        outbox.TryTake(out _);

        await client.PostAsJsonAsync("/api/rentorders",
            new PlaceRentOrderApiRequest { CarId = car!.Id });

        outbox.TryTake(out var entry).Should().BeTrue();
        entry!.Topic.Should().Be("rent-order-placed");
        var message = entry.Message.Should().BeOfType<RentOrderPlacedMessage>().Subject;
        message.CarId.Should().Be(car.Id);
        message.OrderId.Should().BeGreaterThan(0);
    }
}
