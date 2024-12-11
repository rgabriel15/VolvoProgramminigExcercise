using Base.Application.DTOs;
using Base.Application.Interfaces.Services;
using Base.Application.Interfaces.Validators;
using Base.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public abstract class BaseController<T> : ControllerBase
    where T : BaseDto
{
    #region Constants
    protected IBaseService<T> Service { get; private set; }
    protected IBaseValidator<T> Validator { get; private set; }
    #endregion

    #region Constructors
    protected BaseController(IBaseService<T> service
        , IBaseValidator<T> validator)
    {
        Service = service;
        Validator = validator;
    }
    #endregion

    #region Methods
    [HttpGet]
    public virtual async Task<IActionResult> GetAsync([FromQuery] ulong id)
    {
        var dto = await Service.GetAsync(id);
        return dto.Id < 1
            ? NoContent()
            : Ok(dto);
    }

    [HttpGet("[action]")]
    public virtual async Task<IActionResult> ListAsync([FromQuery] uint pageNumber = BaseListEntity<BaseEntity>.DefaultPageNumber
        , [FromQuery] ushort pageSize = BaseListEntity<BaseEntity>.DefaultPageSize)
    {
        var dto = await Service.ListAsNoTrackingAsync(pageNumber: pageNumber, pageSize: pageSize);
        return (dto?.List?.Count ?? 0) == 0
            ? NoContent()
            : Ok(dto);
    }

    [HttpPost]
    public virtual async Task<IActionResult> PostAsync([FromBody] T dto)
    {
        if (!Validator.IsValid(dto))
        {
            return BadRequest();
        }

        dto = await Service.AddAsync(dto);
        return dto.Id < 1
            ? Problem(statusCode: StatusCodes.Status422UnprocessableEntity)
            : new ObjectResult(dto)
            {
                StatusCode = StatusCodes.Status201Created
            };
    }

    [HttpPut]
    public virtual async Task<IActionResult> PutAsync([FromQuery] ulong id, [FromBody] T dto)
    {
        if (!Validator.IsValid(dto))
        {
            return BadRequest();
        }

        dto.Id = id;
        dto = await Service.UpdateAsync(dto);
        return dto.Id < 1
            ? Problem(statusCode: StatusCodes.Status422UnprocessableEntity)
            : Ok(dto);
    }

    [HttpDelete]
    public virtual async Task<IActionResult> DeleteAsync([FromQuery] ulong id)
    {
        var deletedId = await Service.DeleteAsync(id);
        return deletedId > 0
            ? Ok(id)
            : Problem(statusCode: StatusCodes.Status422UnprocessableEntity);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The number of registers affected.</returns>
    [HttpDelete("[action]")]
    public virtual async Task<IActionResult> LogicalDeleteAsync([FromQuery] ulong id)
    {
        var deletedId = await Service.LogicalDeleteAsync(id);
        return deletedId <= 0
            ? Problem(statusCode: StatusCodes.Status422UnprocessableEntity)
            : Ok(id);
    }
    #endregion
}
