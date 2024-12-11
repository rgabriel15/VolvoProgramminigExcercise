using Base.Application.Interfaces.Validators;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public sealed class VehicleController : BaseController<VehicleDto>
{
    #region Constructors
    public VehicleController(IVehicleService service
        , IBaseValidator<VehicleDto> validator)
        : base(service, validator)
    {
    }
    #endregion

    #region Methods
    [HttpGet("[action]")]
    public async Task<IActionResult> GetByChassisIdAsync([FromQuery] ulong chassisId)
    {
        var dto = await ((IVehicleService)Service).GetByChassisIdAsync(chassisId);
        return dto.Id < 1
            ? NoContent()
            : Ok(dto);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByChassisSeriesAndNumberAsync([FromQuery] string chassisSeries
        , [FromQuery] uint chassisNumber)
    {
        var dto = await ((IVehicleService)Service).GetByChassisSeriesAndNumberAsync(
            chassisSeries: chassisSeries
            , chassisNumber: chassisNumber);
        return dto.Id < 1
            ? NoContent()
            : Ok(dto);
    }
    #endregion
}
