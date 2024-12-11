using Base.Application.Services;
using Serilog;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interfaces.Services;
using Vehicle.Domain.Entities;
using Vehicle.Domain.Interfaces.Repositories;
using Base.Application.Interfaces.Mappers;
using Base.Infrastructure;
using Base.Application.DTOs;
using Base.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.Json;
using System.Runtime.InteropServices;

namespace Vehicle.Application.Services;
public sealed class VehicleService
    : BaseService<VehicleEntity, VehicleDto>
    , IVehicleService
{
    #region Constructors
    public VehicleService(ILogger logger
        , IVehicleRepository repository
        , IBaseMapper<VehicleEntity, VehicleDto> mapper)
        : base(logger, repository, mapper)
    {
    }
    #endregion

    #region Methods
    public override async Task<VehicleDto> AddAsync(VehicleDto dto)
    {
        var oldDto = await GetByChassisIdAsync(dto.ChassisId);

        if (oldDto.Id > 0)
        {
            return Activator.CreateInstance<VehicleDto>();
        }

        var newDto = await base.AddAsync(dto);
        return newDto;
    }

    public override async Task<VehicleDto> UpdateAsync(VehicleDto dto)
    {
        var oldDto = await GetByChassisIdAsync(dto.ChassisId);

        if (oldDto.Id > 0
            && oldDto.Id != dto.Id)
        {
            return Activator.CreateInstance<VehicleDto>();
        }

        var newDto = await base.UpdateAsync(dto);
        return newDto;
    }

    public async Task<VehicleDto> GetByChassisIdAsync(ulong chassisId)
    {
        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = await ((IVehicleRepository)Repository).GetByChassisIdAsync(chassisId, cancellationToken: cancellationTokenSource.Token);

        if (entity.Id < 1)
        {
            return Activator.CreateInstance<VehicleDto>();
        }

        var dto = Mapper.ToDto(entity);
        return dto;
    }

    public async Task<VehicleDto> GetByChassisSeriesAndNumberAsync(string chassisSeries, uint chassisNumber)
    {
        if (string.IsNullOrWhiteSpace(chassisSeries))
        {
            return Activator.CreateInstance<VehicleDto>();
        }

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = await ((IVehicleRepository)Repository).GetByChassisSeriesAndNumberAsync(
            chassisSeries: chassisSeries
            , chassisNumber: chassisNumber
            , cancellationToken: cancellationTokenSource.Token);

        if (entity.Id < 1)
        {
            return Activator.CreateInstance<VehicleDto>();
        }

        var dto = Mapper.ToDto(entity);
        return dto;
    }

    public async Task<BaseListDto<VehicleDto>> ListByVehicleTypeIdAsync(ulong vehicleTypeId, uint pageNumber, ushort pageSize)
    {
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        if (pageSize > BaseListEntity<BaseEntity>.MaxPageSize)
        {
            pageSize = BaseListEntity<BaseEntity>.MaxPageSize;
        }

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entityList = await ((IVehicleRepository)Repository).ListByVehicleTypeIdAsync(
            vehicleTypeId: vehicleTypeId
            , pageNumber: pageNumber
            , pageSize: pageSize
            , cancellationToken: cancellationTokenSource.Token);

        var dtoList = new BaseListDto<VehicleDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        if ((entityList?.List?.Count ?? 0) == 0)
        {
            return dtoList;
        }

        var list = new List<VehicleDto>();

        foreach (var entity in entityList!.List)
        {
            var dto = Mapper.ToDto(entity);
            list.Add(dto);
        }

        dtoList.List = list;
        return dtoList;
    }

    public static async Task<ushort> MockAsync(IServiceProvider serviceProvider)
    {
        var fileName = "vehicle.json";
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
