---
title: "BidiBrowser"
description: "Represents a browser connected using the Bidi protocol."
editUrl: false
prev: false
next: false
sidebar:
  label: "BidiBrowser"
  hidden: true
---

<a id="PuppeteerSharp_Bidi_BidiBrowser"></a>

Namespace: [PuppeteerSharp.Bidi](/api/PuppeteerSharp.Bidi/)


```csharp
public class BidiBrowser : Browser, IBrowser, IDisposable, IAsyncDisposable
```


Represents a browser connected using the Bidi protocol.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Browser](/api/PuppeteerSharp/Browser/)

**Implements:** [IBrowser](/api/PuppeteerSharp/IBrowser/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L42)

## Properties

<a id="PuppeteerSharp_Bidi_BidiBrowser_DebugInfo_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_DebugInfo"></a>

### DebugInfo


```csharp
public override DebugInfo DebugInfo { get; }
```


Get debug information from Puppeteer.

**Value:** [DebugInfo](/api/PuppeteerSharp/DebugInfo/)

Currently, includes pending protocol calls. In the future, more info might be added.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L81)

<a id="PuppeteerSharp_Bidi_BidiBrowser_DefaultContext_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_DefaultContext"></a>

### DefaultContext


```csharp
public override IBrowserContext DefaultContext { get; }
```


Returns the default browser context. The default browser context can not be closed.

**Value:** [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/) — The default context.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L78)

<a id="PuppeteerSharp_Bidi_BidiBrowser_IsClosed_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_IsClosed"></a>

### IsClosed


```csharp
public override bool IsClosed { get; }
```


Gets a value indicating if the browser is closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L66)

<a id="PuppeteerSharp_Bidi_BidiBrowser_IsConnected_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_IsConnected"></a>

### IsConnected


```csharp
public override bool IsConnected { get; }
```


Indicates that the browser is connected.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L69)

<a id="PuppeteerSharp_Bidi_BidiBrowser_Target_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_Target"></a>

### Target


```csharp
public override ITarget Target { get; }
```


A target associated with the browser.

**Value:** [ITarget](/api/PuppeteerSharp/ITarget/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L75)

<a id="PuppeteerSharp_Bidi_BidiBrowser_WebSocketEndpoint_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_WebSocketEndpoint"></a>

### WebSocketEndpoint


```csharp
public override string WebSocketEndpoint { get; }
```


Gets the Browser websocket url.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

Browser websocket endpoint which can be used as an argument to `ILoggerFactory)`.
The format is `ws://${host}:${port}/devtools/browser/[id]`
You can find the `webSocketDebuggerUrl` from `http://${host}:${port}/json/version`.
Learn more about the devtools protocol &lt;a href="https://chromedevtools.github.io/devtools-protocol"&gt;https://chromedevtools.github.io/devtools-protocol&lt;/a&gt;
and the browser endpoint &lt;a href="https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target"&gt;https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target&lt;/a&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L72)

## Methods

<a id="PuppeteerSharp_Bidi_BidiBrowser_AddScreenAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_AddScreenAsync_PuppeteerSharp_AddScreenParams_"></a>

### AddScreenAsync(AddScreenParams)


```csharp
public override Task<ScreenInfo> AddScreenAsync(AddScreenParams @params)
```


Adds a new screen with specified parameters and returns a [ScreenInfo](/api/PuppeteerSharp/ScreenInfo/) object, including the new screen's ID.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `params` | [AddScreenParams](/api/PuppeteerSharp/AddScreenParams/) | The parameters for the new screen. |

**Returns:** [Task&lt;ScreenInfo&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the added screen information.

Only supported in headless mode.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L191)

<a id="PuppeteerSharp_Bidi_BidiBrowser_BrowserContexts_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_BrowserContexts"></a>

### BrowserContexts()


```csharp
public override IBrowserContext[] BrowserContexts()
```


Returns an array of all open [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/). In a newly created browser, this will return a single instance of [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/).

