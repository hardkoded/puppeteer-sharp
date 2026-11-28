---
title: "IBrowser"
description: "Provides methods to interact with a browser."
editUrl: false
prev: false
next: false
sidebar:
  label: "IBrowser"
  hidden: true
---

<a id="PuppeteerSharp_IBrowser"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IBrowser : IDisposable, IAsyncDisposable
```


Provides methods to interact with a browser.

**Derived:** [BidiBrowser](/api/PuppeteerSharp.Bidi/BidiBrowser/), [Browser](/api/PuppeteerSharp/Browser/), [CdpBrowser](/api/PuppeteerSharp.Cdp/CdpBrowser/)

**Example**

An example of using a [IBrowser](./) to create a [IPage](/api/PuppeteerSharp/IPage/):


```csharp
var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com");
await browser.CloseAsync();
```


An example of disconnecting from and reconnecting to a [Browser](/api/PuppeteerSharp/Browser/):


```csharp
var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var browserWSEndpoint = browser.WebSocketEndpoint;
browser.Disconnect();
var browser2 = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint });
await browser2.CloseAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L33)

## Properties

<a id="PuppeteerSharp_IBrowser_BrowserType_"></a>
<a id="PuppeteerSharp_IBrowser_BrowserType"></a>

### BrowserType


```csharp
SupportedBrowser BrowserType { get; }
```


Returns the browser type. Chrome, Chromium or Firefox.

**Value:** [SupportedBrowser](/api/PuppeteerSharp/SupportedBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L114)

<a id="PuppeteerSharp_IBrowser_DebugInfo_"></a>
<a id="PuppeteerSharp_IBrowser_DebugInfo"></a>

### DebugInfo


```csharp
DebugInfo DebugInfo { get; }
```


Get debug information from Puppeteer.

**Value:** [DebugInfo](/api/PuppeteerSharp/DebugInfo/)

Currently, includes pending protocol calls. In the future, more info might be added.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L159)

<a id="PuppeteerSharp_IBrowser_DefaultContext_"></a>
<a id="PuppeteerSharp_IBrowser_DefaultContext"></a>

### DefaultContext


```csharp
IBrowserContext DefaultContext { get; }
```


Returns the default browser context. The default browser context can not be closed.

**Value:** [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/) — The default context.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L109)

<a id="PuppeteerSharp_IBrowser_DefaultWaitForTimeout_"></a>
<a id="PuppeteerSharp_IBrowser_DefaultWaitForTimeout"></a>

### DefaultWaitForTimeout


```csharp
int DefaultWaitForTimeout { get; set; }
```


Default wait time in milliseconds. Defaults to 30 seconds.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L119)

<a id="PuppeteerSharp_IBrowser_IsClosed_"></a>
<a id="PuppeteerSharp_IBrowser_IsClosed"></a>

### IsClosed


```csharp
bool IsClosed { get; }
```


Gets a value indicating if the browser is closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L124)

<a id="PuppeteerSharp_IBrowser_IsConnected_"></a>
<a id="PuppeteerSharp_IBrowser_IsConnected"></a>

### IsConnected


```csharp
bool IsConnected { get; }
```


Indicates that the browser is connected.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L129)

<a id="PuppeteerSharp_IBrowser_Process_"></a>
<a id="PuppeteerSharp_IBrowser_Process"></a>

### Process


```csharp
Process Process { get; }
```


Gets the spawned browser process. Returns `null` if the browser instance was created with `Puppeteer.ConnectAsync()` method.

