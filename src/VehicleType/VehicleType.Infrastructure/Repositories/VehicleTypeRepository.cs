using Base.Infrastructure;
using Base.Infrastructure.Repositories;
using VehicleType.Domain.Entities;
using VehicleType.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace VehicleType.Infrastructure.Repositories;
public sealed class VehicleTypeRepository :
    BaseRepository<VehicleTypeEntity>
    , IVehicleTypeRepository
{
    #region Constructors
    public VehicleTypeRepository(ILogger logger
        , EfContext efContext
        , IHttpContextAccessor httpContextAccessor)
        : base(logger, efContext, httpContextAccessor)
    {
    }
    #endregion

    #region Methods
    public async Task<VehicleTypeEntity> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await EfContext
                .VehicleTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.IsActive
                    && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                , cancellationToken)
                ?? Activator.CreateInstance<VehicleTypeEntity>();
            return entity;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return Activator.CreateInstance<VehicleTypeEntity>();
        }
    }
    #endregion
}
