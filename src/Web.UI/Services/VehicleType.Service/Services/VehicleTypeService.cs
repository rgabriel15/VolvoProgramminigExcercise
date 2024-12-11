using Web.UI.Services.Base.Service.Services;
using Web.UI.Services.ClientException.Service.Interfaces;
using Web.UI.Services.VehicleType.Service.Interfaces;
using Web.UI.Services.VehicleType.Service.Models;

namespace Web.UI.Services.VehicleType.Service.Services;
internal sealed class VehicleTypeService : BaseService<VehicleTypeModel>, IVehicleTypeService
{
    #region Constants
    private const string BaseUrl = "vehicleType";
    #endregion

    #region Constructors
    public VehicleTypeService(IHttpClientFactory httpClientFactory
        , IHttpContextAccessor httpContextAccessor
        , IClientExceptionService clientExceptionService)
        : base(httpClientFactory, httpContextAccessor, clientExceptionService, BaseUrl)
    {
    }
    #endregion
}
