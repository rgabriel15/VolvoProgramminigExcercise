using Web.UI.Services.Chassis.Service.Models;
using Web.UI.Services.Vehicle.Service.Models;
using Web.UI.Services.VehicleType.Service.Models;

namespace Web.UI;
internal sealed class StateContainer
{
    #region Properties
    internal event Action? OnChange;

    private IReadOnlyCollection<ChassisModel> chassisList = [];
    internal IReadOnlyCollection<ChassisModel> ChassisList
    {
        get => chassisList;
        set
        {
            chassisList = value;
            OnChange?.Invoke();
        }
    }

    private IReadOnlyCollection<VehicleModel> vehicles = [];
    internal IReadOnlyCollection<VehicleModel> Vehicles
    {
        get => vehicles;
        set
        {
            vehicles = value;
            OnChange?.Invoke();
        }
    }

    private IReadOnlyCollection<VehicleTypeModel> vehicleTypes = [];
    internal IReadOnlyCollection<VehicleTypeModel> VehicleTypes
    {
        get => vehicleTypes;
        set
        {
            vehicleTypes = value;
            OnChange?.Invoke();
        }
    }

    internal ChassisModel Chassis { get; set; } = new ChassisModel();
    internal VehicleModel Vehicle { get; set; } = Activator.CreateInstance<VehicleModel>();
    internal VehicleTypeModel VehicleType { get; set; } = new VehicleTypeModel();
    #endregion
}
