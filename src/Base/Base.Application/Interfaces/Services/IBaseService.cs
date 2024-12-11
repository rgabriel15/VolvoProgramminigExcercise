using Base.Application.DTOs;

namespace Base.Application.Interfaces.Services;
public interface IBaseService<T>
    where T : BaseDto
{
    Task<T> GetAsync(ulong id);
    Task<IReadOnlyCollection<T>> ListAsync(ulong[] ids);
    Task<BaseListDto<T>> ListAsync(uint pageNumber, ushort pageSize);
    Task<BaseListDto<T>> ListAsNoTrackingAsync(uint pageNumber, ushort pageSize);
    Task<T> AddAsync(T dto);
    Task<ushort> AddRangeAsync(IEnumerable<T> list);
    Task<T> UpdateAsync(T dto);
    Task<ulong> DeleteAsync(ulong id);
    Task<ulong> LogicalDeleteAsync(ulong id);
}
