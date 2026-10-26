---
title: "BidiFrame"
description: "Provides methods to interact with a single page frame in Chromium. One IPage instance might have multiple IFrame instances. At every point of time, page expo..."
editUrl: false
prev: false
next: false
sidebar:
  label: "BidiFrame"
  hidden: true
---

<a id="PuppeteerSharp_Bidi_BidiFrame"></a>

Namespace: [PuppeteerSharp.Bidi](/api/PuppeteerSharp.Bidi/)


```csharp
public class BidiFrame : Frame, IFrame
```


Provides methods to interact with a single page frame in Chromium. One [IPage](/api/PuppeteerSharp/IPage/) instance might have multiple [IFrame](/api/PuppeteerSharp/IFrame/) instances.
At every point of time, page exposes its current frame tree via the [MainFrame](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_MainFrame) and [ChildFrames](/api/PuppeteerSharp/IFrame/#PuppeteerSharp_IFrame_ChildFrames) properties.

[IFrame](/api/PuppeteerSharp/IFrame/) object's lifecycle is controlled by three events, dispatched on the page object
- [FrameAttached](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameAttached) - fires when the frame gets attached to the page. A Frame can be attached to the page only once
- [FrameNavigated](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameNavigated) - fired when the frame commits navigation to a different URL
- [FrameDetached](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameDetached) - fired when the frame gets detached from the page.  A Frame can be detached from the page only once.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Frame](/api/PuppeteerSharp/Frame/)

**Implements:** [IFrame](/api/PuppeteerSharp/IFrame/)

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L42)

## Properties

<a id="PuppeteerSharp_Bidi_BidiFrame_ChildFrames_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_ChildFrames"></a>

### ChildFrames


```csharp
public override IReadOnlyCollection<IFrame> ChildFrames { get; }
```


Gets the child frames of the this frame.

**Value:** [IReadOnlyCollection&lt;IFrame&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlycollection-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L64)

<a id="PuppeteerSharp_Bidi_BidiFrame_Client_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_Client"></a>

### Client


```csharp
public override ICDPSession Client { get; protected set; }
```


Gets the CDPSession associated with this environment.

**Value:** [ICDPSession](/api/PuppeteerSharp/ICDPSession/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L81)

<a id="PuppeteerSharp_Bidi_BidiFrame_Detached_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_Detached"></a>

### Detached


```csharp
public override bool Detached { get; }
```


Gets a value indicating if the frame is detached or not.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L75)

<a id="PuppeteerSharp_Bidi_BidiFrame_Page_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_Page"></a>

### Page


```csharp
public override IPage Page { get; }
```


The [IPage](/api/PuppeteerSharp/IPage/) associated with the frame.

**Value:** [IPage](/api/PuppeteerSharp/IPage/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L78)

<a id="PuppeteerSharp_Bidi_BidiFrame_Url_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_Url"></a>

### Url


```csharp
public override string Url { get; }
```


Gets the frame's url.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L72)

## Methods

<a id="PuppeteerSharp_Bidi_BidiFrame_AddStyleTagAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_AddStyleTagAsync_PuppeteerSharp_AddTagOptions_"></a>

### AddStyleTagAsync(AddTagOptions)


```csharp
public override Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
```


Adds a `<link rel="stylesheet">` tag into the page with the desired url or a `<link rel="stylesheet">` tag with the content.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [AddTagOptions](/api/PuppeteerSharp/AddTagOptions/) | add style tag options. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L111)

<a id="PuppeteerSharp_Bidi_BidiFrame_ExtensionRealms_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_ExtensionRealms"></a>

### ExtensionRealms()


```csharp
public override IReadOnlyList<Realm> ExtensionRealms()
```


Retrieves the list of extension realms within this frame.

**Returns:** [IReadOnlyList&lt;Realm&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1) — A list of [Realm](/api/PuppeteerSharp/Realm/) instances representing extension execution contexts.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L564)

