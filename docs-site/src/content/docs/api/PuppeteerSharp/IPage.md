---
title: "IPage"
description: "Provides methods to interact with a single tab in Chromium. One IBrowser instance might have multiple IPage instances."
editUrl: false
prev: false
next: false
sidebar:
  label: "IPage"
  hidden: true
---

<a id="PuppeteerSharp_IPage"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IPage : IDisposable, IAsyncDisposable
```


Provides methods to interact with a single tab in Chromium. One [IBrowser](/api/PuppeteerSharp/IBrowser/) instance might have multiple [IPage](./) instances.

**Derived:** [BidiPage](/api/PuppeteerSharp.Bidi/BidiPage/), [CdpPage](/api/PuppeteerSharp.Cdp/CdpPage/), [Page](/api/PuppeteerSharp/Page/)

**Example**

This example creates a page, navigates it to a URL, and then saves a screenshot:


```csharp
var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com");
await page.ScreenshotAsync("screenshot.png");
await browser.CloseAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L27)

## Properties

<a id="PuppeteerSharp_IPage_Accessibility_"></a>
<a id="PuppeteerSharp_IPage_Accessibility"></a>

### Accessibility


```csharp
IAccessibility Accessibility { get; }
```


Gets the accessibility.

**Value:** [IAccessibility](/api/PuppeteerSharp.PageAccessibility/IAccessibility/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L196)

<a id="PuppeteerSharp_IPage_Bluetooth_"></a>
<a id="PuppeteerSharp_IPage_Bluetooth"></a>

### Bluetooth


```csharp
IBluetoothEmulation Bluetooth { get; }
```


Gets this page's bluetooth emulation.

**Value:** [IBluetoothEmulation](/api/PuppeteerSharp/IBluetoothEmulation/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L304)

<a id="PuppeteerSharp_IPage_Browser_"></a>
<a id="PuppeteerSharp_IPage_Browser"></a>

### Browser


```csharp
IBrowser Browser { get; }
```


Get the browser the page belongs to.

**Value:** [IBrowser](/api/PuppeteerSharp/IBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L201)

<a id="PuppeteerSharp_IPage_BrowserContext_"></a>
<a id="PuppeteerSharp_IPage_BrowserContext"></a>

### BrowserContext


```csharp
IBrowserContext BrowserContext { get; }
```


Get the browser context that the page belongs to.

**Value:** [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L206)

<a id="PuppeteerSharp_IPage_Client_"></a>
<a id="PuppeteerSharp_IPage_Client"></a>

### Client


```csharp
ICDPSession Client { get; }
```


Chrome DevTools Protocol session.

**Value:** [ICDPSession](/api/PuppeteerSharp/ICDPSession/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L211)

<a id="PuppeteerSharp_IPage_Coverage_"></a>
<a id="PuppeteerSharp_IPage_Coverage"></a>

### Coverage


```csharp
ICoverage Coverage { get; }
```


Gets this page's coverage.

**Value:** [ICoverage](/api/PuppeteerSharp.PageCoverage/ICoverage/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L216)

<a id="PuppeteerSharp_IPage_DefaultNavigationTimeout_"></a>
<a id="PuppeteerSharp_IPage_DefaultNavigationTimeout"></a>

### DefaultNavigationTimeout


```csharp
int DefaultNavigationTimeout { get; set; }
```


This setting will change the default maximum time for the following methods:
- `GoToAsync()`
- [GoBackAsync(NavigationOptions)](#PuppeteerSharp_IPage_GoBackAsync_PuppeteerSharp_NavigationOptions_)
- [GoForwardAsync(NavigationOptions)](#PuppeteerSharp_IPage_GoForwardAsync_PuppeteerSharp_NavigationOptions_)
- [ReloadAsync(ReloadOptions)](#PuppeteerSharp_IPage_ReloadAsync_PuppeteerSharp_ReloadOptions_)
- `SetContentAsync()`
- [WaitForNavigationAsync(NavigationOptions)](#PuppeteerSharp_IPage_WaitForNavigationAsync_PuppeteerSharp_NavigationOptions_)
**NOTE** [DefaultNavigationTimeout](#PuppeteerSharp_IPage_DefaultNavigationTimeout) takes priority over. [DefaultTimeout](#PuppeteerSharp_IPage_DefaultTimeout)

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L228)

<a id="PuppeteerSharp_IPage_DefaultTimeout_"></a>
<a id="PuppeteerSharp_IPage_DefaultTimeout"></a>

### DefaultTimeout


```csharp
int DefaultTimeout { get; set; }
```


This setting will change the default maximum times for the following methods:
- [GoBackAsync(NavigationOptions)](#PuppeteerSharp_IPage_GoBackAsync_PuppeteerSharp_NavigationOptions_)
- [GoForwardAsync(NavigationOptions)](#PuppeteerSharp_IPage_GoForwardAsync_PuppeteerSharp_NavigationOptions_)
- `GoToAsync()`
- [ReloadAsync(ReloadOptions)](#PuppeteerSharp_IPage_ReloadAsync_PuppeteerSharp_ReloadOptions_)
- `SetContentAsync()`
- `WaitForFunctionAsync()`
- [WaitForNavigationAsync(NavigationOptions)](#PuppeteerSharp_IPage_WaitForNavigationAsync_PuppeteerSharp_NavigationOptions_)
- `WaitForRequestAsync()`
- `WaitForResponseAsync()`
- `WaitForSelectorAsync()`
- `WaitForExpressionAsync()`.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L244)

<a id="PuppeteerSharp_IPage_Frames_"></a>
<a id="PuppeteerSharp_IPage_Frames"></a>

### Frames


```csharp
IFrame[] Frames { get; }
```


Gets all frames attached to the page.

**Value:** `IFrame\[\]` — An array of all frames attached to the page.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L250)

<a id="PuppeteerSharp_IPage_IsClosed_"></a>
<a id="PuppeteerSharp_IPage_IsClosed"></a>

### IsClosed


```csharp
bool IsClosed { get; }
```


Get an indication that the page has been closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L255)

<a id="PuppeteerSharp_IPage_IsDragInterceptionEnabled_"></a>
<a id="PuppeteerSharp_IPage_IsDragInterceptionEnabled"></a>

### IsDragInterceptionEnabled


```csharp
[Obsolete("We no longer support intercepting drag payloads. Use the new drag APIs found on ElementHandle to drag (or just use the Page.Mouse)")]
bool IsDragInterceptionEnabled { get; }
```


`true` if drag events are being intercepted, `false` otherwise.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L260)

<a id="PuppeteerSharp_IPage_IsJavaScriptEnabled_"></a>
<a id="PuppeteerSharp_IPage_IsJavaScriptEnabled"></a>

### IsJavaScriptEnabled


```csharp
bool IsJavaScriptEnabled { get; }
```


If the page has JavaScript enabled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L324)

<a id="PuppeteerSharp_IPage_IsServiceWorkerBypassed_"></a>
<a id="PuppeteerSharp_IPage_IsServiceWorkerBypassed"></a>

### IsServiceWorkerBypassed


```csharp
bool IsServiceWorkerBypassed { get; }
```


`true` if the service worker are being bypassed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L329)

<a id="PuppeteerSharp_IPage_Keyboard_"></a>
<a id="PuppeteerSharp_IPage_Keyboard"></a>

### Keyboard


```csharp
IKeyboard Keyboard { get; }
```


Gets this page's keyboard.

**Value:** [IKeyboard](/api/PuppeteerSharp.Input/IKeyboard/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L268)

<a id="PuppeteerSharp_IPage_MainFrame_"></a>
<a id="PuppeteerSharp_IPage_MainFrame"></a>

### MainFrame


```csharp
IFrame MainFrame { get; }
```


Gets page's main frame.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/)

Page is guaranteed to have a main frame which persists during navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L276)

<a id="PuppeteerSharp_IPage_Mouse_"></a>
<a id="PuppeteerSharp_IPage_Mouse"></a>

### Mouse


```csharp
IMouse Mouse { get; }
```


Gets this page's mouse.

**Value:** [IMouse](/api/PuppeteerSharp.Input/IMouse/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L281)

<a id="PuppeteerSharp_IPage_Target_"></a>
<a id="PuppeteerSharp_IPage_Target"></a>

### Target


```csharp
[Obsolete("All the functionality provided by the Target should be already exposed by the page itself.")]
ITarget Target { get; }
```


Gets that target this page was created from.

**Value:** [ITarget](/api/PuppeteerSharp/ITarget/)

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L286)

<a id="PuppeteerSharp_IPage_Touchscreen_"></a>
<a id="PuppeteerSharp_IPage_Touchscreen"></a>

### Touchscreen


```csharp
ITouchscreen Touchscreen { get; }
```


Gets this page's touchscreen.

**Value:** [ITouchscreen](/api/PuppeteerSharp.Input/ITouchscreen/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L294)

<a id="PuppeteerSharp_IPage_Tracing_"></a>
<a id="PuppeteerSharp_IPage_Tracing"></a>

### Tracing


```csharp
ITracing Tracing { get; }
```


Gets this page's tracing.

**Value:** [ITracing](/api/PuppeteerSharp/ITracing/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L299)

<a id="PuppeteerSharp_IPage_Url_"></a>
<a id="PuppeteerSharp_IPage_Url"></a>

### Url


```csharp
string Url { get; }
```


Shortcut for `page.MainFrame.Url`.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L309)

<a id="PuppeteerSharp_IPage_Viewport_"></a>
<a id="PuppeteerSharp_IPage_Viewport"></a>

### Viewport


```csharp
ViewPortOptions Viewport { get; }
```


Gets this page's viewport.

**Value:** [ViewPortOptions](/api/PuppeteerSharp/ViewPortOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L314)

<a id="PuppeteerSharp_IPage_Workers_"></a>
<a id="PuppeteerSharp_IPage_Workers"></a>

### Workers


```csharp
WebWorker[] Workers { get; }
```


Gets all workers in the page.

**Value:** `WebWorker\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L319)

## Events

<a id="PuppeteerSharp_IPage_Close"></a>

### Close


```csharp
event EventHandler Close
```


Raised when the page closes.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L32)

<a id="PuppeteerSharp_IPage_Console"></a>

### Console


```csharp
event EventHandler<ConsoleEventArgs> Console
```


Raised when JavaScript within the page calls one of console API methods, e.g. `console.log` or `console.dir`. Also emitted if the page throws an error or a warning.
The arguments passed into `console.log` appear as arguments on the event handler.

**Value:** [EventHandler&lt;ConsoleEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

**Example**

An example of handling [Console](#PuppeteerSharp_IPage_Console) event:


```csharp
page.Console += (sender, e) =>
{
    for (var i = 0; i < e.Message.Args.Count; ++i)
    {
        System.Console.WriteLine($"{i}: {e.Message.Args[i]}");
    }
}
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L52)

<a id="PuppeteerSharp_IPage_DOMContentLoaded"></a>

### DOMContentLoaded


```csharp
event EventHandler DOMContentLoaded
```


