---
title: "Page"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Page"
  hidden: true
---

<a id="PuppeteerSharp_Page"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class Page : IPage, IDisposable, IAsyncDisposable
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IPage](/api/PuppeteerSharp/IPage/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

**Derived:** [BidiPage](/api/PuppeteerSharp.Bidi/BidiPage/), [CdpPage](/api/PuppeteerSharp.Cdp/CdpPage/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L25)

## Fields

<a id="PuppeteerSharp_Page_SupportedMetrics"></a>

### SupportedMetrics


```csharp
public static readonly IEnumerable<string> SupportedMetrics
```


List of supported metrics.

**Value:** [IEnumerable&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L31)

## Properties

<a id="PuppeteerSharp_Page_Browser_"></a>
<a id="PuppeteerSharp_Page_Browser"></a>

### Browser


```csharp
protected abstract Browser Browser { get; }
```


Browser.

**Value:** [Browser](/api/PuppeteerSharp/Browser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L255)

<a id="PuppeteerSharp_Page_BrowserContext_"></a>
<a id="PuppeteerSharp_Page_BrowserContext"></a>

### BrowserContext


```csharp
public abstract IBrowserContext BrowserContext { get; }
```


Get the browser context that the page belongs to.

**Value:** [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L202)

<a id="PuppeteerSharp_Page_Client_"></a>
<a id="PuppeteerSharp_Page_Client"></a>

### Client


```csharp
public abstract CDPSession Client { get; }
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Value:** [CDPSession](/api/PuppeteerSharp/CDPSession/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L132)

<a id="PuppeteerSharp_Page_DefaultNavigationTimeout_"></a>
<a id="PuppeteerSharp_Page_DefaultNavigationTimeout"></a>

### DefaultNavigationTimeout


```csharp
public int DefaultNavigationTimeout { get; set; }
```


This setting will change the default maximum time for the following methods:
- `NavigationOptions)`
- [GoBackAsync(NavigationOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_GoBackAsync_PuppeteerSharp_NavigationOptions_)
- [GoForwardAsync(NavigationOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_GoForwardAsync_PuppeteerSharp_NavigationOptions_)
- [ReloadAsync(ReloadOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ReloadAsync_PuppeteerSharp_ReloadOptions_)
- `NavigationOptions)`
- [WaitForNavigationAsync(NavigationOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_WaitForNavigationAsync_PuppeteerSharp_NavigationOptions_)
**NOTE** [DefaultNavigationTimeout](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_DefaultNavigationTimeout) takes priority over. [DefaultTimeout](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_DefaultTimeout)

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L135)

<a id="PuppeteerSharp_Page_DefaultTimeout_"></a>
<a id="PuppeteerSharp_Page_DefaultTimeout"></a>

### DefaultTimeout


```csharp
public int DefaultTimeout { get; set; }
```


This setting will change the default maximum times for the following methods:
- [GoBackAsync(NavigationOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_GoBackAsync_PuppeteerSharp_NavigationOptions_)
- [GoForwardAsync(NavigationOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_GoForwardAsync_PuppeteerSharp_NavigationOptions_)
- `NavigationOptions)`
- [ReloadAsync(ReloadOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ReloadAsync_PuppeteerSharp_ReloadOptions_)
- `NavigationOptions)`
- `Object%5b%5d)`
- [WaitForNavigationAsync(NavigationOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_WaitForNavigationAsync_PuppeteerSharp_NavigationOptions_)
- `WaitForOptions)`
- `WaitForOptions)`
- `WaitForSelectorOptions)`
- `WaitForFunctionOptions)`.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L142)

<a id="PuppeteerSharp_Page_Frames_"></a>
<a id="PuppeteerSharp_Page_Frames"></a>

### Frames


```csharp
public abstract IFrame[] Frames { get; }
```


Gets all frames attached to the page.

**Value:** `IFrame\[\]` — An array of all frames attached to the page.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L152)

<a id="PuppeteerSharp_Page_HasConsoleEventListeners_"></a>
<a id="PuppeteerSharp_Page_HasConsoleEventListeners"></a>

### HasConsoleEventListeners


```csharp
protected bool HasConsoleEventListeners { get; }
```


Whether the [Console](#PuppeteerSharp_Page_Console) event has listeners.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L250)

<a id="PuppeteerSharp_Page_IsClosed_"></a>
<a id="PuppeteerSharp_Page_IsClosed"></a>

### IsClosed


```csharp
public bool IsClosed { get; protected set; }
```


Get an indication that the page has been closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L207)

<a id="PuppeteerSharp_Page_IsDragInterceptionEnabled_"></a>
<a id="PuppeteerSharp_Page_IsDragInterceptionEnabled"></a>

### IsDragInterceptionEnabled


```csharp
public bool IsDragInterceptionEnabled { get; protected set; }
```


`true` if drag events are being intercepted, `false` otherwise.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L223)

<a id="PuppeteerSharp_Page_IsJavaScriptEnabled_"></a>
<a id="PuppeteerSharp_Page_IsJavaScriptEnabled"></a>

### IsJavaScriptEnabled


```csharp
public abstract bool IsJavaScriptEnabled { get; }
```


If the page has JavaScript enabled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L215)

<a id="PuppeteerSharp_Page_IsServiceWorkerBypassed_"></a>
<a id="PuppeteerSharp_Page_IsServiceWorkerBypassed"></a>

### IsServiceWorkerBypassed


```csharp
public bool IsServiceWorkerBypassed { get; protected set; }
```


`true` if the service worker are being bypassed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L166)

<a id="PuppeteerSharp_Page_MainFrame_"></a>
<a id="PuppeteerSharp_Page_MainFrame"></a>

### MainFrame


```csharp
public abstract IFrame MainFrame { get; }
```


Gets page's main frame.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/)

Page is guaranteed to have a main frame which persists during navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L149)

<a id="PuppeteerSharp_Page_ScreenshotBurstModeOn_"></a>
<a id="PuppeteerSharp_Page_ScreenshotBurstModeOn"></a>

### ScreenshotBurstModeOn


```csharp
protected bool ScreenshotBurstModeOn { get; set; }
```


Whether burst mode is on.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L260)

<a id="PuppeteerSharp_Page_ScreenshotBurstModeOptions_"></a>
<a id="PuppeteerSharp_Page_ScreenshotBurstModeOptions"></a>

### ScreenshotBurstModeOptions


```csharp
protected ScreenshotOptions ScreenshotBurstModeOptions { get; set; }
```


Screenshot burst mode options.

**Value:** [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L265)

<a id="PuppeteerSharp_Page_Target_"></a>
<a id="PuppeteerSharp_Page_Target"></a>

### Target


```csharp
public abstract Target Target { get; }
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Value:** [Target](/api/PuppeteerSharp/Target/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L175)

<a id="PuppeteerSharp_Page_TimeoutSettings_"></a>
<a id="PuppeteerSharp_Page_TimeoutSettings"></a>

### TimeoutSettings


```csharp
public TimeoutSettings TimeoutSettings { get; set; }
```


Timeout settings.

**Value:** [TimeoutSettings](/api/PuppeteerSharp/TimeoutSettings/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L220)

<a id="PuppeteerSharp_Page_Url_"></a>
<a id="PuppeteerSharp_Page_Url"></a>

### Url


```csharp
public string Url { get; }
```


Shortcut for `page.MainFrame.Url`.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L169)

<a id="PuppeteerSharp_Page_Viewport_"></a>
<a id="PuppeteerSharp_Page_Viewport"></a>

### Viewport


```csharp
public ViewPortOptions Viewport { get; protected set; }
```


Gets this page's viewport.

**Value:** [ViewPortOptions](/api/PuppeteerSharp/ViewPortOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L196)

<a id="PuppeteerSharp_Page_WebMcp_"></a>
<a id="PuppeteerSharp_Page_WebMcp"></a>

### WebMcp


```csharp
public virtual CdpWebMcp WebMcp { get; }
```


Experimental API for WebMCP. Requires Chrome 149+ with
--enable-features=WebMCPTesting,DevToolsWebMCPSupport flags enabled.
Only supported with CDP protocol.

**Value:** [CdpWebMcp](/api/PuppeteerSharp.Cdp/CdpWebMcp/)

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | Thrown when not supported by the current browser/protocol. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L163)

<a id="PuppeteerSharp_Page_Workers_"></a>
<a id="PuppeteerSharp_Page_Workers"></a>

### Workers


```csharp
public abstract WebWorker[] Workers { get; }
```


Gets all workers in the page.

**Value:** `WebWorker\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L155)

## Events

<a id="PuppeteerSharp_Page_Close"></a>

### Close


```csharp
public event EventHandler Close
```


Raised when the page closes.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L123)

<a id="PuppeteerSharp_Page_Console"></a>

### Console


```csharp
public event EventHandler<ConsoleEventArgs> Console
```


Raised when JavaScript within the page calls one of console API methods, e.g. `console.log` or `console.dir`. Also emitted if the page throws an error or a warning.
The arguments passed into `console.log` appear as arguments on the event handler.

**Value:** [EventHandler&lt;ConsoleEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

**Example**

An example of handling [Console](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Console) event:


```csharp
page.Console += (sender, e) =>
{
    for (var i = 0; i < e.Message.Args.Count; ++i)
    {
        System.Console.WriteLine($"{i}: {e.Message.Args[i]}");
    }
}
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L84)

<a id="PuppeteerSharp_Page_DOMContentLoaded"></a>

### DOMContentLoaded


```csharp
public event EventHandler DOMContentLoaded
```


Raised when the JavaScript `DOMContentLoaded` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/Events/DOMContentLoaded"&gt;https://developer.mozilla.org/en-US/docs/Web/Events/DOMContentLoaded&lt;/a&gt; event is dispatched.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L81)

<a id="PuppeteerSharp_Page_Dialog"></a>

### Dialog


```csharp
public event EventHandler<DialogEventArgs> Dialog
```