<a id="PuppeteerSharp_Bidi_BidiFrame_FrameElementAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_FrameElementAsync"></a>

### FrameElementAsync()


```csharp
public override Task<ElementHandle> FrameElementAsync()
```


The frame element associated with this frame (if any).

**Returns:** [Task&lt;ElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the frame element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L543)

<a id="PuppeteerSharp_Bidi_BidiFrame_GetDeviceRequestPromptManager_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_GetDeviceRequestPromptManager"></a>

### GetDeviceRequestPromptManager()


```csharp
protected override DeviceRequestPromptManager GetDeviceRequestPromptManager()
```


Gets the prompts manager for the current client.

**Returns:** [DeviceRequestPromptManager](/api/PuppeteerSharp/DeviceRequestPromptManager/) — The [DeviceRequestPromptManager](/api/PuppeteerSharp/DeviceRequestPromptManager/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L621)

<a id="PuppeteerSharp_Bidi_BidiFrame_GoToAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_GoToAsync_System_String_PuppeteerSharp_NavigationOptions_"></a>

### GoToAsync(string, NavigationOptions)


```csharp
public override Task<IResponse> GoToAsync(string url, NavigationOptions options)
```


Navigates to an url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | URL to navigate page to. The url should include scheme, e.g. https://. |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | Navigation parameters. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

`WaitUntilNavigation%5b%5d)` will throw an error if:
- there's an SSL error (e.g. in case of self-signed certificates).
- target URL is invalid.
- the `timeout` is exceeded during navigation.
- the remote server does not respond or is unreachable.
- the main resource failed to load.
- the URL is blocked by blocklist/allowlist rules.

`WaitUntilNavigation%5b%5d)` will not throw an error when any valid HTTP status code is returned by the remote server,
including 404 "Not Found" and 500 "Internal Server Error".  The status code for such responses can be retrieved by calling [Status](/api/PuppeteerSharp/IResponse/#PuppeteerSharp_IResponse_Status)

&gt; **NOTE** `WaitUntilNavigation%5b%5d)` either throws an error or returns a main resource response.
The only exceptions are navigation to `about:blank` or navigation to the same URL with a different hash, which would succeed and return `null`.

&gt; **NOTE** Headless mode doesn't support navigation to a PDF document. See the &lt;see fref="https://bugs.chromium.org/p/chromium/issues/detail?id=761295"&gt;upstream issue&lt;/see&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L209)

<a id="PuppeteerSharp_Bidi_BidiFrame_SetContentAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_SetContentAsync_System_String_PuppeteerSharp_NavigationOptions_"></a>

### SetContentAsync(string, NavigationOptions)


```csharp
[Obsolete]
public override Task SetContentAsync(string html, NavigationOptions options)
```


Sets the HTML markup to the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `html` | [string](https://learn.microsoft.com/dotnet/api/system.string) | HTML markup to assign to the page. |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | The options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L178)

<a id="PuppeteerSharp_Bidi_BidiFrame_WaitForNavigationAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiFrame_WaitForNavigationAsync_PuppeteerSharp_NavigationOptions_"></a>

### WaitForNavigationAsync(NavigationOptions)


```csharp
public override Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
```


This resolves when the frame navigates to a new URL or reloads.
It is useful for when you run code which will indirectly cause the frame to navigate.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | navigation options. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response.
    In case of multiple redirects, the navigation will resolve with the response of the last redirect.
    In case of navigation to a different anchor or navigation due to History API usage, the navigation will resolve with `null`.

Usage of the `History API` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/History_API"&gt;https://developer.mozilla.org/en-US/docs/Web/API/History_API&lt;/a&gt; to change the URL is considered a navigation.

**Example**

```csharp
var navigationTask = Page.WaitForNavigationAsync();
await Page.MainFrame.ClickAsync("a.my-link");
await navigationTask;
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiFrame.cs#L266)
