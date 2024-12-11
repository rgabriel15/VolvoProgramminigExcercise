using Base.Application.DTOs;
using Base.Domain.Entities;

namespace Base.Application.Interfaces.Mappers;
public interface IBaseMapper<TEntity, TDto>
    where TEntity : BaseEntity
    where TDto : BaseDto
{
    public TEntity ToEntity(TDto dto);
    public TDto ToDto(TEntity entity);
}