Raised when a JavaScript dialog appears, such as `alert`, `prompt`, `confirm` or `beforeunload`. Puppeteer can respond to the dialog via [Dialog](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Dialog)'s [Accept(string)](/api/PuppeteerSharp/Dialog/#PuppeteerSharp_Dialog_Accept_System_String_) or [Dismiss()](/api/PuppeteerSharp/Dialog/#PuppeteerSharp_Dialog_Dismiss) methods.

**Value:** [EventHandler&lt;DialogEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L78)

<a id="PuppeteerSharp_Page_Error"></a>

### Error


```csharp
public event EventHandler<ErrorEventArgs> Error
```


Raised when the page crashes

**Value:** [EventHandler&lt;ErrorEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L72)

<a id="PuppeteerSharp_Page_FrameAttached"></a>

### FrameAttached


```csharp
public event EventHandler<FrameEventArgs> FrameAttached
```


Raised when a frame is attached.

**Value:** [EventHandler&lt;FrameEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L87)

<a id="PuppeteerSharp_Page_FrameDetached"></a>

### FrameDetached


```csharp
public event EventHandler<FrameEventArgs> FrameDetached
```


Raised when a frame is detached.

**Value:** [EventHandler&lt;FrameEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L90)

<a id="PuppeteerSharp_Page_FrameNavigated"></a>

### FrameNavigated


```csharp
public event EventHandler<FrameNavigatedEventArgs> FrameNavigated
```


Raised when a frame is navigated to a new url.

**Value:** [EventHandler&lt;FrameNavigatedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L93)

<a id="PuppeteerSharp_Page_Issue"></a>

### Issue


```csharp
public event EventHandler<IssueEventArgs> Issue
```


Raised when a DevTools issue is reported.

**Value:** [EventHandler&lt;IssueEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L111)

<a id="PuppeteerSharp_Page_Load"></a>

### Load


```csharp
public event EventHandler Load
```


Raised when the JavaScript `load` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/Events/load"&gt;https://developer.mozilla.org/en-US/docs/Web/Events/load&lt;/a&gt; event is dispatched.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L69)

<a id="PuppeteerSharp_Page_Metrics"></a>

### Metrics


```csharp
public event EventHandler<MetricEventArgs> Metrics
```


Raised when the JavaScript code makes a call to `console.timeStamp`. For the list of metrics see [MetricsAsync()](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_MetricsAsync).

**Value:** [EventHandler&lt;MetricEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L75)

<a id="PuppeteerSharp_Page_PageError"></a>

### PageError


```csharp
public event EventHandler<PageErrorEventArgs> PageError
```


Raised when an uncaught exception happens within the page.

**Value:** [EventHandler&lt;PageErrorEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L114)

<a id="PuppeteerSharp_Page_Popup"></a>

### Popup


```csharp
public event EventHandler<PopupEventArgs> Popup
```


Raised when the page opens a new tab or window.

**Value:** [EventHandler&lt;PopupEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L126)

<a id="PuppeteerSharp_Page_Request"></a>

### Request


```csharp
public event EventHandler<RequestEventArgs> Request
```


Raised when a page issues a request. The [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request) object is read-only.
In order to intercept and mutate requests, see [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_)

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L99)

<a id="PuppeteerSharp_Page_RequestFailed"></a>

### RequestFailed


```csharp
public event EventHandler<RequestEventArgs> RequestFailed
```


Raised when a request fails, for example by timing out.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L105)

<a id="PuppeteerSharp_Page_RequestFinished"></a>

### RequestFinished


```csharp
public event EventHandler<RequestEventArgs> RequestFinished
```


Raised when a request finishes successfully.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L102)

<a id="PuppeteerSharp_Page_RequestServedFromCache"></a>

### RequestServedFromCache


```csharp
public event EventHandler<RequestEventArgs> RequestServedFromCache
```


Raised when a request ended up loading from cache.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L108)

<a id="PuppeteerSharp_Page_Response"></a>

### Response


```csharp
public event EventHandler<ResponseCreatedEventArgs> Response
```


Raised when a [Response](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Response) is received.

**Value:** [EventHandler&lt;ResponseCreatedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

**Example**

An example of handling [Response](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Response) event:


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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L96)

<a id="PuppeteerSharp_Page_WorkerCreated"></a>

### WorkerCreated


```csharp
public event EventHandler<WorkerEventArgs> WorkerCreated
```


Emitted when a dedicated WebWorker (&lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API"&gt;https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API&lt;/a&gt;) is spawned by the page.

**Value:** [EventHandler&lt;WorkerEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L117)

<a id="PuppeteerSharp_Page_WorkerDestroyed"></a>

### WorkerDestroyed


```csharp
public event EventHandler<WorkerEventArgs> WorkerDestroyed
```


Emitted when a dedicated WebWorker (&lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API"&gt;https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API&lt;/a&gt;) is terminated.

**Value:** [EventHandler&lt;WorkerEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L120)

## Methods

<a id="PuppeteerSharp_Page_AddRequestInterceptor_"></a>
<a id="PuppeteerSharp_Page_AddRequestInterceptor_System_Func_PuppeteerSharp_IRequest_System_Threading_Tasks_Task__"></a>

### `AddRequestInterceptor(Func<IRequest, Task>)`


```csharp
public void AddRequestInterceptor(Func<IRequest, Task> interceptionTask)
```


`Int32%7d)`, `Int32%7d)`, and `Int32%7d)` can accept an optional `priority` to activate Cooperative Intercept Mode.
In Cooperative Mode, all interception tasks are guaranteed to run and all async handlers are awaited.
The interception is resolved to the highest-priority resolution.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `interceptionTask` | [Func&lt;IRequest, Task&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Interception task. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1126)

<a id="PuppeteerSharp_Page_AddScriptTagAsync_"></a>
<a id="PuppeteerSharp_Page_AddScriptTagAsync_PuppeteerSharp_AddTagOptions_"></a>

### AddScriptTagAsync(AddTagOptions)


```csharp
public Task<IElementHandle> AddScriptTagAsync(AddTagOptions options)
```


Adds a `<script>` tag into the page with the desired url or content.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [AddTagOptions](/api/PuppeteerSharp/AddTagOptions/) | add script tag options. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame.

Shortcut for `page.MainFrame.AddScriptTagAsync(options)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L359)

<a id="PuppeteerSharp_Page_AddScriptTagAsync_System_String_"></a>

### AddScriptTagAsync(string)


```csharp
public Task<IElementHandle> AddScriptTagAsync(string url)
```


Adds a `<script>` tag into the page with the desired url or content.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | script url. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the added tag when the script's onload fires or when the script content was injected into frame.

Shortcut for `page.MainFrame.AddScriptTagAsync(new AddTagOptions { Url = url })`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L363)

<a id="PuppeteerSharp_Page_AddStyleTagAsync_"></a>
<a id="PuppeteerSharp_Page_AddStyleTagAsync_PuppeteerSharp_AddTagOptions_"></a>

### AddStyleTagAsync(AddTagOptions)


```csharp
public Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
```


Adds a `<link rel="stylesheet">` tag into the page with the desired url or a `<link rel="stylesheet">` tag with the content.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [AddTagOptions](/api/PuppeteerSharp/AddTagOptions/) | add style tag options. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame.

Shortcut for `page.MainFrame.AddStyleTagAsync(options)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L367)

<a id="PuppeteerSharp_Page_AddStyleTagAsync_System_String_"></a>

### AddStyleTagAsync(string)


```csharp
public Task<IElementHandle> AddStyleTagAsync(string url)
```


Adds a `<link rel="stylesheet">` tag into the page with the desired url or a `<link rel="stylesheet">` tag with the content.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | stylesheet url. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame.

Shortcut for `page.MainFrame.AddStyleTagAsync(new AddTagOptions { Url = url })`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L371)

<a id="PuppeteerSharp_Page_AuthenticateAsync_"></a>
<a id="PuppeteerSharp_Page_AuthenticateAsync_PuppeteerSharp_Credentials_"></a>

### AuthenticateAsync(Credentials)


```csharp
public abstract Task AuthenticateAsync(Credentials credentials)
```


Provide credentials for http authentication &lt;a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication"&gt;https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication&lt;/a&gt;.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `credentials` | [Credentials](/api/PuppeteerSharp/Credentials/) | The credentials. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the message is sent to the browser.

To disable authentication, pass `null`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L882)

<a id="PuppeteerSharp_Page_BringToFrontAsync_"></a>
<a id="PuppeteerSharp_Page_BringToFrontAsync"></a>

### BringToFrontAsync()


```csharp
public abstract Task BringToFrontAsync()
```


Brings page to front (activates tab).

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to Chromium.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1082)

<a id="PuppeteerSharp_Page_CaptureHeapSnapshotAsync_"></a>
<a id="PuppeteerSharp_Page_CaptureHeapSnapshotAsync_PuppeteerSharp_HeapSnapshotOptions_"></a>

### CaptureHeapSnapshotAsync(HeapSnapshotOptions)


```csharp
public abstract Task CaptureHeapSnapshotAsync(HeapSnapshotOptions options)
```


Captures a snapshot of the JavaScript heap and writes it to a file.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [HeapSnapshotOptions](/api/PuppeteerSharp/HeapSnapshotOptions/) | Heap snapshot options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the heap snapshot is captured and written to disk.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L277)

<a id="PuppeteerSharp_Page_ClickAsync_"></a>
<a id="PuppeteerSharp_Page_ClickAsync_System_String_PuppeteerSharp_Input_ClickOptions_"></a>

### ClickAsync(string, ClickOptions)


```csharp
public Task ClickAsync(string selector, ClickOptions options = null)
```


Fetches an element with &lt;code class="paramref"&gt;selector&lt;/code&gt;, scrolls it into view if needed, and then uses [Mouse](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Mouse) to click in the center of the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to click. If there are multiple elements satisfying the selector, the first will be clicked. |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | click options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching &lt;code class="paramref"&gt;selector&lt;/code&gt; is successfully clicked.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching &lt;code class="paramref"&gt;selector&lt;/code&gt;. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L843)

