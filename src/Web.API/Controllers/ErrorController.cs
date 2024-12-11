using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Web.API.Controllers;

[AllowAnonymous]
[Route("/")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class ErrorController : ControllerBase
{
    #region Constants
    private readonly ILogger Logger;
    #endregion

    #region Constructors
    public ErrorController(ILogger logger)
    {
        Logger = logger;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Error Development
    /// </summary>
    /// <returns></returns>
    [Route("error-development")]
    public IActionResult ErrorLocalDevelopment([FromServices] IWebHostEnvironment webHostEnvironment)
    {
        if (webHostEnvironment.IsProduction())
        {
            var msg = "This shouldn't be invoked in production environments.";
            throw new InvalidOperationException(msg);
        }

        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var errorMsg = context!.Error.ToString();
        Logger.Error(errorMsg);

        return Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            detail: errorMsg,
            title: "Error");
    }

    /// <summary>
    /// Error
    /// </summary>
    /// <returns></returns>
    [Route("error")]
    public IActionResult Error([FromServices] IWebHostEnvironment webHostEnvironment)
    {
        if (!webHostEnvironment.IsProduction())
        {
            var msg = "This shouldn't be invoked in non-production environments.";
            throw new InvalidOperationException(msg);
        }

        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var errorMsg = context!.Error.ToString();
        Logger.Error(errorMsg);

        return Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            detail: context!.Error.Message,
            title: "Error");
    }
    #endregion
}
