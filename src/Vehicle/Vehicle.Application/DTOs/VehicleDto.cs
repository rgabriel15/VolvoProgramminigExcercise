using System.ComponentModel.DataAnnotations;
using Base.Application.DTOs;
using Chassis.Domain.Entities;
using VehicleType.Domain.Entities;

namespace Vehicle.Application.DTOs;
public sealed record VehicleDto : BaseDto
{
    #region Properties
    [Required]
    public required ulong ChassisId { get; set; }

    public ChassisEntity? Chassis { get; set; }

    [Required]
    public required ulong VehicleTypeId { get; set; }

    public VehicleTypeEntity? VehicleType { get; set; }

    [Required]
    [StringLength(100)]
    public required string Color { get; set; }
    #endregion
}
