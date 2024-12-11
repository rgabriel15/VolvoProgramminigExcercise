using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Base.Domain.Entities;
using Chassis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VehicleType.Domain.Entities;

namespace Vehicle.Domain.Entities;
[Index(propertyName: nameof(ChassisId), IsUnique = true)]
public sealed class VehicleEntity : BaseEntity
{
    #region Properties
    [Required]
    [Column(TypeName = nameof(SqlDbType.BigInt))]
    public required ulong ChassisId { get; set; }

    [ForeignKey(nameof(ChassisId))]
    [DeleteBehavior(DeleteBehavior.ClientNoAction)]
    public ChassisEntity? Chassis { get; set; }

    [Required]
    [Column(TypeName = nameof(SqlDbType.BigInt))]
    public required ulong VehicleTypeId { get; set; }

    [ForeignKey(nameof(VehicleTypeId))]
    [DeleteBehavior(DeleteBehavior.ClientNoAction)]
    public VehicleTypeEntity? VehicleType { get; set; }

    [Required]
    [Column(TypeName = "varchar(100)")]
    public required string Color { get; set; }
    #endregion
}
