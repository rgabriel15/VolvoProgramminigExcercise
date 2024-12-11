using Base.Domain.Entities;

namespace Base.Domain.Interfaces.Repositories;
public interface IBaseRepository<T>
    where T : BaseEntity
{
    Task<T> GetAsync(ulong id, CancellationToken cancellationToken);
    Task<IEnumerable<T>> ListAsync(ulong[] ids, CancellationToken cancellationToken);
    Task<BaseListEntity<T>> ListAsync(uint pageNumber, ushort pageSize, CancellationToken cancellationToken);
    Task<BaseListEntity<T>> ListAsNoTrackingAsync(uint pageNumber, ushort pageSize, CancellationToken cancellationToken);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken);
    Task<ushort> AddRangeAsync(IEnumerable<T> list, CancellationToken cancellationToken);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task<ulong> DeleteAsync(ulong id, CancellationToken cancellationToken);
}
