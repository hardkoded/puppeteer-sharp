---
title: "Browser"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Browser"
  hidden: true
---

<a id="PuppeteerSharp_Browser"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class Browser : IBrowser, IDisposable, IAsyncDisposable
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IBrowser](/api/PuppeteerSharp/IBrowser/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

**Derived:** [BidiBrowser](/api/PuppeteerSharp.Bidi/BidiBrowser/), [CdpBrowser](/api/PuppeteerSharp.Cdp/CdpBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L23)

## Properties

<a id="PuppeteerSharp_Browser_BrowserType_"></a>
<a id="PuppeteerSharp_Browser_BrowserType"></a>

### BrowserType


```csharp
public SupportedBrowser BrowserType { get; protected init; }
```


Returns the browser type. Chrome, Chromium or Firefox.

**Value:** [SupportedBrowser](/api/PuppeteerSharp/SupportedBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L48)

<a id="PuppeteerSharp_Browser_DebugInfo_"></a>
<a id="PuppeteerSharp_Browser_DebugInfo"></a>

### DebugInfo


```csharp
public abstract DebugInfo DebugInfo { get; }
```


Get debug information from Puppeteer.

**Value:** [DebugInfo](/api/PuppeteerSharp/DebugInfo/)

Currently, includes pending protocol calls. In the future, more info might be added.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L66)

<a id="PuppeteerSharp_Browser_DefaultContext_"></a>
<a id="PuppeteerSharp_Browser_DefaultContext"></a>

### DefaultContext


```csharp
public virtual IBrowserContext DefaultContext { get; protected set; }
```


Returns the default browser context. The default browser context can not be closed.

**Value:** [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/) — The default context.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L57)

<a id="PuppeteerSharp_Browser_DefaultWaitForTimeout_"></a>
<a id="PuppeteerSharp_Browser_DefaultWaitForTimeout"></a>

### DefaultWaitForTimeout


```csharp
public int DefaultWaitForTimeout { get; set; }
```


Default wait time in milliseconds. Defaults to 30 seconds.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L60)

<a id="PuppeteerSharp_Browser_IsClosed_"></a>
<a id="PuppeteerSharp_Browser_IsClosed"></a>

### IsClosed


```csharp
public abstract bool IsClosed { get; }
```


Gets a value indicating if the browser is closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L54)

<a id="PuppeteerSharp_Browser_IsConnected_"></a>
<a id="PuppeteerSharp_Browser_IsConnected"></a>

### IsConnected


```csharp
public virtual bool IsConnected { get; }
```


Indicates that the browser is connected.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L63)

<a id="PuppeteerSharp_Browser_Process_"></a>
<a id="PuppeteerSharp_Browser_Process"></a>

### Process


```csharp
public Process Process { get; }
```


Gets the spawned browser process. Returns `null` if the browser instance was created with `ILoggerFactory)` method.

**Value:** [Process](https://learn.microsoft.com/dotnet/api/system.diagnostics.process)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L51)

<a id="PuppeteerSharp_Browser_Target_"></a>
<a id="PuppeteerSharp_Browser_Target"></a>

### Target


```csharp
public virtual ITarget Target { get; }
```


A target associated with the browser.

**Value:** [ITarget](/api/PuppeteerSharp/ITarget/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L69)

<a id="PuppeteerSharp_Browser_WebSocketEndpoint_"></a>
<a id="PuppeteerSharp_Browser_WebSocketEndpoint"></a>

### WebSocketEndpoint


```csharp
public virtual string WebSocketEndpoint { get; }
```


Gets the Browser websocket url.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

Browser websocket endpoint which can be used as an argument to `ILoggerFactory)`.
The format is `ws://${host}:${port}/devtools/browser/[id]`
You can find the `webSocketDebuggerUrl` from `http://${host}:${port}/json/version`.
Learn more about the devtools protocol &lt;a href="https://chromedevtools.github.io/devtools-protocol"&gt;https://chromedevtools.github.io/devtools-protocol&lt;/a&gt;
and the browser endpoint &lt;a href="https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target"&gt;https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target&lt;/a&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L45)

## Events

<a id="PuppeteerSharp_Browser_Closed"></a>

### Closed


```csharp
public event EventHandler Closed
```


Raised when the [IBrowser](/api/PuppeteerSharp/IBrowser/) gets closed.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L27)

<a id="PuppeteerSharp_Browser_Disconnected"></a>

### Disconnected


```csharp
public event EventHandler Disconnected
```


