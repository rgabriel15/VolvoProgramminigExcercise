using Base.Infrastructure;
using Base.Tests;
using Base.Tests.Features;
using VehicleType.Domain.Entities;
using VehicleType.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Base.Domain.Entities;

namespace VehicleType.Tests.Features;
public sealed class InfrastructureUnityTests : BaseInfrastructureUnityTests<VehicleTypeEntity>
{
    #region Constructors
    public InfrastructureUnityTests()
        : base(typeof(IVehicleTypeRepository))
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
        var repository = serviceProvider.GetRequiredService<IVehicleTypeRepository>();
        var utcNow = DateTime.UtcNow;
        var newEntity = new VehicleTypeEntity
        {
            IsActive = true,
            CreationDate = utcNow,
            UpdateDate = utcNow,
            Name = Helper.Helper.GetRandomString(100),
            NumberOfPassengers = byte.MaxValue,
        };

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        newEntity = await repository.AddAsync(newEntity
            , cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(newEntity);
        Assert.True(newEntity.Id > 0);
        var added = await repository.GetAsync(newEntity.Id, cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(added);
        Assert.Equal(newEntity.Id, added.Id);
        Assert.Equal(newEntity.Name, added.Name);
        Assert.Equal(newEntity.NumberOfPassengers, added.NumberOfPassengers);
    }

    [Fact]
    public async Task GetByNameAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        var repository = (IVehicleTypeRepository)serviceProvider.GetRequiredService(RepositoryType);
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
        Assert.False(string.IsNullOrWhiteSpace(first.Name));
        var entity = await repository.GetByNameAsync(name: first.Name, cancellationToken: cancellationTokenSource.Token);
        Assert.NotNull(entity);
        Assert.Equal(first.Id, entity.Id);
        Assert.Equal(first.Name, entity.Name);
    }
    #endregion
}
