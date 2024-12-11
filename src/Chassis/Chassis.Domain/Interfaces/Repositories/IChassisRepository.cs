using Base.Domain.Entities;
using Base.Domain.Interfaces.Repositories;
using Chassis.Domain.Entities;

namespace Chassis.Domain.Interfaces.Repositories;
public interface IChassisRepository : IBaseRepository<ChassisEntity>
{
    Task<ChassisEntity> GetByChassisSeriesAndNumberAsync(string chassisSeries, uint chassisNumber, CancellationToken cancellationToken);
    Task<BaseListEntity<ChassisEntity>> ListUnassignedAsync(uint pageNumber, ushort pageSize, CancellationToken cancellationToken);
}