Raised when puppeteer gets disconnected from the Chromium instance. This might happen because one of the following
- Chromium is closed or crashed
- [Disconnect()](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_Disconnect) method was called

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L30)

<a id="PuppeteerSharp_Browser_TargetChanged"></a>

### TargetChanged


```csharp
public event EventHandler<TargetChangedArgs> TargetChanged
```


Raised when the url of a target changes

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L33)

<a id="PuppeteerSharp_Browser_TargetCreated"></a>

### TargetCreated


```csharp
public event EventHandler<TargetChangedArgs> TargetCreated
```


Raised when a target is created, for example when a new page is opened by `window.open` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/Window/open"&gt;https://developer.mozilla.org/en-US/docs/Web/API/Window/open&lt;/a&gt; or [NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_NewPageAsync_PuppeteerSharp_CreatePageOptions_).

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L36)

<a id="PuppeteerSharp_Browser_TargetDestroyed"></a>

### TargetDestroyed


```csharp
public event EventHandler<TargetChangedArgs> TargetDestroyed
```


Raised when a target is destroyed, for example when a page is closed

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L39)

<a id="PuppeteerSharp_Browser_TargetDiscovered"></a>

### TargetDiscovered


```csharp
public event EventHandler<TargetChangedArgs> TargetDiscovered
```


Raised when a target is discovered

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L42)

## Methods

<a id="PuppeteerSharp_Browser_AddScreenAsync_"></a>
<a id="PuppeteerSharp_Browser_AddScreenAsync_PuppeteerSharp_AddScreenParams_"></a>

### AddScreenAsync(AddScreenParams)


```csharp
public abstract Task<ScreenInfo> AddScreenAsync(AddScreenParams @params)
```


Adds a new screen with specified parameters and returns a [ScreenInfo](/api/PuppeteerSharp/ScreenInfo/) object, including the new screen's ID.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `params` | [AddScreenParams](/api/PuppeteerSharp/AddScreenParams/) | The parameters for the new screen. |

**Returns:** [Task&lt;ScreenInfo&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the added screen information.

Only supported in headless mode.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L101)

<a id="PuppeteerSharp_Browser_BrowserContexts_"></a>
<a id="PuppeteerSharp_Browser_BrowserContexts"></a>

### BrowserContexts()


```csharp
public abstract IBrowserContext[] BrowserContexts()
```


Returns an array of all open [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/). In a newly created browser, this will return a single instance of [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/).

**Returns:** `IBrowserContext\[\]` — An array of [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/) objects.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L95)

<a id="PuppeteerSharp_Browser_ClearCustomQueryHandlers_"></a>
<a id="PuppeteerSharp_Browser_ClearCustomQueryHandlers"></a>

### ClearCustomQueryHandlers()


```csharp
public void ClearCustomQueryHandlers()
```


Clears all registered handlers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L208)

<a id="PuppeteerSharp_Browser_CloseAsync_"></a>
<a id="PuppeteerSharp_Browser_CloseAsync"></a>

### CloseAsync()


```csharp
public abstract Task CloseAsync()
```


Closes Chromium and all of its pages (if any were opened). The browser object itself is considered disposed and cannot be used anymore.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L147)

<a id="PuppeteerSharp_Browser_CreateBrowserContextAsync_"></a>
<a id="PuppeteerSharp_Browser_CreateBrowserContextAsync_PuppeteerSharp_BrowserContextOptions_"></a>

### CreateBrowserContextAsync(BrowserContextOptions)


```csharp
public abstract Task<IBrowserContext> CreateBrowserContextAsync(BrowserContextOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L92)

<a id="PuppeteerSharp_Browser_CreateCDPSessionAsync_"></a>
<a id="PuppeteerSharp_Browser_CreateCDPSessionAsync"></a>

### CreateCDPSessionAsync()


```csharp
public Task<ICDPSession> CreateCDPSessionAsync()
```


Creates a Chrome Devtools Protocol session attached to the browser.

**Returns:** [Task&lt;ICDPSession&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [ICDPSession](/api/PuppeteerSharp/ICDPSession/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L232)

<a id="PuppeteerSharp_Browser_DeleteCookieAsync_"></a>
<a id="PuppeteerSharp_Browser_DeleteCookieAsync_PuppeteerSharp_CookieParam___"></a>

### DeleteCookieAsync(params CookieParam[])


```csharp
public Task DeleteCookieAsync(params CookieParam[] cookies)
```


Removes cookies from the default browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieParam\[\]` | Complete [CookieParam](/api/PuppeteerSharp/CookieParam/) objects to be removed. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L224)

