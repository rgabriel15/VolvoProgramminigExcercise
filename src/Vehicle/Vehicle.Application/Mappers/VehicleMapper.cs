using Base.Application.Interfaces.Mappers;
using Vehicle.Application.DTOs;
using Vehicle.Domain.Entities;

namespace Vehicle.Application.Mappers;
public sealed class VehicleMapper : IBaseMapper<VehicleEntity, VehicleDto>
{
    #region Methods
    public VehicleEntity ToEntity(VehicleDto dto)
    {
        var entity = new VehicleEntity
        {
            Id = dto.Id,
            IsActive = dto.IsActive,
            CreationDate = dto.CreationDate,
            UpdateDate = dto.UpdateDate,
            ChassisId = dto.ChassisId,
            VehicleTypeId = dto.VehicleTypeId,
            Color = dto.Color,
        };

        return entity;
    }

    public VehicleDto ToDto(VehicleEntity entity)
    {
        var dto = new VehicleDto
        {
            Id = entity.Id,
            IsActive = entity.IsActive,
            CreationDate = entity.CreationDate,
            UpdateDate = entity.UpdateDate,
            ChassisId = entity.ChassisId,
            VehicleTypeId = entity.VehicleTypeId,
            Color = entity.Color,
            Chassis = entity.Chassis,
            VehicleType = entity.VehicleType,
        };

        return dto;
    }
    #endregion
}
