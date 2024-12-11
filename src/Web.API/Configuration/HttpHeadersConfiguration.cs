using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Web.API.Configuration;

public static class HttpHeadersConfiguration
{
    #region Constants
    public static readonly string AccessControlHeaders = string.Join(",",
        HeaderNames.Authorization,
        HeaderNames.Accept,
        HeaderNames.ContentType,
        HeaderNames.Origin,
        HeaderNames.XRequestedWith,
        "locale");

    private static readonly string PermissionsPolicyValues = string.Join(",",
        "accelerometer=()",
        "ambient-light-sensor=()",
        "autoplay=()",
        "battery=()",
        "camera=()",
        "display-capture=()",
        "document-domain=()",
        "encrypted-media=()",
        "execution-while-not-rendered=()",
        "execution-while-out-of-viewport=()",
        "gyroscope=()",
        "magnetometer=()",
        "microphone=()",
        "midi=()",
        "navigation-override=()",
        "payment=()",
        "picture-in-picture=()",
        "publickey-credentials-get=()",
        "sync-xhr=()",
        "usb=()",
        "wake-lock=()",
        "xr-spatial-tracking=()");

    private static readonly string ContentSecurityPolicyValues = string.Join(';',
        "base-uri 'self'",
        "block-all-mixed-content",
        "child-src 'self'",
        "form-action 'self'",
        "frame-ancestors 'none'",
        "manifest-src 'self'",
        "object-src 'none'",
        "script-src 'self'",
        "style-src 'self'",
        "img-src 'self' data:",
        "font-src 'self'",
        "upgrade-insecure-requests",
        "script-src-elem 'self'",
        "style-src-attr 'self'",
        "img-src 'self'",
        "font-src 'self'");
    #endregion

    #region Methods
    internal static IApplicationBuilder UseHttpHeaders(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            context.Response.Headers.Append(HeaderNames.ContentSecurityPolicy, ContentSecurityPolicyValues);

            // Set CORS-related headers in app.UseCors() middleware (handled outside this class)

            // Set standard security headers
            context.Response.Headers.Append(HeaderNames.StrictTransportSecurity, "max-age=31536000; includeSubDomains");
            context.Response.Headers.Append(HeaderNames.XFrameOptions, "DENY");
            context.Response.Headers.Append(HeaderNames.XContentTypeOptions, "nosniff");
            context.Response.Headers.Append("Referrer-Policy", "no-referrer-when-downgrade");
            context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");
            context.Response.Headers.Append(HeaderNames.CacheControl, "no-cache, no-store, must-revalidate");
            context.Response.Headers.Append(HeaderNames.Pragma, "no-cache");
            context.Response.Headers.Append(HeaderNames.XPoweredBy, "none");
            context.Response.Headers.Append(HeaderNames.Server, "none");
            context.Response.Headers.Append(HeaderNames.XXSSProtection, "1; mode=block");

            context.Response.Headers.Append("Permissions-Policy", PermissionsPolicyValues);

            // Add HSTS and other security headers for HTTPS requests
            if (context.Request.IsHttps)
            {
                context.Response.Headers.Append("Expect-CT", "max-age=0");
            }

            await next.Invoke();
        });
    }
    #endregion
}
