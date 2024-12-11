using Base.Application.Interfaces.Mappers;
using ClientException.Application.DTOs;
using ClientException.Domain.Entities;

namespace ClientException.Application.Mappers;
public sealed class ClientExceptionMapper : IBaseMapper<ClientExceptionEntity, ClientExceptionDto>
{
    #region Methods
    public ClientExceptionEntity ToEntity(ClientExceptionDto dto)
    {
        var entity = new ClientExceptionEntity
        {
            Id = dto.Id,
            IsActive = dto.IsActive,
            CreationDate = dto.CreationDate,
            UpdateDate = dto.UpdateDate,
            ErrorMessage = dto.ErrorMessage,
            StackTrace = dto.StackTrace,
            ClientAppName = dto.ClientAppName,
            Date = dto.Date,
        };

        return entity;
    }

    public ClientExceptionDto ToDto(ClientExceptionEntity entity)
    {
        var dto = new ClientExceptionDto
        {
            Id = entity.Id,
            IsActive = entity.IsActive,
            CreationDate = entity.CreationDate,
            UpdateDate = entity.UpdateDate,
            ErrorMessage = entity.ErrorMessage,
            StackTrace = entity.StackTrace,
            ClientAppName = entity.ClientAppName,
            Date = entity.Date,
        };

        return dto;
    }
    #endregion
}
