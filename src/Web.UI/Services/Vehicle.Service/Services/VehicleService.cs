using Web.UI.Services.Base.Service.Services;
using Web.UI.Services.ClientException.Service.Interfaces;
using Web.UI.Services.Vehicle.Service.Interfaces;
using Web.UI.Services.Vehicle.Service.Models;

namespace Web.UI.Services.Vehicle.Service.Services;
internal sealed class VehicleService : BaseService<VehicleModel>, IVehicleService
{
    #region Constants
    private const string BaseUrl = "vehicle";
    #endregion

    #region Constructors
    public VehicleService(IHttpClientFactory httpClientFactory
        , IHttpContextAccessor httpContextAccessor
        , IClientExceptionService clientExceptionService)
        : base(httpClientFactory, httpContextAccessor, clientExceptionService, BaseUrl)
    {
    }
    #endregion
}
