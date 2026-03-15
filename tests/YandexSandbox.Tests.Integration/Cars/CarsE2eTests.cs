using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using YandexSandbox.Api.Requests;
using YandexSandbox.Api.Responses;

namespace YandexSandbox.Tests.Integration.Cars;

public class CarsE2eTests
{
    [Fact]
    public async Task CreateCar_GetById_GetAll()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/cars", new CreateCarApiRequest
        {
            Make = "Porsche", Model = "911", Year = 2025,
            Color = "Red", Mileage = 0, Vin = "WP0AB2A75FS123456"
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<CarApiResponse>();
        created!.Make.Should().Be("Porsche");
        created.Model.Should().Be("911");
        created.Year.Should().Be(2025);
        created.Id.Should().BeGreaterThan(0);

        var getByIdResponse = await client.GetAsync($"/api/cars/{created.Id}");
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getByIdResponse.Content.ReadFromJsonAsync<CarApiResponse>();
        fetched!.Id.Should().Be(created.Id);
        fetched.Make.Should().Be("Porsche");
        fetched.Vin.Should().Be("WP0AB2A75FS123456");

        var getAllResponse = await client.GetAsync("/api/cars");
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var all = await getAllResponse.Content.ReadFromJsonAsync<List<CarApiResponse>>();
        all.Should().Contain(c => c.Id == created.Id && c.Make == "Porsche");
    }
}
