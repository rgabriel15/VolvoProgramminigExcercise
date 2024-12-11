using System.ComponentModel.DataAnnotations;
using Web.UI.Services.Base.Service.Models;
using Web.UI.Services.Chassis.Service.Models;
using Web.UI.Services.VehicleType.Service.Models;

namespace Web.UI.Services.Vehicle.Service.Models;
internal sealed record VehicleModel : BaseModel
{
    #region Properties
    [Required]
    public ulong ChassisId { get; set; }

    public ChassisModel? Chassis { get; set; }

    [Required]
    public ulong VehicleTypeId { get; set; }

    public VehicleTypeModel? VehicleType { get; set; }

    [Required]
    [StringLength(100)]
    public string Color { get; set; } = System.Drawing.Color.White.Name;
    #endregion
}
