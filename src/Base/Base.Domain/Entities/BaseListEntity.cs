using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Domain.Entities;

[NotMapped]
public sealed class BaseListEntity<T>
    where T : BaseEntity
{
    #region Constants
    public const uint DefaultPageNumber = 1;
    public const ushort DefaultPageSize = 5000;
    public const ushort MaxPageSize = 5000;
    #endregion

    #region Properties
    [Range(1, uint.MaxValue)]
    public uint PageNumber { get; set; } = DefaultPageNumber;

    [Range(1, MaxPageSize)]
    public ushort PageSize { get; set; } = DefaultPageSize;

    public IReadOnlyCollection<T> List { get; set; } = [];
    #endregion
}