Raised when the JavaScript `DOMContentLoaded` [https://developer.mozilla.org/en-US/docs/Web/Events/DOMContentLoaded](https://developer.mozilla.org/en-US/docs/Web/Events/DOMContentLoaded) event is dispatched.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L62)

<a id="PuppeteerSharp_IPage_Dialog"></a>

### Dialog


```csharp
event EventHandler<DialogEventArgs> Dialog
```


Raised when a JavaScript dialog appears, such as `alert`, `prompt`, `confirm` or `beforeunload`. Puppeteer can respond to the dialog via [Dialog](#PuppeteerSharp_IPage_Dialog)'s [Accept(string)](/api/PuppeteerSharp/Dialog/#PuppeteerSharp_Dialog_Accept_System_String_) or [Dismiss()](/api/PuppeteerSharp/Dialog/#PuppeteerSharp_Dialog_Dismiss) methods.

**Value:** [EventHandler&lt;DialogEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L57)

<a id="PuppeteerSharp_IPage_Error"></a>

### Error


```csharp
event EventHandler<ErrorEventArgs> Error
```


Raised when the page crashes

**Value:** [EventHandler&lt;ErrorEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L68)

<a id="PuppeteerSharp_IPage_ExceptionThrown"></a>

### ExceptionThrown


```csharp
event EventHandler<RemoteExceptionEventArgs> ExceptionThrown
```


Raised when an uncaught exception happens within the page or one of its dedicated workers.
Contains a [RemoteException](/api/PuppeteerSharp/RemoteException/). This is the successor of [PageError](#PuppeteerSharp_IPage_PageError).

**Value:** [EventHandler&lt;RemoteExceptionEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

The [Handle](/api/PuppeteerSharp/RemoteException/#PuppeteerSharp_RemoteException_Handle) of the exception is disposed right after the event has been
dispatched. Call [Move()](/api/PuppeteerSharp/IJSHandle/#PuppeteerSharp_IJSHandle_Move) on it synchronously inside the handler to keep it alive.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L130)

<a id="PuppeteerSharp_IPage_FrameAttached"></a>

### FrameAttached


```csharp
event EventHandler<FrameEventArgs> FrameAttached
```


Raised when a frame is attached.

**Value:** [EventHandler&lt;FrameEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L74)

<a id="PuppeteerSharp_IPage_FrameDetached"></a>

### FrameDetached


```csharp
event EventHandler<FrameEventArgs> FrameDetached
```


Raised when a frame is detached.

**Value:** [EventHandler&lt;FrameEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L79)

<a id="PuppeteerSharp_IPage_FrameNavigated"></a>

### FrameNavigated


```csharp
event EventHandler<FrameNavigatedEventArgs> FrameNavigated
```


Raised when a frame is navigated to a new url.

**Value:** [EventHandler&lt;FrameNavigatedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L84)

<a id="PuppeteerSharp_IPage_FrameNavigatedNewDocument"></a>

### FrameNavigatedNewDocument


```csharp
event EventHandler<FrameEventArgs> FrameNavigatedNewDocument
```


Raised when a frame is navigated to a new document.
Emitted after [FrameNavigated](#PuppeteerSharp_IPage_FrameNavigated).

**Value:** [EventHandler&lt;FrameEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L96)

<a id="PuppeteerSharp_IPage_FrameNavigatedSameDocument"></a>

### FrameNavigatedSameDocument


```csharp
event EventHandler<FrameEventArgs> FrameNavigatedSameDocument
```


Raised when a frame navigates within the same document, for example to a different fragment or via the History API.
Emitted after [FrameNavigated](#PuppeteerSharp_IPage_FrameNavigated).

**Value:** [EventHandler&lt;FrameEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L90)

<a id="PuppeteerSharp_IPage_Issue"></a>

### Issue


```csharp
event EventHandler<IssueEventArgs> Issue
```


Raised when a DevTools issue is reported.

**Value:** [EventHandler&lt;IssueEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L111)

<a id="PuppeteerSharp_IPage_Load"></a>

### Load


```csharp
event EventHandler Load
```


Raised when the JavaScript `load` [https://developer.mozilla.org/en-US/docs/Web/Events/load](https://developer.mozilla.org/en-US/docs/Web/Events/load) event is dispatched.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L101)

<a id="PuppeteerSharp_IPage_Metrics"></a>

### Metrics


```csharp
event EventHandler<MetricEventArgs> Metrics
```


Raised when the JavaScript code makes a call to `console.timeStamp`. For the list of metrics see [MetricsAsync()](#PuppeteerSharp_IPage_MetricsAsync).

**Value:** [EventHandler&lt;MetricEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L106)

<a id="PuppeteerSharp_IPage_PageError"></a>

### PageError


```csharp
event EventHandler<PageErrorEventArgs> PageError
```


Raised when an uncaught exception happens within the page.

**Value:** [EventHandler&lt;PageErrorEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

Prefer [ExceptionThrown](#PuppeteerSharp_IPage_ExceptionThrown), which exposes the exception as a [RemoteException](/api/PuppeteerSharp/RemoteException/)
(text, name, stack trace, location and a live handle to the thrown value) instead of a reconstructed error.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L120)

<a id="PuppeteerSharp_IPage_Popup"></a>

### Popup


```csharp
event EventHandler<PopupEventArgs> Popup
```


Raised when the page opens a new tab or window.

**Value:** [EventHandler&lt;PopupEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L135)

<a id="PuppeteerSharp_IPage_Request"></a>

### Request


```csharp
event EventHandler<RequestEventArgs> Request
```


Raised when a page issues a request. The [Request](#PuppeteerSharp_IPage_Request) object is read-only.
In order to intercept and mutate requests, see [SetRequestInterceptionAsync(bool)](#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_)

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L141)

<a id="PuppeteerSharp_IPage_RequestFailed"></a>

### RequestFailed


```csharp
event EventHandler<RequestEventArgs> RequestFailed
```


Raised when a request fails, for example by timing out.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L146)

<a id="PuppeteerSharp_IPage_RequestFinished"></a>

### RequestFinished


```csharp
event EventHandler<RequestEventArgs> RequestFinished
```


Raised when a request finishes successfully.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L151)

<a id="PuppeteerSharp_IPage_RequestServedFromCache"></a>

### RequestServedFromCache


```csharp
event EventHandler<RequestEventArgs> RequestServedFromCache
```


Raised when a request ended up loading from cache.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L156)

<a id="PuppeteerSharp_IPage_Response"></a>

### Response


```csharp
event EventHandler<ResponseCreatedEventArgs> Response
```


Raised when a [Response](#PuppeteerSharp_IPage_Response) is received.

**Value:** [EventHandler&lt;ResponseCreatedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

**Example**

An example of handling [Response](#PuppeteerSharp_IPage_Response) event:


```csharp
var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
page.Response += async(sender, e) =>
{
    if (e.Response.Url.Contains("script.js"))
    {
        tcs.TrySetResult(await e.Response.TextAsync());
    }
};

await Task.WhenAll(
    page.GoToAsync(TestConstants.ServerUrl + "/grid.html"),
    tcs.Task);
Console.WriteLine(await tcs.Task);
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L181)

<a id="PuppeteerSharp_IPage_WorkerCreated"></a>

### WorkerCreated


```csharp
event EventHandler<WorkerEventArgs> WorkerCreated
```


Emitted when a dedicated WebWorker ([https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API)) is spawned by the page.

**Value:** [EventHandler&lt;WorkerEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L186)

<a id="PuppeteerSharp_IPage_WorkerDestroyed"></a>

### WorkerDestroyed


```csharp
event EventHandler<WorkerEventArgs> WorkerDestroyed
```


Emitted when a dedicated WebWorker ([https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API)) is terminated.

**Value:** [EventHandler&lt;WorkerEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L191)

## Methods

<a id="PuppeteerSharp_IPage_AddRequestInterceptor_"></a>
<a id="PuppeteerSharp_IPage_AddRequestInterceptor_System_Func_PuppeteerSharp_IRequest_System_Threading_Tasks_Task__"></a>

### `AddRequestInterceptor(Func<IRequest, Task>)`


```csharp
void AddRequestInterceptor(Func<IRequest, Task> interceptionTask)
```


`RespondAsync()`, `AbortAsync()`, and `ContinueAsync()` can accept an optional `priority` to activate Cooperative Intercept Mode.
In Cooperative Mode, all interception tasks are guaranteed to run and all async handlers are awaited.
The interception is resolved to the highest-priority resolution.
In the event of a tie, `abort` &gt; `respond` &gt; `continue`.
The overrides of continue, the response of respond and the error code of abort are each taken from the call of that kind with the highest priority, or from the latest one if several calls of that kind share that priority.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `interceptionTask` | [Func&lt;IRequest, Task&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Interception task. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1706)

<a id="PuppeteerSharp_IPage_AddScriptTagAsync_"></a>
<a id="PuppeteerSharp_IPage_AddScriptTagAsync_PuppeteerSharp_AddTagOptions_"></a>

### AddScriptTagAsync(AddTagOptions)


```csharp
Task<IElementHandle> AddScriptTagAsync(AddTagOptions options)
```


Adds a `<script>` tag into the page with the desired url or content.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [AddTagOptions](/api/PuppeteerSharp/AddTagOptions/) | add script tag options. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame.

Shortcut for `page.MainFrame.AddScriptTagAsync(options)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L340)

<a id="PuppeteerSharp_IPage_AddScriptTagAsync_System_String_"></a>

### AddScriptTagAsync(string)


```csharp
Task<IElementHandle> AddScriptTagAsync(string url)
```


Adds a `<script>` tag into the page with the desired url or content.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | script url. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame.

Shortcut for `page.MainFrame.AddScriptTagAsync(new AddTagOptions { Url = url })`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L350)

<a id="PuppeteerSharp_IPage_AddStyleTagAsync_"></a>
<a id="PuppeteerSharp_IPage_AddStyleTagAsync_PuppeteerSharp_AddTagOptions_"></a>

### AddStyleTagAsync(AddTagOptions)


```csharp
Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
```


Adds a `<link rel="stylesheet">` tag into the page with the desired url or a `<link rel="stylesheet">` tag with the content.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [AddTagOptions](/api/PuppeteerSharp/AddTagOptions/) | add style tag options. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame.

Shortcut for `page.MainFrame.AddStyleTagAsync(options)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L361)

<a id="PuppeteerSharp_IPage_AddStyleTagAsync_System_String_"></a>

### AddStyleTagAsync(string)


```csharp
Task<IElementHandle> AddStyleTagAsync(string url)
```


Adds a `<link rel="stylesheet">` tag into the page with the desired url or a `<link rel="stylesheet">` tag with the content.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | stylesheet url. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame.

Shortcut for `page.MainFrame.AddStyleTagAsync(new AddTagOptions { Url = url })`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L371)

<a id="PuppeteerSharp_IPage_AuthenticateAsync_"></a>
<a id="PuppeteerSharp_IPage_AuthenticateAsync_PuppeteerSharp_Credentials_"></a>

### AuthenticateAsync(Credentials)


```csharp
Task AuthenticateAsync(Credentials credentials)
```


Provide credentials for http authentication [https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication](https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `credentials` | [Credentials](/api/PuppeteerSharp/Credentials/) | The credentials. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the message is sent to the browser.

To disable authentication, pass `null`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L381)

<a id="PuppeteerSharp_IPage_BringToFrontAsync_"></a>
<a id="PuppeteerSharp_IPage_BringToFrontAsync"></a>

### BringToFrontAsync()


```csharp
Task BringToFrontAsync()
```


Brings page to front (activates tab).

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to Chromium.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L387)

<a id="PuppeteerSharp_IPage_CaptureHeapSnapshotAsync_"></a>
<a id="PuppeteerSharp_IPage_CaptureHeapSnapshotAsync_PuppeteerSharp_HeapSnapshotOptions_"></a>

### CaptureHeapSnapshotAsync(HeapSnapshotOptions)


```csharp
Task CaptureHeapSnapshotAsync(HeapSnapshotOptions options)
```


Captures a snapshot of the JavaScript heap and writes it to a file.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [HeapSnapshotOptions](/api/PuppeteerSharp/HeapSnapshotOptions/) | Heap snapshot options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the heap snapshot is captured and written to disk.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L943)

<a id="PuppeteerSharp_IPage_ClickAsync_"></a>
<a id="PuppeteerSharp_IPage_ClickAsync_System_String_PuppeteerSharp_Input_ClickOptions_"></a>

### ClickAsync(string, ClickOptions)


```csharp
Task ClickAsync(string selector, ClickOptions options = null)
```


Fetches an element with `selector`, scrolls it into view if needed, and then uses [Mouse](#PuppeteerSharp_IPage_Mouse) to click in the center of the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to click. If there are multiple elements satisfying the selector, the first will be clicked. |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | click options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching `selector` is successfully clicked.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching `selector`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L396)

<a id="PuppeteerSharp_IPage_CloseAsync_"></a>
<a id="PuppeteerSharp_IPage_CloseAsync_PuppeteerSharp_PageCloseOptions_"></a>

### CloseAsync(PageCloseOptions)


```csharp
Task CloseAsync(PageCloseOptions options = null)
```


Closes the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [PageCloseOptions](/api/PuppeteerSharp/PageCloseOptions/) | Close options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L403)

<a id="PuppeteerSharp_IPage_CreateCDPSessionAsync_"></a>
<a id="PuppeteerSharp_IPage_CreateCDPSessionAsync"></a>

### CreateCDPSessionAsync()


```csharp
Task<ICDPSession> CreateCDPSessionAsync()
```


Creates a Chrome Devtools Protocol session attached to the page.

**Returns:** [Task&lt;ICDPSession&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [ICDPSession](/api/PuppeteerSharp/ICDPSession/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1718)

<a id="PuppeteerSharp_IPage_DeleteCookieAsync_"></a>
<a id="PuppeteerSharp_IPage_DeleteCookieAsync_PuppeteerSharp_CookieParam___"></a>

### DeleteCookieAsync(params CookieParam[])


```csharp
Task DeleteCookieAsync(params CookieParam[] cookies)
```


Deletes cookies from the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieParam\[\]` | Cookies to delete. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L424)

<a id="PuppeteerSharp_IPage_EmulateAsync_"></a>
<a id="PuppeteerSharp_IPage_EmulateAsync_PuppeteerSharp_Mobile_DeviceDescriptor_"></a>

### EmulateAsync(DeviceDescriptor)


```csharp
Task EmulateAsync(DeviceDescriptor options)
```


Emulates given device metrics and user agent.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [DeviceDescriptor](/api/PuppeteerSharp.Mobile/DeviceDescriptor/) | Emulation options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

This method is a shortcut for calling two methods:
[SetViewportAsync(ViewPortOptions)](#PuppeteerSharp_IPage_SetViewportAsync_PuppeteerSharp_ViewPortOptions_)
`SetUserAgentAsync()`
To aid emulation, puppeteer provides a list of device descriptors which can be obtained via the [Devices](/api/PuppeteerSharp/Puppeteer/#PuppeteerSharp_Puppeteer_Devices).
[EmulateAsync(DeviceDescriptor)](#PuppeteerSharp_IPage_EmulateAsync_PuppeteerSharp_Mobile_DeviceDescriptor_) will resize the page. A lot of websites don't expect phones to change size, so you should emulate before navigating to the page.

**Example**

var iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];
using(var page = await browser.NewPageAsync())
{
    await page.EmulateAsync(iPhone);
    await page.goto('https://www.google.com');
}

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L448)

<a id="PuppeteerSharp_IPage_EmulateCPUThrottlingAsync_"></a>
<a id="PuppeteerSharp_IPage_EmulateCPUThrottlingAsync_System_Nullable_System_Decimal__"></a>

### EmulateCPUThrottlingAsync(decimal?)


```csharp
Task EmulateCPUThrottlingAsync(decimal? factor = null)
```


Enables CPU throttling to emulate slow CPUs.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `factor` | [decimal?](https://learn.microsoft.com/dotnet/api/system.decimal) | Throttling rate as a slowdown factor (1 is no throttle, 2 is 2x slowdown, etc). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L455)

<a id="PuppeteerSharp_IPage_EmulateFocusedPageAsync_"></a>
<a id="PuppeteerSharp_IPage_EmulateFocusedPageAsync_System_Boolean_"></a>

### EmulateFocusedPageAsync(bool)


```csharp
Task EmulateFocusedPageAsync(bool enabled)
```


Emulates focus state of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether to emulate focus. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L462)

<a id="PuppeteerSharp_IPage_EmulateIdleStateAsync_"></a>
<a id="PuppeteerSharp_IPage_EmulateIdleStateAsync_PuppeteerSharp_EmulateIdleOverrides_"></a>

### EmulateIdleStateAsync(EmulateIdleOverrides)


```csharp
Task EmulateIdleStateAsync(EmulateIdleOverrides idleOverrides = null)
```


Emulates the idle state.
If no arguments set, clears idle state emulation.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `idleOverrides` | [EmulateIdleOverrides](/api/PuppeteerSharp/EmulateIdleOverrides/) | Overrides. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to the browser.

**Example**

```csharp
// set idle emulation
await page.EmulateIdleStateAsync(new EmulateIdleOverrides() {IsUserActive = true, IsScreenUnlocked = false});
// do some checks here
...
// clear idle emulation
await page.EmulateIdleStateAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L480)

<a id="PuppeteerSharp_IPage_EmulateLocaleAsync_"></a>
<a id="PuppeteerSharp_IPage_EmulateLocaleAsync_System_String_"></a>

### EmulateLocaleAsync(string)


```csharp
Task EmulateLocaleAsync(string locale = null)
```


Emulates the locale of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `locale` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Locale to emulate on the page (e.g., "en-US", "de-DE"). Passing `null` disables locale emulation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the locale has been set.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L572)

<a id="PuppeteerSharp_IPage_EmulateMediaFeaturesAsync_"></a>
<a id="PuppeteerSharp_IPage_EmulateMediaFeaturesAsync_System_Collections_Generic_IEnumerable_PuppeteerSharp_MediaFeatureValue__"></a>

### `EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue>)`


```csharp
Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
```


Given an array of media feature objects, emulates CSS media features on the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `features` | [IEnumerable&lt;MediaFeatureValue&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1) | Features to apply. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Emulate features task.

**Example**

```csharp
await page.EmulateMediaFeaturesAsync(new MediaFeature[]{ new MediaFeature { MediaFeature =  MediaFeature.PrefersColorScheme, Value = "dark" }});
await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches)");
// → true
await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches)");
// → false
await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
// → false
await page.EmulateMediaFeaturesAsync(new MediaFeature[]{ new MediaFeature { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" }});
await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches)");
// → true
await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
// → false
await page.EmulateMediaFeaturesAsync(new MediaFeature[]
{
  new MediaFeature { MediaFeature = MediaFeature.PrefersColorScheme, Value = "dark" },
  new MediaFeature { MediaFeature = MediaFeature.PrefersReducedMotion, Value = "reduce" },
});
await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: dark)').matches)");
// → true
await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: light)').matches)");
// → false
await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
// → false
await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-reduced-motion: reduce)').matches)");
// → true
await page.EvaluateFunctionAsync<bool>("() => matchMedia('(prefers-color-scheme: no-preference)').matches)");
// → false
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L520)

<a id="PuppeteerSharp_IPage_EmulateMediaTypeAsync_"></a>
<a id="PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_"></a>

### EmulateMediaTypeAsync(MediaType)


```csharp
Task EmulateMediaTypeAsync(MediaType type)
```


Emulates a media such as screen or print.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `type` | [MediaType](/api/PuppeteerSharp.Media/MediaType/) | Media to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Emulate media type task.

**Example**

```csharp
await page.EvaluateFunctionAsync<bool>("() => matchMedia('screen').matches)");
// → true
await page.EvaluateFunctionAsync<bool>("() => matchMedia('print').matches)");
// → true
await page.EmulateMediaTypeAsync(MediaType.Print);
await page.EvaluateFunctionAsync<bool>("() => matchMedia('screen').matches)");
// → false
await page.EvaluateFunctionAsync<bool>("() => matchMedia('print').matches)");
// → true
await page.EmulateMediaTypeAsync(MediaType.None);
await page.EvaluateFunctionAsync<bool>("() => matchMedia('screen').matches)");
// → true
await page.EvaluateFunctionAsync<bool>("() => matchMedia('print').matches)");
// → true
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L547)

<a id="PuppeteerSharp_IPage_EmulateNetworkConditionsAsync_"></a>
<a id="PuppeteerSharp_IPage_EmulateNetworkConditionsAsync_PuppeteerSharp_NetworkConditions_"></a>

### EmulateNetworkConditionsAsync(NetworkConditions)


```csharp
Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions)
```


Emulates network conditions.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `networkConditions` | [NetworkConditions](/api/PuppeteerSharp/NetworkConditions/) | Passing `null` disables network condition emulation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Result task.

**NOTE** This does not affect WebSockets and WebRTC PeerConnections (see https://crbug.com/563644).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L557)

<a id="PuppeteerSharp_IPage_EmulateTimezoneAsync_"></a>
<a id="PuppeteerSharp_IPage_EmulateTimezoneAsync_System_String_"></a>

### EmulateTimezoneAsync(string)


```csharp
Task EmulateTimezoneAsync(string timezoneId)
```


Changes the timezone of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timezoneId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Timezone to set. See [ICU’s `metaZones.txt`](https://cs.chromium.org/chromium/src/third_party/icu/source/data/misc/metaZones.txt?rcl=faee8bc70570192d82d2978a71e2a615788597d1) for a list of supported timezone IDs. Passing `null` disables timezone emulation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The viewport task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L565)

<a id="PuppeteerSharp_IPage_EmulateVisionDeficiencyAsync_"></a>
<a id="PuppeteerSharp_IPage_EmulateVisionDeficiencyAsync_PuppeteerSharp_VisionDeficiency_"></a>

### EmulateVisionDeficiencyAsync(VisionDeficiency)


```csharp
Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
```


Simulates the given vision deficiency on the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `type` | [VisionDeficiency](/api/PuppeteerSharp/VisionDeficiency/) | The type of deficiency to simulate, or [None](/api/PuppeteerSharp/VisionDeficiency/#PuppeteerSharp_VisionDeficiency_None) to reset. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to the browser.

**Example**

await Page.EmulateVisionDeficiencyAsync(VisionDeficiency.Achromatopsia);
await Page.ScreenshotAsync("Achromatopsia.png").

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L583)

<a id="PuppeteerSharp_IPage_EvaluateExpressionAsync_"></a>
<a id="PuppeteerSharp_IPage_EvaluateExpressionAsync_System_String_"></a>

### EvaluateExpressionAsync(string)


```csharp
Task EvaluateExpressionAsync(string script)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.

**Example**

An example of scraping information from all hyperlinks on the page.


```csharp
var hyperlinkInfo = await page.EvaluateExpressionAsync(@"
    Array
       .from(document.querySelectorAll('a'))
       .map(n => ({
           text: n.innerText,
           href: n.getAttribute('href'),
           target: n.getAttribute('target')
        }))
");
Console.WriteLine(hyperlinkInfo.ToString()); // Displays JSON array of hyperlinkInfo objects
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L609)

<a id="PuppeteerSharp_IPage_EvaluateExpressionAsync__1_System_String_"></a>

### `EvaluateExpressionAsync<T>(string)`


```csharp
Task<T> EvaluateExpressionAsync<T>(string script)
```


Executes a script in browser context.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type to deserialize the result to. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L621)

<a id="PuppeteerSharp_IPage_EvaluateExpressionHandleAsync_"></a>
<a id="PuppeteerSharp_IPage_EvaluateExpressionHandleAsync_System_String_"></a>

### EvaluateExpressionHandleAsync(string)


```csharp
Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L631)

<a id="PuppeteerSharp_IPage_EvaluateExpressionOnNewDocumentAsync_"></a>
<a id="PuppeteerSharp_IPage_EvaluateExpressionOnNewDocumentAsync_System_String_"></a>

### EvaluateExpressionOnNewDocumentAsync(string)


```csharp
Task<NewDocumentScriptEvaluation> EvaluateExpressionOnNewDocumentAsync(string expression)
```


Adds a function which would be invoked in one of the following scenarios:
- whenever the page is navigated
- whenever the child frame is attached or navigated. In this case, the function is invoked in the context of the newly attached frame.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `expression` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Javascript expression to be evaluated in browser context. |

**Returns:** [Task&lt;NewDocumentScriptEvaluation&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task.

The function is invoked after the document was created but before any of its scripts were run. This is useful to amend JavaScript environment, e.g. to seed `Math.random`.

**Example**

An example of overriding the navigator.languages property before the page loads:


```csharp
await page.EvaluateExpressionOnNewDocumentAsync("window.__example = true;");
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L649)

<a id="PuppeteerSharp_IPage_EvaluateFunctionAsync_"></a>
<a id="PuppeteerSharp_IPage_EvaluateFunctionAsync_System_String_System_Object___"></a>

### EvaluateFunctionAsync(string, params object[])


```csharp
Task EvaluateFunctionAsync(string script, params object[] args)
```


Executes a function in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to script. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
[IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L662)

<a id="PuppeteerSharp_IPage_EvaluateFunctionAsync__1_System_String_System_Object___"></a>

### `EvaluateFunctionAsync<T>(string, params object[])`


```csharp
Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
```


Executes a function in browser context.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type to deserialize the result to. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to script. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
[IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L676)

<a id="PuppeteerSharp_IPage_EvaluateFunctionHandleAsync_"></a>
<a id="PuppeteerSharp_IPage_EvaluateFunctionHandleAsync_System_String_System_Object___"></a>

### EvaluateFunctionHandleAsync(string, params object[])


```csharp
Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `pageFunction` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Function arguments. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
[IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L688)

<a id="PuppeteerSharp_IPage_EvaluateFunctionOnNewDocumentAsync_"></a>
<a id="PuppeteerSharp_IPage_EvaluateFunctionOnNewDocumentAsync_System_String_System_Object___"></a>

### EvaluateFunctionOnNewDocumentAsync(string, params object[])


```csharp
Task<NewDocumentScriptEvaluation> EvaluateFunctionOnNewDocumentAsync(string pageFunction, params object[] args)
```


Adds a function which would be invoked in one of the following scenarios:
- whenever the page is navigated
- whenever the child frame is attached or navigated. In this case, the function is invoked in the context of the newly attached frame.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `pageFunction` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `pageFunction`. |

**Returns:** [Task&lt;NewDocumentScriptEvaluation&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task.

The function is invoked after the document was created but before any of its scripts were run. This is useful to amend JavaScript environment, e.g. to seed `Math.random`.

**Example**

An example of overriding the navigator.languages property before the page loads:


```csharp
await page.EvaluateFunctionOnNewDocumentAsync("() => window.__example = true");
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L707)

<a id="PuppeteerSharp_IPage_ExposeFunctionAsync_"></a>
<a id="PuppeteerSharp_IPage_ExposeFunctionAsync_System_String_System_Action_"></a>

### ExposeFunctionAsync(string, Action)


```csharp
Task ExposeFunctionAsync(string name, Action puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes `puppeteerFunction` in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves when `puppeteerFunction` completes.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Action](https://learn.microsoft.com/dotnet/api/system.action) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the `puppeteerFunction` returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `ExposeFunctionAsync()` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L723)

<a id="PuppeteerSharp_IPage_ExposeFunctionAsync__1_System_String_System_Func___0__"></a>

### `ExposeFunctionAsync<TResult>(string, Func<TResult>)`


```csharp
Task ExposeFunctionAsync<TResult>(string name, Func<TResult> puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes `puppeteerFunction` in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves to the return value of `puppeteerFunction`.

**Type parameters**

| Name | Description |
| --- | --- |
| `TResult` | The result of `puppeteerFunction`. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Func&lt;TResult&gt;](https://learn.microsoft.com/dotnet/api/system.func-1) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the `puppeteerFunction` returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `ExposeFunctionAsync&lt;T&gt;()` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L819)

<a id="PuppeteerSharp_IPage_ExposeFunctionAsync__2_System_String_System_Func___0___1__"></a>

### `ExposeFunctionAsync<T, TResult>(string, Func<T, TResult>)`


```csharp
Task ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes `puppeteerFunction` in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves to the return value of `puppeteerFunction`.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The parameter of `puppeteerFunction`. |
| `TResult` | The result of `puppeteerFunction`. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Func&lt;T, TResult&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the `puppeteerFunction` returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `ExposeFunctionAsync&lt;T&gt;()` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L745)

<a id="PuppeteerSharp_IPage_ExposeFunctionAsync__3_System_String_System_Func___0___1___2__"></a>

### `ExposeFunctionAsync<T1, T2, TResult>(string, Func<T1, T2, TResult>)`


```csharp
Task ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes `puppeteerFunction` in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves to the return value of `puppeteerFunction`.

**Type parameters**

| Name | Description |
| --- | --- |
| `T1` | The first parameter of `puppeteerFunction`. |
| `T2` | The second parameter of `puppeteerFunction`. |
| `TResult` | The result of `puppeteerFunction`. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Func&lt;T1, T2, TResult&gt;](https://learn.microsoft.com/dotnet/api/system.func-3) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the `puppeteerFunction` returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `ExposeFunctionAsync&lt;T&gt;()` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L802)

<a id="PuppeteerSharp_IPage_ExposeFunctionAsync__4_System_String_System_Func___0___1___2___3__"></a>

### `ExposeFunctionAsync<T1, T2, T3, TResult>(string, Func<T1, T2, T3, TResult>)`


```csharp
Task ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes `puppeteerFunction` in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves to the return value of `puppeteerFunction`.

**Type parameters**

| Name | Description |
| --- | --- |
| `T1` | The first parameter of `puppeteerFunction`. |
| `T2` | The second parameter of `puppeteerFunction`. |
| `T3` | The third parameter of `puppeteerFunction`. |
| `TResult` | The result of `puppeteerFunction`. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Func&lt;T1, T2, T3, TResult&gt;](https://learn.microsoft.com/dotnet/api/system.func-4) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the `puppeteerFunction` returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `ExposeFunctionAsync&lt;T&gt;()` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L783)

<a id="PuppeteerSharp_IPage_ExposeFunctionAsync__5_System_String_System_Func___0___1___2___3___4__"></a>

### `ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string, Func<T1, T2, T3, T4, TResult>)`


```csharp
Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes `puppeteerFunction` in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves to the return value of `puppeteerFunction`.

**Type parameters**

| Name | Description |
| --- | --- |
| `T1` | The first parameter of `puppeteerFunction`. |
| `T2` | The second parameter of `puppeteerFunction`. |
| `T3` | The third parameter of `puppeteerFunction`. |
| `T4` | The fourth parameter of `puppeteerFunction`. |
| `TResult` | The result of `puppeteerFunction`. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Func&lt;T1, T2, T3, T4, TResult&gt;](https://learn.microsoft.com/dotnet/api/system.func-5) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the `puppeteerFunction` returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `ExposeFunctionAsync&lt;T&gt;()` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L763)

<a id="PuppeteerSharp_IPage_ExtensionRealms_"></a>
<a id="PuppeteerSharp_IPage_ExtensionRealms"></a>

### ExtensionRealms()


```csharp
IReadOnlyList<Realm> ExtensionRealms()
```


Returns the extension content-script realms associated with the page's main frame.

**Returns:** [IReadOnlyList&lt;Realm&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1) — A read-only list of extension [Realm](/api/PuppeteerSharp/Realm/) instances.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1745)

<a id="PuppeteerSharp_IPage_FocusAsync_"></a>
<a id="PuppeteerSharp_IPage_FocusAsync_System_String_"></a>

### FocusAsync(string)


```csharp
Task FocusAsync(string selector)
```


Fetches an element with `selector` and focuses it.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to focus. If there are multiple elements satisfying the selector, the first will be focused. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching `selector` is successfully focused.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching `selector`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L827)

<a id="PuppeteerSharp_IPage_GetContentAsync_"></a>
<a id="PuppeteerSharp_IPage_GetContentAsync_PuppeteerSharp_GetContentOptions_"></a>

### GetContentAsync(GetContentOptions)


```csharp
Task<string> GetContentAsync(GetContentOptions options = null)
```


Gets the full HTML contents of the page, including the doctype.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [GetContentOptions](/api/PuppeteerSharp/GetContentOptions/) | Options. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the HTML content.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L835)

<a id="PuppeteerSharp_IPage_GetCookiesAsync_"></a>
<a id="PuppeteerSharp_IPage_GetCookiesAsync_System_String___"></a>

### GetCookiesAsync(params string[])


```csharp
Task<CookieParam[]> GetCookiesAsync(params string[] urls)
```


Returns the page's cookies.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `urls` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Url's to return cookies for. |

**Returns:** [Task&lt;CookieParam\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Array of cookies.

If no URLs are specified, this method returns cookies for the current page URL.
If URLs are specified, only cookies for those URLs are returned.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L846)

<a id="PuppeteerSharp_IPage_GetTitleAsync_"></a>
<a id="PuppeteerSharp_IPage_GetTitleAsync"></a>

### GetTitleAsync()


```csharp
Task<string> GetTitleAsync()
```


Returns page's title.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — page's title.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L853)

<a id="PuppeteerSharp_IPage_GoBackAsync_"></a>
<a id="PuppeteerSharp_IPage_GoBackAsync_PuppeteerSharp_NavigationOptions_"></a>

### GoBackAsync(NavigationOptions)


```csharp
Task<IResponse> GoBackAsync(NavigationOptions options = null)
```


Navigate to the previous page in history.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | Navigation parameters. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task that resolves to the main resource response. In case of multiple redirects,
    the navigation will resolve with the response of the last redirect. If can not go back, resolves to null.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L861)

<a id="PuppeteerSharp_IPage_GoForwardAsync_"></a>
<a id="PuppeteerSharp_IPage_GoForwardAsync_PuppeteerSharp_NavigationOptions_"></a>

### GoForwardAsync(NavigationOptions)


```csharp
Task<IResponse> GoForwardAsync(NavigationOptions options = null)
```


Navigate to the next page in history.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | Navigation parameters. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task that resolves to the main resource response. In case of multiple redirects,
    the navigation will resolve with the response of the last redirect. If can not go forward, resolves to null.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L869)

<a id="PuppeteerSharp_IPage_GoToAsync_"></a>
<a id="PuppeteerSharp_IPage_GoToAsync_System_String_PuppeteerSharp_NavigationOptions_"></a>

### GoToAsync(string, NavigationOptions)


```csharp
Task<IResponse> GoToAsync(string url, NavigationOptions options)
```


Navigates to an url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | URL to navigate page to. The url should include scheme, e.g. https://. |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | Navigation parameters. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

`GoToAsync()` will throw an error if:
- there's an SSL error (e.g. in case of self-signed certificates).
- target URL is invalid.
- the `timeout` is exceeded during navigation.
- the remote server does not respond or is unreachable.
- the main resource failed to load.

`GoToAsync()` will not throw an error when any valid HTTP status code is returned by the remote server,
including 404 "Not Found" and 500 "Internal Server Error".  The status code for such responses can be retrieved by calling [Status](/api/PuppeteerSharp/IResponse/#PuppeteerSharp_IResponse_Status)

&gt; **NOTE** `GoToAsync()` either throws an error or returns a main resource response.
The only exceptions are navigation to `about:blank` or navigation to the same URL with a different hash, which would succeed and return `null`.

&gt; **NOTE** Headless mode doesn't support navigation to a PDF document. See the &lt;see fref="https://bugs.chromium.org/p/chromium/issues/detail?id=761295"&gt;upstream issue&lt;/see&gt;.

Shortcut for. `GoToAsync()`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L910)

<a id="PuppeteerSharp_IPage_GoToAsync_System_String_PuppeteerSharp_WaitUntilNavigation_"></a>

### GoToAsync(string, WaitUntilNavigation)


```csharp
Task<IResponse> GoToAsync(string url, WaitUntilNavigation waitUntil)
```


Navigates to an url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | URL to navigate page to. The url should include scheme, e.g. https://. |
| `waitUntil` | [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/) | When to consider navigation succeeded. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L919)

<a id="PuppeteerSharp_IPage_GoToAsync_System_String_System_Nullable_System_Int32__PuppeteerSharp_WaitUntilNavigation___"></a>

### GoToAsync(string, int?, WaitUntilNavigation[])


```csharp
Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null)
```


Navigates to an url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | URL to navigate page to. The url should include scheme, e.g. https://. |
| `timeout` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Maximum navigation time in milliseconds, defaults to 30 seconds, pass `0` to disable timeout. |
| `waitUntil` | `WaitUntilNavigation\[\]` | When to consider navigation succeeded, defaults to [Load](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Load). Given an array of [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/), navigation is considered to be successful after all events have been fired. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L879)

<a id="PuppeteerSharp_IPage_HasDevToolsAsync_"></a>
<a id="PuppeteerSharp_IPage_HasDevToolsAsync"></a>

### HasDevToolsAsync()


```csharp
Task<bool> HasDevToolsAsync()
```


Returns true if DevTools is attached to the current page.
Use [OpenDevToolsAsync()](#PuppeteerSharp_IPage_OpenDevToolsAsync) to get the DevTools page.

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A `Task&lt;T&gt;` that completes with true if DevTools is attached.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L417)

<a id="PuppeteerSharp_IPage_HoverAsync_"></a>
<a id="PuppeteerSharp_IPage_HoverAsync_System_String_"></a>

### HoverAsync(string)


```csharp
Task HoverAsync(string selector)
```


Fetches an element with `selector`, scrolls it into view if needed, and then uses [Mouse](#PuppeteerSharp_IPage_Mouse) to hover over the center of the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to hover. If there are multiple elements satisfying the selector, the first will be hovered. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching `selector` is successfully hovered.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching `selector`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L927)

<a id="PuppeteerSharp_IPage_Locator_"></a>
<a id="PuppeteerSharp_IPage_Locator_System_String_"></a>

### Locator(string)


```csharp
Locator Locator(string selector)
```


Creates a locator for the provided selector. See [Locator](/api/PuppeteerSharp.Locators/Locator/) for
details and supported actions.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to locate an element. |

**Returns:** [Locator](/api/PuppeteerSharp.Locators/Locator/) — A locator for the provided selector.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1603)

<a id="PuppeteerSharp_IPage_LocatorFunction_"></a>
<a id="PuppeteerSharp_IPage_LocatorFunction_System_String_"></a>

### LocatorFunction(string)


```csharp
Locator LocatorFunction(string func)
```


Creates a locator for the provided function. See [Locator](/api/PuppeteerSharp.Locators/Locator/) for
details and supported actions.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `func` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A JavaScript function expression to evaluate. |

**Returns:** [Locator](/api/PuppeteerSharp.Locators/Locator/) — A locator for the provided function.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1611)

<a id="PuppeteerSharp_IPage_MetricsAsync_"></a>
<a id="PuppeteerSharp_IPage_MetricsAsync"></a>

### MetricsAsync()


```csharp
Task<Dictionary<string, decimal>> MetricsAsync()
```


Returns metrics.

**Returns:** [Task&lt;Dictionary&lt;string, decimal&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves into a list of metrics.

All timestamps are in monotonic time: monotonically increasing time in seconds since an arbitrary point in the past.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L936)

<a id="PuppeteerSharp_IPage_OpenDevToolsAsync_"></a>
<a id="PuppeteerSharp_IPage_OpenDevToolsAsync"></a>

### OpenDevToolsAsync()


```csharp
Task<IPage> OpenDevToolsAsync()
```


Opens DevTools for this page if not already open and returns the DevTools page.
This method is only available in Chrome.

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A `Task&lt;T&gt;` that completes with the DevTools page.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L410)

<a id="PuppeteerSharp_IPage_PdfAsync_"></a>
<a id="PuppeteerSharp_IPage_PdfAsync_System_String_"></a>

### PdfAsync(string)


```csharp
Task PdfAsync(string file)
```


generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the PDF to. paths are resolved using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the PDF is generated.

Generating a pdf is currently only supported in Chrome headless.
The media type emulated with [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) persists on the page, so call it with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) before generating the PDF to use screen styles.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L954)

<a id="PuppeteerSharp_IPage_PdfAsync_System_String_PuppeteerSharp_PdfOptions_"></a>

### PdfAsync(string, PdfOptions)


```csharp
Task PdfAsync(string file, PdfOptions options)
```


generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the PDF to. paths are resolved using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |
| `options` | [PdfOptions](/api/PuppeteerSharp/PdfOptions/) | pdf options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the PDF is generated.

Generating a pdf is currently only supported in Chrome headless.
The media type emulated with [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) persists on the page, so call it with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) before generating the PDF to use screen styles.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L966)

<a id="PuppeteerSharp_IPage_PdfDataAsync_"></a>
<a id="PuppeteerSharp_IPage_PdfDataAsync"></a>

### PdfDataAsync()


```csharp
Task<byte[]> PdfDataAsync()
```


Generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the PDF data.

Generating a pdf is currently only supported in Chrome headless.
The media type emulated with [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) persists on the page, so call it with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) before generating the PDF to use screen styles.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L976)

<a id="PuppeteerSharp_IPage_PdfDataAsync_PuppeteerSharp_PdfOptions_"></a>

### PdfDataAsync(PdfOptions)


```csharp
Task<byte[]> PdfDataAsync(PdfOptions options)
```


Generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [PdfOptions](/api/PuppeteerSharp/PdfOptions/) | pdf options. |

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the PDF data.

Generating a pdf is currently only supported in Chrome headless.
The media type emulated with [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) persists on the page, so call it with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) before generating the PDF to use screen styles.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L987)

<a id="PuppeteerSharp_IPage_PdfStreamAsync_"></a>
<a id="PuppeteerSharp_IPage_PdfStreamAsync"></a>

### PdfStreamAsync()


```csharp
Task<Stream> PdfStreamAsync()
```


generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the PDF data.

Generating a pdf is currently only supported in Chrome headless.
The media type emulated with [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) persists on the page, so call it with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) before generating the PDF to use screen styles.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L997)

<a id="PuppeteerSharp_IPage_PdfStreamAsync_PuppeteerSharp_PdfOptions_"></a>

### PdfStreamAsync(PdfOptions)


```csharp
Task<Stream> PdfStreamAsync(PdfOptions options)
```


Generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [PdfOptions](/api/PuppeteerSharp/PdfOptions/) | pdf options. |

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the PDF data.

Generating a pdf is currently only supported in Chrome headless.
The media type emulated with [EmulateMediaTypeAsync(MediaType)](#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) persists on the page, so call it with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) before generating the PDF to use screen styles.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1008)

<a id="PuppeteerSharp_IPage_QueryObjectsAsync_"></a>
<a id="PuppeteerSharp_IPage_QueryObjectsAsync_PuppeteerSharp_IJSHandle_"></a>

### QueryObjectsAsync(IJSHandle)


```csharp
Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle)
```


The method iterates JavaScript heap and finds all the objects with the given prototype.
Shortcut for `page.MainFrame.GetExecutionContextAsync().QueryObjectsAsync(prototypeHandle)`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `prototypeHandle` | [IJSHandle](/api/PuppeteerSharp/IJSHandle/) | A handle to the object prototype. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves to a handle to an array of objects with this prototype.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1016)

<a id="PuppeteerSharp_IPage_QuerySelectorAllAsync_"></a>
<a id="PuppeteerSharp_IPage_QuerySelectorAllAsync_System_String_"></a>

### QuerySelectorAllAsync(string)


```csharp
Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
```


Runs `document.querySelectorAll` within the page. If no elements match the selector, the return value resolve to `Empty&lt;T&gt;`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query page for. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1024)

<a id="PuppeteerSharp_IPage_QuerySelectorAllAsync_System_String_PuppeteerSharp_QueryOptions_"></a>

### QuerySelectorAllAsync(string, QueryOptions)


```csharp
Task<IElementHandle[]> QuerySelectorAllAsync(string selector, QueryOptions options)
```


Runs `document.querySelectorAll` within the page. If no elements match the selector, the return value resolve to `Empty&lt;T&gt;`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query page for. |
| `options` | [QueryOptions](/api/PuppeteerSharp/QueryOptions/) | Optional query options. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1033)

<a id="PuppeteerSharp_IPage_QuerySelectorAllHandleAsync_"></a>
<a id="PuppeteerSharp_IPage_QuerySelectorAllHandleAsync_System_String_"></a>

### QuerySelectorAllHandleAsync(string)


```csharp
Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
```


A utility function to be used with `EvaluateFunctionAsync&lt;T&gt;()`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query page for. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [IJSHandle](/api/PuppeteerSharp/IJSHandle/) of `document.querySelectorAll` result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1040)

<a id="PuppeteerSharp_IPage_QuerySelectorAsync_"></a>
<a id="PuppeteerSharp_IPage_QuerySelectorAsync_System_String_"></a>

### QuerySelectorAsync(string)


```csharp
Task<IElementHandle> QuerySelectorAsync(string selector)
```


The method runs `document.querySelector` within the page. If no element matches the selector, the return value resolve to `null`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query page for. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to [IElementHandle](/api/PuppeteerSharp/IElementHandle/) pointing to the frame element.

Shortcut for `page.MainFrame.QuerySelectorAsync(selector)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1051)

<a id="PuppeteerSharp_IPage_RecordAsync_"></a>
<a id="PuppeteerSharp_IPage_RecordAsync_PuppeteerSharp_RecordOptions_"></a>

### RecordAsync(RecordOptions)


```csharp
Task<ScreenRecording> RecordAsync(RecordOptions options = null)
```


Records this [IPage](./) using the Chrome DevTools Protocol
[Page.startScreenRecording](https://chromedevtools.github.io/devtools-protocol/tot/Page/#method-startScreenRecording) API.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [RecordOptions](/api/PuppeteerSharp/RecordOptions/) | Recording options. |

**Returns:** [Task&lt;ScreenRecording&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves to a [ScreenRecording](/api/PuppeteerSharp/ScreenRecording/) that can be used to stop the recording.

Outputs mp4 video stream.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1091)

<a id="PuppeteerSharp_IPage_ReloadAsync_"></a>
<a id="PuppeteerSharp_IPage_ReloadAsync_PuppeteerSharp_ReloadOptions_"></a>

### ReloadAsync(ReloadOptions)


```csharp
Task<IResponse> ReloadAsync(ReloadOptions options)
```


Reloads the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ReloadOptions](/api/PuppeteerSharp/ReloadOptions/) | Navigation options. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1068)

<a id="PuppeteerSharp_IPage_ReloadAsync_System_Nullable_System_Int32__PuppeteerSharp_WaitUntilNavigation___"></a>

### ReloadAsync(int?, WaitUntilNavigation[])


```csharp
Task<IResponse> ReloadAsync(int? timeout = null, WaitUntilNavigation[] waitUntil = null)
```


Reloads the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timeout` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Maximum navigation time in milliseconds, defaults to 30 seconds, pass `0` to disable timeout. |
| `waitUntil` | `WaitUntilNavigation\[\]` | When to consider navigation succeeded, defaults to [Load](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Load). Given an array of [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/), navigation is considered to be successful after all events have been fired. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1060)

<a id="PuppeteerSharp_IPage_RemoveExposedFunctionAsync_"></a>
<a id="PuppeteerSharp_IPage_RemoveExposedFunctionAsync_System_String_"></a>

### RemoveExposedFunctionAsync(string)


```csharp
Task RemoveExposedFunctionAsync(string name)
```


Removes a previously added function via `ExposeFunctionAsync()`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function to remove. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L730)

<a id="PuppeteerSharp_IPage_RemoveRequestInterceptor_"></a>
<a id="PuppeteerSharp_IPage_RemoveRequestInterceptor_System_Func_PuppeteerSharp_IRequest_System_Threading_Tasks_Task__"></a>

### `RemoveRequestInterceptor(Func<IRequest, Task>)`


```csharp
void RemoveRequestInterceptor(Func<IRequest, Task> interceptionTask)
```


Removes a previously added request interceptor.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `interceptionTask` | [Func&lt;IRequest, Task&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Interception task. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1712)

<a id="PuppeteerSharp_IPage_RemoveScriptToEvaluateOnNewDocumentAsync_"></a>
<a id="PuppeteerSharp_IPage_RemoveScriptToEvaluateOnNewDocumentAsync_System_String_"></a>

### RemoveScriptToEvaluateOnNewDocumentAsync(string)


```csharp
Task RemoveScriptToEvaluateOnNewDocumentAsync(string identifier)
```


Removes script that injected into page by [EvaluateExpressionOnNewDocumentAsync(string)](#PuppeteerSharp_IPage_EvaluateExpressionOnNewDocumentAsync_System_String_) or [EvaluateExpressionOnNewDocumentAsync(string)](#PuppeteerSharp_IPage_EvaluateExpressionOnNewDocumentAsync_System_String_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `identifier` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function identifier. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the script is removed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1725)

<a id="PuppeteerSharp_IPage_ResizeAsync_"></a>
<a id="PuppeteerSharp_IPage_ResizeAsync_System_Int32_System_Int32_"></a>

### ResizeAsync(int, int)


```csharp
Task ResizeAsync(int contentWidth, int contentHeight)
```


Resizes the browser window of this page so that the content area (excluding browser UI) has the specified width and height.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `contentWidth` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | The desired width of the content area in pixels. |
| `contentHeight` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | The desired height of the content area in pixels. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the resize operation is complete.

This is an experimental API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1678)

<a id="PuppeteerSharp_IPage_ScreencastAsync_"></a>
<a id="PuppeteerSharp_IPage_ScreencastAsync_PuppeteerSharp_ScreencastOptions_"></a>

### ScreencastAsync(ScreencastOptions)


```csharp
[Obsolete("Use RecordAsync instead.")]
Task<ScreenRecorder> ScreencastAsync(ScreencastOptions options = null)
```


Captures a screencast of this [IPage](./).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ScreencastOptions](/api/PuppeteerSharp/ScreencastOptions/) | Screencast options. |

**Returns:** [Task&lt;ScreenRecorder&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to a [ScreenRecorder](/api/PuppeteerSharp/ScreenRecorder/) that can be used to stop the recording.

:::caution[Obsolete]
This API is obsolete.
:::

By default, all recordings will be WebM format using the VP9 video codec, with a frame rate of 30 FPS.
You must have ffmpeg installed on your system.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1079)

<a id="PuppeteerSharp_IPage_ScreenshotAsync_"></a>
<a id="PuppeteerSharp_IPage_ScreenshotAsync_System_String_"></a>

### ScreenshotAsync(string)


```csharp
Task ScreenshotAsync(string file)
```


Captures a screenshot of this [IPage](./).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the image to. The screenshot type will be inferred from file extension. If path is a relative path, then it is resolved relative to current working directory. If no path is provided, the image won't be saved to the disk. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The screenshot task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1100)

<a id="PuppeteerSharp_IPage_ScreenshotAsync_System_String_PuppeteerSharp_ScreenshotOptions_"></a>

### ScreenshotAsync(string, ScreenshotOptions)


```csharp
Task ScreenshotAsync(string file, ScreenshotOptions options)
```


Takes a screenshot of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the image to. The screenshot type will be inferred from file extension. If path is a relative path, then it is resolved relative to current working directory. If no path is provided, the image won't be saved to the disk. |
| `options` | [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Screenshot options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The screenshot task.

While a screenshot is being taken in an [IBrowser](/api/PuppeteerSharp/IBrowser/), the following methods will
automatically wait for the screenshot to finish to prevent interference with the screenshot process:
[NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowserContext/#PuppeteerSharp_IBrowserContext_NewPageAsync_PuppeteerSharp_CreatePageOptions_),
[NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_NewPageAsync_PuppeteerSharp_CreatePageOptions_) and [CloseAsync(PageCloseOptions)](#PuppeteerSharp_IPage_CloseAsync_PuppeteerSharp_PageCloseOptions_).
Calling [BringToFrontAsync()](#PuppeteerSharp_IPage_BringToFrontAsync) will not wait for existing screenshot operations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1117)

<a id="PuppeteerSharp_IPage_ScreenshotBase64Async_"></a>
<a id="PuppeteerSharp_IPage_ScreenshotBase64Async"></a>

### ScreenshotBase64Async()


```csharp
Task<string> ScreenshotBase64Async()
```


Takes a screenshot of the page.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [string](https://learn.microsoft.com/dotnet/api/system.string) containing the image data as base64.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1123)

<a id="PuppeteerSharp_IPage_ScreenshotBase64Async_PuppeteerSharp_ScreenshotOptions_"></a>

### ScreenshotBase64Async(ScreenshotOptions)


```csharp
Task<string> ScreenshotBase64Async(ScreenshotOptions options)
```


Takes a screenshot of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [string](https://learn.microsoft.com/dotnet/api/system.string) containing the image data as base64.

While a screenshot is being taken in an [IBrowser](/api/PuppeteerSharp/IBrowser/), the following methods will
automatically wait for the screenshot to finish to prevent interference with the screenshot process:
[NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowserContext/#PuppeteerSharp_IBrowserContext_NewPageAsync_PuppeteerSharp_CreatePageOptions_),
[NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_NewPageAsync_PuppeteerSharp_CreatePageOptions_) and [CloseAsync(PageCloseOptions)](#PuppeteerSharp_IPage_CloseAsync_PuppeteerSharp_PageCloseOptions_).
Calling [BringToFrontAsync()](#PuppeteerSharp_IPage_BringToFrontAsync) will not wait for existing screenshot operations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1137)

<a id="PuppeteerSharp_IPage_ScreenshotDataAsync_"></a>
<a id="PuppeteerSharp_IPage_ScreenshotDataAsync"></a>

### ScreenshotDataAsync()


```csharp
Task<byte[]> ScreenshotDataAsync()
```


Takes a screenshot of the page.

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1143)

<a id="PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_"></a>

### ScreenshotDataAsync(ScreenshotOptions)


```csharp
Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
```


Takes a screenshot of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the image data.

While a screenshot is being taken in an [IBrowser](/api/PuppeteerSharp/IBrowser/), the following methods will
automatically wait for the screenshot to finish to prevent interference with the screenshot process:
[NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowserContext/#PuppeteerSharp_IBrowserContext_NewPageAsync_PuppeteerSharp_CreatePageOptions_),
[NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_NewPageAsync_PuppeteerSharp_CreatePageOptions_) and [CloseAsync(PageCloseOptions)](#PuppeteerSharp_IPage_CloseAsync_PuppeteerSharp_PageCloseOptions_).
Calling [BringToFrontAsync()](#PuppeteerSharp_IPage_BringToFrontAsync) will not wait for existing screenshot operations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1157)

<a id="PuppeteerSharp_IPage_ScreenshotStreamAsync_"></a>
<a id="PuppeteerSharp_IPage_ScreenshotStreamAsync"></a>

### ScreenshotStreamAsync()


```csharp
Task<Stream> ScreenshotStreamAsync()
```


Takes a screenshot of the page.

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1163)

<a id="PuppeteerSharp_IPage_ScreenshotStreamAsync_PuppeteerSharp_ScreenshotOptions_"></a>

### ScreenshotStreamAsync(ScreenshotOptions)


```csharp
Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
```


Takes a screenshot of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the image data.

While a screenshot is being taken in an [IBrowser](/api/PuppeteerSharp/IBrowser/), the following methods will
automatically wait for the screenshot to finish to prevent interference with the screenshot process:
[NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowserContext/#PuppeteerSharp_IBrowserContext_NewPageAsync_PuppeteerSharp_CreatePageOptions_),
[NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_NewPageAsync_PuppeteerSharp_CreatePageOptions_) and [CloseAsync(PageCloseOptions)](#PuppeteerSharp_IPage_CloseAsync_PuppeteerSharp_PageCloseOptions_).
Calling [BringToFrontAsync()](#PuppeteerSharp_IPage_BringToFrontAsync) will not wait for existing screenshot operations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1177)

<a id="PuppeteerSharp_IPage_SelectAsync_"></a>
<a id="PuppeteerSharp_IPage_SelectAsync_System_String_System_String___"></a>

### SelectAsync(string, params string[])


```csharp
Task<string[]> SelectAsync(string selector, params string[] values)
```


Triggers a change and input event once all the provided options have been selected.
If there's no &lt;select&gt; element matching selector, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query page for. |
| `values` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Values of options to select. If the &lt;select&gt; has the multiple attribute, all values are considered, otherwise only the first one is taken into account. |

**Returns:** [Task&lt;string\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Returns an array of option values that have been successfully selected.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching `selector`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1189)

<a id="PuppeteerSharp_IPage_SetBurstModeOffAsync_"></a>
<a id="PuppeteerSharp_IPage_SetBurstModeOffAsync"></a>

### SetBurstModeOffAsync()


```csharp
Task SetBurstModeOffAsync()
```


Resets the background color and Viewport after taking Screenshots using BurstMode.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The burst mode off.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1195)

<a id="PuppeteerSharp_IPage_SetBypassCSPAsync_"></a>
<a id="PuppeteerSharp_IPage_SetBypassCSPAsync_System_Boolean_"></a>

### SetBypassCSPAsync(bool)


```csharp
Task SetBypassCSPAsync(bool enabled)
```


Toggles bypassing page's Content-Security-Policy.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | sets bypassing of page's Content-Security-Policy. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the message is sent to the browser.

CSP bypassing happens at the moment of CSP initialization rather then evaluation.
Usually this means that [SetBypassCSPAsync(bool)](#PuppeteerSharp_IPage_SetBypassCSPAsync_System_Boolean_) should be called before navigating to the domain.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1206)

<a id="PuppeteerSharp_IPage_SetBypassServiceWorkerAsync_"></a>
<a id="PuppeteerSharp_IPage_SetBypassServiceWorkerAsync_System_Boolean_"></a>

### SetBypassServiceWorkerAsync(bool)


```csharp
Task SetBypassServiceWorkerAsync(bool bypass)
```


Toggles ignoring of service worker for each request.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `bypass` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | When `true` bypasses service worker. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message is sent to the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1732)

<a id="PuppeteerSharp_IPage_SetCacheEnabledAsync_"></a>
<a id="PuppeteerSharp_IPage_SetCacheEnabledAsync_System_Boolean_"></a>

### SetCacheEnabledAsync(bool)


```csharp
Task SetCacheEnabledAsync(bool enabled = true)
```


Toggles ignoring cache for each request based on the enabled state. By default, caching is enabled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | sets the `enabled` state of the cache. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1213)

<a id="PuppeteerSharp_IPage_SetContentAsync_"></a>
<a id="PuppeteerSharp_IPage_SetContentAsync_System_String_PuppeteerSharp_NavigationOptions_"></a>

### SetContentAsync(string, NavigationOptions)


```csharp
[Obsolete("Use SetContentAsync(string, SetContentOptions) instead. The networkidle0 and networkidle2 wait conditions never worked reliably with SetContent.")]
Task SetContentAsync(string html, NavigationOptions options)
```


Sets the HTML markup to the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `html` | [string](https://learn.microsoft.com/dotnet/api/system.string) | HTML markup to assign to the page. |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | The navigations options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1222)

<a id="PuppeteerSharp_IPage_SetContentAsync_System_String_PuppeteerSharp_SetContentOptions_"></a>

### SetContentAsync(string, SetContentOptions)


```csharp
Task SetContentAsync(string html, SetContentOptions options = null)
```


Sets the HTML markup to the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `html` | [string](https://learn.microsoft.com/dotnet/api/system.string) | HTML markup to assign to the page. |
| `options` | [SetContentOptions](/api/PuppeteerSharp/SetContentOptions/) | The options. The [Networkidle0](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Networkidle0) and [Networkidle2](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Networkidle2) wait conditions are not supported for SetContent. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1235)

<a id="PuppeteerSharp_IPage_SetCookieAsync_"></a>
<a id="PuppeteerSharp_IPage_SetCookieAsync_PuppeteerSharp_CookieParam___"></a>

### SetCookieAsync(params CookieParam[])


```csharp
Task SetCookieAsync(params CookieParam[] cookies)
```


Clears all of the current cookies and then sets the cookies for the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieParam\[\]` | Cookies to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1242)

