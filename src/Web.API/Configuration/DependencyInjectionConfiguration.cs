using Base.Application.Interfaces.Mappers;
using Base.Application.Interfaces.Services;
using Base.Application.Interfaces.Validators;
using Base.Application.Services;
using Base.Infrastructure;
using Chassis.Application.DTOs;
using Chassis.Application.Interfaces.Services;
using Chassis.Application.Mappers;
using Chassis.Application.Services;
using Chassis.Application.Validators;
using Chassis.Domain.Entities;
using Chassis.Domain.Interfaces.Repositories;
using Chassis.Infrastructure.Repositories;
using ClientException.Application.DTOs;
using ClientException.Application.Interfaces.Services;
using ClientException.Application.Mappers;
using ClientException.Application.Services;
using ClientException.Application.Validators;
using ClientException.Domain.Entities;
using ClientException.Domain.Interfaces.Repositories;
using ClientException.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Vehicle.Application.DTOs;
using Vehicle.Application.Interfaces.Services;
using Vehicle.Application.Mappers;
using Vehicle.Application.Services;
using Vehicle.Application.Validators;
using Vehicle.Domain.Entities;
using Vehicle.Domain.Interfaces.Repositories;
using Vehicle.Infrastructure.Repositories;
using VehicleType.Application.DTOs;
using VehicleType.Application.Interfaces.Services;
using VehicleType.Application.Mappers;
using VehicleType.Application.Services;
using VehicleType.Application.Validators;
using VehicleType.Domain.Entities;
using VehicleType.Domain.Interfaces.Repositories;
using VehicleType.Infrastructure.Repositories;
using ILogger = Serilog.ILogger;

namespace Web.API.Configuration;

/// <summary>
/// DependencyInjection
/// </summary>
internal static class DependencyInjectionConfiguration
{
    #region Methods
    internal static IServiceCollection AddDependencyInjection(
        this IServiceCollection services
        , ILogger logger
        , bool useInMemoryDatabase = false)
    {
        _ = services
            .RemoveAll<EfContext>()
            .RemoveAll<DbContextOptions<EfContext>>();

        if (useInMemoryDatabase)
        {
            _ = services.AddDbContext<EfContext>(opt =>
            {
                var databaseName = "database-test";

                _ = opt
                    .UseInMemoryDatabase(
                        databaseName: databaseName
                        , inMemoryOptionsAction: opt => opt.EnableNullChecks(false))
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            Log.Logger.Information("InMemory database enabled.");
        }
        else
        {
            _ = services.AddDbContext<EfContext>();
        }

        return services
            .AddSingleton(logger)
            .AddSingleton<ICacheService, CacheService>()

            .AddSingleton<IBaseMapper<ClientExceptionEntity, ClientExceptionDto>, ClientExceptionMapper>()
            .AddScoped<IClientExceptionRepository, ClientExceptionRepository>()
            .AddScoped<IClientExceptionService, ClientExceptionService>()
            .AddSingleton<IBaseValidator<ClientExceptionDto>, ClientExceptionValidators>()

            .AddSingleton<IBaseMapper<ChassisEntity, ChassisDto>, ChassisMapper>()
            .AddSingleton<IBaseValidator<ChassisDto>, ChassisValidators>()
            .AddScoped<IChassisRepository, ChassisRepository>()
            .AddScoped<IChassisService, ChassisService>()

            .AddSingleton<IBaseMapper<VehicleEntity, VehicleDto>, VehicleMapper>()
            .AddSingleton<IBaseValidator<VehicleDto>, VehicleValidators>()
            .AddScoped<IVehicleRepository, VehicleRepository>()
            .AddScoped<IVehicleService, VehicleService>()

            .AddSingleton<IBaseMapper<VehicleTypeEntity, VehicleTypeDto>, VehicleTypeMapper>()
            .AddSingleton<IBaseValidator<VehicleTypeDto>, VehicleTypeValidators>()
            .AddScoped<IVehicleTypeRepository, VehicleTypeRepository>()
            .AddScoped<IVehicleTypeService, VehicleTypeService>();
    }
    #endregion
}
