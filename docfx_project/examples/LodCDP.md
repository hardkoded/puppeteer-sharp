# How to log messages between Puppeteer-Sharp and Chromium

_Contributors: [Darío Kondratiuk](https://www.hardkoded.com/)_

## Problem

You want to log the messages sent by your app to Chromium and the messages received by Chromium. 

## Solution

Add [Serilog.Extensions.Logging.File](https://www.nuget.org/packages/Serilog.Extensions.Logging.File/) Nuget package.

First we need to create an `ILoggerFactory`

```js
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