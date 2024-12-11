using Base.Application.Interfaces.Validators;
using VehicleType.Application.DTOs;
using VehicleType.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public sealed class VehicleTypeController : BaseController<VehicleTypeDto>
{
    #region Constructors
    public VehicleTypeController(IVehicleTypeService service
        , IBaseValidator<VehicleTypeDto> validator)
        : base(service, validator)
    {
    }
    #endregion
}