<a id="PuppeteerSharp_Page_CloseAsync_"></a>
<a id="PuppeteerSharp_Page_CloseAsync_PuppeteerSharp_PageCloseOptions_"></a>

### CloseAsync(PageCloseOptions)


```csharp
public abstract Task CloseAsync(PageCloseOptions options = null)
```


Closes the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [PageCloseOptions](/api/PuppeteerSharp/PageCloseOptions/) | Close options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L831)

<a id="PuppeteerSharp_Page_CreateCDPSessionAsync_"></a>
<a id="PuppeteerSharp_Page_CreateCDPSessionAsync"></a>

### CreateCDPSessionAsync()


```csharp
public Task<ICDPSession> CreateCDPSessionAsync()
```


Creates a Chrome Devtools Protocol session attached to the page.

**Returns:** [Task&lt;ICDPSession&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [ICDPSession](/api/PuppeteerSharp/ICDPSession/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1134)

<a id="PuppeteerSharp_Page_CreateScreenRecording_"></a>
<a id="PuppeteerSharp_Page_CreateScreenRecording_PuppeteerSharp_RecordOptions_"></a>

### CreateScreenRecording(RecordOptions)


```csharp
protected abstract ScreenRecording CreateScreenRecording(RecordOptions options)
```


Creates a protocol-specific screen recording instance.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [RecordOptions](/api/PuppeteerSharp/RecordOptions/) | Recording options. |

**Returns:** [ScreenRecording](/api/PuppeteerSharp/ScreenRecording/) — A screen recording instance.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1400)

<a id="PuppeteerSharp_Page_DeleteCookieAsync_"></a>
<a id="PuppeteerSharp_Page_DeleteCookieAsync_PuppeteerSharp_CookieParam___"></a>

### DeleteCookieAsync(params CookieParam[])


```csharp
public abstract Task DeleteCookieAsync(params CookieParam[] cookies)
```


Deletes cookies from the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieParam\[\]` | Cookies to delete. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L356)

<a id="PuppeteerSharp_Page_Dispose_"></a>
<a id="PuppeteerSharp_Page_Dispose"></a>

### Dispose()


```csharp
public void Dispose()
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1103)

<a id="PuppeteerSharp_Page_Dispose_System_Boolean_"></a>

### Dispose(bool)


```csharp
protected virtual void Dispose(bool disposing)
```


Dispose resources.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `disposing` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Indicates whether disposal was initiated by [Dispose()](#PuppeteerSharp_Page_Dispose) operation. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1323)

<a id="PuppeteerSharp_Page_DisposeAsync_"></a>
<a id="PuppeteerSharp_Page_DisposeAsync"></a>

### DisposeAsync()


```csharp
public ValueTask DisposeAsync()
```


Performs application-defined tasks associated with freeing, releasing, or
resetting unmanaged resources asynchronously.

**Returns:** [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask) — A task that represents the asynchronous dispose operation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1110)

<a id="PuppeteerSharp_Page_EmulateAsync_"></a>
<a id="PuppeteerSharp_Page_EmulateAsync_PuppeteerSharp_Mobile_DeviceDescriptor_"></a>

### EmulateAsync(DeviceDescriptor)


```csharp
public Task EmulateAsync(DeviceDescriptor options)
```


Emulates given device metrics and user agent.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [DeviceDescriptor](/api/PuppeteerSharp.Mobile/DeviceDescriptor/) | Emulation options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

This method is a shortcut for calling two methods:
[SetViewportAsync(ViewPortOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetViewportAsync_PuppeteerSharp_ViewPortOptions_)
`UserAgentMetadata)`
To aid emulation, puppeteer provides a list of device descriptors which can be obtained via the [Devices](/api/PuppeteerSharp/Puppeteer/#PuppeteerSharp_Puppeteer_Devices).
[EmulateAsync(DeviceDescriptor)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EmulateAsync_PuppeteerSharp_Mobile_DeviceDescriptor_) will resize the page. A lot of websites don't expect phones to change size, so you should emulate before navigating to the page.

**Example**

var iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];
using(var page = await browser.NewPageAsync())
{
    await page.EmulateAsync(iPhone);
    await page.goto('https://www.google.com');
}

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L502)

<a id="PuppeteerSharp_Page_EmulateCPUThrottlingAsync_"></a>
<a id="PuppeteerSharp_Page_EmulateCPUThrottlingAsync_System_Nullable_System_Decimal__"></a>

### EmulateCPUThrottlingAsync(decimal?)


```csharp
public abstract Task EmulateCPUThrottlingAsync(decimal? factor = null)
```


Enables CPU throttling to emulate slow CPUs.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `factor` | [decimal?](https://learn.microsoft.com/dotnet/api/system.decimal) | Throttling rate as a slowdown factor (1 is no throttle, 2 is 2x slowdown, etc). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1097)

<a id="PuppeteerSharp_Page_EmulateFocusedPageAsync_"></a>
<a id="PuppeteerSharp_Page_EmulateFocusedPageAsync_System_Boolean_"></a>

### EmulateFocusedPageAsync(bool)


```csharp
public abstract Task EmulateFocusedPageAsync(bool enabled)
```


Emulates focus state of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether to emulate focus. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1100)

<a id="PuppeteerSharp_Page_EmulateIdleStateAsync_"></a>
<a id="PuppeteerSharp_Page_EmulateIdleStateAsync_PuppeteerSharp_EmulateIdleOverrides_"></a>

### EmulateIdleStateAsync(EmulateIdleOverrides)


```csharp
public abstract Task EmulateIdleStateAsync(EmulateIdleOverrides idleOverrides = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1094)

<a id="PuppeteerSharp_Page_EmulateLocaleAsync_"></a>
<a id="PuppeteerSharp_Page_EmulateLocaleAsync_System_String_"></a>

### EmulateLocaleAsync(string)


```csharp
public abstract Task EmulateLocaleAsync(string locale = null)
```


Emulates the locale of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `locale` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Locale to emulate on the page (e.g., "en-US", "de-DE"). Passing `null` disables locale emulation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the locale has been set.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1091)

<a id="PuppeteerSharp_Page_EmulateMediaFeaturesAsync_"></a>
<a id="PuppeteerSharp_Page_EmulateMediaFeaturesAsync_System_Collections_Generic_IEnumerable_PuppeteerSharp_MediaFeatureValue__"></a>

### `EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue>)`


```csharp
public abstract Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L496)

<a id="PuppeteerSharp_Page_EmulateMediaTypeAsync_"></a>
<a id="PuppeteerSharp_Page_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_"></a>

### EmulateMediaTypeAsync(MediaType)


```csharp
public abstract Task EmulateMediaTypeAsync(MediaType type)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L493)

<a id="PuppeteerSharp_Page_EmulateNetworkConditionsAsync_"></a>
<a id="PuppeteerSharp_Page_EmulateNetworkConditionsAsync_PuppeteerSharp_NetworkConditions_"></a>

### EmulateNetworkConditionsAsync(NetworkConditions)


```csharp
public abstract Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions)
```


Emulates network conditions.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `networkConditions` | [NetworkConditions](/api/PuppeteerSharp/NetworkConditions/) | Passing `null` disables network condition emulation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Result task.

**NOTE** This does not affect WebSockets and WebRTC PeerConnections (see https://crbug.com/563644).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L347)

<a id="PuppeteerSharp_Page_EmulateTimezoneAsync_"></a>
<a id="PuppeteerSharp_Page_EmulateTimezoneAsync_System_String_"></a>

### EmulateTimezoneAsync(string)


```csharp
public abstract Task EmulateTimezoneAsync(string timezoneId)
```


Changes the timezone of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timezoneId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Timezone to set. See &lt;a href="https://cs.chromium.org/chromium/src/third_party/icu/source/data/misc/metaZones.txt?rcl=faee8bc70570192d82d2978a71e2a615788597d1"&gt;ICU’s `metaZones.txt`&lt;/a&gt; for a list of supported timezone IDs. Passing `null` disables timezone emulation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The viewport task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1088)

<a id="PuppeteerSharp_Page_EmulateVisionDeficiencyAsync_"></a>
<a id="PuppeteerSharp_Page_EmulateVisionDeficiencyAsync_PuppeteerSharp_VisionDeficiency_"></a>

### EmulateVisionDeficiencyAsync(VisionDeficiency)


```csharp
public abstract Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1085)

<a id="PuppeteerSharp_Page_EvaluateExpressionAsync_"></a>
<a id="PuppeteerSharp_Page_EvaluateExpressionAsync_System_String_"></a>

### EvaluateExpressionAsync(string)


```csharp
public Task EvaluateExpressionAsync(string script)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L857)

<a id="PuppeteerSharp_Page_EvaluateExpressionAsync__1_System_String_"></a>

### `EvaluateExpressionAsync<T>(string)`


```csharp
public Task<T> EvaluateExpressionAsync<T>(string script)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L861)

<a id="PuppeteerSharp_Page_EvaluateExpressionHandleAsync_"></a>
<a id="PuppeteerSharp_Page_EvaluateExpressionHandleAsync_System_String_"></a>

### EvaluateExpressionHandleAsync(string)


```csharp
public Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L319)

<a id="PuppeteerSharp_Page_EvaluateExpressionOnNewDocumentAsync_"></a>
<a id="PuppeteerSharp_Page_EvaluateExpressionOnNewDocumentAsync_System_String_"></a>

### EvaluateExpressionOnNewDocumentAsync(string)


```csharp
public abstract Task<NewDocumentScriptEvaluation> EvaluateExpressionOnNewDocumentAsync(string expression)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L335)

<a id="PuppeteerSharp_Page_EvaluateFunctionAsync_"></a>
<a id="PuppeteerSharp_Page_EvaluateFunctionAsync_System_String_System_Object___"></a>

### EvaluateFunctionAsync(string, params object[])


