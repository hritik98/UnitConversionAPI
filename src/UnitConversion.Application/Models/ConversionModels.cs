namespace UnitConversion.Application.Models;

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
