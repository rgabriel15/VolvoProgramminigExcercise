using Base.Domain.Entities;
using Base.Domain.Interfaces.Repositories;
using Base.Infrastructure;
using Base.Tests.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Tests.Features;
public abstract class BaseInfrastructureUnityTests<T> : IBaseInfrastructureUnityTests
    where T : BaseEntity
{
    #region Constants
    protected Type RepositoryType { get; private set; }
    #endregion

    #region Constructors
    protected BaseInfrastructureUnityTests(Type repositoryType)
    {
        RepositoryType = repositoryType;
    }
    #endregion

    #region Methods
    public abstract Task<ushort> LoadMockDataAsync(IServiceProvider serviceProvider);

    [Fact]
    public virtual async Task GetAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var repository = (IBaseRepository<T>)serviceProvider.GetRequiredService(RepositoryType);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var list = await repository.ListAsNoTrackingAsync(
            pageNumber: BaseListEntity<T>.DefaultPageNumber
            , pageSize: BaseListEntity<T>.DefaultPageSize
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        var entity = await repository.GetAsync(first.Id, cancellationToken: cancellationTokenSource.Token);
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
        var repository = (IBaseRepository<T>)serviceProvider.GetRequiredService(RepositoryType);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = await repository.ListAsNoTrackingAsync(
            pageNumber: BaseListEntity<T>.DefaultPageNumber
            , pageSize: BaseListEntity<T>.DefaultPageSize
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(entity);
        Assert.NotEmpty(entity.List);
        Assert.All(entity.List, x => Assert.True(
            x.IsActive
            && x.Id > 0));
    }

    [Fact]
    public virtual async Task AddAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var repository = (IBaseRepository<T>)serviceProvider.GetRequiredService(RepositoryType);
        var newEntity = Activator.CreateInstance<T>();
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        newEntity = await repository.AddAsync(newEntity, cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(newEntity);
        Assert.True(newEntity.Id > 0);
        var entity = await repository.GetAsync(newEntity.Id, cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(entity);
        Assert.Equal(newEntity.Id, entity.Id);
    }

    [Fact]
    public virtual async Task UpdateAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var repository = (IBaseRepository<T>)serviceProvider.GetRequiredService(RepositoryType);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var list = await repository.ListAsync(
            pageNumber: BaseListEntity<T>.DefaultPageNumber
            , pageSize: BaseListEntity<T>.DefaultPageSize
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        first.IsActive = !first.IsActive;

        var updated = await repository.UpdateAsync(first, cancellationToken: cancellationTokenSource.Token);
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
        var repository = (IBaseRepository<T>)serviceProvider.GetRequiredService(RepositoryType);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var list = await repository.ListAsNoTrackingAsync(
            pageNumber: BaseListEntity<T>.DefaultPageNumber
            , pageSize: BaseListEntity<T>.DefaultPageSize
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);

        var deletedId = await repository.DeleteAsync(first.Id, cancellationToken: cancellationTokenSource.Token);
        Assert.Equal(first.Id, deletedId);
        var deleted = await repository.GetAsync(first.Id, cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(deleted);
        Assert.Equal((ulong)0, deleted.Id);
    }
    #endregion
}
