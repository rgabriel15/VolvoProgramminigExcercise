using Base.Application.DTOs;
using Base.Application.Interfaces.Services;
using Chassis.Application.DTOs;

namespace Chassis.Application.Interfaces.Services;
public interface IChassisService : IBaseService<ChassisDto>
{
    Task<ChassisDto> GetByChassisSeriesAndNumberAsync(string chassisSeries, uint chassisNumber);
    Task<BaseListDto<ChassisDto>> ListUnassignedAsync(uint pageNumber, ushort pageSize);
}
