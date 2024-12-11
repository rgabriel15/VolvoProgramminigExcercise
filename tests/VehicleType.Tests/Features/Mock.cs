using System.Diagnostics;
using System.Text.Json;
using Base.Application.Interfaces.Mappers;
using VehicleType.Application.DTOs;
using VehicleType.Application.Interfaces.Services;
using VehicleType.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace VehicleType.Tests.Features;
public static class Mock
{
    #region Methods
    public static async Task<ushort> LoadAsync(IServiceProvider serviceProvider)
    {
        var fileName = "vehicleType.json";
        var path = new FileInfo(Environment.ProcessPath!).Directory!.FullName;
        path = Path.Combine(path, "Assets", fileName);

        if (!File.Exists(path))
        {
            var msg = $"Mock file not found. Path: {path}";
            Debug.WriteLine(msg);
            throw new FileNotFoundException(msg, path);
        }

        var json = await File.ReadAllTextAsync(path);
        var entityList = JsonSerializer.Deserialize<List<VehicleTypeEntity>>(json);

        if ((entityList?.Count ?? 0) < 1)
        {
            var msg = $"Mock file has no valid records. Path: {path}";
            Debug.WriteLine(msg);
            throw new InvalidOperationException(msg);
        }

        var dtoList = new List<VehicleTypeDto>();
        var mapper = serviceProvider.GetRequiredService<IBaseMapper<VehicleTypeEntity, VehicleTypeDto>>();

        foreach (var entity in entityList!)
        {
            var dto = mapper.ToDto(entity);
            dtoList.Add(dto);
        }

        var service = serviceProvider.GetRequiredService<IVehicleTypeService>();

        var count = await service.AddRangeAsync(dtoList!);
        Debug.WriteLine($"Mock: loaded {count} registers from file {path}");
        return count;
    }
    #endregion
}
