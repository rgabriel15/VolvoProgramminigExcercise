using System.ComponentModel.DataAnnotations;
using Web.UI.Services.Base.Service.Models;

namespace Web.UI.Services.VehicleType.Service.Models;
internal sealed record VehicleTypeModel : BaseModel
{
    #region Properties
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, byte.MaxValue)]
    public byte NumberOfPassengers { get; set; } = 1;
    #endregion
}
