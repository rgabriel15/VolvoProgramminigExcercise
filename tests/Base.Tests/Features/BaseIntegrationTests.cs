using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Base.Application.DTOs;
using Base.Tests.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Base.Tests.Features;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "<Pending>")]
public abstract class BaseIntegrationTests<T> : IBaseIntegrationTests
    where T : BaseDto
{
    #region Constants
    private readonly string BaseUrl;
    #endregion

    #region Constructors
    protected BaseIntegrationTests(string baseUrl)
    {
        BaseUrl = !string.IsNullOrWhiteSpace(baseUrl) ? baseUrl : throw new ArgumentNullException(nameof(baseUrl));
    }
    #endregion

    #region Methods
    public abstract Task<ushort> LoadMockDataAsync(IServiceProvider serviceProvider);

    [Fact]
    public virtual async Task PostAsync()
    {
        using var app = new ApiApplicationFactory();
        await using var scope = app.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        _ = await LoadMockDataAsync(serviceProvider);
        using var client = app.CreateClient();
        var dto = Activator.CreateInstance<T>();
        var res = await client.PostAsJsonAsync(BaseUrl, dto);
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        var json = await res.Content.ReadAsStringAsync();
        var jsonOptions = serviceProvider.GetService<IOptions<JsonOptions>>();
        var added = JsonSerializer.Deserialize<T>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(added);
        Assert.True(added.Id > 0);
    }

    [Fact]
    public virtual async Task DeleteAsync()
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
        var list = JsonSerializer.Deserialize<BaseListDto<T>>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);

        url = $"{BaseUrl}?id={first.Id}";

        res = await client.DeleteAsync(url);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        json = await res.Content.ReadAsStringAsync();
        Assert.True(ulong.TryParse(json, out var deletedId));
        Assert.Equal(first.Id, deletedId);

        url = $"{BaseUrl}?id={first.Id}";
        res = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public virtual async Task GetAsync()
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
        var list = JsonSerializer.Deserialize<BaseListDto<T>>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);

        url = $"{BaseUrl}?id={first.Id}";
        res = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        json = await res.Content.ReadAsStringAsync();
        var entity = JsonSerializer.Deserialize<T>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(entity);
        Assert.Equal(first.Id, entity.Id);
    }

    [Fact]
    public virtual async Task ListAsync()
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
        var dto = JsonSerializer.Deserialize<BaseListDto<T>>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(dto);
        Assert.NotEmpty(dto.List);
        Assert.All(dto.List, x => Assert.True(
            x.IsActive
            && x.Id > 0));
    }

    [Fact]
    public virtual async Task LogicalDeleteAsync()
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
        var list = JsonSerializer.Deserialize<BaseListDto<T>>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);

        url = $"{BaseUrl}/logicalDelete?id={first.Id}";
        res = await client.DeleteAsync(url);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        json = await res.Content.ReadAsStringAsync();
        var deletedId = JsonSerializer.Deserialize<ulong>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.Equal(first.Id, deletedId);

        url = $"{BaseUrl}?id={first.Id}";
        res = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
    }

    [Fact]
    public virtual async Task UpdateAsync()
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
        var list = JsonSerializer.Deserialize<BaseListDto<T>>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(list);
        Assert.NotEmpty(list.List);
        var first = list.List.First();
        Assert.NotNull(first);
        Assert.True(first.Id > 0);
        first.IsActive = !first.IsActive;

        url = $"{BaseUrl}?id={first.Id}";
        res = await client.PutAsJsonAsync(url, first);
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        json = await res.Content.ReadAsStringAsync();
        var updated = JsonSerializer.Deserialize<T>(json, jsonOptions!.Value.JsonSerializerOptions);
        Assert.NotNull(updated);
        Assert.Equal(first.Id, updated.Id);
        Assert.Equal(first.IsActive, updated.IsActive);
    }
    #endregion
}
