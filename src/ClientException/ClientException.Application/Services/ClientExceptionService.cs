using Base.Application.Services;
using Serilog;
using ClientException.Application.DTOs;
using ClientException.Application.Interfaces.Services;
using ClientException.Domain.Entities;
using ClientException.Domain.Interfaces.Repositories;
using Base.Application.Interfaces.Mappers;

namespace ClientException.Application.Services;
public sealed class ClientExceptionService
    : BaseService<ClientExceptionEntity, ClientExceptionDto>
    , IClientExceptionService
{
    #region Constructors
    public ClientExceptionService(ILogger logger
        , IClientExceptionRepository repository
        , IBaseMapper<ClientExceptionEntity, ClientExceptionDto> mapper)
        : base(logger, repository, mapper)
    {
    }
    #endregion

    #region Methods
    public override async Task<ClientExceptionDto> AddAsync(ClientExceptionDto dto)
    {
        var newDto = await base.AddAsync(dto);
        return newDto;
        #endregion
    }
}
