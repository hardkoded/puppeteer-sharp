# How to test a Chrome Extension
_Contributors: [Dario Kondratiuk](https://github.com/kblok)_

## Problem

You need to use Puppeteer Sharp in an application set up for AOT compilation.

## Solution

You shouldn't need to do anything special to use Puppeteer Sharp in an AOT environment. The library is already prepared for it.\
The only challenge you might face is if you use any custom class to pass into or get from an Evaluate function. In that case you will need to provide a serialization context to PuppeteerSharp.\
Let's say you have a class like this:

```csharp
public class TestClass
{
    public string Name { get; set; }
}
```

You need to create a serialization context like this:

```csharp
[JsonSerializable(typeof(TestClass))]
public partial class DemoJsonSerializationContext : JsonSerializerContext
{}

```

_For more information about `JsonSerializerContext` see [How to use source generation in System.Text.Json](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation?WT.mc_id=DT-MVP-5003814)._

Once you have your own context you have to pass it to PuppeteerSharp before launching the browser:

```csharp
Puppeteer.ExtraJsonSerializerContext = DemoJsonSerializationContext.Default;
```

`ExtraJsonSerializerContext` will be used the first time PuppeteerSharp serializes or deserializes any object. So it's important to set it before launching the browser. Once set, you can't change it.

## Example

```csharp
class MainClass
{
    public static async Task Main(string[] args)
    {
        Puppeteer.ExtraJsonSerializerContext = DemoJsonSerializationContext.Default;
        var options = new LaunchOptions { Headless = true };

        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        await using var browser = await Puppeteer.LaunchAsync(options);
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync("https://www.google.com");

        var result = await page.EvaluateFunctionAsync<TestClass>("test => test", new TestClass { Name = "Dario"});
    }
}

public class TestClass
{
    public string Name { get; set; }
}

[JsonSerializable(typeof(TestClass))]
public partial class DemoJsonSerializationContext : JsonSerializerContext
{}
```
