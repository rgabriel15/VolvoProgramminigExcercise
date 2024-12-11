using System.Diagnostics;
using System.Text.Json;
using Base.Application.Interfaces.Mappers;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interfaces.Services;
using Vehicle.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Vehicle.Tests.Features;
public static class Mock
{
    #region Methods
    public static async Task<ushort> LoadAsync(IServiceProvider serviceProvider)
    {
        var fileName = "vehicle.json";
        var path = new FileInfo(Environment.ProcessPath!).Directory!.FullName;
        path = Path.Combine(path, "Assets", fileName);

        if (!File.Exists(path))
        {
            var msg = $"Mock file not found. Path: {path}";
            Debug.WriteLine(msg);
            throw new FileNotFoundException(msg, path);
        }

        var json = await File.ReadAllTextAsync(path);
        var entityList = JsonSerializer.Deserialize<List<VehicleEntity>>(json);

        if ((entityList?.Count ?? 0) < 1)
        {
            var msg = $"Mock file has no valid records. Path: {path}";
            Debug.WriteLine(msg);
            throw new InvalidOperationException(msg);
        }

        var dtoList = new List<VehicleDto>();
        var mapper = serviceProvider.GetRequiredService<IBaseMapper<VehicleEntity, VehicleDto>>();

        foreach (var entity in entityList!)
        {
            var dto = mapper.ToDto(entity);
            dtoList.Add(dto);
        }

        var service = serviceProvider.GetRequiredService<IVehicleService>();

        var count = await service.AddRangeAsync(dtoList!);
        Debug.WriteLine($"Mock: loaded {count} registers from file {path}");
        return count;
    }
    #endregion
}
