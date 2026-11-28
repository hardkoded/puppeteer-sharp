---
title: "IBrowserContext"
description: "BrowserContexts provide a way to operate multiple independent browser sessions. When a browser is launched, it has a single IBrowserContext used by default...."
editUrl: false
prev: false
next: false
sidebar:
  label: "IBrowserContext"
  hidden: true
---

<a id="PuppeteerSharp_IBrowserContext"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IBrowserContext : IDisposable, IAsyncDisposable
```


BrowserContexts provide a way to operate multiple independent browser sessions. When a browser is launched, it has
a single [IBrowserContext](./) used by default. The method [NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_NewPageAsync_PuppeteerSharp_CreatePageOptions_) creates a [IPage](/api/PuppeteerSharp/IPage/) in the default [IBrowserContext](./).

**Derived:** [BidiBrowserContext](/api/PuppeteerSharp.Bidi/BidiBrowserContext/), [BrowserContext](/api/PuppeteerSharp/BrowserContext/), [CdpBrowserContext](/api/PuppeteerSharp.Cdp/CdpBrowserContext/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L11)

## Properties

<a id="PuppeteerSharp_IBrowserContext_Browser_"></a>
<a id="PuppeteerSharp_IBrowserContext_Browser"></a>

### Browser


```csharp
IBrowser Browser { get; }
```


Gets the browser this browser context belongs to.

**Value:** [IBrowser](/api/PuppeteerSharp/IBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L74)

<a id="PuppeteerSharp_IBrowserContext_Id_"></a>
<a id="PuppeteerSharp_IBrowserContext_Id"></a>

### Id


```csharp
string Id { get; }
```


Browser Context Id.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L69)

<a id="PuppeteerSharp_IBrowserContext_IsClosed_"></a>
<a id="PuppeteerSharp_IBrowserContext_IsClosed"></a>

### IsClosed


```csharp
bool IsClosed { get; }
```


Whether this [IBrowserContext](IBrowserContext) is closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L79)

## Events

<a id="PuppeteerSharp_IBrowserContext_Console"></a>

### Console


```csharp
event EventHandler<ConsoleEventArgs> Console
```


Raised when JavaScript within any page or worker in the browser context calls one of the console API methods, e.g. `console.log` or `console.dir`.
Only messages from pages and workers that PuppeteerSharp has attached to are delivered, e.g. via `PageAsync()` or `PagesAsync()`. `Targets()` only lists targets and does not attach to them.
Service and shared workers are not attached automatically: their messages are only delivered after `WorkerAsync()` has been called for their target.
With WebDriver BiDi, service and shared worker console messages are not reported.

**Value:** [EventHandler&lt;ConsoleEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L19)

<a id="PuppeteerSharp_IBrowserContext_Request"></a>

### Request


```csharp
event EventHandler<RequestEventArgs> Request
```


Raised when any page in the browser context issues a request. Contains an [IRequest](/api/PuppeteerSharp/IRequest/) instance.
Use [IPage](/api/PuppeteerSharp/IPage/) or [IFrame](/api/PuppeteerSharp/IFrame/) from the request to identify the originating context.
Events are only delivered for pages that have an [IPage](/api/PuppeteerSharp/IPage/) instance, for example one created via [PagesAsync(bool)](#PuppeteerSharp_IBrowserContext_PagesAsync_System_Boolean_).
These listeners only observe requests. Do not call `Continue`, `Abort` or `Respond` from them. To intercept requests, use [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_) with an [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request) listener.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L64)

<a id="PuppeteerSharp_IBrowserContext_RequestFailed"></a>

### RequestFailed


```csharp
event EventHandler<RequestEventArgs> RequestFailed
```


Raised when a request issued by any page in the browser context fails, for example by timing out.
HTTP error responses, such as 404 or 503, complete with [RequestFinished](#PuppeteerSharp_IBrowserContext_RequestFinished) instead.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L51)

<a id="PuppeteerSharp_IBrowserContext_RequestFinished"></a>

### RequestFinished


```csharp
event EventHandler<RequestEventArgs> RequestFinished
```


Raised when a request issued by any page in the browser context finishes successfully.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L56)

<a id="PuppeteerSharp_IBrowserContext_RequestServedFromCache"></a>

### RequestServedFromCache


```csharp
event EventHandler<RequestEventArgs> RequestServedFromCache
```


Raised when a request issued by any page in the browser context is served from the memory cache.

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L39)

<a id="PuppeteerSharp_IBrowserContext_Response"></a>

### Response


```csharp
event EventHandler<ResponseCreatedEventArgs> Response
```


Raised when a response is received for a request issued by any page in the browser context.
Use [Request](/api/PuppeteerSharp/IResponse/#PuppeteerSharp_IResponse_Request) to access the originating request.

**Value:** [EventHandler&lt;ResponseCreatedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L45)

<a id="PuppeteerSharp_IBrowserContext_TargetChanged"></a>

### TargetChanged


```csharp
event EventHandler<TargetChangedArgs> TargetChanged
```


Raised when the url of a target changes

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L24)

<a id="PuppeteerSharp_IBrowserContext_TargetCreated"></a>

### TargetCreated


```csharp
event EventHandler<TargetChangedArgs> TargetCreated
```


Raised when a target is created, for example when a new page is opened by `window.open` [https://developer.mozilla.org/en-US/docs/Web/API/Window/open](https://developer.mozilla.org/en-US/docs/Web/API/Window/open) or [NewPageAsync(CreatePageOptions)](#PuppeteerSharp_IBrowserContext_NewPageAsync_PuppeteerSharp_CreatePageOptions_).

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L29)

<a id="PuppeteerSharp_IBrowserContext_TargetDestroyed"></a>

### TargetDestroyed


```csharp
event EventHandler<TargetChangedArgs> TargetDestroyed
```


Raised when a target is destroyed, for example when a page is closed

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L34)

## Methods

<a id="PuppeteerSharp_IBrowserContext_ClearPermissionOverridesAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_ClearPermissionOverridesAsync"></a>

### ClearPermissionOverridesAsync()


```csharp
Task ClearPermissionOverridesAsync()
```


Clears all permission overrides for the browser context.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L85)

<a id="PuppeteerSharp_IBrowserContext_CloseAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_CloseAsync"></a>

### CloseAsync()


```csharp
Task CloseAsync()
```


Closes the browser context. All the targets that belong to the browser context will be closed.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L118)

<a id="PuppeteerSharp_IBrowserContext_DeleteCookieAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_DeleteCookieAsync_PuppeteerSharp_CookieParam___"></a>

### DeleteCookieAsync(params CookieParam[])


```csharp
Task DeleteCookieAsync(params CookieParam[] cookies)
```


Removes cookies from the browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieParam[]` | Complete [CookieParam](/api/PuppeteerSharp/CookieParam/) objects to be removed. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L105)