<a id="PuppeteerSharp_Browser_DeleteMatchingCookiesAsync_"></a>
<a id="PuppeteerSharp_Browser_DeleteMatchingCookiesAsync_PuppeteerSharp_DeleteCookiesRequest___"></a>

### DeleteMatchingCookiesAsync(params DeleteCookiesRequest[])


```csharp
public Task DeleteMatchingCookiesAsync(params DeleteCookiesRequest[] filters)
```


Deletes cookies matching the provided filters from the default [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `filters` | `DeleteCookiesRequest\[\]` | Filters to match cookies against. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L228)

<a id="PuppeteerSharp_Browser_Disconnect_"></a>
<a id="PuppeteerSharp_Browser_Disconnect"></a>

### Disconnect()


```csharp
public abstract void Disconnect()
```


Disconnects Puppeteer from the browser, but leaves the process running. After calling [Disconnect()](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_Disconnect), the browser object is considered disposed and cannot be used anymore.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L144)

<a id="PuppeteerSharp_Browser_Dispose_"></a>
<a id="PuppeteerSharp_Browser_Dispose"></a>

### Dispose()


```csharp
public void Dispose()
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L241)

<a id="PuppeteerSharp_Browser_Dispose_System_Boolean_"></a>

### Dispose(bool)


```csharp
protected virtual void Dispose(bool disposing)
```


Closes `Connection` and any Chromium [Process](#PuppeteerSharp_Browser_Process) that was
created by Puppeteer.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `disposing` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Indicates whether disposal was initiated by [Dispose()](#PuppeteerSharp_Browser_Dispose) operation. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L281)

<a id="PuppeteerSharp_Browser_DisposeAsync_"></a>
<a id="PuppeteerSharp_Browser_DisposeAsync"></a>

### DisposeAsync()


```csharp
public ValueTask DisposeAsync()
```


Closes `Connection` and any Chromium [Process](#PuppeteerSharp_Browser_Process) that was
created by Puppeteer.

**Returns:** [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask) — ValueTask.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L252)

<a id="PuppeteerSharp_Browser_ExtensionsAsync_"></a>
<a id="PuppeteerSharp_Browser_ExtensionsAsync"></a>

### ExtensionsAsync()


```csharp
public abstract Task<IReadOnlyDictionary<string, Extension>> ExtensionsAsync()
```


Returns a map of installed extensions, keyed by extension ID.

**Returns:** [Task&lt;IReadOnlyDictionary&lt;string, Extension&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to a dictionary of extension ID to [Extension](/api/PuppeteerSharp/Extension/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L113)

<a id="PuppeteerSharp_Browser_GetCookiesAsync_"></a>
<a id="PuppeteerSharp_Browser_GetCookiesAsync"></a>

### GetCookiesAsync()


```csharp
public Task<CookieParam[]> GetCookiesAsync()
```


Gets all cookies in the default browser context.

**Returns:** [Task&lt;CookieParam\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of [CookieParam](/api/PuppeteerSharp/CookieParam/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L216)

<a id="PuppeteerSharp_Browser_GetPWAStateAsync_"></a>
<a id="PuppeteerSharp_Browser_GetPWAStateAsync_PuppeteerSharp_GetPWAStateOptions_"></a>

### GetPWAStateAsync(GetPWAStateOptions)


```csharp
public abstract Task<PWAState> GetPWAStateAsync(GetPWAStateOptions options)
```


Returns the OS-integration state of an installed Progressive Web App (PWA), such as its badge count and
registered file handlers.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [GetPWAStateOptions](/api/PuppeteerSharp/GetPWAStateOptions/) | Options for querying the PWA state. |

**Returns:** [Task&lt;PWAState&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the PWA state.

Only available over a pipe connection. See [InstallPWAAsync(InstallPWAOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_). Meaningful
only for an app that is currently installed; querying an unknown manifest id rejects.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L125)

<a id="PuppeteerSharp_Browser_GetUserAgentAsync_"></a>
<a id="PuppeteerSharp_Browser_GetUserAgentAsync"></a>

### GetUserAgentAsync()


```csharp
public abstract Task<string> GetUserAgentAsync()
```


Gets the browser's original user agent.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the browser's original user agent.

Pages can override browser user agent with `UserAgentMetadata)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L141)

