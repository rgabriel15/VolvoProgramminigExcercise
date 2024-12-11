using System.Threading.RateLimiting;

namespace Web.API.Configuration;
public static class RateLimiterXConfiguration
{
    #region Methods
    internal static IServiceCollection AddRateLimiterX(this IServiceCollection services)
    {
        return services.AddRateLimiter(config =>
        {
            config.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            config.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                {
                    var permitLimit = 20;

                    if (httpContext.Request.Path.StartsWithSegments(SerilogConfiguration.SerilogUiDefaultEndpoint
                        , StringComparison.InvariantCulture))
                    {
                        permitLimit *= 100;
                    }

                    var partitionKey = httpContext.User.Identity?.Name
                        ?? httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "localhost";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: partitionKey,
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = permitLimit,
                            QueueLimit = 0,
                            Window = TimeSpan.FromSeconds(10)
                        }
                    );
                }
            );
        });
    }
    #endregion
}
