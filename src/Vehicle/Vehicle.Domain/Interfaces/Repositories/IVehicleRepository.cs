using Base.Domain.Entities;
using Base.Domain.Interfaces.Repositories;
using Vehicle.Domain.Entities;

namespace Vehicle.Domain.Interfaces.Repositories;
public interface IVehicleRepository : IBaseRepository<VehicleEntity>
{
    Task<VehicleEntity> GetByChassisIdAsync(ulong chassisId, CancellationToken cancellationToken);
    Task<VehicleEntity> GetByChassisSeriesAndNumberAsync(string chassisSeries, uint chassisNumber, CancellationToken cancellationToken);
    Task<BaseListEntity<VehicleEntity>> ListByVehicleTypeIdAsync(ulong vehicleTypeId, uint pageNumber, ushort pageSize, CancellationToken cancellationToken);
}