**Value:** [Process](https://learn.microsoft.com/dotnet/api/system.diagnostics.process)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L134)

<a id="PuppeteerSharp_IBrowser_Target_"></a>
<a id="PuppeteerSharp_IBrowser_Target"></a>

### Target


```csharp
ITarget Target { get; }
```


A target associated with the browser.

**Value:** [ITarget](/api/PuppeteerSharp/ITarget/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L139)

<a id="PuppeteerSharp_IBrowser_WebSocketEndpoint_"></a>
<a id="PuppeteerSharp_IBrowser_WebSocketEndpoint"></a>

### WebSocketEndpoint


```csharp
string WebSocketEndpoint { get; }
```


Gets the Browser websocket url.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

Browser websocket endpoint which can be used as an argument to `Puppeteer.ConnectAsync()`.
The format is `ws://${host}:${port}/devtools/browser/[id]`
You can find the `webSocketDebuggerUrl` from `http://${host}:${port}/json/version`.
Learn more about the devtools protocol [https://chromedevtools.github.io/devtools-protocol](https://chromedevtools.github.io/devtools-protocol)
and the browser endpoint [https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target](https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L151)

## Events

<a id="PuppeteerSharp_IBrowser_Closed"></a>

### Closed


```csharp
event EventHandler Closed
```


Raised when the [IBrowser](./) gets closed.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L38)

<a id="PuppeteerSharp_IBrowser_Console"></a>

### Console


```csharp
event EventHandler<ConsoleEventArgs> Console
```


Raised when JavaScript within any page or worker in the browser calls one of the console API methods, e.g. `console.log` or `console.dir`.
Only messages from pages and workers that PuppeteerSharp has attached to are delivered, e.g. via `PageAsync()` or `PagesAsync()`. `Targets()` only lists targets and does not attach to them.
Service and shared workers are not attached automatically: their messages are only delivered after `WorkerAsync()` has been called for their target.
With WebDriver BiDi, service and shared worker console messages are not reported.

**Value:** [EventHandler&lt;ConsoleEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L46)

<a id="PuppeteerSharp_IBrowser_Disconnected"></a>

### Disconnected


```csharp
event EventHandler Disconnected
```


Raised when puppeteer gets disconnected from the Chromium instance. This might happen because one of the following
- Chromium is closed or crashed
- [Disconnect()](#PuppeteerSharp_IBrowser_Disconnect) method was called

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L53)

<a id="PuppeteerSharp_IBrowser_Request"></a>

### Request


```csharp
event EventHandler<RequestEventArgs> Request
```


Raised when any page in the browser issues a request. Contains an [IRequest](/api/PuppeteerSharp/IRequest/) instance.
Use [IPage](/api/PuppeteerSharp/IPage/) or [IFrame](/api/PuppeteerSharp/IFrame/) from the request to identify the originating context.
Events are only delivered for pages that have an [IPage](/api/PuppeteerSharp/IPage/) instance, for example one created via [PagesAsync(bool)](#PuppeteerSharp_IBrowser_PagesAsync_System_Boolean_).
These listeners only observe requests. Do not call `Continue`, `Abort` or `Respond` from them. To intercept requests, use [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_) with an [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request) listener.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L103)

<a id="PuppeteerSharp_IBrowser_RequestFailed"></a>

### RequestFailed


```csharp
event EventHandler<RequestEventArgs> RequestFailed
```


Raised when a request issued by any page in the browser fails, for example by timing out.
HTTP error responses, such as 404 or 503, complete with [RequestFinished](#PuppeteerSharp_IBrowser_RequestFinished) instead.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L90)

<a id="PuppeteerSharp_IBrowser_RequestFinished"></a>

### RequestFinished


```csharp
event EventHandler<RequestEventArgs> RequestFinished
```


Raised when a request issued by any page in the browser finishes successfully.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L95)

<a id="PuppeteerSharp_IBrowser_RequestServedFromCache"></a>

### RequestServedFromCache


```csharp
event EventHandler<RequestEventArgs> RequestServedFromCache
```


Raised when a request issued by any page in the browser is served from the memory cache.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L78)

<a id="PuppeteerSharp_IBrowser_Response"></a>

### Response


```csharp
event EventHandler<ResponseCreatedEventArgs> Response
```


Raised when a response is received for a request issued by any page in the browser.
Use [Request](/api/PuppeteerSharp/IResponse/#PuppeteerSharp_IResponse_Request) to access the originating request.

**Value:** [EventHandler&lt;ResponseCreatedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L84)

<a id="PuppeteerSharp_IBrowser_TargetChanged"></a>

### TargetChanged


```csharp
event EventHandler<TargetChangedArgs> TargetChanged
```


Raised when the url of a target changes

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L58)

<a id="PuppeteerSharp_IBrowser_TargetCreated"></a>

### TargetCreated


```csharp
event EventHandler<TargetChangedArgs> TargetCreated
```


