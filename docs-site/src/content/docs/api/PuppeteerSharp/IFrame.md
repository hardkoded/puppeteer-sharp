---
title: "IFrame"
description: "Provides methods to interact with a single page frame in Chromium. One IPage instance might have multiple IFrame instances. At every point of time, page expo..."
editUrl: false
prev: false
next: false
sidebar:
  label: "IFrame"
  hidden: true
---

<a id="PuppeteerSharp_IFrame"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IFrame
```


Provides methods to interact with a single page frame in Chromium. One [IPage](/api/PuppeteerSharp/IPage/) instance might have multiple [IFrame](./) instances.
At every point of time, page exposes its current frame tree via the [MainFrame](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_MainFrame) and [ChildFrames](#PuppeteerSharp_IFrame_ChildFrames) properties.

[IFrame](./) object's lifecycle is controlled by three events, dispatched on the page object
- [FrameAttached](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameAttached) - fires when the frame gets attached to the page. A Frame can be attached to the page only once
- [FrameNavigated](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameNavigated) - fired when the frame commits navigation to a different URL
- [FrameDetached](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameDetached) - fired when the frame gets detached from the page.  A Frame can be detached from the page only once.

**Derived:** [BidiFrame](/api/PuppeteerSharp.Bidi/BidiFrame/), [CdpFrame](/api/PuppeteerSharp.Cdp/CdpFrame/), [Frame](/api/PuppeteerSharp/Frame/)

**Example**

An example of dumping frame tree.


```csharp
var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.google.com/chrome/browser/canary.html");
dumpFrameTree(page.MainFrame, string.Empty);
await browser.CloseAsync();

void dumpFrameTree(IFrame frame, string indent)
{
    Console.WriteLine(indent + frame.Url);
    foreach (var child in frame.ChildFrames)
    {
        dumpFrameTree(child, indent + "  ");
    }
}
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L39)
