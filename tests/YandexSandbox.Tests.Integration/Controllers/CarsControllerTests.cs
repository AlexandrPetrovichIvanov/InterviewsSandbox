using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using YandexSandbox.Api.Models;

namespace YandexSandbox.Tests.Integration.Controllers;

public class CarsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CarsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/api/cars");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cars = await response.Content.ReadFromJsonAsync<List<CarApiResponse>>();
        cars.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreated()
    {
        var request = new CreateCarApiRequest
        {
            Make = "Tesla", Model = "Model 3", Year = 2025,
            Color = "White", Mileage = 0, Vin = "5YJ3E1EA1PF000001"
        };

        var response = await _client.PostAsJsonAsync("/api/cars", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<CarApiResponse>();
        created.Should().NotBeNull();
        created!.Make.Should().Be("Tesla");
        created.Model.Should().Be("Model 3");
        created.Year.Should().Be(2025);
        created.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_AfterCreate_ReturnsCar()
    {
        var request = new CreateCarApiRequest
        {
            Make = "Porsche", Model = "911", Year = 2024, Color = "Red"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/cars", request);
        var created = await createResponse.Content.ReadFromJsonAsync<CarApiResponse>();

        var response = await _client.GetAsync($"/api/cars/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var car = await response.Content.ReadFromJsonAsync<CarApiResponse>();
        car!.Make.Should().Be("Porsche");
        car.Model.Should().Be("911");
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/cars/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithEmptyMake_Returns400()
    {
        var request = new CreateCarApiRequest
        {
            Make = "", Model = "Civic", Year = 2024, Color = "Blue"
        };

        var response = await _client.PostAsJsonAsync("/api/cars", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Make");
    }

    [Fact]
    public async Task Create_WithNegativeMileage_Returns400()
    {
        var request = new CreateCarApiRequest
        {
            Make = "BMW", Model = "X5", Year = 2024, Color = "Black", Mileage = -100
        };

        var response = await _client.PostAsJsonAsync("/api/cars", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Mileage");
    }

    [Fact]
    public async Task Create_WithMultipleErrors_ReturnsAllErrors()
    {
        var request = new CreateCarApiRequest
        {
            Make = "", Model = "", Color = "", Mileage = -1
        };

        var response = await _client.PostAsJsonAsync("/api/cars", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Errors.Should().ContainKey("Make");
        problem.Errors.Should().ContainKey("Model");
        problem.Errors.Should().ContainKey("Color");
        problem.Errors.Should().ContainKey("Mileage");
    }
}