Raised when a target is created, for example when a new page is opened by `window.open` [https://developer.mozilla.org/en-US/docs/Web/API/Window/open](https://developer.mozilla.org/en-US/docs/Web/API/Window/open) or [NewPageAsync(CreatePageOptions)](#PuppeteerSharp_IBrowser_NewPageAsync_PuppeteerSharp_CreatePageOptions_).

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L63)

<a id="PuppeteerSharp_IBrowser_TargetDestroyed"></a>

### TargetDestroyed


```csharp
event EventHandler<TargetChangedArgs> TargetDestroyed
```


Raised when a target is destroyed, for example when a page is closed

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L68)

<a id="PuppeteerSharp_IBrowser_TargetDiscovered"></a>

### TargetDiscovered


```csharp
event EventHandler<TargetChangedArgs> TargetDiscovered
```


Raised when a target is discovered

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L73)

## Methods

<a id="PuppeteerSharp_IBrowser_AddScreenAsync_"></a>
<a id="PuppeteerSharp_IBrowser_AddScreenAsync_PuppeteerSharp_AddScreenParams_"></a>

### AddScreenAsync(AddScreenParams)


```csharp
Task<ScreenInfo> AddScreenAsync(AddScreenParams @params)
```


Adds a new screen with specified parameters and returns a [ScreenInfo](/api/PuppeteerSharp/ScreenInfo/) object, including the new screen's ID.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `params` | [AddScreenParams](/api/PuppeteerSharp/AddScreenParams/) | The parameters for the new screen. |

**Returns:** [Task&lt;ScreenInfo&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the added screen information.

Only supported in headless mode.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L312)

<a id="PuppeteerSharp_IBrowser_BrowserContexts_"></a>
<a id="PuppeteerSharp_IBrowser_BrowserContexts"></a>

### BrowserContexts()


```csharp
IBrowserContext[] BrowserContexts()
```


Returns an array of all open [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/). In a newly created browser, this will return a single instance of [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/).

**Returns:** `IBrowserContext[]` — An array of [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/) objects.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L165)

<a id="PuppeteerSharp_IBrowser_ClearCustomQueryHandlers_"></a>
<a id="PuppeteerSharp_IBrowser_ClearCustomQueryHandlers"></a>

### ClearCustomQueryHandlers()


```csharp
void ClearCustomQueryHandlers()
```


Clears all registered handlers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L283)

<a id="PuppeteerSharp_IBrowser_CloseAsync_"></a>
<a id="PuppeteerSharp_IBrowser_CloseAsync"></a>

### CloseAsync()


```csharp
Task CloseAsync()
```


Closes Chromium and all of its pages (if any were opened). The browser object itself is considered disposed and cannot be used anymore.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L171)

<a id="PuppeteerSharp_IBrowser_CreateBrowserContextAsync_"></a>
<a id="PuppeteerSharp_IBrowser_CreateBrowserContextAsync_PuppeteerSharp_BrowserContextOptions_"></a>

### CreateBrowserContextAsync(BrowserContextOptions)


```csharp
Task<IBrowserContext> CreateBrowserContextAsync(BrowserContextOptions options = null)
```


Creates a new browser context. This won't share cookies/cache with other browser contexts.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [BrowserContextOptions](/api/PuppeteerSharp/BrowserContextOptions/) | Options. |

**Returns:** [Task&lt;IBrowserContext&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a new [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/) object.

**Example**

```csharp
using(var browser = await Puppeteer.LaunchAsync(new LaunchOptions()))
{
    // Create a new browser context.
    var context = await browser.CreateBrowserContextAsync();
    // Create a new page in a pristine context.
    var page = await context.NewPageAsync();
    // Do stuff
    await page.GoToAsync("https://example.com");
}
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L193)

<a id="PuppeteerSharp_IBrowser_CreateCDPSessionAsync_"></a>
<a id="PuppeteerSharp_IBrowser_CreateCDPSessionAsync"></a>

### CreateCDPSessionAsync()


```csharp
Task<ICDPSession> CreateCDPSessionAsync()
```


Creates a Chrome Devtools Protocol session attached to the browser.

**Returns:** [Task&lt;ICDPSession&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [ICDPSession](/api/PuppeteerSharp/ICDPSession/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L449)

<a id="PuppeteerSharp_IBrowser_DeleteCookieAsync_"></a>
<a id="PuppeteerSharp_IBrowser_DeleteCookieAsync_PuppeteerSharp_CookieParam___"></a>

### DeleteCookieAsync(params CookieParam[])


```csharp
Task DeleteCookieAsync(params CookieParam[] cookies)
```


Removes cookies from the default browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieParam[]` | Complete [CookieParam](/api/PuppeteerSharp/CookieParam/) objects to be removed. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L348)

