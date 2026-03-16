using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;
using YandexSandbox.Bll.Interfaces.Messaging.Handlers;
using YandexSandbox.Bll.Interfaces.Messaging.Messages;

namespace YandexSandbox.Tests.Integration.Rent;

public class RentE2eTests
{
    [Fact]
    public async Task FullRentOrderFlow_PlaceOrder_ApproveOrder_OrderIsApproved()
    {
        var factory = new WebApplicationFactory<Program>();
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
        order!.Approved.Should().BeFalse();

        using (var scope = factory.Services.CreateScope())
        {
            var handler = scope.ServiceProvider.GetRequiredService<IRentOrderApprovedMessageHandler>();
            await handler.HandleAsync(new RentOrderApprovedMessage
            {
                OrderId = order.Id,
                ApprovedAt = DateTime.UtcNow
            });
        }

        var checkResponse = await client.GetAsync($"/api/rentorders/{order.Id}");
        var approvedOrder = await checkResponse.Content.ReadFromJsonAsync<RentOrderApiResponse>();
        approvedOrder!.Approved.Should().BeTrue();
    }
}
