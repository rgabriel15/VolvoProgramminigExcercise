using Base.Infrastructure;
using Base.Tests;
using Base.Tests.Features;
using Vehicle.Domain.Entities;
using Vehicle.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Base.Domain.Entities;
using Chassis.Domain.Interfaces.Repositories;

namespace Vehicle.Tests.Features;
public sealed class InfrastructureUnityTests : BaseInfrastructureUnityTests<VehicleEntity>
{
    #region Constructors
    public InfrastructureUnityTests()
        : base(typeof(IVehicleRepository))
    {
    }
    #endregion

    #region Methods
    public override async Task<ushort> LoadMockDataAsync(IServiceProvider serviceProvider)
    {
        var count = await Mock.LoadAsync(serviceProvider);
        _ = await Chassis.Tests.Features.Mock.LoadAsync(serviceProvider);
        return count;
    }

    [Fact]
    public override async Task AddAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var repository = serviceProvider.GetRequiredService<IVehicleRepository>();
        var utcNow = DateTime.UtcNow;
        var newEntity = new VehicleEntity
        {
            IsActive = true,
            CreationDate = utcNow,
            UpdateDate = utcNow,
            ChassisId = 4,
            VehicleTypeId = 1,
            Color = System.Drawing.Color.White.Name,
        };

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        newEntity = await repository.AddAsync(newEntity
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(newEntity);
        Assert.True(newEntity.Id > 0);
        var entity = await repository.GetAsync(newEntity.Id, cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(entity);
        Assert.Equal(newEntity.Id, entity.Id);
    }

    [Fact]
    public async Task GetByChassisIdAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var repository = (IVehicleRepository)serviceProvider.GetRequiredService(RepositoryType);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var list = await repository.ListAsNoTrackingAsync(
            pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.True(first.ChassisId > 0);
        var entity = await repository.GetByChassisIdAsync(first.ChassisId, cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(entity);
        Assert.Equal(first.Id, entity.Id);
        Assert.Equal(first.ChassisId, entity.ChassisId);
    }

    [Fact]
    public async Task GetByChassisSeriesAndNumberAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var vehicleRepository = (IVehicleRepository)serviceProvider.GetRequiredService(RepositoryType);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var list = await vehicleRepository.ListAsNoTrackingAsync(
            pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.True(first.ChassisId > 0);

        var chassisRepository = serviceProvider.GetRequiredService<IChassisRepository>();
        var chassis = await chassisRepository.GetAsync(first.ChassisId, cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(chassis);
        Assert.Equal(first.ChassisId, chassis.Id);
        Assert.False(string.IsNullOrWhiteSpace(chassis.ChassisSeries));
        Assert.True(chassis.ChassisNumber > 0);

        var entity = await vehicleRepository.GetByChassisSeriesAndNumberAsync(
            chassisSeries: chassis.ChassisSeries
            , chassisNumber: chassis.ChassisNumber
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(entity);
        Assert.Equal(first.Id, entity.Id);
        Assert.Equal(first.ChassisId, entity.ChassisId);
    }

    [Fact]
    public async Task ListByVehicleTypeIdAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var repository = (IVehicleRepository)serviceProvider.GetRequiredService(RepositoryType);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var list = await repository.ListAsNoTrackingAsync(
            pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        Assert.All(list.List, x => Assert.True(
            x.IsActive
            && x.Id > 0
            && x.VehicleTypeId > 0));

        var first = list.List.First();

        list = await repository.ListByVehicleTypeIdAsync(
            vehicleTypeId: first.VehicleTypeId
            , pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        Assert.All(list.List, x => Assert.True(
            x.IsActive
            && x.Id > 0
            && x.VehicleTypeId > 0));
        Assert.Contains(first.Id, list.List.Select(x => x.Id));
    }
    #endregion
}
