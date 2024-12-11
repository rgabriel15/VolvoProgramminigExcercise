using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Base.Domain.Entities;
public abstract class BaseEntity
{
    #region Properties
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(TypeName = nameof(SqlDbType.BigInt))]
    public ulong Id { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreationDate { get; set; }

    [Required]
    public DateTime UpdateDate { get; set; }
    #endregion
}
