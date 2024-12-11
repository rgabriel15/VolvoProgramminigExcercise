using Base.Domain.Entities;
using Base.Infrastructure;
using Base.Tests;
using Base.Tests.Features;
using Chassis.Domain.Entities;
using Chassis.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Chassis.Tests.Features;
public sealed class InfrastructureUnityTests : BaseInfrastructureUnityTests<ChassisEntity>
{
    #region Constructors
    public InfrastructureUnityTests()
        : base(typeof(IChassisRepository))
    {
    }
    #endregion

    #region Methods
    public override async Task<ushort> LoadMockDataAsync(IServiceProvider serviceProvider)
    {
        var count = await Mock.LoadAsync(serviceProvider);
        return count;
    }

    [Fact]
    public override async Task AddAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var repository = serviceProvider.GetRequiredService<IChassisRepository>();
        var newEntity = new ChassisEntity
        {
            ChassisNumber = uint.MaxValue,
            ChassisSeries = Helper.Helper.GetRandomString(100),
        };

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        newEntity = await repository.AddAsync(newEntity
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(newEntity);
        Assert.True(newEntity.Id > 0);
        var added = await repository.GetAsync(newEntity.Id, cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(added);
        Assert.Equal(newEntity.Id, added.Id);
        Assert.Equal(newEntity.ChassisNumber, added.ChassisNumber);
        Assert.Equal(newEntity.ChassisSeries, added.ChassisSeries);
    }

    [Fact]
    public async Task GetByChassisSeriesAndNumberAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var chassisRepository = (IChassisRepository)serviceProvider.GetRequiredService(RepositoryType);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var list = await chassisRepository.ListAsNoTrackingAsync(
            pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.False(string.IsNullOrWhiteSpace(first.ChassisSeries));
        Assert.True(first.ChassisNumber > 0);

        var entity = await chassisRepository.GetByChassisSeriesAndNumberAsync(
            chassisSeries: first.ChassisSeries
            , chassisNumber: first.ChassisNumber
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(entity);
        Assert.Equal(first.Id, entity.Id);
        Assert.Equal(first.ChassisSeries, entity.ChassisSeries);
        Assert.Equal(first.ChassisNumber, entity.ChassisNumber);
    }

    [Fact]
    public async Task ListUnassignedAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var repository = (IChassisRepository)serviceProvider.GetRequiredService(RepositoryType);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = await repository.ListUnassignedAsync(
            pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(entity);
        Assert.NotEmpty(entity.List);
        Assert.All(entity.List, x => Assert.True(
            x.IsActive
            && x.Id > 0));
    }
    #endregion
}
