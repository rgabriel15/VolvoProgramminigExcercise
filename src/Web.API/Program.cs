//dotnet dev-certs https --trust

using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Web.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

var isRunningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
//var connectionString = builder.Configuration.GetConnectionString("AzureDbConnection")!;
//var connectionString = builder.Configuration.GetConnectionString("LocalDbConnection")!;
Log.Logger = new LoggerConfiguration().GetConfiguredLogger();
builder.Host.UseSerilog();

builder
    .Services
    .AddDependencyInjection(logger: Log.Logger, useInMemoryDatabase: true)
    .AddSerilog()
    .AddDistributedMemoryCache()
    .AddRateLimiterX()
    .AddHttpContextAccessor()
    .AddEndpointsApiExplorer();

if (builder.Environment.IsDevelopment())
{
    _ = builder
        .Services
        .AddSwaggerConfig();
}
else if (!isRunningInContainer)
{
    _ = builder
        .Services
        .AddHsts(options =>
        {
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
            options.Preload = true;
        })
        .AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
            options.HttpsPort = 8081;
        });
}

builder
    .Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"));
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

builder
    .Services
    .AddControllers()
    .AddJsonOptions(configure =>
    {
        configure.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        configure.JsonSerializerOptions.Converters.Add(new Web.API.Converters.DBNullJsonConverter());
        configure.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        configure.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();
app.Lifetime.ApplicationStarted.Register(() => Log.Logger.Information("APPLICATION STARTED ({EnvironmentName}). Running in Container: {IsRunningInContainer}", app.Environment.EnvironmentName, isRunningInContainer));
app.Lifetime.ApplicationStopping.Register(() => Log.Logger.Information("APPLICATION STOPPING."));
app.Lifetime.ApplicationStopped.Register(() => Log.Logger.Information("APPLICATION STOPPED."));

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/error");

    if (!isRunningInContainer)
    {
        _ = app
            .UseHsts()
            .UseHttpsRedirection();
    }
}
else
{
    _ = app.UseExceptionHandler("/error-development")
        .UseSwaggerConfig()
        .UseSwaggerUI();
}

app.UseFileServer()
    .UseRateLimiter()
    .UseHttpHeaders()
    .UseRequestLocalizationX()
    .UseStatusCodePages();

//app.UseAuthentication()
//    .UseAuthorization();

app.MapControllers();

//Mock
await using var scope = app.Services.CreateAsyncScope();
var serviceProvider = scope.ServiceProvider;
_ = await VehicleType.Application.Services.VehicleTypeService.MockAsync(serviceProvider);
var isTestRunning = AppDomain
        .CurrentDomain
        .GetAssemblies()
        .Any(assembly => assembly?.FullName?.Contains("xunit", StringComparison.OrdinalIgnoreCase) ?? false);
if (!isTestRunning)
{
    _ = await Chassis.Application.Services.ChassisService.MockAsync(serviceProvider);
    _ = await Vehicle.Application.Services.VehicleService.MockAsync(serviceProvider);
}

await app.RunAsync();

#pragma warning disable S1118 // Utility classes should not have public constructors
public partial class Program { }
#pragma warning restore S1118 // Utility classes should not have public constructors