**Returns:** `IBrowserContext\[\]` — An array of [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/) objects.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L250)

<a id="PuppeteerSharp_Bidi_BidiBrowser_CloseAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_CloseAsync"></a>

### CloseAsync()


```csharp
public override Task CloseAsync()
```


Closes Chromium and all of its pages (if any were opened). The browser object itself is considered disposed and cannot be used anymore.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L135)

<a id="PuppeteerSharp_Bidi_BidiBrowser_CreateBrowserContextAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_CreateBrowserContextAsync_PuppeteerSharp_BrowserContextOptions_"></a>

### CreateBrowserContextAsync(BrowserContextOptions)


```csharp
public override Task<IBrowserContext> CreateBrowserContextAsync(BrowserContextOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L243)

<a id="PuppeteerSharp_Bidi_BidiBrowser_Disconnect_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_Disconnect"></a>

### Disconnect()


```csharp
public override void Disconnect()
```


Disconnects Puppeteer from the browser, but leaves the process running. After calling [Disconnect()](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_Disconnect), the browser object is considered disposed and cannot be used anymore.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L128)

<a id="PuppeteerSharp_Bidi_BidiBrowser_ExtensionsAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_ExtensionsAsync"></a>

### ExtensionsAsync()


```csharp
public override Task<IReadOnlyDictionary<string, Extension>> ExtensionsAsync()
```


Returns a map of installed extensions, keyed by extension ID.

**Returns:** [Task&lt;IReadOnlyDictionary&lt;string, Extension&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to a dictionary of extension ID to [Extension](/api/PuppeteerSharp/Extension/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L215)

<a id="PuppeteerSharp_Bidi_BidiBrowser_GetPWAStateAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_GetPWAStateAsync_PuppeteerSharp_GetPWAStateOptions_"></a>

### GetPWAStateAsync(GetPWAStateOptions)


```csharp
public override Task<PWAState> GetPWAStateAsync(GetPWAStateOptions options)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L231)

<a id="PuppeteerSharp_Bidi_BidiBrowser_GetUserAgentAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_GetUserAgentAsync"></a>

### GetUserAgentAsync()


```csharp
public override Task<string> GetUserAgentAsync()
```


Gets the browser's original user agent.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the browser's original user agent.

Pages can override browser user agent with `UserAgentMetadata)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L125)

<a id="PuppeteerSharp_Bidi_BidiBrowser_GetVersionAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_GetVersionAsync"></a>

### GetVersionAsync()


```csharp
public override Task<string> GetVersionAsync()
```


Gets the browser's version.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — For headless Chromium, this is similar to `HeadlessChrome/61.0.3153.0`. For non-headless, this is similar to `Chrome/61.0.3153.0`.

the format of [GetVersionAsync()](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_GetVersionAsync) might change with future releases of Chromium.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L122)

<a id="PuppeteerSharp_Bidi_BidiBrowser_GetWindowBoundsAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_GetWindowBoundsAsync_System_String_"></a>

### GetWindowBoundsAsync(string)


```csharp
public override Task<WindowBounds> GetWindowBoundsAsync(string windowId)
```


Gets the specified window bounds.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `windowId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The window ID. |

**Returns:** [Task&lt;WindowBounds&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the [WindowBounds](/api/PuppeteerSharp/WindowBounds/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L258)

<a id="PuppeteerSharp_Bidi_BidiBrowser_InstallExtensionAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_InstallExtensionAsync_System_String_PuppeteerSharp_ExtensionInstallOptions_"></a>

### InstallExtensionAsync(string, ExtensionInstallOptions)


```csharp
public override Task<string> InstallExtensionAsync(string path, ExtensionInstallOptions options = null)
```


Installs an unpacked extension and returns the extension ID.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `path` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The path to the unpacked extension directory. |
| `options` | [ExtensionInstallOptions](/api/PuppeteerSharp/ExtensionInstallOptions/) | Options for installing the extension. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the extension ID.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L199)

