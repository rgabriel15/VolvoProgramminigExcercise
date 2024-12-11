using Base.Infrastructure;
using Base.Infrastructure.Repositories;
using ClientException.Domain.Entities;
using ClientException.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace ClientException.Infrastructure.Repositories;
public sealed class ClientExceptionRepository :
    BaseRepository<ClientExceptionEntity>
    , IClientExceptionRepository
{
    #region Constructors
    public ClientExceptionRepository(ILogger logger, EfContext efContext, IHttpContextAccessor httpContextAccessor)
        : base(logger, efContext, httpContextAccessor)
    {
    }
    #endregion
}
