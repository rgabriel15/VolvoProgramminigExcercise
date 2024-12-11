using Base.Application.Interfaces.Validators;
using Base.Domain.Entities;
using Chassis.Application.DTOs;
using Chassis.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public sealed class ChassisController : BaseController<ChassisDto>
{
    #region Constructors
    public ChassisController(IChassisService service
        , IBaseValidator<ChassisDto> validator)
        : base(service, validator)
    {
    }
    #endregion

    #region Methods
    [HttpGet("[action]")]
    public async Task<IActionResult> GetByChassisSeriesAndNumberAsync([FromQuery] string chassisSeries
        , [FromQuery] uint chassisNumber)
    {
        var dto = await ((IChassisService)Service).GetByChassisSeriesAndNumberAsync(
            chassisSeries: chassisSeries
            , chassisNumber: chassisNumber);
        return dto.Id < 1
            ? NoContent()
            : Ok(dto);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> ListUnassignedAsync([FromQuery] uint pageNumber = BaseListEntity<BaseEntity>.DefaultPageNumber
        , [FromQuery] ushort pageSize = BaseListEntity<BaseEntity>.DefaultPageSize)
    {
        var dto = await ((IChassisService)Service).ListUnassignedAsync(pageNumber: pageNumber, pageSize: pageSize);
        return (dto?.List?.Count ?? 0) == 0
            ? NoContent()
            : Ok(dto);
    }
    #endregion
}
