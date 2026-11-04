---
title: "BrowserContext"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "BrowserContext"
  hidden: true
---

<a id="PuppeteerSharp_BrowserContext"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class BrowserContext : IBrowserContext, IDisposable, IAsyncDisposable
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

**Derived:** [BidiBrowserContext](/api/PuppeteerSharp.Bidi/BidiBrowserContext/), [CdpBrowserContext](/api/PuppeteerSharp.Cdp/CdpBrowserContext/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L11)

## Properties

<a id="PuppeteerSharp_BrowserContext_Browser_"></a>
<a id="PuppeteerSharp_BrowserContext_Browser"></a>

### Browser


```csharp
public Browser Browser { get; protected init; }
```


**Value:** [Browser](/api/PuppeteerSharp/Browser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L32)

<a id="PuppeteerSharp_BrowserContext_Id_"></a>
<a id="PuppeteerSharp_BrowserContext_Id"></a>

### Id


```csharp
public string Id { get; protected init; }
```


Browser Context Id.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L26)

<a id="PuppeteerSharp_BrowserContext_IsClosed_"></a>
<a id="PuppeteerSharp_BrowserContext_IsClosed"></a>

### IsClosed


```csharp
public bool IsClosed { get; }
```


Whether this &lt;a href="IBrowserContext"&gt;IBrowserContext&lt;/a&gt; is closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L29)

## Events

<a id="PuppeteerSharp_BrowserContext_TargetChanged"></a>

### TargetChanged


```csharp
public event EventHandler<TargetChangedArgs> TargetChanged
```


Raised when the url of a target changes

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L17)

<a id="PuppeteerSharp_BrowserContext_TargetCreated"></a>

### TargetCreated


```csharp
public event EventHandler<TargetChangedArgs> TargetCreated
```


Raised when a target is created, for example when a new page is opened by `window.open` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/Window/open"&gt;https://developer.mozilla.org/en-US/docs/Web/API/Window/open&lt;/a&gt; or [NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowserContext/#PuppeteerSharp_IBrowserContext_NewPageAsync_PuppeteerSharp_CreatePageOptions_).

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L20)

<a id="PuppeteerSharp_BrowserContext_TargetDestroyed"></a>

### TargetDestroyed


```csharp
public event EventHandler<TargetChangedArgs> TargetDestroyed
```


Raised when a target is destroyed, for example when a page is closed

**Value:** [EventHandler&lt;TargetChangedArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L23)

## Methods

<a id="PuppeteerSharp_BrowserContext_ClearPermissionOverridesAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_ClearPermissionOverridesAsync"></a>

### ClearPermissionOverridesAsync()


```csharp
public abstract Task ClearPermissionOverridesAsync()
```


Clears all permission overrides for the browser context.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L60)

<a id="PuppeteerSharp_BrowserContext_CloseAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_CloseAsync"></a>

### CloseAsync()


```csharp
public abstract Task CloseAsync()
```


Closes the browser context. All the targets that belong to the browser context will be closed.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L51)

<a id="PuppeteerSharp_BrowserContext_DeleteCookieAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_DeleteCookieAsync_PuppeteerSharp_CookieParam___"></a>

### DeleteCookieAsync(params CookieParam[])


```csharp
public Task DeleteCookieAsync(params CookieParam[] cookies)
```


Removes cookies from the browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieParam\[\]` | Complete [CookieParam](/api/PuppeteerSharp/CookieParam/) objects to be removed. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L69)

<a id="PuppeteerSharp_BrowserContext_DeleteMatchingCookiesAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_DeleteMatchingCookiesAsync_PuppeteerSharp_DeleteCookiesRequest___"></a>

### DeleteMatchingCookiesAsync(params DeleteCookiesRequest[])


```csharp
public Task DeleteMatchingCookiesAsync(params DeleteCookiesRequest[] filters)
```


Deletes cookies matching the provided filters in this browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `filters` | `DeleteCookiesRequest\[\]` | Filters to match cookies against. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L89)

<a id="PuppeteerSharp_BrowserContext_Dispose_"></a>
<a id="PuppeteerSharp_BrowserContext_Dispose"></a>

### Dispose()


```csharp
public void Dispose()
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L142)

