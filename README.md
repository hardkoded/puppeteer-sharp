# Puppeteer Sharp

[![NuGet](https://buildstats.info/nuget/PuppeteerSharp)][NugetUrl]
[![Build status](https://ci.appveyor.com/api/projects/status/pwfkjb0c4jfdo7lc/branch/master?svg=true&pendingText=master&failingText=master&passingText=master)][BuildUrl]
[![Demo build status](https://ci.appveyor.com/api/projects/status/10g64a4aa0083wgf/branch/master?svg=true&pendingText=demo&failingText=demo&passingText=demo)][BuildDemoUrl]
[![CodeFactor](https://www.codefactor.io/repository/github/hardkoded/puppeteer-sharp/badge)][CodeFactorUrl]
[![Backers](https://opencollective.com/hardkoded-projects/backers/badge.svg)][Backers]
[![xs:code](https://img.shields.io/static/v1?logo=data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAiIGhlaWdodD0iNDUiIHZpZXdCb3g9IjAgMCAzMCA0NSIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHBhdGggZD0iTTIwLjgzMjEgMzcuNDg4OVYyOS40NzJDMjAuODMyMSAyNi43NjQ1IDE5LjI5NjggMjQuNjY5MiAxNi44NTMxIDI0Ljk1NDJWMjAuMTgzMUMxOS4yOTY4IDIwLjQ2MjggMjAuODMyMSAxOC4zNTcgMjAuODMyMSAxNS42NDQyVjcuNjI3MjhDMjAuODMyMSAwLjI5MTE2NiAyNS4xMjQ2IDAuODEzNjY1IDI5LjM3NDYgMC44MTM2NjVWNC44MDM2NkMyNi45MzA5IDQuNTIzOTQgMjUuMzk1NiA0LjkxNDUgMjUuMzk1NiA3LjYyNzI4VjE1LjY0NDJDMjUuMzk1NiAxOC43NDc2IDI0LjQyODcgMjEuMTE3MyAyMi4zODM0IDIyLjUzMTdDMjQuNDI4NyAyMy45OTg5IDI1LjM5NTYgMjYuMzY4NyAyNS4zOTU2IDI5LjQ3MlYzNy40ODg5QzI1LjM5NTYgNDAuMTk2NCAyNi45MzA5IDQwLjU5MjMgMjkuMzc0NiA0MC4zMTI2VjQ0LjYwMzRDMjUuNjkzMSA0NC41OTgxIDIwLjgzMjEgNDQuODI1MSAyMC44MzIxIDM3LjQ4ODlaIiBmaWxsPSJ3aGl0ZSIvPgo8cGF0aCBkPSJNMC41MDY0MDkgNDQuNTk4MVY0MC4zMDczQzIuOTUwMTYgNDAuNTg3IDQuNDg1NDcgNDAuMTk2NCA0LjQ4NTQ3IDM3LjQ4MzZWMjkuNDcyQzQuNDg1NDcgMjYuMzY4NiA1LjQ1MjM1IDIzLjk5ODkgNy40OTc2NiAyMi41MzE3QzUuNDUyMzUgMjEuMTIyNSA0LjQ4NTQ3IDE4Ljc0NzUgNC40ODU0NyAxNS42NDQyVjcuNjI3MjVDNC40ODU0NyA0LjkxOTc1IDIuOTUwMTYgNC41MjM5MiAwLjUwNjQwOSA0LjgwMzY0VjAuODEzNjM4QzQuNzU2NDEgMC44MTM2MzggOS4wNDg5MSAwLjI4NTg2IDkuMDQ4OTEgNy42MjcyNVYxNS42NDQyQzkuMDQ4OTEgMTguMzUxNyAxMC41ODQyIDIwLjQ2MjggMTMuMDI4IDIwLjE4MzFWMjQuOTU0MkMxMC41ODQyIDI0LjY3NDUgOS4wNDg5MSAyNi43NjQ1IDkuMDQ4OTEgMjkuNDcyVjM3LjQ4ODlDOS4wNDg5MSA0NC44MjUgNC4xOTMyOCA0NC41OTgxIDAuNTA2NDA5IDQ0LjU5ODFaIiBmaWxsPSJ3aGl0ZSIvPgo8L3N2Zz4K&message=xs:code&label=Ping+me+on&color=%23007EFF)](https://xscode.com/profile/kblok)

[NugetUrl]: https://www.nuget.org/packages/PuppeteerSharp/
[BuildUrl]: https://ci.appveyor.com/project/kblok/puppeteer-sharp/branch/master
[BuildDemoUrl]: https://ci.appveyor.com/project/kblok/puppeteer-sharp-0c8w9/branch/master
[CodeFactorUrl]: https://www.codefactor.io/repository/github/hardkoded/puppeteer-sharp
[Backers]: https://opencollective.com/hardkoded-projects

Puppeteer Sharp is a .NET port of the official [Node.JS Puppeteer API](https://github.com/puppeteer/puppeteer).

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
<a id='snippet-screenshotasync'></a>
```cs
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(
    new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.ScreenshotAsync(outputFile);
```
<sup><a href='/lib/PuppeteerSharp.Tests/ScreenshotTests/PageScreenshotTests.cs#L63-L71' title='Snippet source file'>snippet source</a> | <a href='#snippet-screenshotasync' title='Start of snippet'>anchor</a></sup>
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
<sup><a href='/lib/PuppeteerSharp.Tests/ScreenshotTests/ElementHandleScreenshotTests.cs#L21-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-setviewportasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Generate PDF files

<!-- snippet: PdfAsync -->
<a id='snippet-pdfasync'></a>
```cs
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions {Headless = true});
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com"); // In case of fonts being loaded from a CDN, use WaitUntilNavigation.Networkidle0 as a second param.
await page.EvaluateExpressionHandleAsync("document.fonts.ready"); // Wait for fonts to be loaded. Omitting this might result in no text rendered in pdf.
await page.PdfAsync(outputFile);
```
<sup><a href='/lib/PuppeteerSharp.Tests/PageTests/PdfTests.cs#L29-L39' title='Snippet source file'>snippet source</a> | <a href='#snippet-pdfasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Inject HTML

<!-- snippet: SetContentAsync -->
<a id='snippet-setcontentasync'></a>
```cs
await using var page = await browser.NewPageAsync();
await page.SetContentAsync("<div>My Receipt</div>");
var result = await page.GetContentAsync();
```
<sup><a href='/lib/PuppeteerSharp.Tests/PageTests/SetContentTests.cs#L21-L27' title='Snippet source file'>snippet source</a> | <a href='#snippet-setcontentasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Evaluate Javascript

<!-- snippet: Evaluate -->
<a id='snippet-evaluate'></a>
```cs
await using var page = await browser.NewPageAsync();
var seven = await page.EvaluateExpressionAsync<int>("4 + 3");
var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
Console.WriteLine(someObject.a);
```
<sup><a href='/lib/PuppeteerSharp.Tests/QuerySelectorTests/ElementHandleQuerySelectorEvalTests.cs#L19-L24' title='Snippet source file'>snippet source</a> | <a href='#snippet-evaluate' title='Start of snippet'>anchor</a></sup>
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

## Backers

Support us with a monthly donation and help us continue our activities. [Become a backer](https://opencollective.com/hardkoded-projects).

<a href="https://opencollective.com/puppeteer-sharp/backer/0/website" target="_blank"><img src="https://opencollective.com/hardkoded-projects/backer/0/avatar.svg"></a>
<a href="https://opencollective.com/puppeteer-sharp/backer/1/website" target="_blank"><img src="https://opencollective.com/hardkoded-projects/backer/1/avatar.svg"></a>
<a href="https://opencollective.com/puppeteer-sharp/backer/2/website" target="_blank"><img src="https://opencollective.com/hardkoded-projects/backer/2/avatar.svg"></a>
<a href="https://opencollective.com/puppeteer-sharp/backer/3/website" target="_blank"><img src="https://opencollective.com/hardkoded-projects/backer/3/avatar.svg"></a>
<a href="https://opencollective.com/puppeteer-sharp/backer/3/website" target="_blank"><img src="https://opencollective.com/hardkoded-projects/backer/4/avatar.svg"></a>

## Thanks

Thanks to [JetBrains](https://www.jetbrains.com/?from=PuppeteerSharp) for a community Resharper license to use on this project.
