using Base.Tests.Features;
using Base.Tests;
using Chassis.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Chassis.Application.DTOs;
using Chassis.Application.Interfaces.Services;
using Base.Domain.Entities;

namespace Chassis.Tests.Features;
public sealed class ApplicationUnityTests : BaseApplicationUnityTests<ChassisEntity, ChassisDto>
{
    #region Constructors
    public ApplicationUnityTests()
        : base(typeof(IChassisService))
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
        var service = serviceProvider.GetRequiredService<IChassisService>();
        var newDto = new ChassisDto
        {
            ChassisNumber = uint.MaxValue,
            ChassisSeries = Helper.Helper.GetRandomString(100),
        };
        newDto = await service.AddAsync(newDto);
        Assert.NotNull(newDto);
        Assert.True(newDto.Id > 0);
        var added = await service.GetAsync(newDto.Id);
        Assert.NotNull(added);
        Assert.Equal(newDto.Id, added.Id);
        Assert.Equal(newDto.ChassisSeries, added.ChassisSeries);
        Assert.Equal(newDto.ChassisNumber, added.ChassisNumber);
    }

    [Fact]
    public async Task GetByChassisSeriesAndNumberAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var chassisService = (IChassisService)serviceProvider.GetRequiredService(ServiceType);
        var list = await chassisService.ListAsync(pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.False(string.IsNullOrWhiteSpace(first.ChassisSeries));
        Assert.True(first.ChassisNumber > 0);

        var dto = await chassisService.GetByChassisSeriesAndNumberAsync(
            chassisSeries: first.ChassisSeries
            , chassisNumber: first.ChassisNumber);
        Assert.NotNull(dto);
        Assert.Equal(first.Id, dto.Id);
        Assert.Equal(first.ChassisSeries, dto.ChassisSeries);
        Assert.Equal(first.ChassisNumber, dto.ChassisNumber);
    }

    [Fact]
    public async Task ListUnassignedAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = (IChassisService)serviceProvider.GetRequiredService(ServiceType);
        var dto = await service.ListUnassignedAsync(
            pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize);
        Assert.NotNull(dto);
        Assert.NotEmpty(dto.List);
        Assert.All(dto.List, x => Assert.True(
            x.IsActive
            && x.Id > 0));
    }
    #endregion
}
