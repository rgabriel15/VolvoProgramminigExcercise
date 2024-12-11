using Base.Tests;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Chassis.Application.DTOs;
using Base.Tests.Features;
using Base.Application.DTOs;

namespace Chassis.Tests.Features;

public sealed class IntegrationTests : BaseIntegrationTests<ChassisDto>
{
    #region Constants
    private const string BaseUrl = @"/api/v1/chassis";
    #endregion

    #region Constructors
    public IntegrationTests()
        : base(BaseUrl)
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
    public override async Task PostAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        using var client = app.CreateClient();
        var jsonOptions = serviceProvider.GetService<IOptions<JsonOptions>>();
        var newDto = new ChassisDto
        {
            ChassisNumber = uint.MaxValue,
            ChassisSeries = Helper.Helper.GetRandomString(100),
        };

        var res = await client.PostAsJsonAsync(BaseUrl, newDto);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        var added = JsonSerializer.Deserialize<ChassisDto>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(added);
        Assert.True(added.Id > 0);
        Assert.Equal(newDto.ChassisNumber, added.ChassisNumber);
        Assert.Equal(newDto.ChassisSeries, added.ChassisSeries);
    }

    [Fact]
    public async Task GetByChassisSeriesAndNumberAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        using var client = app.CreateClient();
        var jsonOptions = serviceProvider.GetService<IOptions<JsonOptions>>();

        var url = $"{BaseUrl}/list";
        var res = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<BaseListDto<ChassisDto>>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.False(string.IsNullOrWhiteSpace(first.ChassisSeries));
        Assert.True(first.ChassisNumber > 0);

        url = $"{BaseUrl}/GetByChassisSeriesAndNumber?chassisSeries={first.ChassisSeries}&chassisNumber={first.ChassisNumber}";
        res = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        json = await res.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<ChassisDto>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(dto);
        Assert.Equal(first.Id, dto.Id);
    }

    [Fact]
    public async Task ListUnassignedAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        using var client = app.CreateClient();
        var jsonOptions = serviceProvider.GetService<IOptions<JsonOptions>>();

        var url = $"{BaseUrl}/ListUnassigned";
        var res = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<BaseListDto<ChassisDto>>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(dto);
        Assert.NotEmpty(dto.List);
        Assert.All(dto.List, x => Assert.True(
            x.IsActive
            && x.Id > 0));
    }
    #endregion
}