```csharp
public Task EvaluateFunctionAsync(string script, params object[] args)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L865)

<a id="PuppeteerSharp_Page_EvaluateFunctionAsync__1_System_String_System_Object___"></a>

### `EvaluateFunctionAsync<T>(string, params object[])`


```csharp
public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L869)

<a id="PuppeteerSharp_Page_EvaluateFunctionHandleAsync_"></a>
<a id="PuppeteerSharp_Page_EvaluateFunctionHandleAsync_System_String_System_Object___"></a>

### EvaluateFunctionHandleAsync(string, params object[])


```csharp
public Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L323)

<a id="PuppeteerSharp_Page_EvaluateFunctionOnNewDocumentAsync_"></a>
<a id="PuppeteerSharp_Page_EvaluateFunctionOnNewDocumentAsync_System_String_System_Object___"></a>

### EvaluateFunctionOnNewDocumentAsync(string, params object[])


```csharp
public abstract Task<NewDocumentScriptEvaluation> EvaluateFunctionOnNewDocumentAsync(string pageFunction, params object[] args)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L327)

<a id="PuppeteerSharp_Page_ExposeFunctionAsync_"></a>
<a id="PuppeteerSharp_Page_ExposeFunctionAsync_System_String_System_Action_"></a>

### ExposeFunctionAsync(string, Action)


```csharp
public Task ExposeFunctionAsync(string name, Action puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves when &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; completes.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Action](https://learn.microsoft.com/dotnet/api/system.action) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `Action)` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L375)

<a id="PuppeteerSharp_Page_ExposeFunctionAsync_System_String_System_Delegate_"></a>

### ExposeFunctionAsync(string, Delegate)


```csharp
protected abstract Task ExposeFunctionAsync(string name, Delegate puppeteerFunction)
```


Internal add exposed functions.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function name. |
| `puppeteerFunction` | [Delegate](https://learn.microsoft.com/dotnet/api/system.delegate) | Puppeteer function. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) that completes when the function has been added.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1393)

<a id="PuppeteerSharp_Page_ExposeFunctionAsync__1_System_String_System_Func___0__"></a>

### `ExposeFunctionAsync<TResult>(string, Func<TResult>)`


```csharp
public Task ExposeFunctionAsync<TResult>(string name, Func<TResult> puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves to the return value of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;.

**Type parameters**

| Name | Description |
| --- | --- |
| `TResult` | The result of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Func&lt;TResult&gt;](https://learn.microsoft.com/dotnet/api/system.func-1) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `Func%7b%60%600%7d)` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L379)

<a id="PuppeteerSharp_Page_ExposeFunctionAsync__2_System_String_System_Func___0___1__"></a>

### `ExposeFunctionAsync<T, TResult>(string, Func<T, TResult>)`


```csharp
public Task ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves to the return value of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The parameter of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |
| `TResult` | The result of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Func&lt;T, TResult&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `Func%7b%60%600%2c%60%601%7d)` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L383)

<a id="PuppeteerSharp_Page_ExposeFunctionAsync__3_System_String_System_Func___0___1___2__"></a>

### `ExposeFunctionAsync<T1, T2, TResult>(string, Func<T1, T2, TResult>)`


```csharp
public Task ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves to the return value of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;.

**Type parameters**

| Name | Description |
| --- | --- |
| `T1` | The first parameter of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |
| `T2` | The second parameter of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |
| `TResult` | The result of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Func&lt;T1, T2, TResult&gt;](https://learn.microsoft.com/dotnet/api/system.func-3) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `Func%7b%60%600%2c%60%601%2c%60%602%7d)` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L387)

<a id="PuppeteerSharp_Page_ExposeFunctionAsync__4_System_String_System_Func___0___1___2___3__"></a>

### `ExposeFunctionAsync<T1, T2, T3, TResult>(string, Func<T1, T2, T3, TResult>)`


```csharp
public Task ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves to the return value of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;.

**Type parameters**

| Name | Description |
| --- | --- |
| `T1` | The first parameter of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |
| `T2` | The second parameter of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |
| `T3` | The third parameter of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |
| `TResult` | The result of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Func&lt;T1, T2, T3, TResult&gt;](https://learn.microsoft.com/dotnet/api/system.func-4) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `Func%7b%60%600%2c%60%601%2c%60%602%2c%60%603%7d)` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L391)

<a id="PuppeteerSharp_Page_ExposeFunctionAsync__5_System_String_System_Func___0___1___2___3___4__"></a>

### `ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string, Func<T1, T2, T3, T4, TResult>)`


```csharp
public Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> puppeteerFunction)
```


Adds a function called `name` on the page's `window` object.
When called, the function executes &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; in C# and returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) which resolves to the return value of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;.

**Type parameters**

| Name | Description |
| --- | --- |
| `T1` | The first parameter of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |
| `T2` | The second parameter of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |
| `T3` | The third parameter of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |
| `T4` | The fourth parameter of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |
| `TResult` | The result of &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt;. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function on the window object. |
| `puppeteerFunction` | [Func&lt;T1, T2, T3, T4, TResult&gt;](https://learn.microsoft.com/dotnet/api/system.func-5) | Callback function which will be called in Puppeteer's context. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If the &lt;code class="paramref"&gt;puppeteerFunction&lt;/code&gt; returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), it will be awaited.
Functions installed via `Func%7b%60%600%2c%60%601%2c%60%602%2c%60%603%2c%60%604%7d)` survive navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L395)

<a id="PuppeteerSharp_Page_ExtensionRealms_"></a>
<a id="PuppeteerSharp_Page_ExtensionRealms"></a>

### ExtensionRealms()


```csharp
public abstract IReadOnlyList<Realm> ExtensionRealms()
```


Retrieves the list of extension realms inside the main frame of this page.
Shortcut for `MainFrame.ExtensionRealms()`.

**Returns:** [IReadOnlyList&lt;Realm&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1) — A list of [Realm](/api/PuppeteerSharp/Realm/) instances representing extension execution contexts.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1147)

<a id="PuppeteerSharp_Page_FocusAsync_"></a>
<a id="PuppeteerSharp_Page_FocusAsync_System_String_"></a>

### FocusAsync(string)


```csharp
public Task FocusAsync(string selector)
```


Fetches an element with &lt;code class="paramref"&gt;selector&lt;/code&gt; and focuses it.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to focus. If there are multiple elements satisfying the selector, the first will be focused. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching &lt;code class="paramref"&gt;selector&lt;/code&gt; is successfully focused.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching &lt;code class="paramref"&gt;selector&lt;/code&gt;. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L850)

<a id="PuppeteerSharp_Page_GetContentAsync_"></a>
<a id="PuppeteerSharp_Page_GetContentAsync_PuppeteerSharp_GetContentOptions_"></a>

### GetContentAsync(GetContentOptions)


```csharp
public Task<string> GetContentAsync(GetContentOptions options = null)
```


Gets the full HTML contents of the page, including the doctype.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [GetContentOptions](/api/PuppeteerSharp/GetContentOptions/) | Options. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the HTML content.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L404)

<a id="PuppeteerSharp_Page_GetCookiesAsync_"></a>
<a id="PuppeteerSharp_Page_GetCookiesAsync_System_String___"></a>

### GetCookiesAsync(params string[])


```csharp
public abstract Task<CookieParam[]> GetCookiesAsync(params string[] urls)
```


Returns the page's cookies.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `urls` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Url's to return cookies for. |

**Returns:** [Task&lt;CookieParam\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Array of cookies.

If no URLs are specified, this method returns cookies for the current page URL.
If URLs are specified, only cookies for those URLs are returned.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L350)

<a id="PuppeteerSharp_Page_GetTitleAsync_"></a>
<a id="PuppeteerSharp_Page_GetTitleAsync"></a>

### GetTitleAsync()


```csharp
public Task<string> GetTitleAsync()
```


Returns page's title.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — page's title.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L828)

<a id="PuppeteerSharp_Page_GoBackAsync_"></a>
<a id="PuppeteerSharp_Page_GoBackAsync_PuppeteerSharp_NavigationOptions_"></a>

### GoBackAsync(NavigationOptions)


```csharp
public abstract Task<IResponse> GoBackAsync(NavigationOptions options = null)
```


Navigate to the previous page in history.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | Navigation parameters. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task that resolves to the main resource response. In case of multiple redirects,
    the navigation will resolve with the response of the last redirect. If can not go back, resolves to null.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1073)

<a id="PuppeteerSharp_Page_GoForwardAsync_"></a>
<a id="PuppeteerSharp_Page_GoForwardAsync_PuppeteerSharp_NavigationOptions_"></a>

### GoForwardAsync(NavigationOptions)


```csharp
public abstract Task<IResponse> GoForwardAsync(NavigationOptions options = null)
```


Navigate to the next page in history.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | Navigation parameters. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task that resolves to the main resource response. In case of multiple redirects,
    the navigation will resolve with the response of the last redirect. If can not go forward, resolves to null.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1076)

<a id="PuppeteerSharp_Page_GoToAsync_"></a>
<a id="PuppeteerSharp_Page_GoToAsync_System_String_PuppeteerSharp_NavigationOptions_"></a>

### GoToAsync(string, NavigationOptions)


```csharp
public Task<IResponse> GoToAsync(string url, NavigationOptions options)
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

`WaitUntilNavigation%5b%5d)` will not throw an error when any valid HTTP status code is returned by the remote server,
including 404 "Not Found" and 500 "Internal Server Error".  The status code for such responses can be retrieved by calling [Status](/api/PuppeteerSharp/IResponse/#PuppeteerSharp_IResponse_Status)

&gt; **NOTE** `WaitUntilNavigation%5b%5d)` either throws an error or returns a main resource response.
The only exceptions are navigation to `about:blank` or navigation to the same URL with a different hash, which would succeed and return `null`.

&gt; **NOTE** Headless mode doesn't support navigation to a PDF document. See the &lt;see fref="https://bugs.chromium.org/p/chromium/issues/detail?id=761295"&gt;upstream issue&lt;/see&gt;.

Shortcut for. `WaitUntilNavigation%5b%5d)`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L418)

