using System.ComponentModel.DataAnnotations;

namespace Web.UI.Services.Base.Service.Models;
internal sealed record BaseListModel<T>
    where T : BaseModel
{
    #region Constants
    internal const uint DefaultPageNumber = 1;
    internal const ushort DefaultPageSize = 5000;
    internal const ushort MaxPageSize = 5000;
    #endregion

    #region Properties
    [Range(1, uint.MaxValue)]
    public uint PageNumber { get; set; } = DefaultPageNumber;

    [Range(1, 5000)]
    public ushort PageSize { get; set; } = DefaultPageSize;

    public IReadOnlyCollection<T> List { get; set; } = [];
    #endregion
}
