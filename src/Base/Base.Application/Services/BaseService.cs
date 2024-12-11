using Base.Application.DTOs;
using Base.Application.Interfaces.Mappers;
using Base.Application.Interfaces.Services;
using Base.Domain.Entities;
using Base.Domain.Interfaces.Repositories;
using Base.Infrastructure;
using Serilog;

namespace Base.Application.Services;
public abstract class BaseService<TEntity, TDto> : IBaseService<TDto>
    where TEntity : BaseEntity
    where TDto : BaseDto
{
    #region Constants
    protected ILogger Logger { get; private set; }
    protected IBaseRepository<TEntity> Repository { get; private set; }
    protected IBaseMapper<TEntity, TDto> Mapper { get; private set; }
    #endregion

    #region Constructors
    protected BaseService(ILogger logger
        , IBaseRepository<TEntity> repository
        , IBaseMapper<TEntity, TDto> mapper)
    {
        Logger = logger;
        Repository = repository;
        Mapper = mapper;
    }
    #endregion

    #region Methods
    public virtual async Task<TDto> AddAsync(TDto dto)
    {
        var utcNow = DateTime.UtcNow;
        dto.IsActive = true;
        dto.CreationDate = utcNow;
        dto.UpdateDate = utcNow;
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = Mapper.ToEntity(dto);
        var newEntity = await Repository.AddAsync(entity, cancellationToken: cancellationTokenSource.Token);

        if (newEntity.Id < 1)
        {
            return Activator.CreateInstance<TDto>();
        }

        var newDto = Mapper.ToDto(newEntity);
        return newDto;
    }

    public virtual async Task<ushort> AddRangeAsync(IEnumerable<TDto> list)
    {
        if (list?.Any() != true)
        {
            return 0;
        }

        var utcNow = DateTime.UtcNow;
        var entityList = new List<TEntity>();

        foreach (var dto in list)
        {
            dto.IsActive = true;
            dto.CreationDate = utcNow;
            dto.UpdateDate = utcNow;

            var entity = Mapper.ToEntity(dto);
            entityList.Add(entity);
        }

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var count = await Repository.AddRangeAsync(entityList, cancellationToken: cancellationTokenSource.Token);
        return count;
    }

    public virtual async Task<ulong> DeleteAsync(ulong id)
    {
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var deletedId = await Repository.DeleteAsync(id, cancellationToken: cancellationTokenSource.Token);

        return deletedId;
    }

    public virtual async Task<TDto> GetAsync(ulong id)
    {
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = await Repository.GetAsync(id, cancellationToken: cancellationTokenSource.Token);

        if (entity.Id < 1)
        {
            return Activator.CreateInstance<TDto>();
        }

        var dto = Mapper.ToDto(entity);
        return dto;
    }

    public virtual async Task<BaseListDto<TDto>> ListAsync(uint pageNumber, ushort pageSize)
    {
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        if (pageSize > BaseListEntity<TEntity>.MaxPageSize)
        {
            pageSize = BaseListEntity<TEntity>.MaxPageSize;
        }

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entityList = await Repository.ListAsync(pageNumber: pageNumber, pageSize: pageSize, cancellationToken: cancellationTokenSource.Token);

        var dtoList = new BaseListDto<TDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        if ((entityList?.List?.Count ?? 0) == 0)
        {
            return dtoList;
        }

        var list = new List<TDto>();

        foreach (var entity in entityList!.List)
        {
            var dto = Mapper.ToDto(entity);
            list.Add(dto);
        }

        dtoList.List = list;
        return dtoList;
    }

    public virtual async Task<IReadOnlyCollection<TDto>> ListAsync(ulong[] ids)
    {
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entities = await Repository.ListAsync(ids, cancellationToken: cancellationTokenSource.Token);

        if (entities == null)
        {
            return [];
        }

        var dtos = new List<TDto>();

        foreach (var entity in entities)
        {
            var dto = Mapper.ToDto(entity);
            dtos.Add(dto);
        }

        return dtos.AsReadOnly();
    }

    public virtual async Task<BaseListDto<TDto>> ListAsNoTrackingAsync(uint pageNumber, ushort pageSize)
    {
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        if (pageSize > BaseListEntity<TEntity>.MaxPageSize)
        {
            pageSize = BaseListEntity<TEntity>.MaxPageSize;
        }

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entityList = await Repository.ListAsNoTrackingAsync(pageNumber: pageNumber, pageSize: pageSize, cancellationToken: cancellationTokenSource.Token);

        var dtoList = new BaseListDto<TDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        if ((entityList?.List?.Count ?? 0) == 0)
        {
            return dtoList;
        }

        var list = new List<TDto>();

        foreach (var entity in entityList!.List)
        {
            var dto = Mapper.ToDto(entity);
            list.Add(dto);
        }

        dtoList.List = list;
        return dtoList;
    }

    public virtual async Task<ulong> LogicalDeleteAsync(ulong id)
    {
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = await Repository.GetAsync(id, cancellationToken: cancellationTokenSource.Token);

        if (entity == null)
        {
            return 0;
        }

        entity.IsActive = false;
        entity.UpdateDate = DateTime.UtcNow;

        var updatedEntity = await Repository.UpdateAsync(entity, cancellationToken: cancellationTokenSource.Token);

        return updatedEntity?.Id ?? 0;
    }

    public virtual async Task<TDto> UpdateAsync(TDto dto)
    {
        dto.UpdateDate = DateTime.UtcNow;

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = Mapper.ToEntity(dto);
        var newEntity = await Repository.UpdateAsync(entity, cancellationToken: cancellationTokenSource.Token);

        if (newEntity.Id < 1)
        {
            return Activator.CreateInstance<TDto>();
        }

        var newDto = Mapper.ToDto(newEntity!);
        return newDto;
    }
    #endregion
}
