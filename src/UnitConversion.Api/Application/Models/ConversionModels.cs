namespace UnitConversion.Api.Application.Models;

public sealed record ConversionRequest
{
    public string Category { get; init; } = string.Empty;
    public string FromUnit { get; init; } = string.Empty;
    public string ToUnit { get; init; } = string.Empty;
    public double Value { get; init; }
}

public sealed record ConversionResponse
{
    public string Category { get; init; } = string.Empty;
    public string FromUnit { get; init; } = string.Empty;
    public string ToUnit { get; init; } = string.Empty;
    public double Value { get; init; }
    public double ConvertedValue { get; init; }
}

public sealed record UnitDefinition(
    string Name,
    string Symbol,
    Func<double, double> ToBase,
    Func<double, double> FromBase,
    IReadOnlyCollection<string>? Aliases = null)
{
    public IEnumerable<string> LookupValues =>
        new[] { Name, Symbol }.Concat(Aliases ?? Array.Empty<string>());
}
