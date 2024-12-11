using Base.Tests;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ClientException.Application.DTOs;

namespace ClientException.Tests.Features;

public sealed class IntegrationTests
{
    #region Constants
    private const string BaseUrl = @"/api/v1/clientException";
    #endregion

    #region Constructors
    public IntegrationTests()
    {
    }
    #endregion

    #region Methods
    [Fact]
    public async Task PostAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await Mock.LoadAsync(serviceProvider);
        using var client = app.CreateClient();
        var jsonOptions = serviceProvider.GetService<IOptions<JsonOptions>>();
        var utcNow = DateTime.UtcNow;
        var newDto = new ClientExceptionDto
        {
            ErrorMessage = Helper.Helper.GetRandomString(2000),
            StackTrace = Helper.Helper.GetRandomString(2000),
            ClientAppName = Helper.Helper.GetRandomString(40),
            Date = utcNow,
        };

        var res = await client.PostAsJsonAsync(BaseUrl, newDto);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        var added = JsonSerializer.Deserialize<ClientExceptionDto>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(added);
        Assert.True(added.Id > 0);
    }
    #endregion
}
