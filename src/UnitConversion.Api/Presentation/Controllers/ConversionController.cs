using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.Application.Contracts;
using UnitConversion.Api.Application.Models;

namespace UnitConversion.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ConversionController : ControllerBase
{
    private readonly IUnitConversionService _service;

    public ConversionController(IUnitConversionService service)
    {
        _service = service;
    }

    [HttpPost("convert")]
    public ActionResult<ConversionResponse> Convert([FromBody] ConversionRequest request)
    {
        try
        {
            var result = _service.Convert(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("categories")]
    public ActionResult<IReadOnlyCollection<string>> GetCategories()
    {
        return Ok(_service.GetSupportedCategories());
    }

    [HttpGet("units")]
    public ActionResult<IReadOnlyCollection<string>> GetUnits([FromQuery] string category)
    {
        try
        {
            return Ok(_service.GetSupportedUnits(category));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
