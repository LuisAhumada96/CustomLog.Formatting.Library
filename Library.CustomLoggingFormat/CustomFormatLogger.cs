using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace CustomLog.Formatting.Library
{
    public sealed class CustomLogFormatter : ConsoleFormatter, IDisposable
    {
        private readonly IDisposable? _optionsReloadToken;
        private CustomOptions _formatterOptions;
        public CustomLogFormatter(IOptionsMonitor<CustomOptions> options)
            // Case insensitive
            : base(options.CurrentValue.FormatterName)
        {
            (_optionsReloadToken, _formatterOptions) =
            (options.OnChange(ReloadLoggerOptions), options.CurrentValue);
        }


        private void ReloadLoggerOptions(CustomOptions options) =>
            _formatterOptions = options;

        public override void Write<TState>(
            in LogEntry<TState> logEntry,
            IExternalScopeProvider? scopeProvider,
            TextWriter textWriter)
        {
            string? message =
                logEntry.Formatter?.Invoke(
                    logEntry.State, logEntry.Exception);

            if (message is null)
            {
                return;
            }

            var customLogEntry = new LogEntry(logEntry.LogLevel, logEntry.Category, logEntry.EventId, logEntry.State, logEntry.Exception);
            WriteCustomPrefix(textWriter, customLogEntry);
            textWriter.WriteLine(message);
            WriteCustomSuffix(textWriter, customLogEntry);
        }

        private void WriteCustomPrefix(TextWriter textWriter, LogEntry logEntry)
        {
            foreach (var prefix in _formatterOptions.CustomPrefix)
            {
                var value = prefix.Invoke(logEntry);
                if (string.IsNullOrEmpty(value)) continue;
                textWriter.Write($"{_formatterOptions.SeparatorLeft}{prefix.Invoke(logEntry)}{_formatterOptions.SeparatorRight}");
            }
        }

        private void WriteCustomSuffix(TextWriter textWriter, LogEntry logEntry)
        {
            foreach (var suffix in _formatterOptions.CustomSuffix)
            {
                var value = suffix.Invoke(logEntry);
                if (string.IsNullOrEmpty(value)) continue;
                textWriter.Write($"{_formatterOptions.SeparatorLeft}{suffix.Invoke(logEntry)}{_formatterOptions.SeparatorRight}");
            }
        }

        public void Dispose() => _optionsReloadToken?.Dispose();
    }

    public class CustomOptions() : ConsoleFormatterOptions
    {

        public Func<LogEntry, string>[] CustomPrefix { get; set; } = new DefaultLogFormat("MySolution", "MyServerName", new List<string> { "HEADER-NAME-1", "HEADER-NAME-2" }).GetDefaultLogVariables();
        public Func<LogEntry, string>[] CustomSuffix { get; set; } = [];
        public string SeparatorLeft { get; set; } = "[";
        public string SeparatorRight { get; set; } = "]";
        public string FormatterName { get; set; } = "customFormatter";
    }

    public record LogEntry(LogLevel logLevel, string Category, EventId eventId, object state, Exception? Exception);
}
