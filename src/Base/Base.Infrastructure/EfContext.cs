using Chassis.Domain.Entities;
using ClientException.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vehicle.Domain.Entities;
using VehicleType.Domain.Entities;

namespace Base.Infrastructure;
public sealed class EfContext : DbContext
{
    #region Constants
    public static readonly TimeSpan CommandTimeout =
        System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ?
            TimeSpan.FromHours(1)
            : TimeSpan.FromSeconds(60);
    private readonly IWebHostEnvironment Environment;
    private readonly ILoggerFactory LoggerFactory;
    #endregion

    #region Properties
    public DbSet<ClientExceptionEntity> ClientExceptions { get; set; }
    public DbSet<ChassisEntity> Chassis { get; set; }
    public DbSet<VehicleEntity> Vehicles { get; set; }
    public DbSet<VehicleTypeEntity> VehicleTypes { get; set; }
    #endregion

    #region Constructors
    public EfContext(DbContextOptions<EfContext> options
        , IWebHostEnvironment webHostEnvironment
        , ILoggerFactory loggerFactory)
        : base(options)
    {
        Environment = webHostEnvironment;
        LoggerFactory = loggerFactory;
    }
    #endregion

    #region Methods
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        string connectionString;

        //if (Environment.IsProduction())
        //{
        //    connectionString = configuration.GetConnectionString("AzureDbConnection")!;
        //}
        //else
        //{
        //    //connectionString = configuration.GetConnectionString("AzureDbConnection")!;
        //    connectionString = configuration.GetConnectionString("LocalDbConnection")!;
        //}

        connectionString = configuration.GetConnectionString("LocalDbConnection")!;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidDataException("Invalid Database ConnectionString.");
        }

        _ = optionsBuilder.UseSqlServer(
            connectionString: connectionString
            , sqlServerOptionsAction: providerOptions =>
            {
                _ = providerOptions.EnableRetryOnFailure();
                _ = providerOptions.CommandTimeout((int)CommandTimeout.TotalSeconds);
            });

        _ = optionsBuilder
            .UseLoggerFactory(LoggerFactory)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
    }
    #endregion
}
