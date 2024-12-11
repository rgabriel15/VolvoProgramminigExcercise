using Base.Domain.Entities;
using Base.Infrastructure;
using Base.Infrastructure.Repositories;
using Chassis.Domain.Entities;
using Chassis.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Chassis.Infrastructure.Repositories;
public sealed class ChassisRepository :
    BaseRepository<ChassisEntity>
    , IChassisRepository
{
    #region Constructors
    public ChassisRepository(ILogger logger, EfContext efContext, IHttpContextAccessor httpContextAccessor)
        : base(logger, efContext, httpContextAccessor)
    {
    }
    #endregion

    #region Methods
    public async Task<ChassisEntity> GetByChassisSeriesAndNumberAsync(string chassisSeries, uint chassisNumber, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await EfContext
                .Chassis
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.IsActive
                    && x.ChassisSeries == chassisSeries
                    && x.ChassisNumber == chassisNumber
                , cancellationToken)
                ?? Activator.CreateInstance<ChassisEntity>();
            return entity;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return Activator.CreateInstance<ChassisEntity>();
        }
    }

    public async Task<BaseListEntity<ChassisEntity>> ListUnassignedAsync(uint pageNumber
        , ushort pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var offset = (int)((pageNumber - 1) * pageSize);
            var list = await EfContext
                .Chassis
                .AsNoTracking()
                .Where(x =>
                    x.IsActive
                    && !EfContext.Vehicles.Any(v => v.ChassisId == x.Id))
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new BaseListEntity<ChassisEntity>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                List = list ?? []
            };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return new BaseListEntity<ChassisEntity>();
        }
    }
    #endregion
}
