using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Web.API.Configuration;
internal static class SwaggerConfiguration
{
    #region Constants
    private const string VersionCode = "v1";
    private static readonly string ProjectName = Assembly.GetEntryAssembly()?.GetName().Name!;
    #endregion

    #region Methods
    internal static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        return services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                name: VersionCode,
                info: new OpenApiInfo
                {
                    Title = ProjectName,
                    Version = VersionCode.ToLowerInvariant() //Swaggger bug needs lowercase
                });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{ProjectName}.xml");

            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = $"JWT Authorization header using the {JwtBearerDefaults.AuthenticationScheme} scheme." +
                    $"\n\nPut **_ONLY_** your JWT {JwtBearerDefaults.AuthenticationScheme} token on textbox below (**_DON'T_** include the authorization schema e.g. 'Bearer')." +
                    "\n\nExample:" +
                    "\n\n\t12345abcdef",
                Name = HeaderNames.Authorization,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme = "oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });
    }

    internal static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app)
    {
        _ = app.UseSwagger()
            .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($@"/swagger/{VersionCode}/swagger.json", $"{ProjectName} {VersionCode}");
                c.RoutePrefix = string.Empty; //To serve the Swagger UI at the app's root (http://localhost:<port>/)
            });

        return app;
    }
    #endregion
}