<a id="PuppeteerSharp_IPage_SetDragInterceptionAsync_"></a>
<a id="PuppeteerSharp_IPage_SetDragInterceptionAsync_System_Boolean_"></a>

### SetDragInterceptionAsync(bool)


```csharp
[Obsolete("We no longer support intercepting drag payloads. Use the new drag APIs found on ElementHandle to drag (or just use the Page.Mouse)")]
Task SetDragInterceptionAsync(bool enabled)
```


Whether to enable drag interception.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Interception enabled. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

:::caution[Obsolete]
This API is obsolete.
:::

Activating drag interception enables the `Input.drag`,
methods This provides the capability to capture drag events emitted
on the page, which can then be used to simulate drag-and-drop.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1254)

<a id="PuppeteerSharp_IPage_SetExtraHttpHeadersAsync_"></a>
<a id="PuppeteerSharp_IPage_SetExtraHttpHeadersAsync_System_Collections_Generic_Dictionary_System_String_System_String__"></a>

### `SetExtraHttpHeadersAsync(Dictionary<string, string>)`


```csharp
Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
```


Sets extra HTTP headers that will be sent with every request the page initiates.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `headers` | [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) | Additional http headers to be sent with every request. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1264)

<a id="PuppeteerSharp_IPage_SetGeolocationAsync_"></a>
<a id="PuppeteerSharp_IPage_SetGeolocationAsync_PuppeteerSharp_GeolocationOption_"></a>

