using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Base.Domain.Entities;

namespace ClientException.Domain.Entities;
public sealed class ClientExceptionEntity : BaseEntity
{
    #region Properties
    [Required]
    [Column(TypeName = "varchar(2000)")]
    public string ErrorMessage { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "varchar(2000)")]
    public string StackTrace { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "varchar(40)")]
    public string ClientAppName { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }
    #endregion
}
