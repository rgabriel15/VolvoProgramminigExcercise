using Base.Infrastructure;
using Base.Tests;
using Base.Tests.Features;
using ClientException.Domain.Entities;
using ClientException.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ClientException.Tests.Features;
public sealed class InfrastructureUnityTests : BaseInfrastructureUnityTests<ClientExceptionEntity>
{
    #region Constructors
    public InfrastructureUnityTests()
        : base(typeof(IClientExceptionRepository))
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
        var repository = serviceProvider.GetRequiredService<IClientExceptionRepository>();
        var utcNow = DateTime.UtcNow;
        var newEntity = new ClientExceptionEntity
        {
            IsActive = true,
            CreationDate = utcNow,
            UpdateDate = utcNow,
            ErrorMessage = Helper.Helper.GetRandomString(2000),
            StackTrace = Helper.Helper.GetRandomString(2000),
            ClientAppName = Helper.Helper.GetRandomString(40),
            Date = utcNow,
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
    #endregion
}
