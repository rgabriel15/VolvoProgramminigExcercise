using System.ComponentModel.DataAnnotations;
using Base.Application.DTOs;

namespace VehicleType.Application.DTOs;
public sealed record VehicleTypeDto : BaseDto
{
    #region Properties
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Required]
    [Range(1, byte.MaxValue)]
    public required byte NumberOfPassengers { get; set; } = 1;
    #endregion
}
