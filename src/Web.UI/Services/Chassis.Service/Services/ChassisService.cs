using Web.UI.Services.Base.Service.Services;
using Web.UI.Services.Chassis.Service.Interfaces;
using Web.UI.Services.Chassis.Service.Models;
using Web.UI.Services.ClientException.Service.Interfaces;

namespace Web.UI.Services.Chassis.Service.Services;
internal sealed class ChassisService : BaseService<ChassisModel>, IChassisService
{
    #region Constants
    private const string BaseUrl = "chassis";
    #endregion

    #region Constructors
    public ChassisService(IHttpClientFactory httpClientFactory
        , IHttpContextAccessor httpContextAccessor
        , IClientExceptionService clientExceptionService)
        : base(httpClientFactory, httpContextAccessor, clientExceptionService, BaseUrl)
    {
    }
    #endregion
}
