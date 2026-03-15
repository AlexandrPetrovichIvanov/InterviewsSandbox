using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;

namespace YandexSandbox.Tests.Integration.Rent;

public class RentE2eTests
{
    [Fact]
    public async Task FullRentOrderFlow_PlaceOrder_WaitForProcessing_OrderIsProcessed()
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
        order!.Processed.Should().BeFalse();

        RentOrderApiResponse? processedOrder = null;
        for (var i = 0; i < 20; i++)
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
