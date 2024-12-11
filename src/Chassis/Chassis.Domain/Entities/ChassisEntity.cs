using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chassis.Domain.Entities;
[Index(propertyName: nameof(ChassisSeries), additionalPropertyNames: nameof(ChassisNumber), IsUnique = true)]
public sealed class ChassisEntity : BaseEntity
{
    #region Properties
    [Required]
    [Column(TypeName = "varchar(100)")]
    public required string ChassisSeries { get; set; }

    [Required]
    [Column(TypeName = nameof(SqlDbType.BigInt))]
    public required uint ChassisNumber { get; set; }
    #endregion
}