<a id="PuppeteerSharp_Page_GoToAsync_System_String_PuppeteerSharp_WaitUntilNavigation_"></a>

### GoToAsync(string, WaitUntilNavigation)


```csharp
public Task<IResponse> GoToAsync(string url, WaitUntilNavigation waitUntil)
```


Navigates to an url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | URL to navigate page to. The url should include scheme, e.g. https://. |
| `waitUntil` | [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/) | When to consider navigation succeeded. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L426)

<a id="PuppeteerSharp_Page_GoToAsync_System_String_System_Nullable_System_Int32__PuppeteerSharp_WaitUntilNavigation___"></a>

### GoToAsync(string, int?, WaitUntilNavigation[])


```csharp
public Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null)
```


Navigates to an url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | URL to navigate page to. The url should include scheme, e.g. https://. |
| `timeout` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Maximum navigation time in milliseconds, defaults to 30 seconds, pass `0` to disable timeout. |
| `waitUntil` | `WaitUntilNavigation\[\]` | When to consider navigation succeeded, defaults to [Load](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Load). Given an array of [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/), navigation is considered to be successful after all events have been fired. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L422)

<a id="PuppeteerSharp_Page_HasDevToolsAsync_"></a>
<a id="PuppeteerSharp_Page_HasDevToolsAsync"></a>

### HasDevToolsAsync()


```csharp
public abstract Task<bool> HasDevToolsAsync()
```


Returns true if DevTools is attached to the current page.
Use [OpenDevToolsAsync()](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_OpenDevToolsAsync) to get the DevTools page.

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A `Task%601` that completes with true if DevTools is attached.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L837)

<a id="PuppeteerSharp_Page_HoverAsync_"></a>
<a id="PuppeteerSharp_Page_HoverAsync_System_String_"></a>

### HoverAsync(string)


```csharp
public Task HoverAsync(string selector)
```


Fetches an element with &lt;code class="paramref"&gt;selector&lt;/code&gt;, scrolls it into view if needed, and then uses [Mouse](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Mouse) to hover over the center of the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to hover. If there are multiple elements satisfying the selector, the first will be hovered. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching &lt;code class="paramref"&gt;selector&lt;/code&gt; is successfully hovered.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching &lt;code class="paramref"&gt;selector&lt;/code&gt;. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L847)

<a id="PuppeteerSharp_Page_Locator_"></a>
<a id="PuppeteerSharp_Page_Locator_System_String_"></a>

### Locator(string)


```csharp
public Locator Locator(string selector)
```


Creates a locator for the provided selector. See [Locator](/api/PuppeteerSharp.Locators/Locator/) for
details and supported actions.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to locate an element. |

**Returns:** [Locator](/api/PuppeteerSharp.Locators/Locator/) — A locator for the provided selector.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L912)

<a id="PuppeteerSharp_Page_LocatorFunction_"></a>
<a id="PuppeteerSharp_Page_LocatorFunction_System_String_"></a>

### LocatorFunction(string)


```csharp
public Locator LocatorFunction(string func)
```


Creates a locator for the provided function. See [Locator](/api/PuppeteerSharp.Locators/Locator/) for
details and supported actions.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `func` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A JavaScript function expression to evaluate. |

**Returns:** [Locator](/api/PuppeteerSharp.Locators/Locator/) — A locator for the provided function.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L916)

<a id="PuppeteerSharp_Page_MetricsAsync_"></a>
<a id="PuppeteerSharp_Page_MetricsAsync"></a>

### MetricsAsync()


```csharp
public abstract Task<Dictionary<string, decimal>> MetricsAsync()
```


Returns metrics.

**Returns:** [Task&lt;Dictionary&lt;string, decimal&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves into a list of metrics.

All timestamps are in monotonic time: monotonically increasing time in seconds since an arbitrary point in the past.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L274)

<a id="PuppeteerSharp_Page_OnClose_"></a>
<a id="PuppeteerSharp_Page_OnClose"></a>

### OnClose()


```csharp
protected void OnClose()
```


Raises the [Close](#PuppeteerSharp_Page_Close) event.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1345)

<a id="PuppeteerSharp_Page_OnConsole_"></a>
<a id="PuppeteerSharp_Page_OnConsole_PuppeteerSharp_ConsoleEventArgs_"></a>

### OnConsole(ConsoleEventArgs)


```csharp
protected void OnConsole(ConsoleEventArgs e)
```


Raises the [Console](#PuppeteerSharp_Page_Console) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `e` | [ConsoleEventArgs](/api/PuppeteerSharp/ConsoleEventArgs/) | Event args. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1351)

<a id="PuppeteerSharp_Page_OnError_"></a>
<a id="PuppeteerSharp_Page_OnError_PuppeteerSharp_ErrorEventArgs_"></a>

### OnError(ErrorEventArgs)


```csharp
protected void OnError(ErrorEventArgs e)
```


Raises the [Error](#PuppeteerSharp_Page_Error) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `e` | [ErrorEventArgs](/api/PuppeteerSharp/ErrorEventArgs/) | Event args. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1369)

<a id="PuppeteerSharp_Page_OnMetrics_"></a>
<a id="PuppeteerSharp_Page_OnMetrics_PuppeteerSharp_MetricEventArgs_"></a>

### OnMetrics(MetricEventArgs)


```csharp
protected void OnMetrics(MetricEventArgs e)
```


Raises the [Metrics](#PuppeteerSharp_Page_Metrics) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `e` | [MetricEventArgs](/api/PuppeteerSharp/MetricEventArgs/) | Event args. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1357)

<a id="PuppeteerSharp_Page_OnPageError_"></a>
<a id="PuppeteerSharp_Page_OnPageError_PuppeteerSharp_PageErrorEventArgs_"></a>

### OnPageError(PageErrorEventArgs)


```csharp
protected void OnPageError(PageErrorEventArgs e)
```


Raises the [PageError](#PuppeteerSharp_Page_PageError) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `e` | [PageErrorEventArgs](/api/PuppeteerSharp/PageErrorEventArgs/) | Event args. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1363)

<a id="PuppeteerSharp_Page_OnWorkerCreated_"></a>
<a id="PuppeteerSharp_Page_OnWorkerCreated_PuppeteerSharp_WebWorker_"></a>

### OnWorkerCreated(WebWorker)


```csharp
protected void OnWorkerCreated(WebWorker worker)
```


Raises the [WorkerCreated](#PuppeteerSharp_Page_WorkerCreated) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `worker` | [WebWorker](/api/PuppeteerSharp/WebWorker/) | Worker. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1340)

<a id="PuppeteerSharp_Page_OnWorkerDestroyed_"></a>
<a id="PuppeteerSharp_Page_OnWorkerDestroyed_PuppeteerSharp_WebWorker_"></a>

### OnWorkerDestroyed(WebWorker)


```csharp
protected void OnWorkerDestroyed(WebWorker worker)
```


Raises the [WorkerDestroyed](#PuppeteerSharp_Page_WorkerDestroyed) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `worker` | [WebWorker](/api/PuppeteerSharp/WebWorker/) | Worker. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1334)

<a id="PuppeteerSharp_Page_OpenDevToolsAsync_"></a>
<a id="PuppeteerSharp_Page_OpenDevToolsAsync"></a>

### OpenDevToolsAsync()


```csharp
public abstract Task<IPage> OpenDevToolsAsync()
```


Opens DevTools for this page if not already open and returns the DevTools page.
This method is only available in Chrome.

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A `Task%601` that completes with the DevTools page.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L834)

<a id="PuppeteerSharp_Page_PdfAsync_"></a>
<a id="PuppeteerSharp_Page_PdfAsync_System_String_"></a>

### PdfAsync(string)


```csharp
public Task PdfAsync(string file)
```


generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the PDF to. paths are resolved using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the PDF is generated.

Generating a pdf is currently only supported in Chrome headless.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L430)

<a id="PuppeteerSharp_Page_PdfAsync_System_String_PuppeteerSharp_PdfOptions_"></a>

### PdfAsync(string, PdfOptions)


```csharp
public Task PdfAsync(string file, PdfOptions options)
```


generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the PDF to. paths are resolved using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |
| `options` | [PdfOptions](/api/PuppeteerSharp/PdfOptions/) | pdf options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the PDF is generated.

Generating a pdf is currently only supported in Chrome headless.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L433)

<a id="PuppeteerSharp_Page_PdfDataAsync_"></a>
<a id="PuppeteerSharp_Page_PdfDataAsync"></a>

### PdfDataAsync()


```csharp
public Task<byte[]> PdfDataAsync()
```


Generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the PDF data.

Generating a pdf is currently only supported in Chrome headless.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L468)

<a id="PuppeteerSharp_Page_PdfDataAsync_PuppeteerSharp_PdfOptions_"></a>

### PdfDataAsync(PdfOptions)


```csharp
public Task<byte[]> PdfDataAsync(PdfOptions options)
```


Generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [PdfOptions](/api/PuppeteerSharp/PdfOptions/) | pdf options. |

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the PDF data.

Generating a pdf is currently only supported in Chrome headless.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L471)

<a id="PuppeteerSharp_Page_PdfInternalAsync_"></a>
<a id="PuppeteerSharp_Page_PdfInternalAsync_System_IO_Stream_PuppeteerSharp_PdfOptions_"></a>

### PdfInternalAsync(Stream, PdfOptions)


```csharp
protected abstract Task PdfInternalAsync(Stream outputStream, PdfOptions options)
```


PDF implementation.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `outputStream` | [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) | Where to write the PDF bytes. |
| `options` | [PdfOptions](/api/PuppeteerSharp/PdfOptions/) | PDF options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — PDF data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1377)

<a id="PuppeteerSharp_Page_PdfStreamAsync_"></a>
<a id="PuppeteerSharp_Page_PdfStreamAsync"></a>

### PdfStreamAsync()


```csharp
public Task<Stream> PdfStreamAsync()
```


generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the PDF data.

Generating a pdf is currently only supported in Chrome headless.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L450)