### SetGeolocationAsync(GeolocationOption)


```csharp
Task SetGeolocationAsync(GeolocationOption options)
```


Sets the page's geolocation.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [GeolocationOption](/api/PuppeteerSharp/GeolocationOption/) | Geolocation options. Pass `null` to remove the geolocation override. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

Consider using `OverridePermissionsAsync()` to grant permissions for the page to read its geolocation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1274)

<a id="PuppeteerSharp_IPage_SetJavaScriptEnabledAsync_"></a>
<a id="PuppeteerSharp_IPage_SetJavaScriptEnabledAsync_System_Boolean_"></a>

### SetJavaScriptEnabledAsync(bool)


```csharp
Task SetJavaScriptEnabledAsync(bool enabled)
```


Enables/Disables Javascript on the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether or not to enable JavaScript on the page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1281)

<a id="PuppeteerSharp_IPage_SetOfflineModeAsync_"></a>
<a id="PuppeteerSharp_IPage_SetOfflineModeAsync_System_Boolean_"></a>

### SetOfflineModeAsync(bool)


```csharp
Task SetOfflineModeAsync(bool value)
```


Set offline mode for the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `value` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | When `true` enables offline mode for the page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Result task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1288)

<a id="PuppeteerSharp_IPage_SetRequestInterceptionAsync_"></a>
<a id="PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_"></a>

