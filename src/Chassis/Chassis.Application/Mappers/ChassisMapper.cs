using Base.Application.Interfaces.Mappers;
using Chassis.Application.DTOs;
using Chassis.Domain.Entities;

namespace Chassis.Application.Mappers;
public sealed class ChassisMapper : IBaseMapper<ChassisEntity, ChassisDto>
{
    #region Methods
    public ChassisEntity ToEntity(ChassisDto dto)
    {
        var entity = new ChassisEntity
        {
            Id = dto.Id,
            IsActive = dto.IsActive,
            CreationDate = dto.CreationDate,
            UpdateDate = dto.UpdateDate,
            ChassisNumber = dto.ChassisNumber,
            ChassisSeries = dto.ChassisSeries,
        };

        return entity;
    }

    public ChassisDto ToDto(ChassisEntity entity)
    {
        var dto = new ChassisDto
        {
            Id = entity.Id,
            IsActive = entity.IsActive,
            CreationDate = entity.CreationDate,
            UpdateDate = entity.UpdateDate,
            ChassisNumber = entity.ChassisNumber,
            ChassisSeries = entity.ChassisSeries,
        };

        return dto;
    }
    #endregion
}
