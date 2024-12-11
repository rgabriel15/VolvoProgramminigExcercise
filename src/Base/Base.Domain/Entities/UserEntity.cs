using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Domain.Entities;

[Index(propertyName: nameof(EmailAddress), IsUnique = true)]
public sealed class UserEntity : BaseEntity
{
    #region Properties
    [Required]
    [Column(TypeName = "varchar(60)")]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Column(TypeName = "varchar(254)")]
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Taxpayer Identification. Ex: Brazilian CPF.
    /// </summary>
    [Column(TypeName = "varchar(18)")]
    [StringLength(14)]
    public string TaxId { get; set; } = string.Empty;

    [Required]
    public bool GenderIsMale { get; set; } = true;

    [Required]
    public DateOnly Birthdate { get; set; }

    [Required]
    public DateTime TermsOfUseAcceptanceDate { get; set; }

    [NotMapped]
    public DateTime? LicenseExpiration { get; set; }
    #endregion
}
