using System.Text.Json.Serialization;

namespace Web.UI.Services.Base.Service.Models;
internal abstract record BaseModel
{
    #region Properties
    public ulong Id { get; set; }

    [JsonIgnore]
    public bool IsActive { get; set; } = true;

    [JsonIgnore]
    public DateTime CreationDate { get; set; }

    [JsonIgnore]
    public DateTime UpdateDate { get; set; }
    #endregion
}
