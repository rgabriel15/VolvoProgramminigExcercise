using Base.Tests;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Vehicle.Application.DTOs;
using Base.Application.DTOs;
using Base.Tests.Features;
using Chassis.Application.DTOs;

namespace Vehicle.Tests.Features;

public sealed class IntegrationTests : BaseIntegrationTests<VehicleDto>
{
    #region Constants
    private const string BaseUrl = @"/api/v1/vehicle";
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
        _ = await Chassis.Tests.Features.Mock.LoadAsync(serviceProvider);
        _ = await VehicleType.Tests.Features.Mock.LoadAsync(serviceProvider);
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
        var newDto = new VehicleDto
        {
            ChassisId = uint.MaxValue,
            VehicleTypeId = uint.MaxValue,
            Color = System.Drawing.Color.White.Name,
        };

        var res = await client.PostAsJsonAsync(BaseUrl, newDto);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        var added = JsonSerializer.Deserialize<VehicleDto>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(added);
        Assert.True(added.Id > 0);
    }

    [Fact]
    public async Task PostWithExistingChassisIdAsync()
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
        var list = JsonSerializer.Deserialize<BaseListDto<VehicleDto>>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.True(first.ChassisId > 0);

        var newDto = new VehicleDto
        {
            ChassisId = first.ChassisId,
            VehicleTypeId = 1,
            Color = System.Drawing.Color.White.Name,
        };

        res = await client.PostAsJsonAsync(BaseUrl, newDto);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
    }

    [Fact]
    public async Task GetByChassisIdAsync()
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
        var list = JsonSerializer.Deserialize<BaseListDto<VehicleDto>>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);

        url = $"{BaseUrl}/GetByChassisId?chassisId={first.ChassisId}";
        res = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        json = await res.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<VehicleDto>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(dto);
        Assert.Equal(first.Id, dto.Id);
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
        var list = JsonSerializer.Deserialize<BaseListDto<VehicleDto>>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        Assert.True(first.ChassisId > 0);

        url = $"api/v1/chassis?id={first.ChassisId}";
        res = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        json = await res.Content.ReadAsStringAsync();
        var chassis = JsonSerializer.Deserialize<ChassisDto>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(chassis);
        Assert.Equal(first.Id, chassis.Id);
        Assert.False(string.IsNullOrWhiteSpace(chassis.ChassisSeries));
        Assert.True(chassis.ChassisNumber > 0);

        url = $"{BaseUrl}/GetByChassisSeriesAndNumber?chassisSeries={chassis.ChassisSeries}&chassisNumber={chassis.ChassisNumber}";
        res = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        json = await res.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<VehicleDto>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(dto);
        Assert.Equal(first.Id, dto.Id);
    }
    #endregion
}