<a id="PuppeteerSharp_Page_PdfStreamAsync_PuppeteerSharp_PdfOptions_"></a>

### PdfStreamAsync(PdfOptions)


```csharp
public Task<Stream> PdfStreamAsync(PdfOptions options)
```


Generates a pdf of the page with [Print](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Print) css media. To generate a pdf with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen) media call [EmulateMediaTypeAsync(MediaType)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_) with [Screen](/api/PuppeteerSharp.Media/MediaType/#PuppeteerSharp_Media_MediaType_Screen).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [PdfOptions](/api/PuppeteerSharp/PdfOptions/) | pdf options. |

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the PDF data.

Generating a pdf is currently only supported in Chrome headless.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L453)

<a id="PuppeteerSharp_Page_PerformScreenshotAsync_"></a>
<a id="PuppeteerSharp_Page_PerformScreenshotAsync_PuppeteerSharp_ScreenshotType_PuppeteerSharp_ScreenshotOptions_"></a>

### PerformScreenshotAsync(ScreenshotType, ScreenshotOptions)


```csharp
protected abstract Task<string> PerformScreenshotAsync(ScreenshotType type, ScreenshotOptions options)
```


Screenshot implementation.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `type` | [ScreenshotType](/api/PuppeteerSharp/ScreenshotType/) | Screenshot type. |
| `options` | [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Options. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The screenshot as a base64 string.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1385)

<a id="PuppeteerSharp_Page_QueryObjectsAsync_"></a>
<a id="PuppeteerSharp_Page_QueryObjectsAsync_PuppeteerSharp_IJSHandle_"></a>

### QueryObjectsAsync(IJSHandle)


```csharp
public abstract Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle)
```


The method iterates JavaScript heap and finds all the objects with the given prototype.
Shortcut for `page.MainFrame.GetExecutionContextAsync().QueryObjectsAsync(prototypeHandle)`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `prototypeHandle` | [IJSHandle](/api/PuppeteerSharp/IJSHandle/) | A handle to the object prototype. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves to a handle to an array of objects with this prototype.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L338)

<a id="PuppeteerSharp_Page_QuerySelectorAllAsync_"></a>
<a id="PuppeteerSharp_Page_QuerySelectorAllAsync_System_String_"></a>

### QuerySelectorAllAsync(string)


```csharp
public Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
```


Runs `document.querySelectorAll` within the page. If no elements match the selector, the return value resolve to `Empty%60%601`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query page for. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L288)

<a id="PuppeteerSharp_Page_QuerySelectorAllAsync_System_String_PuppeteerSharp_QueryOptions_"></a>

### QuerySelectorAllAsync(string, QueryOptions)


```csharp
public Task<IElementHandle[]> QuerySelectorAllAsync(string selector, QueryOptions options)
```


Runs `document.querySelectorAll` within the page. If no elements match the selector, the return value resolve to `Empty%60%601`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query page for. |
| `options` | [QueryOptions](/api/PuppeteerSharp/QueryOptions/) | Optional query options. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L292)

<a id="PuppeteerSharp_Page_QuerySelectorAllHandleAsync_"></a>
<a id="PuppeteerSharp_Page_QuerySelectorAllHandleAsync_System_String_"></a>

### QuerySelectorAllHandleAsync(string)


```csharp
public Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
```


A utility function to be used with `Object%5b%5d)`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query page for. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [IJSHandle](/api/PuppeteerSharp/IJSHandle/) of `document.querySelectorAll` result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L296)

<a id="PuppeteerSharp_Page_QuerySelectorAsync_"></a>
<a id="PuppeteerSharp_Page_QuerySelectorAsync_System_String_"></a>

### QuerySelectorAsync(string)


```csharp
public Task<IElementHandle> QuerySelectorAsync(string selector)
```


The method runs `document.querySelector` within the page. If no element matches the selector, the return value resolve to `null`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query page for. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to [IElementHandle](/api/PuppeteerSharp/IElementHandle/) pointing to the frame element.

Shortcut for `page.MainFrame.QuerySelectorAsync(selector)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L284)

<a id="PuppeteerSharp_Page_RecordAsync_"></a>
<a id="PuppeteerSharp_Page_RecordAsync_PuppeteerSharp_RecordOptions_"></a>

### RecordAsync(RecordOptions)


```csharp
[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The caller is responsible for disposing the returned recording.")]
public Task<ScreenRecording> RecordAsync(RecordOptions options = null)
```


Records this [IPage](/api/PuppeteerSharp/IPage/) using the Chrome DevTools Protocol
&lt;a href="https://chromedevtools.github.io/devtools-protocol/tot/Page/#method-startScreenRecording"&gt;Page.startScreenRecording&lt;/a&gt; API.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [RecordOptions](/api/PuppeteerSharp/RecordOptions/) | Recording options. |

**Returns:** [Task&lt;ScreenRecording&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves to a [ScreenRecording](/api/PuppeteerSharp/ScreenRecording/) that can be used to stop the recording.

Outputs mp4 video stream.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L523)

<a id="PuppeteerSharp_Page_ReloadAsync_"></a>
<a id="PuppeteerSharp_Page_ReloadAsync_PuppeteerSharp_ReloadOptions_"></a>

### ReloadAsync(ReloadOptions)


```csharp
public abstract Task<IResponse> ReloadAsync(ReloadOptions options)
```


Reloads the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ReloadOptions](/api/PuppeteerSharp/ReloadOptions/) | Navigation options. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L885)

<a id="PuppeteerSharp_Page_ReloadAsync_System_Nullable_System_Int32__PuppeteerSharp_WaitUntilNavigation___"></a>

### ReloadAsync(int?, WaitUntilNavigation[])


```csharp
public Task<IResponse> ReloadAsync(int? timeout = null, WaitUntilNavigation[] waitUntil = null)
```


Reloads the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timeout` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Maximum navigation time in milliseconds, defaults to 30 seconds, pass `0` to disable timeout. |
| `waitUntil` | `WaitUntilNavigation\[\]` | When to consider navigation succeeded, defaults to [Load](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Load). Given an array of [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/), navigation is considered to be successful after all events have been fired. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L888)

<a id="PuppeteerSharp_Page_RemoveExposedFunctionAsync_"></a>
<a id="PuppeteerSharp_Page_RemoveExposedFunctionAsync_System_String_"></a>

### RemoveExposedFunctionAsync(string)


```csharp
public abstract Task RemoveExposedFunctionAsync(string name)
```


Removes a previously added function via `Action)`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function to remove. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L401)

<a id="PuppeteerSharp_Page_RemoveRequestInterceptor_"></a>
<a id="PuppeteerSharp_Page_RemoveRequestInterceptor_System_Func_PuppeteerSharp_IRequest_System_Threading_Tasks_Task__"></a>

### `RemoveRequestInterceptor(Func<IRequest, Task>)`


```csharp
public void RemoveRequestInterceptor(Func<IRequest, Task> interceptionTask)
```


Removes a previously added request interceptor.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `interceptionTask` | [Func&lt;IRequest, Task&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Interception task. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1130)

<a id="PuppeteerSharp_Page_RemoveScriptToEvaluateOnNewDocumentAsync_"></a>
<a id="PuppeteerSharp_Page_RemoveScriptToEvaluateOnNewDocumentAsync_System_String_"></a>

### RemoveScriptToEvaluateOnNewDocumentAsync(string)


```csharp
public abstract Task RemoveScriptToEvaluateOnNewDocumentAsync(string identifier)
```


Removes script that injected into page by [EvaluateExpressionOnNewDocumentAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EvaluateExpressionOnNewDocumentAsync_System_String_) or [EvaluateExpressionOnNewDocumentAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EvaluateExpressionOnNewDocumentAsync_System_String_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `identifier` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function identifier. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the script is removed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L332)

<a id="PuppeteerSharp_Page_ResizeAsync_"></a>
<a id="PuppeteerSharp_Page_ResizeAsync_System_Int32_System_Int32_"></a>

### ResizeAsync(int, int)


```csharp
public abstract Task ResizeAsync(int contentWidth, int contentHeight)
```


Resizes the browser window of this page so that the content area (excluding browser UI) has the specified width and height.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `contentWidth` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | The desired width of the content area in pixels. |
| `contentHeight` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | The desired height of the content area in pixels. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the resize operation is complete.

This is an experimental API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L310)

<a id="PuppeteerSharp_Page_ScreencastAsync_"></a>
<a id="PuppeteerSharp_Page_ScreencastAsync_PuppeteerSharp_ScreencastOptions_"></a>

### ScreencastAsync(ScreencastOptions)


```csharp
[Obsolete("Use RecordAsync instead.")]
[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The caller is responsible for disposing the returned recorder.")]
public Task<ScreenRecorder> ScreencastAsync(ScreencastOptions options = null)
```


Captures a screencast of this [IPage](/api/PuppeteerSharp/IPage/).

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L587)

<a id="PuppeteerSharp_Page_ScreenshotAsync_"></a>
<a id="PuppeteerSharp_Page_ScreenshotAsync_System_String_"></a>

### ScreenshotAsync(string)


```csharp
public Task ScreenshotAsync(string file)
```


Captures a screenshot of this [IPage](/api/PuppeteerSharp/IPage/).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the image to. The screenshot type will be inferred from file extension. If path is a relative path, then it is resolved relative to current working directory. If no path is provided, the image won't be saved to the disk. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The screenshot task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L667)

<a id="PuppeteerSharp_Page_ScreenshotAsync_System_String_PuppeteerSharp_ScreenshotOptions_"></a>

### ScreenshotAsync(string, ScreenshotOptions)


```csharp
public Task ScreenshotAsync(string file, ScreenshotOptions options)
```


Takes a screenshot of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the image to. The screenshot type will be inferred from file extension. If path is a relative path, then it is resolved relative to current working directory. If no path is provided, the image won't be saved to the disk. |
| `options` | [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Screenshot options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The screenshot task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L670)

