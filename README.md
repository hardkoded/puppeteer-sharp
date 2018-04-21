# Puppeteer Sharp

[![NuGet](https://img.shields.io/nuget/v/PuppeteerSharp.svg?style=flat-square&label=nuget&colorB=green)][NugetUrl]
[![Build status](https://ci.appveyor.com/api/projects/status/pwfkjb0c4jfdo7lc/branch/master?svg=true)][BuildUrl]

[NugetUrl]: https://www.nuget.org/packages/PuppeteerSharp/
[BuildUrl]: https://ci.appveyor.com/project/kblok/puppeteer-sharp/branch/master

Puppeteer Sharp is a .NET port of the official [Node.JS Puppeteer API](https://github.com/GoogleChrome/puppeteer). 

# Usage

## Take screenshots

```cs
await Downloader.CreateDefault().DownloadRevisionAsync(chromiumRevision);
var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = true
}, chromiumRevision);
var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.ScreenshotAsync(outputFile));
```

You can also change the view port before generating the screenshot


```cs
await page.SetViewport(new ViewPortOptions
{
    Width = 500,
    Height = 500
});
```


## Generate PDF files

```cs
await Downloader.CreateDefault().DownloadRevisionAsync(chromiumRevision);
var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = true
}, chromiumRevision);
var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.PdfAsync(outputFile));
```

## Inject HTML

```cs
using(var page = await Browser.NewPageAsync())
{
    await page.SetContentAsync("<div>My Receipt</div>");
    var result = await page.GetContentAsync();
    await page.PdfAsync(outputFile);
    SaveHtmlToDB(result);
}
```

## Evaluate Javascript

```cs
using (var page = await Browser.NewPageAsync())
{
    var seven = await page.EvaluateFunctionAsync<int>(“4 + 3”);
    var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
    Console.WriteLine(someObject.a);
}
```

## Wait For Selector

```cs
using (var page = await Browser.NewPageAsync())
{
    await page.GoToAsync("http://www.spapage.com");
    await page.WaitForSelectorAsync("div.main-content")
    await page.PdfAsync(outputFile));
}
```

## Wait For Function
```cs
using (var page = await Browser.NewPageAsync())
{
    await page.GoToAsync("http://www.spapage.com");
    var watchDog = page.WaitForFunctionAsync("window.innerWidth < 100");
    await Page.SetViewport(new ViewPortOptions { Width = 50, Height = 50 });
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
 * [April 2018](http://www.hardkoded.com/blogs/puppeteer-sharp-monthly-april-2018)
 * [March 2018](http://www.hardkoded.com/blogs/puppeteer-sharp-monthly-march-2018)
 * [February 2018](http://www.hardkoded.com/blogs/puppeteer-sharp-monthly-february-2018)

# Roadmap
Getting to all the 523 tests Puppeteer has, will be a long and fun journey. So, this will be the roadmap for Puppeteer Sharp 1.0:

## 0.1 First Minimum Viable Product
The first 0.1 will include:
* Browser download
* Basic browser operations: create a browser, a page and navigate a page.
* Take screenshots.
* Print to PDF.

## 0.2 Repository cleanup
This version won't include a new version. It will be about improving the repository:

* Setup CI.
* Create basic documentation (Readme, contributing, code of conduct).

## 0.3 Puppeteer
It will implement all [Puppeteer related tests](https://github.com/GoogleChrome/puppeteer/blob/master/test/test.js#L108).

## 0.4 Page
It will implement all Page tests except the ones testing the evaluate method.
As this will be quite a big version, I think we will publish many 0.3.X versions before 0.4.

## 0.5 Frames
It will implement all Frame tests.

## 0.6 Simple interactions
It will implement all tests related to setting values to inputs and clicking on elements.

## 0.7 Page Improvements
It will implement all missing Page tests.

## 0.8 Element Handle and JS Handle
It will implement all tests releated to Element handles and JS handles.

## 0.X Intermediate versions
At this point, We will have implemented most features, except the ones which are javascript related.
I believe there will be many versions between 0.6 and 1.0.

## 1.0 Puppeteer the world!
The 1.0 version will have all (or most) Puppeteer features implemented. I don't know if we'll be able to cover 100% of Puppeteer features, due to differences between both technologies, but we'll do our best.

# Progress

* Tests on Google's Puppeteer: 405.
* Tests on Puppeteer Sharp: 80.
* Passing tests: 80.

