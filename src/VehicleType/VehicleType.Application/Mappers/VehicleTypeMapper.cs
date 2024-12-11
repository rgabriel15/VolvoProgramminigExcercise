using Base.Application.Interfaces.Mappers;
using VehicleType.Application.DTOs;
using VehicleType.Domain.Entities;

namespace VehicleType.Application.Mappers;
public sealed class VehicleTypeMapper : IBaseMapper<VehicleTypeEntity, VehicleTypeDto>
{
    #region Methods
    public VehicleTypeEntity ToEntity(VehicleTypeDto dto)
    {
        var entity = new VehicleTypeEntity
        {
            Id = dto.Id,
            IsActive = dto.IsActive,
            CreationDate = dto.CreationDate,
            UpdateDate = dto.UpdateDate,
            Name = dto.Name,
            NumberOfPassengers = dto.NumberOfPassengers,
        };

        return entity;
    }

    public VehicleTypeDto ToDto(VehicleTypeEntity entity)
    {
        var dto = new VehicleTypeDto
        {
            Id = entity.Id,
            IsActive = entity.IsActive,
            CreationDate = entity.CreationDate,
            UpdateDate = entity.UpdateDate,
            Name = entity.Name,
            NumberOfPassengers = entity.NumberOfPassengers,
        };

        return dto;
    }
    #endregion
}