<a id="PuppeteerSharp_IBrowserContext_DeleteMatchingCookiesAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_DeleteMatchingCookiesAsync_PuppeteerSharp_DeleteCookiesRequest___"></a>

### DeleteMatchingCookiesAsync(params DeleteCookiesRequest[])


```csharp
Task DeleteMatchingCookiesAsync(params DeleteCookiesRequest[] filters)
```


Deletes cookies matching the provided filters in this browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `filters` | `DeleteCookiesRequest[]` | Filters to match cookies against. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L112)

<a id="PuppeteerSharp_IBrowserContext_GetCookiesAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_GetCookiesAsync"></a>

### GetCookiesAsync()


```csharp
Task<CookieParam[]> GetCookiesAsync()
```


Gets all cookies in the browser context.

**Returns:** [Task&lt;CookieParam\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of [CookieParam](/api/PuppeteerSharp/CookieParam/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L91)

<a id="PuppeteerSharp_IBrowserContext_NewPageAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_NewPageAsync_PuppeteerSharp_CreatePageOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L125)

<a id="PuppeteerSharp_IBrowserContext_OverridePermissionsAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_OverridePermissionsAsync_System_String_System_Collections_Generic_IEnumerable_PuppeteerSharp_OverridePermission__"></a>

### `OverridePermissionsAsync(string, IEnumerable<OverridePermission>)`


```csharp
Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions)
```


Overrides the browser context permissions.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `origin` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The origin to grant permissions to, e.g. "https://example.com". |
| `permissions` | [IEnumerable&lt;OverridePermission&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1) | An array of permissions to grant. All permissions that are not listed here will be automatically denied. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

**Example**

var context = browser.DefaultBrowserContext;
await context.OverridePermissionsAsync("https://html5demos.com", new List&lt;string&gt; {"geolocation"});

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L142)

<a id="PuppeteerSharp_IBrowserContext_PagesAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_PagesAsync_System_Boolean_"></a>

### PagesAsync(bool)


```csharp
Task<IPage[]> PagesAsync(bool includeAll = false)
```


An array of all pages inside the browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `includeAll` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Experimental. When true, includes all kinds of pages. |

**Returns:** [Task&lt;IPage\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of all open pages.
    Non visible pages, such as `"background_page"`, will not be listed here.
    You can find them using [PageAsync()](/api/PuppeteerSharp/ITarget/#PuppeteerSharp_ITarget_PageAsync).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L159)

<a id="PuppeteerSharp_IBrowserContext_SetCookieAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_SetCookieAsync_PuppeteerSharp_CookieData___"></a>

### SetCookieAsync(params CookieData[])


```csharp
Task SetCookieAsync(params CookieData[] cookies)
```


Sets cookies in the browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieData[]` | Cookies to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L98)

<a id="PuppeteerSharp_IBrowserContext_SetPermissionAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_SetPermissionAsync_System_String_PuppeteerSharp_PermissionEntry___"></a>

### SetPermissionAsync(string, params PermissionEntry[])


```csharp
Task SetPermissionAsync(string origin, params PermissionEntry[] permissions)
```


Sets the permission for a specific origin.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `origin` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The origin to set the permission for, e.g. "https://example.com". Use "*" for all origins (CDP only). |
| `permissions` | `PermissionEntry[]` | The permissions to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L150)

<a id="PuppeteerSharp_IBrowserContext_Targets_"></a>
<a id="PuppeteerSharp_IBrowserContext_Targets"></a>

### Targets()


```csharp
ITarget[] Targets()
```


Gets an array of all active targets inside the browser context.

**Returns:** `ITarget[]` — An array of all active targets inside the browser context.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L165)

<a id="PuppeteerSharp_IBrowserContext_WaitForTargetAsync_"></a>
<a id="PuppeteerSharp_IBrowserContext_WaitForTargetAsync_System_Func_PuppeteerSharp_ITarget_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L181)
