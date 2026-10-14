namespace UnitConversion.Domain.Models;

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
