using Base.Application.Interfaces.Validators;
using ClientException.Application.DTOs;
using ClientException.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public sealed class ClientExceptionController : ControllerBase
{
    #region Constants
    private readonly IClientExceptionService Service;
    private readonly IBaseValidator<ClientExceptionDto> ClientExceptionValidator;
    #endregion

    #region Constructors
    public ClientExceptionController(IClientExceptionService service
        , IBaseValidator<ClientExceptionDto> clientExceptionValidator)
    {
        Service = service;
        ClientExceptionValidator = clientExceptionValidator;
    }
    #endregion

    #region Methods
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] ClientExceptionDto dto)
    {
        if (!ClientExceptionValidator.IsValid(dto))
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
    #endregion
}
