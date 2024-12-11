using Base.Tests.Features;
using Base.Tests;
using ClientException.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using ClientException.Application.DTOs;
using ClientException.Application.Interfaces.Services;

namespace ClientException.Tests.Features;
public sealed class ApplicationUnityTests : BaseApplicationUnityTests<ClientExceptionEntity, ClientExceptionDto>
{
    #region Constructors
    public ApplicationUnityTests()
        : base(typeof(IClientExceptionService))
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
        var service = serviceProvider.GetRequiredService<IClientExceptionService>();
        var utcNow = DateTime.UtcNow;
        var newDto = new ClientExceptionDto
        {
            ErrorMessage = Helper.Helper.GetRandomString(2000),
            StackTrace = Helper.Helper.GetRandomString(2000),
            ClientAppName = Helper.Helper.GetRandomString(40),
            Date = utcNow,
        };
        newDto = await service.AddAsync(newDto);
        Assert.NotNull(newDto);
        Assert.True(newDto.Id > 0);
        var dto = await service.GetAsync(newDto.Id);
        Assert.NotNull(dto);
        Assert.Equal(newDto.Id, dto.Id);
    }
    #endregion
}
