using MudBlazor;
using MudBlazor.Services;
using Web.UI;
using Web.UI.Components;
using Web.UI.Services.Base.Service.Models;
using Web.UI.Services.Base.Service.Services;
using Web.UI.Services.Chassis.Service.Interfaces;
using Web.UI.Services.Chassis.Service.Services;
using Web.UI.Services.ClientException.Service.Interfaces;
using Web.UI.Services.ClientException.Service.Services;
using Web.UI.Services.Vehicle.Service.Interfaces;
using Web.UI.Services.Vehicle.Service.Services;
using Web.UI.Services.VehicleType.Service.Interfaces;
using Web.UI.Services.VehicleType.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.NewestOnTop = false;
        config.SnackbarConfiguration.ShowCloseIcon = true;
        config.SnackbarConfiguration.VisibleStateDuration = 10000;
        config.SnackbarConfiguration.HideTransitionDuration = 500;
        config.SnackbarConfiguration.ShowTransitionDuration = 500;
        config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
    })
    .AddHttpContextAccessor()
    .AddAntiforgery();

builder
    .Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder
    .Services
    .AddScoped<StateContainer>()
    .AddScoped<IClientExceptionService, ClientExceptionService>()
    .AddScoped<IChassisService, ChassisService>()
    .AddScoped<IVehicleService, VehicleService>()
    .AddScoped<IVehicleTypeService, VehicleTypeService>();

builder
    .Services
    .AddHttpClient("VolvoProgrammingExerciseClientV1", configure =>
    {
        var url = builder.Configuration.GetValue<string>("VolvoProgrammingExerciseClientV1:ApiV1Url")!;
        configure.BaseAddress = new Uri(url);
        configure.Timeout = BaseService<BaseModel>.Timeout;
    });

if (!builder.Environment.IsDevelopment())
{
    _ = builder
        .Services
        .AddHsts(options =>
        {
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
            options.Preload = true;
        })
        .AddHttpsRedirection(options => options.RedirectStatusCode = StatusCodes.Status301MovedPermanently);
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error", createScopeForErrors: true);
    _ = app.UseHsts();
}

app.UseHttpsRedirection()
    .UseStaticFiles()
    .UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