### SetRequestInterceptionAsync(bool)


```csharp
Task SetRequestInterceptionAsync(bool value)
```


Activating request interception enables `AbortAsync()`,
`ContinueAsync()` and `RespondAsync()` methods.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `value` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether to enable request interception.. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The request interception task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1296)

<a id="PuppeteerSharp_IPage_SetUserAgentAsync_"></a>
<a id="PuppeteerSharp_IPage_SetUserAgentAsync_PuppeteerSharp_SetUserAgentOptions_"></a>

### SetUserAgentAsync(SetUserAgentOptions)


```csharp
Task SetUserAgentAsync(SetUserAgentOptions options)
```


Sets the user agent, user agent metadata, and platform.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [SetUserAgentOptions](/api/PuppeteerSharp/SetUserAgentOptions/) | User agent options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1311)

<a id="PuppeteerSharp_IPage_SetUserAgentAsync_System_String_PuppeteerSharp_UserAgentMetadata_"></a>

### SetUserAgentAsync(string, UserAgentMetadata)


```csharp
Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentMetadata = null)
```


Sets the user agent to be used in this page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `userAgent` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Specific user agent to use in this page. |
| `userAgentMetadata` | [UserAgentMetadata](/api/PuppeteerSharp/UserAgentMetadata/) | Specific user agent client hint data to use in this page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1304)

