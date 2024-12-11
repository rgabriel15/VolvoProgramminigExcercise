using Web.UI.Services.Base.Service.Models;

namespace Web.UI.Services.ClientException.Service.Models;
internal sealed record ClientExceptionModel : BaseModel
{
    #region Properties
    public required string ErrorMessage { get; set; }
    public required string StackTrace { get; set; }
    public static string ClientAppName => "VolvoProgrammingExerciseClientApp";
    #endregion
}
