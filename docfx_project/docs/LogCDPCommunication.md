# How to log messages between Puppeteer-Sharp and Chromium

_Contributors: [Darío Kondratiuk](https://www.hardkoded.com/), [Barnabas Szenasi](https://outisnemo.com/)_

## Problem

You want to log the messages sent by your app to Chromium and the messages received by Chromium. Below you can find two solutions for the most common logging frameworks.

## Log CDP messages to files using Serilog

Add [Serilog.Extensions.Logging.File](https://www.nuget.org/packages/Serilog.Extensions.Logging.File/) Nuget package.

First we need to create an `ILoggerFactory`

```cs
private static ILoggerFactory GetLoggerFactory(string file)
{
    var factory = new LoggerFactory();
    var filter = new FilterLoggerSettings
    {
        { "Connection", LogLevel.Trace }
    };

    factory.WithFilter(filter).AddFile(file, LogLevel.Trace);

    return factory;
}
```

Now we can use `GetLoggerFactory` to inject a logger into Puppeteer.

```cs
using (var browser = await Puppeteer.LaunchAsync(browserOptions, GetLoggerFactory(fileName)))
{
    //Some code
}
```

## Log CDP messages to console using Microsoft.Extensions.Logging

Add [Microsoft.Extensions.Logging.Console](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Console) Nuget package.

First we need to create an `ILoggerFactory`

```cs
private static ILoggerFactory GetLoggerFactory()
{
    var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
    {
        builder
            .AddConsole(options => options.Format = ConsoleLoggerFormat.Systemd)
            .SetMinimumLevel(LogLevel.Trace);
    });

    return loggerFactory;
}
```

Now we can use `GetLoggerFactory` to inject a logger into Puppeteer.LaunchAsync or Puppeteer.ConnectAsync method.

```cs
using (var browser = await Puppeteer.LaunchAsync(browserOptions, GetLoggerFactory()))
{
    //Some code
}
```