<a id="PuppeteerSharp_IPage_SetViewportAsync_"></a>
<a id="PuppeteerSharp_IPage_SetViewportAsync_PuppeteerSharp_ViewPortOptions_"></a>

### SetViewportAsync(ViewPortOptions)


```csharp
Task SetViewportAsync(ViewPortOptions viewport)
```


Sets the viewport.
In the case of multiple pages in a single browser, each page can have its own viewport size.
[SetViewportAsync(ViewPortOptions)](#PuppeteerSharp_IPage_SetViewportAsync_PuppeteerSharp_ViewPortOptions_) will resize the page. A lot of websites don't expect phones to change size, so you should set the viewport before navigating to the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `viewport` | [ViewPortOptions](/api/PuppeteerSharp/ViewPortOptions/) | Viewport options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The viewport task.

**Example**

using(var page = await browser.NewPageAsync())
{
    await page.SetViewPortAsync(new ViewPortOptions
    {
        Width = 640,
        Height = 480,
        DeviceScaleFactor = 1
    });
    await page.goto('https://www.example.com');
}

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1334)

<a id="PuppeteerSharp_IPage_TabInfoAsync_"></a>
<a id="PuppeteerSharp_IPage_TabInfoAsync"></a>

### TabInfoAsync()


```csharp
Task<TabInfo> TabInfoAsync()
```


