using System.ComponentModel.DataAnnotations;
using Base.Application.DTOs;

namespace Chassis.Application.DTOs;
public sealed record ChassisDto : BaseDto
{
    #region Properties
    [Required]
    [StringLength(100)]
    public required string ChassisSeries { get; set; }

    [Required]
    public required uint ChassisNumber { get; set; }
    #endregion
}
