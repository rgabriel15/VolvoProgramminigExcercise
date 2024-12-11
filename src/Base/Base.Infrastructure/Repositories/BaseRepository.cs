using Base.Domain.Entities;
using Base.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Infrastructure.Repositories;
public abstract class BaseRepository<T> : IBaseRepository<T>
    where T : BaseEntity
{
    #region Constants
    protected ILogger Logger { get; private set; }
    protected EfContext EfContext { get; private set; }
    protected IHttpContextAccessor HttpContextAccessor { get; private set; }
    private readonly Type ForeignKeyAttributeType = typeof(ForeignKeyAttribute);
    #endregion

    #region Constructors
    protected BaseRepository(ILogger logger
        , EfContext efContext
        , IHttpContextAccessor httpContextAccessor)
    {
        Logger = logger;
        EfContext = efContext;
        HttpContextAccessor = httpContextAccessor;
    }
    #endregion

    #region Methods
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            var valueTask = await EfContext.AddAsync(entity, cancellationToken);
            _ = await EfContext.SaveChangesAsync(cancellationToken);
            return valueTask.Entity ?? Activator.CreateInstance<T>();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return Activator.CreateInstance<T>();
        }
    }

    public virtual async Task<ushort> AddRangeAsync(IEnumerable<T> list, CancellationToken cancellationToken)
    {
        var executionStrategy = EfContext.Database.CreateExecutionStrategy();
        var count = 0;
        await executionStrategy.ExecuteAsync(
            operation: async (cancellationToken) =>
            {
                using var transaction = await EfContext.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    await EfContext.AddRangeAsync(list, cancellationToken);
                    count = await EfContext.SaveChangesAsync(cancellationToken);

                    if (count == list.Count())
                    {
                        await transaction.CommitAsync(cancellationToken);
                    }
                    else
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        count = 0;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, ex.ToString());
                    await transaction.RollbackAsync(cancellationToken);
                    count = 0;
                }
            }
            , cancellationToken: cancellationToken);

        return (ushort)count;
    }
    public virtual async Task<ulong> DeleteAsync(ulong id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await EfContext.FindAsync<T>(id, cancellationToken);

            if (entity == null)
            {
                return 0;
            }

            var entityEntry = EfContext.Remove(entity);

            if (entityEntry.Entity == null)
            {
                return 0;
            }

            _ = await EfContext.SaveChangesAsync(cancellationToken);
            return entityEntry.Entity.Id;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return 0;
        }
    }

    public virtual async Task<T> GetAsync(ulong id, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await EfContext
                .Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.IsActive
                    && x.Id == id
                , cancellationToken)
                ?? Activator.CreateInstance<T>();
            return entity;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return Activator.CreateInstance<T>();
        }
    }

    public virtual async Task<IEnumerable<T>> ListAsync(ulong[] ids, CancellationToken cancellationToken)
    {
        try
        {
            var entities = await EfContext
                .Set<T>()
                .Where(x => ids.Contains(x.Id)
                    && x.IsActive)
                .ToListAsync(cancellationToken);

            return entities;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return [];
        }
    }

    public virtual async Task<BaseListEntity<T>> ListAsync(uint pageNumber, ushort pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var offset = (int)((pageNumber - 1) * pageSize);
            var list = await EfContext
                .Set<T>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new BaseListEntity<T>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                List = list ?? []
            };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return new BaseListEntity<T>();
        }
    }

    public virtual async Task<BaseListEntity<T>> ListAsNoTrackingAsync(uint pageNumber
        , ushort pageSize, CancellationToken cancellationToken)
    {
        try
        {
            var offset = (int)((pageNumber - 1) * pageSize);
            var list = await EfContext
                .Set<T>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new BaseListEntity<T>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                List = list ?? []
            };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return new BaseListEntity<T>();
        }
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            if (EfContext.Entry(entity).State == EntityState.Detached)
            {
                var source = (await EfContext.FindAsync<T>([entity.Id], cancellationToken))
                    ?? throw new KeyNotFoundException($"Source entity not found ({entity.GetType()}, {nameof(entity.Id)} = {entity.Id}).");

                var propertyInfos = typeof(T)
                    .GetProperties()
                    .Where(x => x.CanRead
                        && x.CanWrite
                        && !x.CustomAttributes
                            .Any(y => y.AttributeType == ForeignKeyAttributeType));

                foreach (var pi in propertyInfos)
                {
                    pi.SetValue(source, pi.GetValue(entity));
                }

                entity = source;
            }
            else
            {
                EfContext.Entry(entity).State = EntityState.Modified;
            }

            EfContext.Entry(entity).Property(p => p.Id).IsModified = false;
            EfContext.Entry(entity).Property(p => p.CreationDate).IsModified = false;
            var entryEntity = EfContext.Update(entity);
            _ = await EfContext.SaveChangesAsync(cancellationToken);

            return entryEntity.Entity;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.ToString());
            return Activator.CreateInstance<T>();
        }
    }
    #endregion
}
