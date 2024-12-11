using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using System.Globalization;

namespace Web.API.Configuration;
internal static class SerilogConfiguration
{
    #region Constants
    private const string BasePath = "Logs";
    private const uint FileSizeLimitBytes = 1024 * 1024 * 8;
    private static readonly ITextFormatter TextFormatter = new CompactJsonFormatter();
    internal const string DatabaseTableName = "Logs";
    internal const string SerilogUiDefaultEndpoint = "/serilog-ui";
    #endregion

    #region Methods
    internal static Logger GetConfiguredLogger(this LoggerConfiguration loggerConfiguration)
    {
        _ = loggerConfiguration
            .Enrich.WithExceptionDetails()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture)
            .WriteTo.File(
                path: Path.Combine(BasePath, "all_.log")
                , formatter: TextFormatter
                , rollingInterval: RollingInterval.Day
                , fileSizeLimitBytes: FileSizeLimitBytes
                , rollOnFileSizeLimit: true);

        var enumType = typeof(LogEventLevel);

        foreach (var logLevel in Enum.GetValues<LogEventLevel>())
        {
            var name = Enum.GetName(
                enumType: enumType
                , value: logLevel)!
            .ToLowerInvariant();

            _ = loggerConfiguration
                .WriteTo
                .Logger(l => l.Filter.ByIncludingOnly(e => e.Level == logLevel)
                .WriteTo
                .File(path: Path.Combine(BasePath, $"{name}_.log")
                    , formatter: TextFormatter
                    , rollingInterval: RollingInterval.Day
                    , fileSizeLimitBytes: FileSizeLimitBytes
                    , rollOnFileSizeLimit: true));
        }

        var logger = loggerConfiguration.CreateLogger();
        return logger;
    }
    #endregion
}

