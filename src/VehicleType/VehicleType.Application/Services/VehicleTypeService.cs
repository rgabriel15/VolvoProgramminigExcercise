using Base.Application.Services;
using Serilog;
using VehicleType.Application.DTOs;
using VehicleType.Application.Interfaces.Services;
using VehicleType.Domain.Entities;
using VehicleType.Domain.Interfaces.Repositories;
using Base.Application.Interfaces.Mappers;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.Json;
using Base.Infrastructure;
using Vehicle.Application.Interfaces.Services;
using Base.Domain.Entities;
using Base.Application.Interfaces.Services;
using Base.Application.DTOs;
using System.Runtime.InteropServices;
using System.IO;

namespace VehicleType.Application.Services;
public sealed class VehicleTypeService
    : BaseService<VehicleTypeEntity, VehicleTypeDto>
    , IVehicleTypeService
{
    #region Constants
    private readonly IVehicleService VehicleService;
    private readonly ICacheService CacheService;
    #endregion

    #region Constructors
    public VehicleTypeService(ILogger logger
        , IVehicleTypeRepository repository
        , IBaseMapper<VehicleTypeEntity, VehicleTypeDto> mapper
        , IVehicleService vehicleService
        , ICacheService cacheService)
        : base(logger, repository, mapper)
    {
        VehicleService = vehicleService;
        CacheService = cacheService;
    }
    #endregion

    #region Methods
    public override async Task<VehicleTypeDto> AddAsync(VehicleTypeDto dto)
    {
        dto.Name = dto.Name.Trim();
        var oldDto = await GetByNameAsync(dto.Name);

        if (oldDto.Id > 0)
        {
            return Activator.CreateInstance<VehicleTypeDto>();
        }

        var cacheKey = typeof(BaseListDto<VehicleTypeDto>).Name;
        var task = base.AddAsync(dto);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        await Task.WhenAll(
            CacheService.RemoveAsync(key: cacheKey, cancellationToken: cancellationTokenSource.Token)
            , task);
        var newDto = await task;
        return newDto;
    }

    public override async Task<ulong> DeleteAsync(ulong id)
    {
        var vehicles = await VehicleService.ListByVehicleTypeIdAsync(
            vehicleTypeId: id
            , pageNumber: BaseListEntity<BaseEntity>.DefaultPageNumber
            , pageSize: BaseListEntity<BaseEntity>.DefaultPageSize);

        if (vehicles.List.Count > 0)
        {
            return 0;
        }

        var cacheKey = typeof(BaseListDto<VehicleTypeDto>).Name;
        var task = base.DeleteAsync(id);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        await Task.WhenAll(
            CacheService.RemoveAsync(key: cacheKey, cancellationToken: cancellationTokenSource.Token)
            , task);
        var deletedId = await task;
        return deletedId;
    }

    public override async Task<BaseListDto<VehicleTypeDto>> ListAsNoTrackingAsync(uint pageNumber, ushort pageSize)
    {
        var cacheKey = typeof(BaseListDto<VehicleTypeDto>).Name;
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = await CacheService.GetAsync<BaseListDto<VehicleTypeDto>>(
            key: cacheKey
            , cancellationToken: cancellationTokenSource.Token);

        if (entity != null)
        {
            return entity;
        }

        entity = await base.ListAsNoTrackingAsync(pageNumber, pageSize);
        return entity;
    }

    public override async Task<VehicleTypeDto> UpdateAsync(VehicleTypeDto dto)
    {
        dto.Name = dto.Name.Trim();
        var oldDto = await GetByNameAsync(dto.Name);

        if (oldDto.Id > 0
            && dto.Id != oldDto.Id)
        {
            return Activator.CreateInstance<VehicleTypeDto>();
        }

        var cacheKey = typeof(BaseListDto<VehicleTypeDto>).Name;
        var task = base.UpdateAsync(dto);
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        await Task.WhenAll(
            CacheService.RemoveAsync(key: cacheKey, cancellationToken: cancellationTokenSource.Token)
            , task);
        var newDto = await task;
        return newDto;
    }

    public async Task<VehicleTypeDto> GetByNameAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Activator.CreateInstance<VehicleTypeDto>();
        }

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = await ((IVehicleTypeRepository)Repository).GetByNameAsync(
            name: name
            , cancellationToken: cancellationTokenSource.Token);

        if (entity.Id < 1)
        {
            return Activator.CreateInstance<VehicleTypeDto>();
        }

        var dto = Mapper.ToDto(entity);
        return dto;
    }

    public static async Task<ushort> MockAsync(IServiceProvider serviceProvider)
    {
        var fileName = "vehicleType.json";
        var path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(new FileInfo(Environment.ProcessPath!).Directory!.FullName, "Assets", fileName)
            : Path.Combine(Path.DirectorySeparatorChar.ToString(), Environment.CurrentDirectory, "Assets", fileName);

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
