using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UnitConversion.Api.Tests.Integration;

public class ConversionApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ConversionApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("length", "meter", "foot", 1.0, 3.28084)]
    [InlineData("weight", "kilogram", "pound", 1.0, 2.20462)]
    [InlineData("temperature", "celsius", "fahrenheit", 20.0, 68.0)]
    public async Task Convert_ReturnsExpectedValue(string category, string fromUnit, string toUnit, double value, double expected)
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/conversion/convert", new
        {
            category,
            fromUnit,
            toUnit,
            value
        });

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        var convertedValue = payload.GetProperty("convertedValue").GetDouble();

        Assert.Equal(expected, convertedValue, 3);
    }

    [Theory]
    [InlineData("length", "m", "ft", 2.5, 8.2021)]
    [InlineData("mass", "kg", "lb", 1.0, 2.20462)]
    [InlineData("temperature", "C", "F", 20.0, 68.0)]
    public async Task Convert_AcceptsCommonUnitNamesAndSymbols(string category, string fromUnit, string toUnit, double value, double expected)
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/conversion/convert", new
        {
            category,
            fromUnit,
            toUnit,
            value
        });

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        var convertedValue = payload.GetProperty("convertedValue").GetDouble();

        Assert.Equal(expected, convertedValue, 3);
    }

    [Fact]
    public async Task Convert_WithUnsupportedUnit_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/conversion/convert", new
        {
            category = "length",
            fromUnit = "mile",
            toUnit = "unknown",
            value = 5
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
