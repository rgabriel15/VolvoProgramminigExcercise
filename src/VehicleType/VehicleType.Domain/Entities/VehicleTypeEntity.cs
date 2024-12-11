using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Base.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace VehicleType.Domain.Entities;
[Index(propertyName: nameof(Name), IsUnique = true)]
public sealed class VehicleTypeEntity : BaseEntity
{
    #region Properties
    [Required]
    [Column(TypeName = "varchar(100)")]
    public required string Name { get; set; }

    [Required]
    [Column(TypeName = nameof(SqlDbType.SmallInt))]
    public required byte NumberOfPassengers { get; set; }
    #endregion
}