<a id="PuppeteerSharp_BrowserContext_Dispose_System_Boolean_"></a>

### Dispose(bool)


```csharp
protected virtual void Dispose(bool disposing)
```


Closes the browser context. All the targets that belong to the browser context will be closed.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `disposing` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Indicates whether disposal was initiated by [Dispose()](#PuppeteerSharp_BrowserContext_Dispose) operation. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L204)

<a id="PuppeteerSharp_BrowserContext_DisposeAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_DisposeAsync"></a>

### DisposeAsync()


```csharp
public ValueTask DisposeAsync()
```


Closes the browser context. All the targets that belong to the browser context will be closed.

**Returns:** [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask) — ValueTask.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L152)

<a id="PuppeteerSharp_BrowserContext_GetCookiesAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_GetCookiesAsync"></a>

### GetCookiesAsync()


```csharp
public abstract Task<CookieParam[]> GetCookiesAsync()
```


Gets all cookies in the browser context.

**Returns:** [Task&lt;CookieParam\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of [CookieParam](/api/PuppeteerSharp/CookieParam/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L63)

<a id="PuppeteerSharp_BrowserContext_NewPageAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_NewPageAsync_PuppeteerSharp_CreatePageOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L48)

<a id="PuppeteerSharp_BrowserContext_OverridePermissionsAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_OverridePermissionsAsync_System_String_System_Collections_Generic_IEnumerable_PuppeteerSharp_OverridePermission__"></a>

### `OverridePermissionsAsync(string, IEnumerable<OverridePermission>)`


```csharp
public abstract Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L54)

<a id="PuppeteerSharp_BrowserContext_PagesAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_PagesAsync_System_Boolean_"></a>

### PagesAsync(bool)


```csharp
public abstract Task<IPage[]> PagesAsync(bool includeAll = false)
```


An array of all pages inside the browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `includeAll` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Experimental. When true, includes all kinds of pages. |

**Returns:** [Task&lt;IPage\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of all open pages.
    Non visible pages, such as `"background_page"`, will not be listed here.
    You can find them using [PageAsync()](/api/PuppeteerSharp/ITarget/#PuppeteerSharp_ITarget_PageAsync).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L45)

<a id="PuppeteerSharp_BrowserContext_SetCookieAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_SetCookieAsync_PuppeteerSharp_CookieData___"></a>

### SetCookieAsync(params CookieData[])


```csharp
public abstract Task SetCookieAsync(params CookieData[] cookies)
```


Sets cookies in the browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieData\[\]` | Cookies to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L66)

<a id="PuppeteerSharp_BrowserContext_SetPermissionAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_SetPermissionAsync_System_String_PuppeteerSharp_PermissionEntry___"></a>

### SetPermissionAsync(string, params PermissionEntry[])


```csharp
public abstract Task SetPermissionAsync(string origin, params PermissionEntry[] permissions)
```


Sets the permission for a specific origin.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `origin` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The origin to set the permission for, e.g. "https://example.com". Use "*" for all origins (CDP only). |
| `permissions` | `PermissionEntry\[\]` | The permissions to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L57)

<a id="PuppeteerSharp_BrowserContext_Targets_"></a>
<a id="PuppeteerSharp_BrowserContext_Targets"></a>

### Targets()


```csharp
public abstract ITarget[] Targets()
```


Gets an array of all active targets inside the browser context.

**Returns:** `ITarget\[\]` — An array of all active targets inside the browser context.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L38)

<a id="PuppeteerSharp_BrowserContext_WaitForTargetAsync_"></a>
<a id="PuppeteerSharp_BrowserContext_WaitForTargetAsync_System_Func_PuppeteerSharp_ITarget_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContext.cs#L41)
