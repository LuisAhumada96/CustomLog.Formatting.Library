using Microsoft.AspNetCore.Http;

namespace CustomLog.Formatting.Library
{
    public record DefaultLogFormat(string solution, string serverName = null, List<string> defaultHttpHeaders = null)
    {
        public Func<LogEntry, string> DATE_TIME { get; } = (entry) => DateTime.UtcNow.ToString("O");
        public Func<LogEntry, string> SERVER_NAME { get; } = (entry) => serverName ?? System.Net.Dns.GetHostName();

        public Func<LogEntry, string> SOLUTION { get; } = (entry) => solution;

        public Func<LogEntry, string> TYPE { get; } = (entry) => entry.logLevel.ToString().ToUpper();
        public Func<LogEntry, string> SERVICE { get; } = (entry) => entry.Category;

        public Func<LogEntry, string>[] GetDefaultLogVariables()
        {
            var defaultList = new List<Func<LogEntry, string>>() { DATE_TIME, SOLUTION, SERVER_NAME, SERVICE, TYPE };
            defaultList.AddRange(GetLogHttpHeaders());
            return defaultList.ToArray();
        }

        private Func<LogEntry, string>[] GetLogHttpHeaders()
        {
            var funcList = new List<Func<LogEntry, string>>();
            if (defaultHttpHeaders == null) return [(entry) => "NO-HTTP-HEADER-SPECIFIED"];
            foreach (var header in defaultHttpHeaders)
            {
                funcList.Add((entry) =>
                {
                    return GetHttpRequest(header);
                });
            }

            return funcList.ToArray();
        }

        private string GetHttpRequest(string header)
        {
            HttpContext _httpContext = new HttpContextAccessor().HttpContext;
            if (_httpContext == null) return "";
            return _httpContext.Request.Headers.TryGetValue(header, out var value) ? $"{header}: {value}" : $"{header}-NOT-FOUND";
        }
    }
}
