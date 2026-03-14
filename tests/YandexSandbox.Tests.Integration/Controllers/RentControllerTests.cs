using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;

namespace YandexSandbox.Tests.Integration.Controllers;

public class RentControllerTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RentControllerTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IHostedService>();
                });
            });
        _client = _factory.CreateClient();
    }

    private async Task<CarApiResponse> CreateCarAsync()
    {
        var request = new CreateCarApiRequest
        {
            Make = "Toyota", Model = "Camry", Year = 2025, Color = "White"
        };
        var response = await _client.PostAsJsonAsync("/api/cars", request);
        return (await response.Content.ReadFromJsonAsync<CarApiResponse>())!;
    }

    [Fact]
    public async Task PlaceOrder_WithValidCarId_ReturnsCreated()
    {
        var car = await CreateCarAsync();

        var response = await _client.PostAsJsonAsync("/api/rent/orders",
            new PlaceOrderApiRequest { CarId = car.Id });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<RentOrderApiResponse>();
        order!.CarId.Should().Be(car.Id);
        order.Processed.Should().BeFalse();
        order.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task PlaceOrder_WhenCarNotFound_Returns422()
    {
        var response = await _client.PostAsJsonAsync("/api/rent/orders",
            new PlaceOrderApiRequest { CarId = 99999 });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Detail.Should().Contain("99999");
    }

    [Fact]
    public async Task PlaceOrder_WhenCarAlreadyRented_Returns422()
    {
        var car = await CreateCarAsync();
        await _client.PostAsJsonAsync("/api/rent/orders",
            new PlaceOrderApiRequest { CarId = car.Id });

        var response = await _client.PostAsJsonAsync("/api/rent/orders",
            new PlaceOrderApiRequest { CarId = car.Id });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Detail.Should().Contain("not available");
    }

    [Fact]
    public async Task PlaceOrder_WithInvalidCarId_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/rent/orders",
            new PlaceOrderApiRequest { CarId = 0 });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CheckOrder_WhenOrderExists_ReturnsOrder()
    {
        var car = await CreateCarAsync();
        var placeResponse = await _client.PostAsJsonAsync("/api/rent/orders",
            new PlaceOrderApiRequest { CarId = car.Id });
        var placed = (await placeResponse.Content.ReadFromJsonAsync<RentOrderApiResponse>())!;

        var response = await _client.GetAsync($"/api/rent/orders/{placed.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<RentOrderApiResponse>();
        order!.Id.Should().Be(placed.Id);
        order.CarId.Should().Be(car.Id);
    }

    [Fact]
    public async Task CheckOrder_WhenOrderNotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/rent/orders/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
