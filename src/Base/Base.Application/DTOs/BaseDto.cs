using System.Text.Json.Serialization;

namespace Base.Application.DTOs;
public abstract record BaseDto
{
    #region Properties
    public ulong Id { get; set; }

    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public DateTime CreationDate { get; set; }

    [JsonIgnore]
    public DateTime UpdateDate { get; set; }
    #endregion
}