<a id="PuppeteerSharp_IBrowser_DeleteMatchingCookiesAsync_"></a>
<a id="PuppeteerSharp_IBrowser_DeleteMatchingCookiesAsync_PuppeteerSharp_DeleteCookiesRequest___"></a>

### DeleteMatchingCookiesAsync(params DeleteCookiesRequest[])


```csharp
Task DeleteMatchingCookiesAsync(params DeleteCookiesRequest[] filters)
```


Deletes cookies matching the provided filters from the default [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `filters` | `DeleteCookiesRequest[]` | Filters to match cookies against. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L355)

<a id="PuppeteerSharp_IBrowser_Disconnect_"></a>
<a id="PuppeteerSharp_IBrowser_Disconnect"></a>

### Disconnect()


```csharp
void Disconnect()
```


Disconnects Puppeteer from the browser, but leaves the process running. After calling [Disconnect()](#PuppeteerSharp_IBrowser_Disconnect), the browser object is considered disposed and cannot be used anymore.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L198)

<a id="PuppeteerSharp_IBrowser_ExtensionsAsync_"></a>
<a id="PuppeteerSharp_IBrowser_ExtensionsAsync"></a>

### ExtensionsAsync()


```csharp
Task<IReadOnlyDictionary<string, Extension>> ExtensionsAsync()
```


Returns a map of installed extensions, keyed by extension ID.

**Returns:** [Task&lt;IReadOnlyDictionary&lt;string, Extension&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to a dictionary of extension ID to [Extension](/api/PuppeteerSharp/Extension/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L376)

<a id="PuppeteerSharp_IBrowser_GetCookiesAsync_"></a>
<a id="PuppeteerSharp_IBrowser_GetCookiesAsync"></a>

### GetCookiesAsync()


```csharp
Task<CookieParam[]> GetCookiesAsync()
```


Gets all cookies in the default browser context.

**Returns:** [Task&lt;CookieParam\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of [CookieParam](/api/PuppeteerSharp/CookieParam/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L334)

<a id="PuppeteerSharp_IBrowser_GetPWAStateAsync_"></a>
<a id="PuppeteerSharp_IBrowser_GetPWAStateAsync_PuppeteerSharp_GetPWAStateOptions_"></a>

### GetPWAStateAsync(GetPWAStateOptions)


```csharp
Task<PWAState> GetPWAStateAsync(GetPWAStateOptions options)
```


Returns the OS-integration state of an installed Progressive Web App (PWA), such as its badge count and
registered file handlers.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [GetPWAStateOptions](/api/PuppeteerSharp/GetPWAStateOptions/) | Options for querying the PWA state. |

**Returns:** [Task&lt;PWAState&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the PWA state.

Only available over a pipe connection. See [InstallPWAAsync(InstallPWAOptions)](#PuppeteerSharp_IBrowser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_). Meaningful
only for an app that is currently installed; querying an unknown manifest id rejects.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L425)

<a id="PuppeteerSharp_IBrowser_GetUserAgentAsync_"></a>
<a id="PuppeteerSharp_IBrowser_GetUserAgentAsync"></a>

### GetUserAgentAsync()


```csharp
Task<string> GetUserAgentAsync()
```


Gets the browser's original user agent.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the browser's original user agent.

Pages can override browser user agent with `IPage.SetUserAgentAsync()`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L207)

<a id="PuppeteerSharp_IBrowser_GetVersionAsync_"></a>
<a id="PuppeteerSharp_IBrowser_GetVersionAsync"></a>

### GetVersionAsync()


```csharp
Task<string> GetVersionAsync()
```


Gets the browser's version.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — For headless Chromium, this is similar to `HeadlessChrome/61.0.3153.0`. For non-headless, this is similar to `Chrome/61.0.3153.0`.

the format of [GetVersionAsync()](#PuppeteerSharp_IBrowser_GetVersionAsync) might change with future releases of Chromium.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L216)

<a id="PuppeteerSharp_IBrowser_GetWindowBoundsAsync_"></a>
<a id="PuppeteerSharp_IBrowser_GetWindowBoundsAsync_System_String_"></a>

### GetWindowBoundsAsync(string)


```csharp
Task<WindowBounds> GetWindowBoundsAsync(string windowId)
```


Gets the specified window bounds.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `windowId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The window ID. |

**Returns:** [Task&lt;WindowBounds&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the [WindowBounds](/api/PuppeteerSharp/WindowBounds/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L290)

<a id="PuppeteerSharp_IBrowser_InstallExtensionAsync_"></a>
<a id="PuppeteerSharp_IBrowser_InstallExtensionAsync_System_String_PuppeteerSharp_ExtensionInstallOptions_"></a>

### InstallExtensionAsync(string, ExtensionInstallOptions)


```csharp
Task<string> InstallExtensionAsync(string path, ExtensionInstallOptions options = null)
```


Installs an unpacked extension and returns the extension ID.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `path` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The path to the unpacked extension directory. |
| `options` | [ExtensionInstallOptions](/api/PuppeteerSharp/ExtensionInstallOptions/) | Options for installing the extension. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the extension ID.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L363)

<a id="PuppeteerSharp_IBrowser_InstallPWAAsync_"></a>
<a id="PuppeteerSharp_IBrowser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_"></a>

### InstallPWAAsync(InstallPWAOptions)


```csharp
Task<string> InstallPWAAsync(InstallPWAOptions options)
```


Installs a Progressive Web App (PWA) and returns its manifest id.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [InstallPWAOptions](/api/PuppeteerSharp/InstallPWAOptions/) | Options for installing the PWA. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the manifest id.

Only available when connected to the browser over a pipe connection. Set [Pipe](/api/PuppeteerSharp/LaunchOptions/#PuppeteerSharp_LaunchOptions_Pipe)
to `true`; it defaults to `false`. The underlying `PWA` CDP domain is not exposed over a
WebSocket connection.
The returned manifest id echoes [ManifestId](/api/PuppeteerSharp/InstallPWAOptions/#PuppeteerSharp_InstallPWAOptions_ManifestId), so it can be passed directly
to [LaunchPWAAsync(LaunchPWAOptions)](#PuppeteerSharp_IBrowser_LaunchPWAAsync_PuppeteerSharp_LaunchPWAOptions_), [GetPWAStateAsync(GetPWAStateOptions)](#PuppeteerSharp_IBrowser_GetPWAStateAsync_PuppeteerSharp_GetPWAStateOptions_), or
[UninstallPWAAsync(UninstallPWAOptions)](#PuppeteerSharp_IBrowser_UninstallPWAAsync_PuppeteerSharp_UninstallPWAOptions_).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L391)

<a id="PuppeteerSharp_IBrowser_LaunchPWAAsync_"></a>
<a id="PuppeteerSharp_IBrowser_LaunchPWAAsync_PuppeteerSharp_LaunchPWAOptions_"></a>

### LaunchPWAAsync(LaunchPWAOptions)


```csharp
Task<IPage> LaunchPWAAsync(LaunchPWAOptions options)
```


Launches an installed Progressive Web App (PWA) and resolves with the [IPage](/api/PuppeteerSharp/IPage/) backing the app window.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LaunchPWAOptions](/api/PuppeteerSharp/LaunchPWAOptions/) | Options for launching the PWA. |

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the page backing the app window.

Only available over a pipe connection. See [InstallPWAAsync(InstallPWAOptions)](#PuppeteerSharp_IBrowser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_).
`PWA.launch` resolves with the id of the launched *tab* target. Puppeteer does not expose tab
targets through [Targets()](#PuppeteerSharp_IBrowser_Targets); this method instead resolves with the tab's child page target
(the app's web contents). If Chromium focuses an existing app window, this returns that window's
existing page.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L413)

<a id="PuppeteerSharp_IBrowser_NewPageAsync_"></a>
<a id="PuppeteerSharp_IBrowser_NewPageAsync_PuppeteerSharp_CreatePageOptions_"></a>

### NewPageAsync(CreatePageOptions)


```csharp
Task<IPage> NewPageAsync(CreatePageOptions options = null)
```


Creates a new page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [CreatePageOptions](/api/PuppeteerSharp/CreatePageOptions/) | Options for creating the page. |

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a new [IPage](/api/PuppeteerSharp/IPage/) object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L223)

<a id="PuppeteerSharp_IBrowser_PagesAsync_"></a>
<a id="PuppeteerSharp_IBrowser_PagesAsync_System_Boolean_"></a>

### PagesAsync(bool)


```csharp
Task<IPage[]> PagesAsync(bool includeAll = false)
```


Returns a Task which resolves to an array of all open pages.
Non visible pages, such as `"background_page"`, will not be listed here. You can find them using [PageAsync()](/api/PuppeteerSharp/Target/#PuppeteerSharp_Target_PageAsync).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `includeAll` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Experimental. When true, includes all kinds of pages. |

**Returns:** [Task&lt;IPage\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of all open pages inside the Browser.
    In case of multiple browser contexts, the method will return an array with all the pages in all browser contexts.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L233)

<a id="PuppeteerSharp_IBrowser_RegisterCustomQueryHandler_"></a>
<a id="PuppeteerSharp_IBrowser_RegisterCustomQueryHandler_System_String_PuppeteerSharp_CustomQueryHandler_"></a>

### RegisterCustomQueryHandler(string, CustomQueryHandler)


```csharp
void RegisterCustomQueryHandler(string name, CustomQueryHandler queryHandler)
```


Registers a custom query handler.
After registration, the handler can be used everywhere where a selector is
expected by prepending the selection string with `name/`. The name is
only allowed to consist of lower- and upper case latin letters.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The name that the custom query handler will be registered under. |
| `queryHandler` | [CustomQueryHandler](/api/PuppeteerSharp/CustomQueryHandler/) | The query handler to register. |

**Example**

Puppeteer.RegisterCustomQueryHandler("text", "{ … }");
var aHandle = await page.QuerySelectorAsync("text/…").

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L272)

<a id="PuppeteerSharp_IBrowser_RemoveScreenAsync_"></a>
<a id="PuppeteerSharp_IBrowser_RemoveScreenAsync_System_String_"></a>

### RemoveScreenAsync(string)


```csharp
Task RemoveScreenAsync(string screenId)
```


Removes the screen associated with the given screen ID, unless it is the primary screen.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `screenId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The ID of the screen to remove. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the screen is removed.

Only supported in headless mode. Fails if the primary screen ID is specified.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L320)

<a id="PuppeteerSharp_IBrowser_RestrictNetworkAsync_"></a>
<a id="PuppeteerSharp_IBrowser_RestrictNetworkAsync_PuppeteerSharp_NetworkRestrictions_"></a>

### RestrictNetworkAsync(NetworkRestrictions)


```csharp
Task RestrictNetworkAsync(NetworkRestrictions restrictions = null)
```


Sets the network restrictions (blocklist or allowlist) and re-applies them to all targets.
To clear all restrictions, call it without arguments or pass `null`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `restrictions` | [NetworkRestrictions](/api/PuppeteerSharp/NetworkRestrictions/) | The network restrictions to apply. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the restrictions were applied.

Only supported for Chrome and the CDP protocol.
Targets that are already on a blocked URL when this method is called stay attached and stay on
the blocked page. This differs from launching the browser with network restrictions, where such
targets are detached on the initial attach.



The restrictions have the same WebTransport and WebRTC limitations as
[BlockList](/api/PuppeteerSharp/ConnectOptions/#PuppeteerSharp_ConnectOptions_BlockList) and [Allowlist](/api/PuppeteerSharp/ConnectOptions/#PuppeteerSharp_ConnectOptions_Allowlist).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L443)

<a id="PuppeteerSharp_IBrowser_ScreensAsync_"></a>
<a id="PuppeteerSharp_IBrowser_ScreensAsync"></a>

### ScreensAsync()


```csharp
Task<ScreenInfo[]> ScreensAsync()
```


Gets a list of [ScreenInfo](/api/PuppeteerSharp/ScreenInfo/) objects.

**Returns:** [Task&lt;ScreenInfo\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to an array of screen information objects.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L304)

<a id="PuppeteerSharp_IBrowser_SetCookieAsync_"></a>
<a id="PuppeteerSharp_IBrowser_SetCookieAsync_PuppeteerSharp_CookieData___"></a>

### SetCookieAsync(params CookieData[])


```csharp
Task SetCookieAsync(params CookieData[] cookies)
```


Sets cookies in the default browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieData[]` | Cookies to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L341)

<a id="PuppeteerSharp_IBrowser_SetPermissionAsync_"></a>
<a id="PuppeteerSharp_IBrowser_SetPermissionAsync_System_String_PuppeteerSharp_PermissionEntry___"></a>

### SetPermissionAsync(string, params PermissionEntry[])


```csharp
Task SetPermissionAsync(string origin, params PermissionEntry[] permissions)
```


Sets the permission for a specific origin on the default browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `origin` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The origin to set the permission for, e.g. "https://example.com". Use "*" for all origins (CDP only). |
| `permissions` | `PermissionEntry[]` | The permissions to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L328)

<a id="PuppeteerSharp_IBrowser_SetWindowBoundsAsync_"></a>
<a id="PuppeteerSharp_IBrowser_SetWindowBoundsAsync_System_String_PuppeteerSharp_WindowBounds_"></a>

### SetWindowBoundsAsync(string, WindowBounds)


```csharp
Task SetWindowBoundsAsync(string windowId, WindowBounds windowBounds)
```


Sets the specified window bounds.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `windowId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The window ID. |
| `windowBounds` | [WindowBounds](/api/PuppeteerSharp/WindowBounds/) | The bounds to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the bounds have been set.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L298)

<a id="PuppeteerSharp_IBrowser_Targets_"></a>
<a id="PuppeteerSharp_IBrowser_Targets"></a>

### Targets()


```csharp
ITarget[] Targets()
```


Returns An Array of all active targets.

**Returns:** `ITarget[]` — An Array of all active targets.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L239)

<a id="PuppeteerSharp_IBrowser_UninstallExtensionAsync_"></a>
<a id="PuppeteerSharp_IBrowser_UninstallExtensionAsync_System_String_"></a>

### UninstallExtensionAsync(string)


```csharp
Task UninstallExtensionAsync(string id)
```


Uninstalls a previously installed extension by its ID.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `id` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The extension ID to uninstall. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the extension is uninstalled.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L370)

<a id="PuppeteerSharp_IBrowser_UninstallPWAAsync_"></a>
<a id="PuppeteerSharp_IBrowser_UninstallPWAAsync_PuppeteerSharp_UninstallPWAOptions_"></a>

### UninstallPWAAsync(UninstallPWAOptions)


```csharp
Task UninstallPWAAsync(UninstallPWAOptions options)
```


Uninstalls a previously installed Progressive Web App (PWA).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [UninstallPWAOptions](/api/PuppeteerSharp/UninstallPWAOptions/) | Options for uninstalling the PWA. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the PWA is uninstalled.

Only available over a pipe connection. See [InstallPWAAsync(InstallPWAOptions)](#PuppeteerSharp_IBrowser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L399)

<a id="PuppeteerSharp_IBrowser_UnregisterCustomQueryHandler_"></a>
<a id="PuppeteerSharp_IBrowser_UnregisterCustomQueryHandler_System_String_"></a>

### UnregisterCustomQueryHandler(string)


```csharp
void UnregisterCustomQueryHandler(string name)
```


Unregister a custom query handler.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The name of the query handler to unregistered. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L278)

<a id="PuppeteerSharp_IBrowser_WaitForTargetAsync_"></a>
<a id="PuppeteerSharp_IBrowser_WaitForTargetAsync_System_Func_PuppeteerSharp_ITarget_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

### `WaitForTargetAsync(Func<ITarget, bool>, WaitForOptions)`


```csharp
Task<ITarget> WaitForTargetAsync(Func<ITarget, bool> predicate, WaitForOptions options = null)
```


This searches for a target in this specific browser context.

&lt;example&gt;


```csharp
await page.EvaluateAsync("() => window.open('https://www.example.com/')");
var newWindowTarget = await browserContext.WaitForTargetAsync((target) => target.Url == "https://www.example.com/");
```


&lt;/example&gt;

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `predicate` | [Func&lt;ITarget, bool&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | A function to be run for every target. |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | options. |

**Returns:** [Task&lt;ITarget&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Resolves to the first target found that matches the predicate function.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L255)
