using Microsoft.Extensions.Logging;

namespace CustomLog.Formatting.Library
{
    public static class ConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddCustomFormatter(
            this ILoggingBuilder builder,
            CustomOptions formatConfiguration) =>
            builder.AddConsole(options => options.FormatterName = formatConfiguration.FormatterName)
                .AddConsoleFormatter<CustomLogFormatter, CustomOptions>(configure =>
                {
                    if (formatConfiguration != null)
                    {
                        configure.CustomPrefix = formatConfiguration.CustomPrefix;
                        configure.IncludeScopes = true;
                        configure.FormatterName = formatConfiguration.FormatterName;
                    }
                }
                );
    }
}
