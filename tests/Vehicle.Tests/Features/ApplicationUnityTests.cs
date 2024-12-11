using Base.Domain.Entities;
using Base.Tests;
using Base.Tests.Features;
using Chassis.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interfaces.Services;
using Vehicle.Domain.Entities;

namespace Vehicle.Tests.Features;
public sealed class ApplicationUnityTests : BaseApplicationUnityTests<VehicleEntity, VehicleDto>
{
    #region Constructors
    public ApplicationUnityTests()
        : base(typeof(IVehicleService))
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
        var service = serviceProvider.GetRequiredService<IVehicleService>();
        var newDto = new VehicleDto
        {
            ChassisId = 4,
            VehicleTypeId = 1,
            Color = System.Drawing.Color.White.Name,
        };
        newDto = await service.AddAsync(newDto);
        Assert.NotNull(newDto);
        Assert.True(newDto.Id > 0);
        var dto = await service.GetAsync(newDto.Id);
        Assert.NotNull(dto);
        Assert.Equal(newDto.Id, dto.Id);
    }

    [Fact]
    public async Task ListByVehicleTypeIdAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = (IVehicleService)serviceProvider.GetRequiredService(ServiceType);
        var list = await service.ListAsync(
            pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        Assert.All(list.List, x => Assert.True(
            x.IsActive
            && x.Id > 0
            && x.VehicleTypeId > 0));

        var first = list.List.First();

        list = await service.ListByVehicleTypeIdAsync(
            vehicleTypeId: first.VehicleTypeId
            , pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        Assert.All(list.List, x => Assert.True(
            x.IsActive
            && x.Id > 0
            && x.VehicleTypeId > 0));
        Assert.Contains(first.Id, list.List.Select(x => x.Id));
    }

    [Fact]
    public async Task AddWithExistingChassisIdAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = serviceProvider.GetRequiredService<IVehicleService>();

        var list = await service.ListAsync(pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.True(first.ChassisId > 0);

        var newDto = new VehicleDto
        {
            ChassisId = first.ChassisId,
            VehicleTypeId = uint.MaxValue,
            Color = System.Drawing.Color.White.Name,
        };
        newDto = await service.AddAsync(newDto);
        Assert.NotNull(newDto);
        Assert.True(newDto.Id == 0);
    }

    [Fact]
    public async Task GetByChassisIdAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = (IVehicleService)serviceProvider.GetRequiredService(ServiceType);
        var list = await service.ListAsync(pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.True(first.ChassisId > 0);
        var dto = await service.GetByChassisIdAsync(first.ChassisId);
        Assert.NotNull(dto);
        Assert.Equal(first.Id, dto.Id);
        Assert.Equal(first.ChassisId, dto.ChassisId);
    }

    [Fact]
    public async Task GetByChassisSeriesAndNumberAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var vehicleService = (IVehicleService)serviceProvider.GetRequiredService(ServiceType);
        var list = await vehicleService.ListAsync(pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.True(first.ChassisId > 0);

        var chassisService = serviceProvider.GetRequiredService<IChassisService>();
        var chassis = await chassisService.GetAsync(first.ChassisId);
        Assert.NotNull(chassis);
        Assert.Equal(first.ChassisId, chassis.Id);
        Assert.False(string.IsNullOrWhiteSpace(chassis.ChassisSeries));
        Assert.True(chassis.ChassisNumber > 0);

        var dto = await vehicleService.GetByChassisSeriesAndNumberAsync(
            chassisSeries: chassis.ChassisSeries
            , chassisNumber: chassis.ChassisNumber);
        Assert.NotNull(dto);
        Assert.Equal(first.Id, dto.Id);
        Assert.Equal(first.ChassisId, dto.ChassisId);
    }
    #endregion
}
