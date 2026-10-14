using UnitConversion.Api.Application.Models;

namespace UnitConversion.Api.Application.Contracts;

public interface IUnitConversionService
{
    IReadOnlyCollection<string> GetSupportedCategories();
    IReadOnlyCollection<string> GetSupportedUnits(string category);
    ConversionResponse Convert(ConversionRequest request);
}