<a id="PuppeteerSharp_Page_ScreenshotBase64Async_"></a>
<a id="PuppeteerSharp_Page_ScreenshotBase64Async"></a>

### ScreenshotBase64Async()


```csharp
public Task<string> ScreenshotBase64Async()
```


Takes a screenshot of the page.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [string](https://learn.microsoft.com/dotnet/api/system.string) containing the image data as base64.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L701)

<a id="PuppeteerSharp_Page_ScreenshotBase64Async_PuppeteerSharp_ScreenshotOptions_"></a>

### ScreenshotBase64Async(ScreenshotOptions)


```csharp
public Task<string> ScreenshotBase64Async(ScreenshotOptions options)
```


Takes a screenshot of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [string](https://learn.microsoft.com/dotnet/api/system.string) containing the image data as base64.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L704)

<a id="PuppeteerSharp_Page_ScreenshotDataAsync_"></a>
<a id="PuppeteerSharp_Page_ScreenshotDataAsync"></a>

### ScreenshotDataAsync()


```csharp
public Task<byte[]> ScreenshotDataAsync()
```


Takes a screenshot of the page.

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L821)

<a id="PuppeteerSharp_Page_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_"></a>

### ScreenshotDataAsync(ScreenshotOptions)


```csharp
public Task<byte[]> ScreenshotDataAsync(ScreenshotOptions options)
```


Takes a screenshot of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L824)

<a id="PuppeteerSharp_Page_ScreenshotStreamAsync_"></a>
<a id="PuppeteerSharp_Page_ScreenshotStreamAsync"></a>

### ScreenshotStreamAsync()


```csharp
public Task<Stream> ScreenshotStreamAsync()
```


Takes a screenshot of the page.

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L694)

<a id="PuppeteerSharp_Page_ScreenshotStreamAsync_PuppeteerSharp_ScreenshotOptions_"></a>

### ScreenshotStreamAsync(ScreenshotOptions)


```csharp
public Task<Stream> ScreenshotStreamAsync(ScreenshotOptions options)
```


Takes a screenshot of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L697)

<a id="PuppeteerSharp_Page_SelectAsync_"></a>
<a id="PuppeteerSharp_Page_SelectAsync_System_String_System_String___"></a>

### SelectAsync(string, params string[])


```csharp
public Task<string[]> SelectAsync(string selector, params string[] values)
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
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching &lt;code class="paramref"&gt;selector&lt;/code&gt;. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L892)

<a id="PuppeteerSharp_Page_SetBurstModeOffAsync_"></a>
<a id="PuppeteerSharp_Page_SetBurstModeOffAsync"></a>

### SetBurstModeOffAsync()


```csharp
public abstract Task SetBurstModeOffAsync()
```


Resets the background color and Viewport after taking Screenshots using BurstMode.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The burst mode off.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1079)

<a id="PuppeteerSharp_Page_SetBypassCSPAsync_"></a>
<a id="PuppeteerSharp_Page_SetBypassCSPAsync_System_Boolean_"></a>

### SetBypassCSPAsync(bool)


```csharp
public abstract Task SetBypassCSPAsync(bool enabled)
```


Toggles bypassing page's Content-Security-Policy.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | sets bypassing of page's Content-Security-Policy. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the message is sent to the browser.

CSP bypassing happens at the moment of CSP initialization rather then evaluation.
Usually this means that [SetBypassCSPAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetBypassCSPAsync_System_Boolean_) should be called before navigating to the domain.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L490)

<a id="PuppeteerSharp_Page_SetBypassServiceWorkerAsync_"></a>
<a id="PuppeteerSharp_Page_SetBypassServiceWorkerAsync_System_Boolean_"></a>

### SetBypassServiceWorkerAsync(bool)


```csharp
public abstract Task SetBypassServiceWorkerAsync(bool bypass)
```


Toggles ignoring of service worker for each request.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `bypass` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | When `true` bypasses service worker. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message is sent to the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1137)

<a id="PuppeteerSharp_Page_SetCacheEnabledAsync_"></a>
<a id="PuppeteerSharp_Page_SetCacheEnabledAsync_System_Boolean_"></a>

### SetCacheEnabledAsync(bool)


```csharp
public abstract Task SetCacheEnabledAsync(bool enabled = true)
```


Toggles ignoring cache for each request based on the enabled state. By default, caching is enabled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | sets the `enabled` state of the cache. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L840)

<a id="PuppeteerSharp_Page_SetContentAsync_"></a>
<a id="PuppeteerSharp_Page_SetContentAsync_System_String_PuppeteerSharp_NavigationOptions_"></a>

### SetContentAsync(string, NavigationOptions)


```csharp
[Obsolete("Use SetContentAsync(string, SetContentOptions) instead. The networkidle0 and networkidle2 wait conditions never worked reliably with SetContent.")]
public Task SetContentAsync(string html, NavigationOptions options)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L407)

<a id="PuppeteerSharp_Page_SetContentAsync_System_String_PuppeteerSharp_SetContentOptions_"></a>

### SetContentAsync(string, SetContentOptions)


```csharp
public Task SetContentAsync(string html, SetContentOptions options = null)
```


Sets the HTML markup to the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `html` | [string](https://learn.microsoft.com/dotnet/api/system.string) | HTML markup to assign to the page. |
| `options` | [SetContentOptions](/api/PuppeteerSharp/SetContentOptions/) | The options. The [Networkidle0](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Networkidle0) and [Networkidle2](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Networkidle2) wait conditions are not supported for SetContent. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L414)

<a id="PuppeteerSharp_Page_SetCookieAsync_"></a>
<a id="PuppeteerSharp_Page_SetCookieAsync_PuppeteerSharp_CookieParam___"></a>

### SetCookieAsync(params CookieParam[])


```csharp
public abstract Task SetCookieAsync(params CookieParam[] cookies)
```


Clears all of the current cookies and then sets the cookies for the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieParam\[\]` | Cookies to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L353)

<a id="PuppeteerSharp_Page_SetDragInterceptionAsync_"></a>
<a id="PuppeteerSharp_Page_SetDragInterceptionAsync_System_Boolean_"></a>

### SetDragInterceptionAsync(bool)


```csharp
public abstract Task SetDragInterceptionAsync(bool enabled)
```


Whether to enable drag interception.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Interception enabled. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

Activating drag interception enables the `Input.drag`,
methods This provides the capability to capture drag events emitted
on the page, which can then be used to simulate drag-and-drop.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L271)

<a id="PuppeteerSharp_Page_SetExtraHttpHeadersAsync_"></a>
<a id="PuppeteerSharp_Page_SetExtraHttpHeadersAsync_System_Collections_Generic_Dictionary_System_String_System_String__"></a>

### `SetExtraHttpHeadersAsync(Dictionary<string, string>)`


```csharp
public abstract Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
```


Sets extra HTTP headers that will be sent with every request the page initiates.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `headers` | [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) | Additional http headers to be sent with every request. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L879)

<a id="PuppeteerSharp_Page_SetGeolocationAsync_"></a>
<a id="PuppeteerSharp_Page_SetGeolocationAsync_PuppeteerSharp_GeolocationOption_"></a>

### SetGeolocationAsync(GeolocationOption)


```csharp
public abstract Task SetGeolocationAsync(GeolocationOption options)
```


Sets the page's geolocation.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [GeolocationOption](/api/PuppeteerSharp/GeolocationOption/) | Geolocation options. Pass `null` to remove the geolocation override. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

Consider using `OverridePermission%7d)` to grant permissions for the page to read its geolocation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L268)

<a id="PuppeteerSharp_Page_SetJavaScriptEnabledAsync_"></a>
<a id="PuppeteerSharp_Page_SetJavaScriptEnabledAsync_System_Boolean_"></a>

### SetJavaScriptEnabledAsync(bool)


```csharp
public abstract Task SetJavaScriptEnabledAsync(bool enabled)
```


Enables/Disables Javascript on the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether or not to enable JavaScript on the page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L487)

<a id="PuppeteerSharp_Page_SetOfflineModeAsync_"></a>
<a id="PuppeteerSharp_Page_SetOfflineModeAsync_System_Boolean_"></a>

### SetOfflineModeAsync(bool)


```csharp
public abstract Task SetOfflineModeAsync(bool value)
```


Set offline mode for the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `value` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | When `true` enables offline mode for the page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Result task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L344)

<a id="PuppeteerSharp_Page_SetRequestInterceptionAsync_"></a>
<a id="PuppeteerSharp_Page_SetRequestInterceptionAsync_System_Boolean_"></a>

### SetRequestInterceptionAsync(bool)


```csharp
public abstract Task SetRequestInterceptionAsync(bool value)
```


Activating request interception enables `AbortAsync`,
`ContinueAsync` and `RespondAsync` methods.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `value` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether to enable request interception.. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The request interception task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L341)

<a id="PuppeteerSharp_Page_SetUserAgentAsync_"></a>
<a id="PuppeteerSharp_Page_SetUserAgentAsync_PuppeteerSharp_SetUserAgentOptions_"></a>

### SetUserAgentAsync(SetUserAgentOptions)


```csharp
public abstract Task SetUserAgentAsync(SetUserAgentOptions options)
```


Sets the user agent, user agent metadata, and platform.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [SetUserAgentOptions](/api/PuppeteerSharp/SetUserAgentOptions/) | User agent options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L876)

<a id="PuppeteerSharp_Page_SetUserAgentAsync_System_String_PuppeteerSharp_UserAgentMetadata_"></a>

### SetUserAgentAsync(string, UserAgentMetadata)


```csharp
public abstract Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentData = null)
```


Sets the user agent to be used in this page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `userAgent` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Specific user agent to use in this page. |
| `userAgentData` | [UserAgentMetadata](/api/PuppeteerSharp/UserAgentMetadata/) | Specific user agent client hint data to use in this page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L873)

<a id="PuppeteerSharp_Page_SetViewportAsync_"></a>
<a id="PuppeteerSharp_Page_SetViewportAsync_PuppeteerSharp_ViewPortOptions_"></a>

### SetViewportAsync(ViewPortOptions)


```csharp
public abstract Task SetViewportAsync(ViewPortOptions viewport)
```


Sets the viewport.
In the case of multiple pages in a single browser, each page can have its own viewport size.
[SetViewportAsync(ViewPortOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetViewportAsync_PuppeteerSharp_ViewPortOptions_) will resize the page. A lot of websites don't expect phones to change size, so you should set the viewport before navigating to the page.

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L499)

<a id="PuppeteerSharp_Page_TabInfoAsync_"></a>
<a id="PuppeteerSharp_Page_TabInfoAsync"></a>

### TabInfoAsync()


```csharp
public abstract Task<TabInfo> TabInfoAsync()
```


Returns information about the browser tab of this page. The result is a snapshot and is not updated when the tab changes.

**Returns:** [Task&lt;TabInfo&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the tab info, or `null` if the page is not in a tab strip (for example, in chrome-headless-shell) or the browser does not report tab information.

This is an experimental API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L316)

<a id="PuppeteerSharp_Page_TapAsync_"></a>
<a id="PuppeteerSharp_Page_TapAsync_System_String_"></a>

### TapAsync(string)


```csharp
public Task TapAsync(string selector)
```


Fetches an element with &lt;code class="paramref"&gt;selector&lt;/code&gt;, scrolls it into view if needed, and then uses [Touchscreen](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Touchscreen) to tap in the center of the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to tap. If there are multiple elements satisfying the selector, the first will be clicked. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching &lt;code class="paramref"&gt;selector&lt;/code&gt; is successfully tapped.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching &lt;code class="paramref"&gt;selector&lt;/code&gt;. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L280)

<a id="PuppeteerSharp_Page_TriggerExtensionActionAsync_"></a>
<a id="PuppeteerSharp_Page_TriggerExtensionActionAsync_PuppeteerSharp_Extension_"></a>

### TriggerExtensionActionAsync(Extension)


```csharp
public abstract Task TriggerExtensionActionAsync(Extension extension)
```


Triggers an extension action for the given extension.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `extension` | [Extension](/api/PuppeteerSharp/Extension/) | The extension to trigger the action for. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the extension action has been triggered.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1140)

<a id="PuppeteerSharp_Page_TypeAsync_"></a>
<a id="PuppeteerSharp_Page_TypeAsync_System_String_System_String_PuppeteerSharp_Input_TypeOptions_"></a>

### TypeAsync(string, string, TypeOptions)


```csharp
public Task TypeAsync(string selector, string text, TypeOptions options = null)
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
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching &lt;code class="paramref"&gt;selector&lt;/code&gt;. |