<a id="PuppeteerSharp_Bidi_BidiBrowser_InstallPWAAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_"></a>

### InstallPWAAsync(InstallPWAOptions)


```csharp
public override Task<string> InstallPWAAsync(InstallPWAOptions options)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L219)

<a id="PuppeteerSharp_Bidi_BidiBrowser_LaunchPWAAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_LaunchPWAAsync_PuppeteerSharp_LaunchPWAOptions_"></a>

### LaunchPWAAsync(LaunchPWAOptions)


```csharp
public override Task<IPage> LaunchPWAAsync(LaunchPWAOptions options)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L227)

<a id="PuppeteerSharp_Bidi_BidiBrowser_NewPageAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_NewPageAsync_PuppeteerSharp_CreatePageOptions_"></a>

### NewPageAsync(CreatePageOptions)


```csharp
public override Task<IPage> NewPageAsync(CreatePageOptions options = null)
```


Creates a new page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [CreatePageOptions](/api/PuppeteerSharp/CreatePageOptions/) | Options for creating the page. |

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a new [IPage](/api/PuppeteerSharp/IPage/) object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L184)

<a id="PuppeteerSharp_Bidi_BidiBrowser_RemoveScreenAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_RemoveScreenAsync_System_String_"></a>

### RemoveScreenAsync(string)


```csharp
public override Task RemoveScreenAsync(string screenId)
```


Removes the screen associated with the given screen ID, unless it is the primary screen.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `screenId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The ID of the screen to remove. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the screen is removed.

Only supported in headless mode. Fails if the primary screen ID is specified.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L195)

<a id="PuppeteerSharp_Bidi_BidiBrowser_ScreensAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_ScreensAsync"></a>

### ScreensAsync()


```csharp
public override Task<ScreenInfo[]> ScreensAsync()
```


Gets a list of [ScreenInfo](/api/PuppeteerSharp/ScreenInfo/) objects.

**Returns:** [Task&lt;ScreenInfo\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to an array of screen information objects.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L187)

<a id="PuppeteerSharp_Bidi_BidiBrowser_SetWindowBoundsAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_SetWindowBoundsAsync_System_String_PuppeteerSharp_WindowBounds_"></a>

### SetWindowBoundsAsync(string, WindowBounds)


```csharp
public override Task SetWindowBoundsAsync(string windowId, WindowBounds windowBounds)
```


Sets the specified window bounds.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `windowId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The window ID. |
| `windowBounds` | [WindowBounds](/api/PuppeteerSharp/WindowBounds/) | The bounds to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the bounds have been set.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L275)

<a id="PuppeteerSharp_Bidi_BidiBrowser_Targets_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_Targets"></a>

### Targets()


```csharp
public override ITarget[] Targets()
```


Returns An Array of all active targets.

**Returns:** `ITarget\[\]` — An Array of all active targets.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L235)

<a id="PuppeteerSharp_Bidi_BidiBrowser_UninstallExtensionAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_UninstallExtensionAsync_System_String_"></a>

### UninstallExtensionAsync(string)


```csharp
public override Task UninstallExtensionAsync(string id)
```


Uninstalls a previously installed extension by its ID.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `id` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The extension ID to uninstall. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the extension is uninstalled.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L208)

<a id="PuppeteerSharp_Bidi_BidiBrowser_UninstallPWAAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowser_UninstallPWAAsync_PuppeteerSharp_UninstallPWAOptions_"></a>

### UninstallPWAAsync(UninstallPWAOptions)


```csharp
public override Task UninstallPWAAsync(UninstallPWAOptions options)
```


Uninstalls a previously installed Progressive Web App (PWA).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [UninstallPWAOptions](/api/PuppeteerSharp/UninstallPWAOptions/) | Options for uninstalling the PWA. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the PWA is uninstalled.

Only available over a pipe connection. See [InstallPWAAsync(InstallPWAOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowser.cs#L223)
