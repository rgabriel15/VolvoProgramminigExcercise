using System.Diagnostics;
using System.Text.Json;
using Base.Application.Interfaces.Mappers;
using ClientException.Application.DTOs;
using ClientException.Application.Interfaces.Services;
using ClientException.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace ClientException.Tests.Features;
public static class Mock
{
    #region Methods
    public static async Task<ushort> LoadAsync(IServiceProvider serviceProvider)
    {
        var fileName = "clientException.json";
        var path = new FileInfo(Environment.ProcessPath!).Directory!.FullName;
        path = Path.Combine(path, "Assets", fileName);

        if (!File.Exists(path))
        {
            var msg = $"Mock file not found. Path: {path}";
            Debug.WriteLine(msg);
            throw new FileNotFoundException(msg, path);
        }

        var json = await File.ReadAllTextAsync(path);
        var entityList = JsonSerializer.Deserialize<List<ClientExceptionEntity>>(json);

        if ((entityList?.Count ?? 0) < 1)
        {
            var msg = $"Mock file has no valid records. Path: {path}";
            Debug.WriteLine(msg);
            throw new InvalidOperationException(msg);
        }

        var dtoList = new List<ClientExceptionDto>();
        var mapper = serviceProvider.GetRequiredService<IBaseMapper<ClientExceptionEntity, ClientExceptionDto>>();

        foreach (var entity in entityList!)
        {
            var dto = mapper.ToDto(entity);
            dtoList.Add(dto);
        }

        var service = serviceProvider.GetRequiredService<IClientExceptionService>();

        var count = await service.AddRangeAsync(dtoList!);
        Debug.WriteLine($"Mock: loaded {count} registers from file {path}");
        return count;
    }
    #endregion
}
