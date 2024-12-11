using Base.Application.Services;
using Serilog;
using Chassis.Application.DTOs;
using Chassis.Application.Interfaces.Services;
using Chassis.Domain.Entities;
using Chassis.Domain.Interfaces.Repositories;
using Base.Application.Interfaces.Mappers;
using Base.Infrastructure;
using Vehicle.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.Json;
using Base.Application.DTOs;
using Base.Domain.Entities;
using System.Runtime.InteropServices;

namespace Chassis.Application.Services;
public sealed class ChassisService
    : BaseService<ChassisEntity, ChassisDto>
    , IChassisService
{
    #region Constants
    private readonly IVehicleService VehicleService;
    #endregion

    #region Constructors
    public ChassisService(ILogger logger
        , IChassisRepository repository
        , IBaseMapper<ChassisEntity, ChassisDto> mapper
        , IVehicleService vehicleService)
        : base(logger, repository, mapper)
    {
        VehicleService = vehicleService;
    }
    #endregion

    #region Methods
    public override async Task<ChassisDto> AddAsync(ChassisDto dto)
    {
        var oldDto = await GetByChassisSeriesAndNumberAsync(chassisSeries: dto.ChassisSeries
            , chassisNumber: dto.ChassisNumber);

        if (oldDto.Id > 0)
        {
            return Activator.CreateInstance<ChassisDto>();
        }

        var newDto = await base.AddAsync(dto);
        return newDto;
    }

    public override async Task<ulong> DeleteAsync(ulong id)
    {
        var vehicle = await VehicleService.GetByChassisIdAsync(chassisId: id);

        if (vehicle.Id > 0)
        {
            return 0;
        }

        var deletedId = await base.DeleteAsync(id);
        return deletedId;
    }

    public override async Task<ChassisDto> UpdateAsync(ChassisDto dto)
    {
        var oldDto = await GetByChassisSeriesAndNumberAsync(chassisSeries: dto.ChassisSeries
            , chassisNumber: dto.ChassisNumber);

        if (oldDto.Id > 0
            && oldDto.Id != dto.Id)
        {
            return Activator.CreateInstance<ChassisDto>();
        }

        var newDto = await base.UpdateAsync(dto);
        return newDto;
    }

    public async Task<ChassisDto> GetByChassisSeriesAndNumberAsync(string chassisSeries, uint chassisNumber)
    {
        if (string.IsNullOrWhiteSpace(chassisSeries))
        {
            return Activator.CreateInstance<ChassisDto>();
        }

        using var cancellationTokenSource = new CancellationTokenSource(EfContext.CommandTimeout);
        var entity = await ((IChassisRepository)Repository).GetByChassisSeriesAndNumberAsync(
            chassisSeries: chassisSeries
            , chassisNumber: chassisNumber
            , cancellationToken: cancellationTokenSource.Token);

        if (entity.Id < 1)
        {
            return Activator.CreateInstance<ChassisDto>();
        }

        var dto = Mapper.ToDto(entity);
        return dto;
    }

    public static async Task<ushort> MockAsync(IServiceProvider serviceProvider)
    {
        var fileName = "chassis.json";
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
        var entityList = JsonSerializer.Deserialize<List<ChassisEntity>>(json);

        if ((entityList?.Count ?? 0) < 1)
        {
            var msg = $"Mock file has no valid records. Path: {path}";
            Debug.WriteLine(msg);
            throw new InvalidOperationException(msg);
        }

        var dtoList = new List<ChassisDto>();
        var mapper = serviceProvider.GetRequiredService<IBaseMapper<ChassisEntity, ChassisDto>>();

        foreach (var entity in entityList!)
        {
            var dto = mapper.ToDto(entity);
            dtoList.Add(dto);
        }

        var service = serviceProvider.GetRequiredService<IChassisService>();

        var count = await service.AddRangeAsync(dtoList!);
        Debug.WriteLine($"Mock: loaded {count} registers from file {path}");
        return count;
    }

    public async Task<BaseListDto<ChassisDto>> ListUnassignedAsync(uint pageNumber, ushort pageSize)
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
        var entityList = await ((IChassisRepository)Repository).ListUnassignedAsync(
            pageNumber: pageNumber
            , pageSize: pageSize
            , cancellationToken: cancellationTokenSource.Token);

        var dtoList = new BaseListDto<ChassisDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        if ((entityList?.List?.Count ?? 0) == 0)
        {
            return dtoList;
        }

        var list = new List<ChassisDto>();

        foreach (var entity in entityList!.List)
        {
            var dto = Mapper.ToDto(entity);
            list.Add(dto);
        }

        dtoList.List = list;
        return dtoList;
    }
    #endregion
}
