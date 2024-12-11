using Base.Application.DTOs;
using Base.Application.Interfaces.Services;
using Vehicle.Application.DTOs;

namespace Vehicle.Application.Interfaces.Services;
public interface IVehicleService : IBaseService<VehicleDto>
{
    Task<VehicleDto> GetByChassisIdAsync(ulong chassisId);
    Task<VehicleDto> GetByChassisSeriesAndNumberAsync(string chassisSeries, uint chassisNumber);
    Task<BaseListDto<VehicleDto>> ListByVehicleTypeIdAsync(ulong vehicleTypeId, uint pageNumber, ushort pageSize);
}