Returns information about the browser tab of this page. The result is a snapshot and is not updated when the tab changes.

**Returns:** [Task&lt;TabInfo&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the tab info, or `null` if the page is not in a tab strip (for example, in chrome-headless-shell) or the browser does not report tab information.

This is an experimental API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1696)

<a id="PuppeteerSharp_IPage_TapAsync_"></a>
<a id="PuppeteerSharp_IPage_TapAsync_System_String_"></a>

### TapAsync(string)


```csharp
Task TapAsync(string selector)
```


Fetches an element with `selector`, scrolls it into view if needed, and then uses [Touchscreen](#PuppeteerSharp_IPage_Touchscreen) to tap in the center of the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to tap. If there are multiple elements satisfying the selector, the first will be clicked. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching `selector` is successfully tapped.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching `selector`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1342)

<a id="PuppeteerSharp_IPage_TriggerExtensionActionAsync_"></a>
<a id="PuppeteerSharp_IPage_TriggerExtensionActionAsync_PuppeteerSharp_Extension_"></a>

### TriggerExtensionActionAsync(Extension)


```csharp
Task TriggerExtensionActionAsync(Extension extension)
```


Triggers an extension action for the given extension.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `extension` | [Extension](/api/PuppeteerSharp/Extension/) | The extension to trigger the action for. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the extension action has been triggered.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1739)

<a id="PuppeteerSharp_IPage_TypeAsync_"></a>
<a id="PuppeteerSharp_IPage_TypeAsync_System_String_System_String_PuppeteerSharp_Input_TypeOptions_"></a>

### TypeAsync(string, string, TypeOptions)


```csharp
Task TypeAsync(string selector, string text, TypeOptions options = null)
```


Sends a `keydown`, `keypress`/`input`, and `keyup` event for each character in the text.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector of an element to type into. If there are multiple elements satisfying the selector, the first will be used. |
| `text` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A text to type into a focused element. |
| `options` | [TypeOptions](/api/PuppeteerSharp.Input/TypeOptions/) | The options to apply to the type operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching `selector`. |

To press a special key, like `Control` or `ArrowDown` use `PressAsync()`.

**Example**

