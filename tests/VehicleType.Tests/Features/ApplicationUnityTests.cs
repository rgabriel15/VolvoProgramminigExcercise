using Base.Tests.Features;
using Base.Tests;
using VehicleType.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using VehicleType.Application.DTOs;
using VehicleType.Application.Interfaces.Services;
using Base.Domain.Entities;

namespace VehicleType.Tests.Features;
public sealed class ApplicationUnityTests : BaseApplicationUnityTests<VehicleTypeEntity, VehicleTypeDto>
{
    #region Constructors
    public ApplicationUnityTests()
        : base(typeof(IVehicleTypeService))
    {
    }
    #endregion

    #region Methods
    public override async Task<ushort> LoadMockDataAsync(IServiceProvider serviceProvider)
    {
        //Disabled because it is added in Web.API.Program.cs
        //var count = await Mock.LoadAsync(serviceProvider);
        //return count;
        return 0;
    }

    [Fact]
    public override async Task AddAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = serviceProvider.GetRequiredService<IVehicleTypeService>();
        var newDto = new VehicleTypeDto
        {
            Name = Helper.Helper.GetRandomString(100),
            NumberOfPassengers = byte.MaxValue,
        };
        newDto = await service.AddAsync(newDto);
        Assert.NotNull(newDto);
        Assert.True(newDto.Id > 0);
        var added = await service.GetAsync(newDto.Id);
        Assert.NotNull(added);
        Assert.Equal(newDto.Id, added.Id);
        Assert.Equal(newDto.Name, added.Name);
        Assert.Equal(newDto.NumberOfPassengers, added.NumberOfPassengers);
    }

    [Fact]
    public async Task GetByNameAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var service = (IVehicleTypeService)serviceProvider.GetRequiredService(ServiceType);
        var list = await service.ListAsync(pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.False(string.IsNullOrWhiteSpace(first.Name));
        var entity = await service.GetAsync(first.Id);
        Assert.NotNull(entity);
        Assert.Equal(first.Id, entity.Id);
        Assert.Equal(first.Name, entity.Name);
    }
    #endregion
}
