using Base.Application.Interfaces.Services;
using VehicleType.Application.DTOs;

namespace VehicleType.Application.Interfaces.Services;
public interface IVehicleTypeService : IBaseService<VehicleTypeDto>
{
    Task<VehicleTypeDto> GetByNameAsync(string name);
}
