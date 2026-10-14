using UnitConversion.Application.Models;

namespace UnitConversion.Application.Contracts;

public interface IUnitConversionService
{
    IReadOnlyCollection<string> GetSupportedCategories();
    IReadOnlyCollection<string> GetSupportedUnits(string category);
    ConversionResponse Convert(ConversionRequest request);
}
