using System.ComponentModel.DataAnnotations;
using Base.Domain.Entities;

namespace Base.Application.DTOs;
public sealed record BaseListDto<T>
    where T : BaseDto
{
    #region Properties
    [Range(1, uint.MaxValue)]
    public uint PageNumber { get; set; } = BaseListEntity<BaseEntity>.DefaultPageNumber;

    [Range(1, BaseListEntity<BaseEntity>.MaxPageSize)]
    public ushort PageSize { get; set; } = BaseListEntity<BaseEntity>.DefaultPageSize;

    public IReadOnlyCollection<T> List { get; set; } = [];
    #endregion
}