```csharp
await page.TypeAsync("#mytextarea", "Hello"); // Types instantly
await page.TypeAsync("#mytextarea", "World", new TypeOptions { Delay = 100 }); // Types slower, like a user
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1361)

<a id="PuppeteerSharp_IPage_WaitForDevicePromptAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForDevicePromptAsync_PuppeteerSharp_WaitForOptions_"></a>

### WaitForDevicePromptAsync(WaitForOptions)


```csharp
Task<DeviceRequestPrompt> WaitForDevicePromptAsync(WaitForOptions options = null)
```


This method is typically coupled with an action that triggers a device
request from an api such as WebBluetooth.

Caution.

This must be called before the device request is made. It will not return a
currently active device prompt.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;DeviceRequestPrompt&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves after the page gets the prompt.

**Example**

```cs

```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1667)

<a id="PuppeteerSharp_IPage_WaitForExpressionAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForExpressionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_"></a>

### WaitForExpressionAsync(string, WaitForFunctionOptions)


```csharp
Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options = null)
```


Waits for an expression to be evaluated to a truthy value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Expression to be evaluated in browser context. |
| `options` | [WaitForFunctionOptions](/api/PuppeteerSharp/WaitForFunctionOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when the `script` returns a truthy value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1370)

<a id="PuppeteerSharp_IPage_WaitForFileChooserAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForFileChooserAsync_PuppeteerSharp_WaitForOptions_"></a>

### WaitForFileChooserAsync(WaitForOptions)


```csharp
Task<FileChooser> WaitForFileChooserAsync(WaitForOptions options = null)
```


Waits for a page to open a file picker.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;FileChooser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves after a page requests a file picker.

In non-headless Chromium, this method results in the native file picker dialog **not showing up** for the user.

**Example**

This method is typically coupled with an action that triggers file choosing.
The following example clicks a button that issues a file chooser, and then
responds with `/tmp/myfile.pdf` as if a user has selected this file.


```csharp
var waitTask = page.WaitForFileChooserAsync();
await Task.WhenAll(
    waitTask,
    page.ClickAsync("#upload-file-button")); // some button that triggers file selection

await waitTask.Result.AcceptAsync('/tmp/myfile.pdf');
```


This must be called *before* the file chooser is launched. It will not return a currently active file chooser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1433)

<a id="PuppeteerSharp_IPage_WaitForFrameAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForFrameAsync_System_Func_PuppeteerSharp_IFrame_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

### `WaitForFrameAsync(Func<IFrame, bool>, WaitForOptions)`


```csharp
Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions options = null)
```


Waits for a frame.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `predicate` | [Func&lt;IFrame, bool&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Function which looks for a matching frame. |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | Options. |

**Returns:** [Task&lt;IFrame&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves when a matching frame was attached to the page.

**Example**

```csharp
var frame = await page.WaitForFrameAsync(frame => frame.Name == "Test");
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1403)

<a id="PuppeteerSharp_IPage_WaitForFrameAsync_System_String_PuppeteerSharp_WaitForOptions_"></a>

### WaitForFrameAsync(string, WaitForOptions)


```csharp
Task<IFrame> WaitForFrameAsync(string url, WaitForOptions options = null)
```


Waits for a frame.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Frame url. |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | Options. |

**Returns:** [Task&lt;IFrame&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves when a matching frame was attached to the page.

**Example**

```csharp
var frame = await page.WaitForFrameAsync(frame => frame.Name == "Test");
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1388)

<a id="PuppeteerSharp_IPage_WaitForFunctionAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForFunctionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_System_Object___"></a>

### WaitForFunctionAsync(string, WaitForFunctionOptions, params object[])


```csharp
Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
```


Waits for a function to be evaluated to a truthy value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `options` | [WaitForFunctionOptions](/api/PuppeteerSharp/WaitForFunctionOptions/) | Optional waiting parameters. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `script`. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when the `script` returns a truthy value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1443)

<a id="PuppeteerSharp_IPage_WaitForFunctionAsync_System_String_System_Object___"></a>

### WaitForFunctionAsync(string, params object[])


```csharp
Task<IJSHandle> WaitForFunctionAsync(string script, params object[] args)
```


Waits for a function to be evaluated to a truthy value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `script`. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when the `script` returns a truthy value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1455)

<a id="PuppeteerSharp_IPage_WaitForNavigationAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForNavigationAsync_PuppeteerSharp_NavigationOptions_"></a>

### WaitForNavigationAsync(NavigationOptions)


```csharp
Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
```


This resolves when the page navigates to a new URL or reloads.
It is useful for when you run code which will indirectly cause the page to navigate.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | navigation options. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response.
    In case of multiple redirects, the navigation will resolve with the response of the last redirect.
    In case of navigation to a different anchor or navigation due to History API usage, the navigation will resolve with `null`.

Usage of the `History API` [https://developer.mozilla.org/en-US/docs/Web/API/History_API](https://developer.mozilla.org/en-US/docs/Web/API/History_API) to change the URL is considered a navigation.

**Example**

```csharp
var navigationTask = page.WaitForNavigationAsync();
await page.ClickAsync("a.my-link");
await navigationTask;
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1478)

<a id="PuppeteerSharp_IPage_WaitForNetworkIdleAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForNetworkIdleAsync_PuppeteerSharp_WaitForNetworkIdleOptions_"></a>

### WaitForNetworkIdleAsync(WaitForNetworkIdleOptions)


```csharp
Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null)
```


Waits for Network Idle.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [WaitForNetworkIdleOptions](/api/PuppeteerSharp/WaitForNetworkIdleOptions/) | Optional waiting parameters. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — returns Task which resolves when network is idle.

**Example**

```csharp
page.EvaluateFunctionAsync("() => fetch('some-url')");
await page.WaitForNetworkIdle(); // The Task resolves after fetch above finishes
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1493)

<a id="PuppeteerSharp_IPage_WaitForRequestAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForRequestAsync_System_Func_PuppeteerSharp_IRequest_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

### `WaitForRequestAsync(Func<IRequest, bool>, WaitForOptions)`


```csharp
Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null)
```


Waits for a request.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `predicate` | [Func&lt;IRequest, bool&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Function which looks for a matching request. |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | Options. |

**Returns:** [Task&lt;IRequest&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves when a matching request was made.

**Example**

```csharp
var request = await page.WaitForRequestAsync(request => request.Url === "http://example.com" && request.Method === HttpMethod.Get;
return request.Url;
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1509)

<a id="PuppeteerSharp_IPage_WaitForRequestAsync_System_String_PuppeteerSharp_WaitForOptions_"></a>

### WaitForRequestAsync(string, WaitForOptions)


```csharp
Task<IRequest> WaitForRequestAsync(string url, WaitForOptions options = null)
```


Waits for a request.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | URL to wait for. |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | Options. |

**Returns:** [Task&lt;IRequest&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves when a matching request was made.

**Example**

```csharp
var firstRequest = await page.WaitForRequestAsync("http://example.com/resource");
return firstRequest.Url;
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1528)

<a id="PuppeteerSharp_IPage_WaitForResponseAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForResponseAsync_System_Func_PuppeteerSharp_IResponse_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

### `WaitForResponseAsync(Func<IResponse, bool>, WaitForOptions)`


```csharp
Task<IResponse> WaitForResponseAsync(Func<IResponse, bool> predicate, WaitForOptions options = null)
```


Waits for a response.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `predicate` | [Func&lt;IResponse, bool&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Function which looks for a matching response. |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | Options. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves when a matching response is received.

**Example**

```csharp
var response = await page.WaitForResponseAsync(response => response.Url === "http://example.com" && response.Status === HttpStatus.Ok;
return response.Url;
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1544)

<a id="PuppeteerSharp_IPage_WaitForResponseAsync_System_Func_PuppeteerSharp_IResponse_System_Threading_Tasks_Task_System_Boolean___PuppeteerSharp_WaitForOptions_"></a>

### `WaitForResponseAsync(Func<IResponse, Task<bool>>, WaitForOptions)`


```csharp
Task<IResponse> WaitForResponseAsync(Func<IResponse, Task<bool>> predicate, WaitForOptions options = null)
```


Waits for a response.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `predicate` | [Func&lt;IResponse, Task&lt;bool&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Function which looks for a matching response. |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | Options. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves when a matching response is received.

**Example**

```csharp
var response = await page.WaitForResponseAsync(response => response.Url === "http://example.com" && response.Status === HttpStatus.Ok;
return response.Url;
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1563)

<a id="PuppeteerSharp_IPage_WaitForResponseAsync_System_String_PuppeteerSharp_WaitForOptions_"></a>

### WaitForResponseAsync(string, WaitForOptions)


```csharp
Task<IResponse> WaitForResponseAsync(string url, WaitForOptions options = null)
```


Waits for a response.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | URL to wait for. |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | Options. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves when a matching response is received.

**Example**

```csharp
var firstResponse = await page.WaitForResponseAsync("http://example.com/resource");
return firstResponse.Url;
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1582)

<a id="PuppeteerSharp_IPage_WaitForSelectorAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForSelectorAsync_System_String_PuppeteerSharp_WaitForSelectorOptions_"></a>

### WaitForSelectorAsync(string, WaitForSelectorOptions)


```csharp
Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
```


Waits for a selector to be added to the DOM.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector of an element to wait for. |
| `options` | [WaitForSelectorOptions](/api/PuppeteerSharp/WaitForSelectorOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when element specified by selector string is added to DOM.
    Resolves to `null` if waiting for `hidden: true` and selector is not found in DOM.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1592)

<a id="PuppeteerSharp_IPage_WaitForXPathAsync_"></a>
<a id="PuppeteerSharp_IPage_WaitForXPathAsync_System_String_PuppeteerSharp_WaitForSelectorOptions_"></a>

### WaitForXPathAsync(string, WaitForSelectorOptions)


```csharp
[Obsolete("Use WaitForSelectorAsync instead")]
Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
```


Waits for a xpath selector to be added to the DOM.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `xpath` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A xpath selector of an element to wait for. |
| `options` | [WaitForSelectorOptions](/api/PuppeteerSharp/WaitForSelectorOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves when element specified by xpath string is added to DOM.
    Resolves to `null` if waiting for `hidden: true` and xpath is not found in DOM.

:::caution[Obsolete]
This API is obsolete.
:::

**Example**

```csharp
var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();
string currentURL = null;
page
    .WaitForXPathAsync("//img")
    .ContinueWith(_ => Console.WriteLine("First URL with image: " + currentURL));
foreach (var current in new[] { "https://example.com", "https://google.com", "https://bbc.com" })
{
    currentURL = current;
    await page.GoToAsync(currentURL);
}
await browser.CloseAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1639)

<a id="PuppeteerSharp_IPage_WindowIdAsync_"></a>
<a id="PuppeteerSharp_IPage_WindowIdAsync"></a>

### WindowIdAsync()


```csharp
Task<string> WindowIdAsync()
```


Returns the page's window ID.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the window ID string.

This is an experimental API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1687)

<a id="PuppeteerSharp_IPage_XPathAsync_"></a>
<a id="PuppeteerSharp_IPage_XPathAsync_System_String_"></a>

### XPathAsync(string)


```csharp
[Obsolete("Use QuerySelectorAsync instead")]
Task<IElementHandle[]> XPathAsync(string expression)
```


Evaluates the XPath expression.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `expression` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Expression to evaluate [https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate](https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate). |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of [IElementHandle](/api/PuppeteerSharp/IElementHandle/).

:::caution[Obsolete]
This API is obsolete.
:::

Shortcut for `page.MainFrame.XPathAsync(expression)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L1650)
