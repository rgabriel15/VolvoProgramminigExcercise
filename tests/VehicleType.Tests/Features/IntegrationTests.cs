using Base.Tests;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using VehicleType.Application.DTOs;
using Base.Tests.Features;

namespace VehicleType.Tests.Features;

public sealed class IntegrationTests : BaseIntegrationTests<VehicleTypeDto>
{
    #region Constants
    private const string BaseUrl = @"/api/v1/vehicleType";
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
        //Disabled because it is added in Web.API.Program.cs
        //var count = await Mock.LoadAsync(serviceProvider);
        //return count;
        return 0;
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
        var newDto = new VehicleTypeDto
        {
            Name = Helper.Helper.GetRandomString(100),
            NumberOfPassengers = byte.MaxValue,
        };

        var res = await client.PostAsJsonAsync(BaseUrl, newDto);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        var added = JsonSerializer.Deserialize<VehicleTypeDto>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(added);
        Assert.True(added.Id > 0);
        Assert.Equal(newDto.Name, added.Name);
        Assert.Equal(newDto.NumberOfPassengers, added.NumberOfPassengers);
    }
    #endregion
}
