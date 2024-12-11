using Base.Application.DTOs;
using Base.Application.Interfaces.Services;
using Base.Domain.Entities;
using Base.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Tests.Features;
public abstract class BaseApplicationUnityTests<TEntity, TDto> : IBaseApplicationUnityTests
    where TEntity : BaseEntity
    where TDto : BaseDto
{
    #region Constants
    protected Type ServiceType { get; private set; }
    #endregion

    #region Constructors
    protected BaseApplicationUnityTests(Type serviceType)
    {
        ServiceType = serviceType;
    }
    #endregion

    #region Methods
    public abstract Task<ushort> LoadMockDataAsync(IServiceProvider serviceProvider);

    [Fact]
    public virtual async Task AddAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = (IBaseService<TDto>)serviceProvider.GetRequiredService(ServiceType);
        var newEntity = Activator.CreateInstance<TDto>();
        newEntity = await service.AddAsync(newEntity);
        Assert.NotNull(newEntity);
        Assert.True(newEntity.Id > 0);
        var entity = await service.GetAsync(newEntity.Id);
        Assert.NotNull(entity);
        Assert.Equal(newEntity.Id, entity.Id);
    }

    [Fact]
    public virtual async Task GetAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = (IBaseService<TDto>)serviceProvider.GetRequiredService(ServiceType);
        var list = await service.ListAsync(pageNumber: BaseListEntity<TEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<TEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        var entity = await service.GetAsync(first.Id);
        Assert.NotNull(entity);
        Assert.Equal(first.Id, entity.Id);
    }

    [Fact]
    public virtual async Task ListAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = (IBaseService<TDto>)serviceProvider.GetRequiredService(ServiceType);
        var dto = await service.ListAsync(pageNumber: BaseListEntity<TEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<TEntity>.DefaultPageSize);
        Assert.NotNull(dto);
        Assert.NotEmpty(dto.List);
        Assert.All(dto.List, x => Assert.True(
            x.IsActive
            && x.Id > 0));
    }

    [Fact]
    public virtual async Task UpdateAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = (IBaseService<TDto>)serviceProvider.GetRequiredService(ServiceType);
        var list = await service.ListAsync(pageNumber: BaseListEntity<TEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<TEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        first.IsActive = !first.IsActive;

        var updated = await service.UpdateAsync(first);
        Assert.NotNull(updated);
        Assert.Equal(first.Id, updated.Id);
        Assert.Equal(first.IsActive, updated.IsActive);
    }

    [Fact]
    public virtual async Task DeleteAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = (IBaseService<TDto>)serviceProvider.GetRequiredService(ServiceType);
        var list = await service.ListAsync(pageNumber: BaseListEntity<TEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<TEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);

        var deletedId = await service.DeleteAsync(first.Id);
        Assert.Equal(first.Id, deletedId);
        var deleted = await service.GetAsync(first.Id);
        Assert.NotNull(deleted);
        Assert.Equal((ulong)0, deleted.Id);
    }

    [Fact]
    public virtual async Task LogicalDeleteAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = (IBaseService<TDto>)serviceProvider.GetRequiredService(ServiceType);
        var list = await service.ListAsync(pageNumber: BaseListEntity<TEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<TEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);

        var deletedId = await service.LogicalDeleteAsync(first.Id);
        Assert.Equal(first.Id, deletedId);
        var deleted = await service.GetAsync(first.Id);
        Assert.NotNull(deleted);
        Assert.Equal((ulong)0, deleted.Id);

    }
    #endregion
}
