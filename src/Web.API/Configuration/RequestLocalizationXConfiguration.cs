using Microsoft.AspNetCore.Localization;
using Serilog;
using System.Globalization;

namespace Web.API.Configuration;
internal static class RequestLocalizationXConfiguration
{
    #region Constants
    internal const string DefaultCultureCode = "en-US";
    #endregion

    #region Methods
    internal static IApplicationBuilder UseRequestLocalizationX(this IApplicationBuilder app
        , string cultureCode = DefaultCultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
        {
            throw new ArgumentException(null, nameof(cultureCode));
        }

        var cultureInfo = new CultureInfo(cultureCode);
        cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
        cultureInfo.NumberFormat.NumberGroupSeparator = ",";
        cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
        cultureInfo.NumberFormat.CurrencyGroupSeparator = ",";

        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        var builder = app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(cultureInfo),
            SupportedCultures =
                [
                    cultureInfo,
                ],

            SupportedUICultures =
                [
                    cultureInfo,
                ]
        });

        Log.Information("Culture set to [{CultureCode}].", cultureCode);

        return builder;
    }
    #endregion
}
