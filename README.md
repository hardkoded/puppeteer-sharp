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

# Google for testing is here!

Puppeteer Sharp v11 introduced some breaking changes. 

![image](https://github.com/hardkoded/puppeteer-sharp/assets/2198466/fa8ef603-d402-4df3-bef6-08a088c29c71)

I recommend you to go to [the release page](https://github.com/hardkoded/puppeteer-sharp/releases/tag/v11.0.0) and take a look at those changes.

But it brings Google for testing!

![image](https://github.com/hardkoded/puppeteer-sharp/assets/2198466/57ab6d98-21e0-43c2-9259-4d05d41ef55b)

Now, PuppeteerSharp uses Google for testing instead of Chromium. And the new supported version is v115!  
You can still you Chromium if you want to by using `SupportedBrowser.Chromium`.

Feel free to [create an issue](https://github.com/hardkoded/puppeteer-sharp/issues/new) if these new changes don't work for you.

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
<sup><a href='/lib/PuppeteerSharp.Tests/ScreenshotTests/PageScreenshotTests.cs#L61-L69' title='Snippet source file'>snippet source</a> | <a href='#snippet-screenshotasync' title='Start of snippet'>anchor</a></sup>
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
<sup><a href='/lib/PuppeteerSharp.Tests/ScreenshotTests/ElementHandleScreenshotTests.cs#L19-L25' title='Snippet source file'>snippet source</a> | <a href='#snippet-setviewportasync' title='Start of snippet'>anchor</a></sup>
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
<sup><a href='/lib/PuppeteerSharp.Tests/PageTests/PdfTests.cs#L28-L38' title='Snippet source file'>snippet source</a> | <a href='#snippet-pdfasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

### Inject HTML

<!-- snippet: SetContentAsync -->
<a id='snippet-setcontentasync'></a>
```cs
await using var page = await browser.NewPageAsync();
await page.SetContentAsync("<div>My Receipt</div>");
var result = await page.GetContentAsync();
```
<sup><a href='/lib/PuppeteerSharp.Tests/PageTests/SetContentTests.cs#L19-L25' title='Snippet source file'>snippet source</a> | <a href='#snippet-setcontentasync' title='Start of snippet'>anchor</a></sup>
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
<sup><a href='/lib/PuppeteerSharp.Tests/QuerySelectorTests/ElementHandleQuerySelectorEvalTests.cs#L17-L22' title='Snippet source file'>snippet source</a> | <a href='#snippet-evaluate' title='Start of snippet'>anchor</a></sup>
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
