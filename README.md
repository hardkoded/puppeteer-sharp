# Puppeteer Sharp

[![NuGet](https://buildstats.info/nuget/PuppeteerSharp)][NugetUrl]
[![Build status](https://ci.appveyor.com/api/projects/status/pwfkjb0c4jfdo7lc/branch/master?svg=true&pendingText=master&failingText=master&passingText=master)][BuildUrl]
[![Demo build status](https://ci.appveyor.com/api/projects/status/10g64a4aa0083wgf/branch/master?svg=true&pendingText=demo&failingText=demo&passingText=demo)][BuildDemoUrl]
[![CodeFactor](https://www.codefactor.io/repository/github/hardkoded/puppeteer-sharp/badge)][CodeFactorUrl]
[![Backers](https://opencollective.com/hardkoded-projects/backers/badge.svg)][Backers]

[NugetUrl]: https://www.nuget.org/packages/PuppeteerSharp/
[BuildUrl]: https://ci.appveyor.com/project/kblok/puppeteer-sharp/branch/master
[BuildDemoUrl]: https://ci.appveyor.com/project/kblok/puppeteer-sharp-0c8w9/branch/master
[CodeFactorUrl]: https://www.codefactor.io/repository/github/hardkoded/puppeteer-sharp
[Backers]: https://opencollective.com/hardkoded-projects

Puppeteer Sharp is a .NET port of the official [Node.JS Puppeteer API](https://github.com/GoogleChrome/puppeteer).

# Puppeteer-Sharp 3 is here!

Check out the [blog post](https://www.hardkoded.com/blog/puppeteer-sharp-3-is-here)!

# Useful links

* [API Documentation](http://www.puppeteersharp.com/api/index.html)
* Slack channel [#puppeteer-sharp](https://www.hardkoded.com/goto/pptr-slack)
* [StackOverflow](https://stackoverflow.com/search?q=puppeteer-sharp)
* [Issues](https://github.com/hardkoded/puppeteer-sharp/issues?utf8=%E2%9C%93&q=is%3Aissue)

# Prerequisites

 * As Puppeteer-Sharp is a NetStandard 2.0 library, the minimum platform versions are .NET Framework 4.6.1 and .NET Core 2.0. [Read more](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).
 * The minimum **Windows** versions supporting the WebSocket library are Windows 8 and Windows Server 2012. [Read more](https://docs.microsoft.com/en-us/dotnet/api/system.net.websockets?redirectedfrom=MSDN&view=netframework-4.7.2). If you need to run Puppeteer-Sharp on Windows 7 you can use [System.Net.WebSockets.Client.Managed](https://www.nuget.org/packages/System.Net.WebSockets.Client.Managed/) through the [LaunchOptions.WebSocketFactory](https://www.puppeteersharp.com/api/PuppeteerSharp.LaunchOptions.html#PuppeteerSharp_LaunchOptions_WebSocketFactory) property.
 * If you have issues running Chrome on Linux, the Puppeteer repo has a [great troubleshooting guide](https://github.com/GoogleChrome/puppeteer/blob/master/docs/troubleshooting.md).
 * X-server is required on Linux.

 # How to Contribute and Provide Feedback

Some of the best ways to contribute are to try things out file bugs and fix issues.

If you have an issue or a question:

* Ask a question on [Stack Overflow](https://stackoverflow.com/search?q=puppeteer-sharp).
* File a [new issue](https://github.com/hardkoded/puppeteer-sharp/issues/new).

## Contributing Guide

See [this document](https://github.com/hardkoded/puppeteer-sharp/blob/master/CONTRIBUTING.md) for information on how to contribute.

# Usage

## Take screenshots

<!-- snippet: ScreenshotAsync -->
<a id='snippet-screenshotasync'></a>
```cs
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(
    new LaunchOptions {Headless = true});
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.ScreenshotAsync(outputFile);
```
<sup><a href='/lib/PuppeteerSharp.Tests/PageTests/ScreenshotTests.cs#L62-L70' title='Snippet source file'>snippet source</a> | <a href='#snippet-screenshotasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

You can also change the view port before generating the screenshot

<!-- snippet: SetViewportAsync -->
<a id='snippet-setviewportasync'></a>
```cs
await Page.SetViewportAsync(new ViewPortOptions
{
    Width = 500,
    Height = 500
});
```
<sup><a href='/lib/PuppeteerSharp.Tests/ElementHandleTests/ScreenshotTests.cs#L19-L25' title='Snippet source file'>snippet source</a> | <a href='#snippet-setviewportasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Generate PDF files

<!-- snippet: PdfAsync -->
<a id='snippet-pdfasync'></a>
```cs
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions {Headless = true});
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.PdfAsync(outputFile);
```
<sup><a href='/lib/PuppeteerSharp.Tests/PageTests/PdfTests.cs#L27-L36' title='Snippet source file'>snippet source</a> | <a href='#snippet-pdfasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Inject HTML

<!-- snippet: SetContentAsync -->
<a id='snippet-setcontentasync'></a>
```cs
await using var page = await browser.NewPageAsync();
await page.SetContentAsync("<div>My Receipt</div>");
var result = await page.GetContentAsync();
```
<sup><a href='/lib/PuppeteerSharp.Tests/PageTests/SetContentTests.cs#L19-L25' title='Snippet source file'>snippet source</a> | <a href='#snippet-setcontentasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Evaluate Javascript

<!-- snippet: Evaluate -->
<a id='snippet-evaluate'></a>
```cs
await using var page = await browser.NewPageAsync();
var seven = await page.EvaluateExpressionAsync<int>("4 + 3");
var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
Console.WriteLine(someObject.a);
```
<sup><a href='/lib/PuppeteerSharp.Tests/ElementHandleTests/EvaluateFunctionTests.cs#L17-L22' title='Snippet source file'>snippet source</a> | <a href='#snippet-evaluate' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Wait For Selector

```cs
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync("http://www.spapage.com");
    await page.WaitForSelectorAsync("div.main-content")
    await page.PdfAsync(outputFile));
}
```

## Wait For Function
```cs
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync("http://www.spapage.com");
    var watchDog = page.WaitForFunctionAsync("()=> window.innerWidth < 100");
    await page.SetViewportAsync(new ViewPortOptions { Width = 50, Height = 50 });
    await watchDog;
}
```

## Connect to a remote browser

```cs
var options = new ConnectOptions()
{
    BrowserWSEndpoint = $"wss://www.externalbrowser.io?token={apikey}"
};

var url = "https://www.google.com/";

using (var browser = await PuppeteerSharp.Puppeteer.ConnectAsync(options))
{
    using (var page = await browser.NewPageAsync())
    {
        await page.GoToAsync(url);
        await page.PdfAsync("wot.pdf");
    }
}
```

# Monthly reports
 * [August 2019](http://www.hardkoded.com/blog/puppeteer-sharp-monthly-aug-2019)
 * [July 2019](http://www.hardkoded.com/blog/puppeteer-sharp-monthly-jul-2019)
 * [June 2019](http://www.hardkoded.com/blog/puppeteer-sharp-monthly-jun-2019)
 * [May 2019](http://www.hardkoded.com/blog/puppeteer-sharp-monthly-may-2019)
 * [April 2019](http://www.hardkoded.com/blog/puppeteer-sharp-monthly-apr-2019)
 * [March 2019](http://www.hardkoded.com/blog/puppeteer-sharp-monthly-mar-2019)
 * [February 2019](http://www.hardkoded.com/blog/puppeteer-sharp-monthly-feb-2019)
 * [January 2019](http://www.hardkoded.com/blog/puppeteer-sharp-monthly-jan-2019)

# Backers

Support us with a monthly donation and help us continue our activities. [Become a backer](https://opencollective.com/hardkoded-projects).

<a href="https://opencollective.com/puppeteer-sharp/backer/0/website" target="_blank"><img src="https://opencollective.com/hardkoded-projects/backer/0/avatar.svg"></a>
<a href="https://opencollective.com/puppeteer-sharp/backer/1/website" target="_blank"><img src="https://opencollective.com/hardkoded-projects/backer/1/avatar.svg"></a>
<a href="https://opencollective.com/puppeteer-sharp/backer/2/website" target="_blank"><img src="https://opencollective.com/hardkoded-projects/backer/2/avatar.svg"></a>
<a href="https://opencollective.com/puppeteer-sharp/backer/3/website" target="_blank"><img src="https://opencollective.com/hardkoded-projects/backer/3/avatar.svg"></a>
<a href="https://opencollective.com/puppeteer-sharp/backer/3/website" target="_blank"><img src="https://opencollective.com/hardkoded-projects/backer/4/avatar.svg"></a>

# Thanks

Thanks to [JetBrains](https://www.jetbrains.com/?from=PuppeteerSharp) for a community Resharper license to use on this project.