To press a special key, like `Control` or `ArrowDown` use `PressOptions)`.

**Example**

```csharp
await page.TypeAsync("#mytextarea", "Hello"); // Types instantly
await page.TypeAsync("#mytextarea", "World", new TypeOptions { Delay = 100 }); // Types slower, like a user
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L853)

<a id="PuppeteerSharp_Page_WaitForDevicePromptAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForDevicePromptAsync_PuppeteerSharp_WaitForOptions_"></a>

### WaitForDevicePromptAsync(WaitForOptions)


```csharp
public Task<DeviceRequestPrompt> WaitForDevicePromptAsync(WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L305)

<a id="PuppeteerSharp_Page_WaitForExpressionAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForExpressionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_"></a>

### WaitForExpressionAsync(string, WaitForFunctionOptions)


```csharp
public Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options = null)
```


Waits for an expression to be evaluated to a truthy value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Expression to be evaluated in browser context. |
| `options` | [WaitForFunctionOptions](/api/PuppeteerSharp/WaitForFunctionOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when the `script` returns a truthy value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L904)

<a id="PuppeteerSharp_Page_WaitForFileChooserAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForFileChooserAsync_PuppeteerSharp_WaitForOptions_"></a>

### WaitForFileChooserAsync(WaitForOptions)


```csharp
public abstract Task<FileChooser> WaitForFileChooserAsync(WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1070)

<a id="PuppeteerSharp_Page_WaitForFrameAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForFrameAsync_System_Func_PuppeteerSharp_IFrame_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

### `WaitForFrameAsync(Func<IFrame, bool>, WaitForOptions)`


```csharp
public Task<IFrame> WaitForFrameAsync(Func<IFrame, bool> predicate, WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L999)

<a id="PuppeteerSharp_Page_WaitForFrameAsync_System_String_PuppeteerSharp_WaitForOptions_"></a>

### WaitForFrameAsync(string, WaitForOptions)


```csharp
public Task<IFrame> WaitForFrameAsync(string url, WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L995)

<a id="PuppeteerSharp_Page_WaitForFunctionAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForFunctionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_System_Object___"></a>

### WaitForFunctionAsync(string, WaitForFunctionOptions, params object[])


```csharp
public Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
```


Waits for a function to be evaluated to a truthy value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `options` | [WaitForFunctionOptions](/api/PuppeteerSharp/WaitForFunctionOptions/) | Optional waiting parameters. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `script`. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when the `script` returns a truthy value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L896)

<a id="PuppeteerSharp_Page_WaitForFunctionAsync_System_String_System_Object___"></a>

### WaitForFunctionAsync(string, params object[])


```csharp
public Task<IJSHandle> WaitForFunctionAsync(string script, params object[] args)
```


Waits for a function to be evaluated to a truthy value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `script`. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when the `script` returns a truthy value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L900)

<a id="PuppeteerSharp_Page_WaitForNavigationAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForNavigationAsync_PuppeteerSharp_NavigationOptions_"></a>

### WaitForNavigationAsync(NavigationOptions)


```csharp
public Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
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

Usage of the `History API` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/History_API"&gt;https://developer.mozilla.org/en-US/docs/Web/API/History_API&lt;/a&gt; to change the URL is considered a navigation.

**Example**

```csharp
var navigationTask = page.WaitForNavigationAsync();
await page.ClickAsync("a.my-link");
await navigationTask;
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L926)

<a id="PuppeteerSharp_Page_WaitForNetworkIdleAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForNetworkIdleAsync_PuppeteerSharp_WaitForNetworkIdleOptions_"></a>

### WaitForNetworkIdleAsync(WaitForNetworkIdleOptions)


```csharp
public Task WaitForNetworkIdleAsync(WaitForNetworkIdleOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L930)

<a id="PuppeteerSharp_Page_WaitForRequestAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForRequestAsync_System_Func_PuppeteerSharp_IRequest_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

### `WaitForRequestAsync(Func<IRequest, bool>, WaitForOptions)`


```csharp
public Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> predicate, WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L968)

<a id="PuppeteerSharp_Page_WaitForRequestAsync_System_String_PuppeteerSharp_WaitForOptions_"></a>

### WaitForRequestAsync(string, WaitForOptions)


```csharp
public Task<IRequest> WaitForRequestAsync(string url, WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L964)

<a id="PuppeteerSharp_Page_WaitForResponseAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForResponseAsync_System_Func_PuppeteerSharp_IResponse_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

### `WaitForResponseAsync(Func<IResponse, bool>, WaitForOptions)`


```csharp
public Task<IResponse> WaitForResponseAsync(Func<IResponse, bool> predicate, WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1037)

<a id="PuppeteerSharp_Page_WaitForResponseAsync_System_Func_PuppeteerSharp_IResponse_System_Threading_Tasks_Task_System_Boolean___PuppeteerSharp_WaitForOptions_"></a>

### `WaitForResponseAsync(Func<IResponse, Task<bool>>, WaitForOptions)`


```csharp
public Task<IResponse> WaitForResponseAsync(Func<IResponse, Task<bool>> predicate, WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1041)

<a id="PuppeteerSharp_Page_WaitForResponseAsync_System_String_PuppeteerSharp_WaitForOptions_"></a>

### WaitForResponseAsync(string, WaitForOptions)


```csharp
public Task<IResponse> WaitForResponseAsync(string url, WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L1033)

<a id="PuppeteerSharp_Page_WaitForSelectorAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForSelectorAsync_System_String_PuppeteerSharp_WaitForSelectorOptions_"></a>

### WaitForSelectorAsync(string, WaitForSelectorOptions)


```csharp
public Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
```


Waits for a selector to be added to the DOM.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector of an element to wait for. |
| `options` | [WaitForSelectorOptions](/api/PuppeteerSharp/WaitForSelectorOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when element specified by selector string is added to DOM.
    Resolves to `null` if waiting for `hidden: true` and selector is not found in DOM.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L908)

<a id="PuppeteerSharp_Page_WaitForXPathAsync_"></a>
<a id="PuppeteerSharp_Page_WaitForXPathAsync_System_String_PuppeteerSharp_WaitForSelectorOptions_"></a>

### WaitForXPathAsync(string, WaitForSelectorOptions)


```csharp
public Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
```


Waits for a xpath selector to be added to the DOM.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `xpath` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A xpath selector of an element to wait for. |
| `options` | [WaitForSelectorOptions](/api/PuppeteerSharp/WaitForSelectorOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves when element specified by xpath string is added to DOM.
    Resolves to `null` if waiting for `hidden: true` and xpath is not found in DOM.

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L921)

<a id="PuppeteerSharp_Page_WindowIdAsync_"></a>
<a id="PuppeteerSharp_Page_WindowIdAsync"></a>

### WindowIdAsync()


```csharp
public abstract Task<string> WindowIdAsync()
```


Returns the page's window ID.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the window ID string.

This is an experimental API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L313)

<a id="PuppeteerSharp_Page_XPathAsync_"></a>
<a id="PuppeteerSharp_Page_XPathAsync_System_String_"></a>

### XPathAsync(string)


```csharp
public Task<IElementHandle[]> XPathAsync(string expression)
```


Evaluates the XPath expression.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `expression` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Expression to evaluate &lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate"&gt;https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate&lt;/a&gt;. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of [IElementHandle](/api/PuppeteerSharp/IElementHandle/).

Shortcut for `page.MainFrame.XPathAsync(expression)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Page.cs#L301)
