# Puppeteer Sharp

[![NuGet](https://buildstats.info/nuget/PuppeteerSharp)][NugetUrl]
[![Build status](https://github.com/hardkoded/puppeteer-sharp/actions/workflows/dotnet.yml/badge.svg)][BuildUrl]
[![Demo build status](https://github.com/hardkoded/puppeteer-sharp/actions/workflows/demo.yml/badge.svg)][BuildDemoUrl]
[![CodeFactor](https://www.codefactor.io/repository/github/hardkoded/puppeteer-sharp/badge)][CodeFactorUrl]
[![Backers](https://opencollective.com/hardkoded-projects/backers/badge.svg)][Backers]

[NugetUrl]: https://www.nuget.org/packages/PuppeteerSharp/
[BuildUrl]: https://github.com/hardkoded/puppeteer-sharp/actions/workflows/dotnet.yml
[BuildDemoUrl]: https://github.com/hardkoded/puppeteer-sharp/actions/workflows/demo.yml
[CodeFactorUrl]: https://www.codefactor.io/repository/github/hardkoded/puppeteer-sharp
[Backers]: https://opencollective.com/hardkoded-projects

Puppeteer Sharp is a .NET port of the official [Node.JS Puppeteer API](https://github.com/puppeteer/puppeteer).

## Recent news

* [Puppeteer-Sharp 2023 recap](https://www.hardkoded.com/blog/puppeteer-sharp-2023-recap).

## Useful links

* [API Documentation](http://www.puppeteersharp.com/api/index.html)
* Slack channel [#puppeteer-sharp](https://www.hardkoded.com/goto/pptr-slack)
* [StackOverflow](https://stackoverflow.com/search?q=puppeteer-sharp)
* [Issues](https://github.com/hardkoded/puppeteer-sharp/issues?utf8=%E2%9C%93&q=is%3Aissue)
* [Blog](https://www.hardkoded.com/)

## Prerequisites

* As Puppeteer-Sharp is a NetStandard 2.0 library, the minimum platform versions are .NET Framework 4.6.1 and .NET Core 2.0. [Read more](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).
* If you have issues running Chrome on Linux, the Puppeteer repo has a [great troubleshooting guide](https://github.com/puppeteer/puppeteer/blob/master/docs/troubleshooting.md).
* X-server is required on Linux.

## How to Contribute and Provide Feedback

Some of the best ways to contribute are to try things out file bugs and fix issues.

If you have an issue or a question:

* Ask a question on [Stack Overflow](https://stackoverflow.com/search?q=puppeteer-sharp).
* File a [new issue](https://github.com/hardkoded/puppeteer-sharp/issues/new).

## Contributing Guide

See [this document](https://github.com/hardkoded/puppeteer-sharp/blob/master/CONTRIBUTING.md) for information on how to contribute.

## Usage

## Take screenshots

<!-- snippet: ScreenshotAsync -->
<a id='snippet-ScreenshotAsync'></a>
```cs
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(
    new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.ScreenshotAsync(outputFile);
```
<sup><a href='https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp.Tests/ScreenshotTests/PageScreenshotTests.cs#L53-L61' title='Snippet source file'>snippet source</a> | <a href='#snippet-ScreenshotAsync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

You can also change the view port before generating the screenshot

<!-- snippet: SetViewportAsync -->
<a id='snippet-SetViewportAsync'></a>
```cs
await Page.SetViewportAsync(new ViewPortOptions
{
    Width = 500,
    Height = 500
});
```
<sup><a href='https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp.Tests/ScreenshotTests/ElementHandleScreenshotTests.cs#L13-L19' title='Snippet source file'>snippet source</a> | <a href='#snippet-SetViewportAsync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Generate PDF files

<!-- snippet: PdfAsync -->
<a id='snippet-PdfAsync'></a>
```cs
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com"); // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.
await page.EvaluateExpressionHandleAsync("document.fonts.ready"); // Wait for fonts to be loaded. Omitting this might result in no text rendered in pdf.
await page.PdfAsync(outputFile);
```
<sup><a href='https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp.Tests/PageTests/PdfTests.cs#L23-L33' title='Snippet source file'>snippet source</a> | <a href='#snippet-PdfAsync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Inject HTML

<!-- snippet: SetContentAsync -->
<a id='snippet-SetContentAsync'></a>
```cs
await using var page = await browser.NewPageAsync();
await page.SetContentAsync("<div>My Receipt</div>");
var result = await page.GetContentAsync();
```
<sup><a href='https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp.Tests/PageTests/SetContentTests.cs#L14-L20' title='Snippet source file'>snippet source</a> | <a href='#snippet-SetContentAsync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Evaluate Javascript

<!-- snippet: Evaluate -->
<a id='snippet-Evaluate'></a>
```cs
await using var page = await browser.NewPageAsync();
var seven = await page.EvaluateExpressionAsync<int>("4 + 3");
var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
Console.WriteLine(someObject.a);
```
<sup><a href='https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp.Tests/QuerySelectorTests/ElementHandleQuerySelectorEvalTests.cs#L16-L21' title='Snippet source file'>snippet source</a> | <a href='#snippet-Evaluate' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Wait For Selector

```cs
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync("http://www.spapage.com");
    await page.WaitForSelectorAsync("div.main-content")
    await page.PdfAsync(outputFile));
}
```

### Wait For Function

```cs
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync("http://www.spapage.com");
    var watchDog = page.WaitForFunctionAsync("()=> window.innerWidth < 100");
    await page.SetViewportAsync(new ViewPortOptions { Width = 50, Height = 50 });
    await watchDog;
}
```

### Connect to a remote browser

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

## Sponsors

A massive thanks to [AWS](https://github.com/aws), who sponsored Puppeteer-sharp from Nov 2023 via the [.NET on AWS Open Source Software Fund](https://github.com/aws/dotnet-foss) and [JetBrains](https://www.jetbrains.com/?from=PuppeteerSharp) for a community Resharper and Rider license to use on this project.

<div style="display:inline">
<img src="https://raw.githubusercontent.com/aaubry/YamlDotNet/master/Sponsors/aws-logo-small.png" width="200" height="200"/>
<img src="https://upload.wikimedia.org/wikipedia/en/thumb/0/08/JetBrains_beam_logo.svg/1200px-JetBrains_beam_logo.svg.png" width="200" height="200"/>
</div>

And a huge thanks to everyone who sponsors this project through [Github sponsors](https://github.com/sponsors/hardkoded):

<!-- sponsors --><a href="https://github.com/tolgabalci"><img src="https://github.com/tolgabalci.png" width="60px" alt="Tolga Balci" /></a><a href="https://github.com/nogginbox"><img src="https://github.com/nogginbox.png" width="60px" alt="Richard Garside" /></a><a href="https://github.com/aws"><img src="https://github.com/aws.png" width="60px" alt="Amazon Web Services" /></a><!-- sponsors -->