<a id="PuppeteerSharp_Browser_GetVersionAsync_"></a>
<a id="PuppeteerSharp_Browser_GetVersionAsync"></a>

### GetVersionAsync()


```csharp
public abstract Task<string> GetVersionAsync()
```


Gets the browser's version.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — For headless Chromium, this is similar to `HeadlessChrome/61.0.3153.0`. For non-headless, this is similar to `Chrome/61.0.3153.0`.

the format of [GetVersionAsync()](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_GetVersionAsync) might change with future releases of Chromium.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L138)

<a id="PuppeteerSharp_Browser_GetWindowBoundsAsync_"></a>
<a id="PuppeteerSharp_Browser_GetWindowBoundsAsync_System_String_"></a>

### GetWindowBoundsAsync(string)


```csharp
public abstract Task<WindowBounds> GetWindowBoundsAsync(string windowId)
```


Gets the specified window bounds.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `windowId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The window ID. |

**Returns:** [Task&lt;WindowBounds&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the [WindowBounds](/api/PuppeteerSharp/WindowBounds/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L235)

<a id="PuppeteerSharp_Browser_InstallExtensionAsync_"></a>
<a id="PuppeteerSharp_Browser_InstallExtensionAsync_System_String_PuppeteerSharp_ExtensionInstallOptions_"></a>

### InstallExtensionAsync(string, ExtensionInstallOptions)


```csharp
public abstract Task<string> InstallExtensionAsync(string path, ExtensionInstallOptions options = null)
```


Installs an unpacked extension and returns the extension ID.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `path` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The path to the unpacked extension directory. |
| `options` | [ExtensionInstallOptions](/api/PuppeteerSharp/ExtensionInstallOptions/) | Options for installing the extension. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the extension ID.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L107)

<a id="PuppeteerSharp_Browser_InstallPWAAsync_"></a>
<a id="PuppeteerSharp_Browser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_"></a>

### InstallPWAAsync(InstallPWAOptions)


```csharp
public abstract Task<string> InstallPWAAsync(InstallPWAOptions options)
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
to [LaunchPWAAsync(LaunchPWAOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_LaunchPWAAsync_PuppeteerSharp_LaunchPWAOptions_), [GetPWAStateAsync(GetPWAStateOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_GetPWAStateAsync_PuppeteerSharp_GetPWAStateOptions_), or
[UninstallPWAAsync(UninstallPWAOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_UninstallPWAAsync_PuppeteerSharp_UninstallPWAOptions_).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L116)

<a id="PuppeteerSharp_Browser_LaunchPWAAsync_"></a>
<a id="PuppeteerSharp_Browser_LaunchPWAAsync_PuppeteerSharp_LaunchPWAOptions_"></a>

### LaunchPWAAsync(LaunchPWAOptions)


```csharp
public abstract Task<IPage> LaunchPWAAsync(LaunchPWAOptions options)
```


Launches an installed Progressive Web App (PWA) and resolves with the [IPage](/api/PuppeteerSharp/IPage/) backing the app window.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LaunchPWAOptions](/api/PuppeteerSharp/LaunchPWAOptions/) | Options for launching the PWA. |

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the page backing the app window.

Only available over a pipe connection. See [InstallPWAAsync(InstallPWAOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_).
`PWA.launch` resolves with the id of the launched *tab* target. Puppeteer does not expose tab
targets through [Targets()](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_Targets); this method instead resolves with the tab's child page target
(the app's web contents). If Chromium focuses an existing app window, this returns that window's
existing page.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L122)

<a id="PuppeteerSharp_Browser_NewPageAsync_"></a>
<a id="PuppeteerSharp_Browser_NewPageAsync_PuppeteerSharp_CreatePageOptions_"></a>

### NewPageAsync(CreatePageOptions)


```csharp
public abstract Task<IPage> NewPageAsync(CreatePageOptions options = null)
```


Creates a new page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [CreatePageOptions](/api/PuppeteerSharp/CreatePageOptions/) | Options for creating the page. |

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a new [IPage](/api/PuppeteerSharp/IPage/) object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L86)

<a id="PuppeteerSharp_Browser_OnClosed_"></a>
<a id="PuppeteerSharp_Browser_OnClosed"></a>

### OnClosed()


```csharp
protected void OnClosed()
```


Emits [Closed](#PuppeteerSharp_Browser_Closed) event.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L289)

<a id="PuppeteerSharp_Browser_OnDisconnected_"></a>
<a id="PuppeteerSharp_Browser_OnDisconnected"></a>

### OnDisconnected()


```csharp
protected void OnDisconnected()
```


Emits [Disconnected](#PuppeteerSharp_Browser_Disconnected) event.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L294)

<a id="PuppeteerSharp_Browser_OnTargetChanged_"></a>
<a id="PuppeteerSharp_Browser_OnTargetChanged_PuppeteerSharp_TargetChangedArgs_"></a>

### OnTargetChanged(TargetChangedArgs)


```csharp
protected void OnTargetChanged(TargetChangedArgs e)
```


Emits [TargetChanged](#PuppeteerSharp_Browser_TargetChanged) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `e` | [TargetChangedArgs](/api/PuppeteerSharp/TargetChangedArgs/) | The event arguments. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L300)

<a id="PuppeteerSharp_Browser_OnTargetCreated_"></a>
<a id="PuppeteerSharp_Browser_OnTargetCreated_PuppeteerSharp_TargetChangedArgs_"></a>

### OnTargetCreated(TargetChangedArgs)


```csharp
protected void OnTargetCreated(TargetChangedArgs e)
```


Emits [TargetCreated](#PuppeteerSharp_Browser_TargetCreated) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `e` | [TargetChangedArgs](/api/PuppeteerSharp/TargetChangedArgs/) | The event arguments. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L306)

<a id="PuppeteerSharp_Browser_OnTargetDestroyed_"></a>
<a id="PuppeteerSharp_Browser_OnTargetDestroyed_PuppeteerSharp_TargetChangedArgs_"></a>

### OnTargetDestroyed(TargetChangedArgs)


```csharp
protected void OnTargetDestroyed(TargetChangedArgs e)
```


Emits [TargetDestroyed](#PuppeteerSharp_Browser_TargetDestroyed) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `e` | [TargetChangedArgs](/api/PuppeteerSharp/TargetChangedArgs/) | The event arguments. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L312)

<a id="PuppeteerSharp_Browser_OnTargetDiscovered_"></a>
<a id="PuppeteerSharp_Browser_OnTargetDiscovered_PuppeteerSharp_TargetChangedArgs_"></a>

### OnTargetDiscovered(TargetChangedArgs)


```csharp
protected void OnTargetDiscovered(TargetChangedArgs e)
```


Emits [TargetDiscovered](#PuppeteerSharp_Browser_TargetDiscovered) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `e` | [TargetChangedArgs](/api/PuppeteerSharp/TargetChangedArgs/) | The event arguments. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L318)

<a id="PuppeteerSharp_Browser_PagesAsync_"></a>
<a id="PuppeteerSharp_Browser_PagesAsync_System_Boolean_"></a>

### PagesAsync(bool)


```csharp
public Task<IPage[]> PagesAsync(bool includeAll = false)
```


Returns a Task which resolves to an array of all open pages.
Non visible pages, such as `"background_page"`, will not be listed here. You can find them using [PageAsync()](/api/PuppeteerSharp/Target/#PuppeteerSharp_Target_PageAsync).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `includeAll` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Experimental. When true, includes all kinds of pages. |

**Returns:** [Task&lt;IPage\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of all open pages inside the Browser.
    In case of multiple browser contexts, the method will return an array with all the pages in all browser contexts.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L132)

<a id="PuppeteerSharp_Browser_RegisterCustomQueryHandler_"></a>
<a id="PuppeteerSharp_Browser_RegisterCustomQueryHandler_System_String_PuppeteerSharp_CustomQueryHandler_"></a>

### RegisterCustomQueryHandler(string, CustomQueryHandler)


```csharp
public void RegisterCustomQueryHandler(string name, CustomQueryHandler queryHandler)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L188)

<a id="PuppeteerSharp_Browser_RemoveScreenAsync_"></a>
<a id="PuppeteerSharp_Browser_RemoveScreenAsync_System_String_"></a>

### RemoveScreenAsync(string)


```csharp
public abstract Task RemoveScreenAsync(string screenId)
```


Removes the screen associated with the given screen ID, unless it is the primary screen.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `screenId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The ID of the screen to remove. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the screen is removed.

Only supported in headless mode. Fails if the primary screen ID is specified.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L104)

<a id="PuppeteerSharp_Browser_RestrictNetworkAsync_"></a>
<a id="PuppeteerSharp_Browser_RestrictNetworkAsync_PuppeteerSharp_NetworkRestrictions_"></a>

### RestrictNetworkAsync(NetworkRestrictions)


```csharp
public virtual Task RestrictNetworkAsync(NetworkRestrictions restrictions = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L128)

<a id="PuppeteerSharp_Browser_ScreensAsync_"></a>
<a id="PuppeteerSharp_Browser_ScreensAsync"></a>

### ScreensAsync()


```csharp
public abstract Task<ScreenInfo[]> ScreensAsync()
```


Gets a list of [ScreenInfo](/api/PuppeteerSharp/ScreenInfo/) objects.

**Returns:** [Task&lt;ScreenInfo\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to an array of screen information objects.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L98)

<a id="PuppeteerSharp_Browser_SetCookieAsync_"></a>
<a id="PuppeteerSharp_Browser_SetCookieAsync_PuppeteerSharp_CookieData___"></a>

### SetCookieAsync(params CookieData[])


```csharp
public Task SetCookieAsync(params CookieData[] cookies)
```


Sets cookies in the default browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieData\[\]` | Cookies to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L220)

<a id="PuppeteerSharp_Browser_SetPermissionAsync_"></a>
<a id="PuppeteerSharp_Browser_SetPermissionAsync_System_String_PuppeteerSharp_PermissionEntry___"></a>

### SetPermissionAsync(string, params PermissionEntry[])


```csharp
public Task SetPermissionAsync(string origin, params PermissionEntry[] permissions)
```


Sets the permission for a specific origin on the default browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `origin` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The origin to set the permission for, e.g. "https://example.com". Use "*" for all origins (CDP only). |
| `permissions` | `PermissionEntry\[\]` | The permissions to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L212)

<a id="PuppeteerSharp_Browser_SetWindowBoundsAsync_"></a>
<a id="PuppeteerSharp_Browser_SetWindowBoundsAsync_System_String_PuppeteerSharp_WindowBounds_"></a>

### SetWindowBoundsAsync(string, WindowBounds)


```csharp
public abstract Task SetWindowBoundsAsync(string windowId, WindowBounds windowBounds)
```


Sets the specified window bounds.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `windowId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The window ID. |
| `windowBounds` | [WindowBounds](/api/PuppeteerSharp/WindowBounds/) | The bounds to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the bounds have been set.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L238)

<a id="PuppeteerSharp_Browser_Targets_"></a>
<a id="PuppeteerSharp_Browser_Targets"></a>

### Targets()


```csharp
public abstract ITarget[] Targets()
```


Returns An Array of all active targets.

**Returns:** `ITarget\[\]` — An Array of all active targets.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L89)

<a id="PuppeteerSharp_Browser_UninstallExtensionAsync_"></a>
<a id="PuppeteerSharp_Browser_UninstallExtensionAsync_System_String_"></a>

### UninstallExtensionAsync(string)


```csharp
public abstract Task UninstallExtensionAsync(string id)
```


Uninstalls a previously installed extension by its ID.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `id` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The extension ID to uninstall. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the extension is uninstalled.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L110)

<a id="PuppeteerSharp_Browser_UninstallPWAAsync_"></a>
<a id="PuppeteerSharp_Browser_UninstallPWAAsync_PuppeteerSharp_UninstallPWAOptions_"></a>

### UninstallPWAAsync(UninstallPWAOptions)


```csharp
public abstract Task UninstallPWAAsync(UninstallPWAOptions options)
```


Uninstalls a previously installed Progressive Web App (PWA).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [UninstallPWAOptions](/api/PuppeteerSharp/UninstallPWAOptions/) | Options for uninstalling the PWA. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the PWA is uninstalled.

Only available over a pipe connection. See [InstallPWAAsync(InstallPWAOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L119)

<a id="PuppeteerSharp_Browser_UnregisterCustomQueryHandler_"></a>
<a id="PuppeteerSharp_Browser_UnregisterCustomQueryHandler_System_String_"></a>

### UnregisterCustomQueryHandler(string)


```csharp
public void UnregisterCustomQueryHandler(string name)
```


Unregister a custom query handler.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The name of the query handler to unregistered. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L204)

<a id="PuppeteerSharp_Browser_WaitForTargetAsync_"></a>
<a id="PuppeteerSharp_Browser_WaitForTargetAsync_System_Func_PuppeteerSharp_ITarget_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

### `WaitForTargetAsync(Func<ITarget, bool>, WaitForOptions)`


```csharp
public Task<ITarget> WaitForTargetAsync(Func<ITarget, bool> predicate, WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Browser.cs#L150)
