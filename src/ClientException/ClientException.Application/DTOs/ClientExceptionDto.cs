using System.ComponentModel.DataAnnotations;
using Base.Application.DTOs;

namespace ClientException.Application.DTOs;
public sealed record ClientExceptionDto : BaseDto
{
    #region Properties
    [Required]
    public required string ErrorMessage { get; set; }

    [Required]
    public required string StackTrace { get; set; }

    [Required]
    public required string ClientAppName { get; set; }

    public DateTime Date { get; set; }
    #endregion
}
