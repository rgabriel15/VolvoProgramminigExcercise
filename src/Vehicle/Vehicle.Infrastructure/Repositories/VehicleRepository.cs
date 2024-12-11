using Base.Infrastructure;
using Base.Infrastructure.Repositories;
using Vehicle.Domain.Entities;
using Vehicle.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Base.Domain.Entities;

namespace Vehicle.Infrastructure.Repositories;
public sealed class VehicleRepository :
    BaseRepository<VehicleEntity>
    , IVehicleRepository
{
    #region Constructors
    public VehicleRepository(ILogger logger, EfContext efContext, IHttpContextAccessor httpContextAccessor)
        : base(logger, efContext, httpContextAccessor)
    {
    }
    #endregion

    #region Methods
    public override async Task<VehicleEntity> GetAsync(ulong id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await EfContext
                .Vehicles
                .AsNoTracking()
                .Include(x => x.Chassis)
                .Include(x => x.VehicleType)
                .FirstOrDefaultAsync(x =>
                    x.IsActive
                    && x.Id == id
                , cancellationToken)
                ?? Activator.CreateInstance<VehicleEntity>();
            return entity;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return Activator.CreateInstance<VehicleEntity>();
        }
    }

    public async Task<VehicleEntity> GetByChassisIdAsync(ulong chassisId, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await EfContext
                .Vehicles
                .AsNoTracking()
                .Include(x => x.Chassis)
                .Include(x => x.VehicleType)
                .FirstOrDefaultAsync(x =>
                    x.IsActive
                    && x.ChassisId == chassisId
                , cancellationToken)
                ?? Activator.CreateInstance<VehicleEntity>();
            return entity;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return Activator.CreateInstance<VehicleEntity>();
        }
    }

    public async Task<VehicleEntity> GetByChassisSeriesAndNumberAsync(string chassisSeries, uint chassisNumber, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await EfContext
                .Vehicles
                .AsNoTracking()
                .Include(x => x.Chassis)
                .Include(x => x.VehicleType)
                .FirstOrDefaultAsync(x =>
                    x.IsActive
                    && x.Chassis != null
                    && x.Chassis.ChassisSeries == chassisSeries
                    && x.Chassis.ChassisNumber == chassisNumber
                , cancellationToken)
                ?? Activator.CreateInstance<VehicleEntity>();
            return entity;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return Activator.CreateInstance<VehicleEntity>();
        }
    }

    public override async Task<BaseListEntity<VehicleEntity>> ListAsync(
        uint pageNumber, ushort pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var offset = (int)((pageNumber - 1) * pageSize);
            var list = await EfContext
                .Vehicles
                .Include(x => x.Chassis)
                .Include(x => x.VehicleType)
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new BaseListEntity<VehicleEntity>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                List = list ?? []
            };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return new BaseListEntity<VehicleEntity>();
        }
    }

    public override async Task<BaseListEntity<VehicleEntity>> ListAsNoTrackingAsync(uint pageNumber
    , ushort pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var offset = (int)((pageNumber - 1) * pageSize);
            var list = await EfContext
                .Vehicles
                .AsNoTracking()
                .Include(x => x.Chassis)
                .Include(x => x.VehicleType)
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new BaseListEntity<VehicleEntity>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                List = list ?? []
            };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return new BaseListEntity<VehicleEntity>();
        }
    }

    public async Task<BaseListEntity<VehicleEntity>> ListByVehicleTypeIdAsync(
        ulong vehicleTypeId, uint pageNumber, ushort pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var offset = (int)((pageNumber - 1) * pageSize);
            var list = await EfContext
                .Vehicles
                .AsNoTracking()
                .Include(x => x.Chassis)
                .Include(x => x.VehicleType)
                .Where(x =>
                    x.IsActive
                    && x.VehicleTypeId == vehicleTypeId)
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new BaseListEntity<VehicleEntity>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                List = list ?? []
            };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return new BaseListEntity<VehicleEntity>();
        }
    }
    #endregion
}
