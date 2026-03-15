using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;
using YandexSandbox.Bll.Interfaces.Messaging;

namespace YandexSandbox.Tests.Integration.Rent;

public class RentE2eTests
{
    [Fact]
    public async Task FullRentOrderFlow_PlaceOrder_ConsumeMessage_OrderIsProcessed()
    {
        var consumerMock = new Mock<IRentOrderProcessingConsumer>();
        var consumed = false;

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(consumerMock.Object);
                });
            });

        var client = factory.CreateClient();

        var carRequest = new CreateCarApiRequest
        {
            Make = "Toyota", Model = "Camry", Year = 2025, Color = "White"
        };
        var carResponse = await client.PostAsJsonAsync("/api/cars", carRequest);
        var car = await carResponse.Content.ReadFromJsonAsync<CarApiResponse>();

        var orderResponse = await client.PostAsJsonAsync("/api/rentorders",
            new PlaceRentOrderApiRequest { CarId = car!.Id });
        orderResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await orderResponse.Content.ReadFromJsonAsync<RentOrderApiResponse>();
        order!.Processed.Should().BeFalse();

        consumerMock.Setup(c => c.ConsumeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                if (consumed) return null;
                consumed = true;
                return new RentOrderProcessedMessage
                {
                    OrderId = order.Id,
                    ProcessedAt = DateTime.UtcNow
                };
            });

        RentOrderApiResponse? processedOrder = null;
        for (var i = 0; i < 10; i++)
        {
            await Task.Delay(500);
            var checkResponse = await client.GetAsync($"/api/rentorders/{order.Id}");
            processedOrder = await checkResponse.Content.ReadFromJsonAsync<RentOrderApiResponse>();
            if (processedOrder!.Processed)
                break;
        }

        processedOrder!.Processed.Should().BeTrue();
    }
}
