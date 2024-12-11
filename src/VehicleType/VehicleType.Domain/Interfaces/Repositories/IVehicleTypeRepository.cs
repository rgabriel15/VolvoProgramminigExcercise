using Base.Domain.Interfaces.Repositories;
using VehicleType.Domain.Entities;

namespace VehicleType.Domain.Interfaces.Repositories;
public interface IVehicleTypeRepository : IBaseRepository<VehicleTypeEntity>
{
    Task<VehicleTypeEntity> GetByNameAsync(string name, CancellationToken cancellationToken);
}
