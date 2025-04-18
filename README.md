
# Custom Log Formatting
[![NuGet](https://img.shields.io/nuget/v/Library.CustomLogFormatting?label=NuGet&logo=nuget&style=flat)](https://www.nuget.org/packages/Library.CustomLogFormatting) [![NuGet](https://img.shields.io/nuget/dt/Library.CustomLogFormatting?logo=nuget&style=flat)](https://www.nuget.org/packages/Library.CustomLogFormatting) [![Codacy Grade](https://img.shields.io/codacy/grade/315c3d5a06a441bda26ffd88e705fa63?style=flat)](https://app.codacy.com/gh/LuisAhumada96/Library.CustomLogFormatting/dashboard)

This library allows you to configurate ordered prefix and suffix values on log messages. It uses Func delegates with string return value to write on message console. It's useful and emerged as a necessity to standarize log messages across a company with multiple software projects.

## Configuration

Go to Program.cs on your execution project and add the following code:
```
builder.Logging.ClearProviders()
               .AddCustomFormatter(new CustomOptions()
               {
                   CustomPrefix = <YOUR <FUNC<LogEntry,string>[] prefix method goes here>,
                   CustomSuffix = <YOUR <FUNC<LogEntry,string>[] suffix method goes here>
                   IncludeScopes = true,
                   FormatterName = "custom-format-logging",
                   SeparatorLeft = "[",
                   SeparatorRight = "]"
               })
```
You can define the separator characters between elements with SeparatorLeft and SeparatorRight.
Both CustomPrefix and CustomSuffix take a FUNC<LogEntry,string>[] where each element is invoked inside the library and its return value written on the log message based on the array order. The library includes its own default format ```DefaultLogFormat``` with the following constructor:


 ```DefaultLogFormat(string solution, string serverName = null, List<string> defaultHttpHeaders = null)```

 It allows to log HTTP Headers and sets log message with the following format:

 [DATETIME][SOLUTION_NAME][SERVER_NAME][SERVICE][LOG_TYPE]...[HTTP-HEADER]

. Here's an example:

```
builder.Logging.ClearProviders()
               .AddCustomFormatter(new CustomOptions()
               {
                   CustomPrefix = new DefaultLogFormat("TOTALPOS", "", new List<string>() { "my-http-header"}).GetDefaultLogVariables(),
                   IncludeScopes = true,
                   FormatterName = "custom-format-logging",
               })
            .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Error);
```

## Credits

- Created by: Luis I. Ahumada
