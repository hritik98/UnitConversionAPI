using UnitConversion.Application.Contracts;
using UnitConversion.Application.Models;
using UnitConversion.Domain.Models;

namespace UnitConversion.Application.Services;

public sealed class UnitConversionService : IUnitConversionService
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, UnitDefinition>> Catalog =
        BuildCatalog();

    public IReadOnlyCollection<string> GetSupportedCategories() =>
        Catalog.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase).ToArray();

    public IReadOnlyCollection<string> GetSupportedUnits(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("Category is required.", nameof(category));
        }

        var normalizedCategory = category.Trim();

        return Catalog.TryGetValue(normalizedCategory, out var units)
            ? units.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase).ToArray()
            : throw new ArgumentException($"Unsupported conversion category '{category}'.", nameof(category));
    }

    public ConversionResponse Convert(ConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = request.Category?.Trim();
        var fromUnit = request.FromUnit?.Trim();
        var toUnit = request.ToUnit?.Trim();

        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("A conversion category is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(fromUnit))
        {
            throw new ArgumentException("The source unit is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(toUnit))
        {
            throw new ArgumentException("The target unit is required.", nameof(request));
        }

        if (double.IsNaN(request.Value) || double.IsInfinity(request.Value))
        {
            throw new ArgumentException("Value must be a finite number.", nameof(request));
        }

        var normalizedCategory = NormalizeCategory(category);

        if (!Catalog.TryGetValue(normalizedCategory, out var availableUnits))
        {
            throw new ArgumentException($"Unsupported conversion category '{category}'.", nameof(request));
        }

        if (!TryResolveUnit(availableUnits, fromUnit, out var sourceUnit))
        {
            throw new ArgumentException($"Unsupported unit '{fromUnit}' for category '{normalizedCategory}'.", nameof(request));
        }

        if (!TryResolveUnit(availableUnits, toUnit, out var targetUnit))
        {
            throw new ArgumentException($"Unsupported unit '{toUnit}' for category '{normalizedCategory}'.", nameof(request));
        }

        var baseValue = sourceUnit.ToBase(request.Value);
        var convertedValue = targetUnit.FromBase(baseValue);

        return new ConversionResponse
        {
            Category = normalizedCategory,
            FromUnit = sourceUnit.Name,
            ToUnit = targetUnit.Name,
            Value = request.Value,
            ConvertedValue = convertedValue
        };
    }

    private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, UnitDefinition>> BuildCatalog()
    {
        var catalog = new Dictionary<string, IReadOnlyDictionary<string, UnitDefinition>>(StringComparer.OrdinalIgnoreCase)
        {
            ["length"] = new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase)
            {
                [NormalizeUnitName("meter")] = new UnitDefinition("meter", "m", value => value, value => value, ["meters", "metre", "metres"]),
                [NormalizeUnitName("kilometer")] = new UnitDefinition("kilometer", "km", value => value * 1000, value => value / 1000, ["kilometres", "kilometers"]),
                [NormalizeUnitName("centimeter")] = new UnitDefinition("centimeter", "cm", value => value / 100, value => value * 100, ["centimetre", "centimetres", "centimeters"]),
                [NormalizeUnitName("foot")] = new UnitDefinition("foot", "ft", value => value * 0.3048, value => value / 0.3048, ["feet"]),
                [NormalizeUnitName("inch")] = new UnitDefinition("inch", "in", value => value * 0.0254, value => value / 0.0254, ["inches"]),
                [NormalizeUnitName("mile")] = new UnitDefinition("mile", "mi", value => value * 1609.344, value => value / 1609.344, ["miles"])
            },
            ["temperature"] = new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase)
            {
                [NormalizeUnitName("celsius")] = new UnitDefinition("celsius", "C", value => value, value => value, ["c", "degrees celsius", "degree celsius"]),
                [NormalizeUnitName("fahrenheit")] = new UnitDefinition("fahrenheit", "F", value => (value - 32d) * 5d / 9d, value => (value * 9d / 5d) + 32d, ["f", "degrees fahrenheit", "degree fahrenheit"]),
                [NormalizeUnitName("kelvin")] = new UnitDefinition("kelvin", "K", value => value - 273.15d, value => value + 273.15d, ["k", "degrees kelvin", "degree kelvin"])
            },
            ["weight"] = new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase)
            {
                [NormalizeUnitName("kilogram")] = new UnitDefinition("kilogram", "kg", value => value, value => value, ["kilograms", "kilogramme", "kilogrammes"]),
                [NormalizeUnitName("gram")] = new UnitDefinition("gram", "g", value => value / 1000d, value => value * 1000d, ["grams"]),
                [NormalizeUnitName("pound")] = new UnitDefinition("pound", "lb", value => value * 0.45359237d, value => value / 0.45359237d, ["pounds", "lbs"]),
                [NormalizeUnitName("ounce")] = new UnitDefinition("ounce", "oz", value => value * 0.028349523125d, value => value / 0.028349523125d, ["ounces"])
            },
            ["mass"] = new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase)
            {
                [NormalizeUnitName("kilogram")] = new UnitDefinition("kilogram", "kg", value => value, value => value, ["kilograms", "kilogramme", "kilogrammes"]),
                [NormalizeUnitName("gram")] = new UnitDefinition("gram", "g", value => value / 1000d, value => value * 1000d, ["grams"]),
                [NormalizeUnitName("pound")] = new UnitDefinition("pound", "lb", value => value * 0.45359237d, value => value / 0.45359237d, ["pounds", "lbs"]),
                [NormalizeUnitName("ounce")] = new UnitDefinition("ounce", "oz", value => value * 0.028349523125d, value => value / 0.028349523125d, ["ounces"])
            }
        };

        return catalog;
    }

    private static bool TryResolveUnit(IReadOnlyDictionary<string, UnitDefinition> availableUnits, string requestedUnit, out UnitDefinition unit)
    {
        var normalized = NormalizeUnitName(requestedUnit);

        if (availableUnits.TryGetValue(normalized, out var resolvedUnit))
        {
            unit = resolvedUnit;
            return true;
        }

        foreach (var candidate in availableUnits.Values)
        {
            if (candidate.LookupValues.Any(alias => NormalizeUnitName(alias).Equals(normalized, StringComparison.OrdinalIgnoreCase)))
            {
                unit = candidate;
                return true;
            }
        }

        unit = null!;
        return false;
    }

    private static string NormalizeCategory(string category)
    {
        var normalized = category.Trim();

        return normalized.Equals("mass", StringComparison.OrdinalIgnoreCase)
            ? "mass"
            : normalized.Equals("weight", StringComparison.OrdinalIgnoreCase)
                ? "weight"
                : normalized;
    }

    private static string NormalizeUnitName(string unitName)
    {
        var normalized = unitName.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Unit name cannot be empty.", nameof(unitName));
        }

        return normalized.Trim('°').Replace("_", string.Empty).Replace("-", string.Empty).Trim();
    }
}
