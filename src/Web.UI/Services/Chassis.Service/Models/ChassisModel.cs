using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Web.UI.Services.Base.Service.Models;

namespace Web.UI.Services.Chassis.Service.Models;
internal sealed record ChassisModel : BaseModel
{
    #region Properties
    [Required]
    [StringLength(100)]
    public string ChassisSeries { get; set; } = string.Empty;

    [Required]
    public uint ChassisNumber { get; set; }

    [JsonIgnore]
    public string ChassisIdAndSeriesAndNumber => $"Chassis Id: {Id} | Chassis Series: {ChassisSeries} | Chassis Number: {ChassisNumber}";
    #endregion
}
