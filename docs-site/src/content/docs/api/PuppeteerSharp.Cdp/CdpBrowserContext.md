---
title: "CdpBrowserContext"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpBrowserContext"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpBrowserContext"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpBrowserContext : BrowserContext, IBrowserContext, IDisposable, IAsyncDisposable
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [BrowserContext](/api/PuppeteerSharp/BrowserContext/)

**Implements:** [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpBrowserContext.cs#L32)

## Methods

<a id="PuppeteerSharp_Cdp_CdpBrowserContext_ClearPermissionOverridesAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpBrowserContext_ClearPermissionOverridesAsync"></a>

### ClearPermissionOverridesAsync()


```csharp
public override Task ClearPermissionOverridesAsync()
```


Clears all permission overrides for the browser context.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpBrowserContext.cs#L114)

<a id="PuppeteerSharp_Cdp_CdpBrowserContext_CloseAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpBrowserContext_CloseAsync"></a>

### CloseAsync()


```csharp
public override Task CloseAsync()
```


Closes the browser context. All the targets that belong to the browser context will be closed.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpBrowserContext.cs#L70)

<a id="PuppeteerSharp_Cdp_CdpBrowserContext_GetCookiesAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpBrowserContext_GetCookiesAsync"></a>

### GetCookiesAsync()


```csharp
public override Task<CookieParam[]> GetCookiesAsync()
```


Gets all cookies in the browser context.

**Returns:** [Task&lt;CookieParam\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of [CookieParam](/api/PuppeteerSharp/CookieParam/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpBrowserContext.cs#L121)

<a id="PuppeteerSharp_Cdp_CdpBrowserContext_NewPageAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpBrowserContext_NewPageAsync_PuppeteerSharp_CreatePageOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpBrowserContext.cs#L62)

<a id="PuppeteerSharp_Cdp_CdpBrowserContext_OverridePermissionsAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpBrowserContext_OverridePermissionsAsync_System_String_System_Collections_Generic_IEnumerable_PuppeteerSharp_OverridePermission__"></a>

### `OverridePermissionsAsync(string, IEnumerable<OverridePermission>)`


```csharp
public override Task OverridePermissionsAsync(string origin, IEnumerable<OverridePermission> permissions)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpBrowserContext.cs#L81)

<a id="PuppeteerSharp_Cdp_CdpBrowserContext_PagesAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpBrowserContext_PagesAsync_System_Boolean_"></a>

### PagesAsync(bool)


```csharp
public override Task<IPage[]> PagesAsync(bool includeAll = false)
```


An array of all pages inside the browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `includeAll` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Experimental. When true, includes all kinds of pages. |

**Returns:** [Task&lt;IPage\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of all open pages.
    Non visible pages, such as `"background_page"`, will not be listed here.
    You can find them using [PageAsync()](/api/PuppeteerSharp/ITarget/#PuppeteerSharp_ITarget_PageAsync).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpBrowserContext.cs#L49)

<a id="PuppeteerSharp_Cdp_CdpBrowserContext_SetCookieAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpBrowserContext_SetCookieAsync_PuppeteerSharp_CookieData___"></a>

### SetCookieAsync(params CookieData[])


```csharp
public override Task SetCookieAsync(params CookieData[] cookies)
```


Sets cookies in the browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieData\[\]` | Cookies to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpBrowserContext.cs#L130)

<a id="PuppeteerSharp_Cdp_CdpBrowserContext_SetPermissionAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpBrowserContext_SetPermissionAsync_System_String_PuppeteerSharp_PermissionEntry___"></a>

### SetPermissionAsync(string, params PermissionEntry[])


```csharp
public override Task SetPermissionAsync(string origin, params PermissionEntry[] permissions)
```


Sets the permission for a specific origin.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `origin` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The origin to set the permission for, e.g. "https://example.com". Use "*" for all origins (CDP only). |
| `permissions` | `PermissionEntry\[\]` | The permissions to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpBrowserContext.cs#L90)

<a id="PuppeteerSharp_Cdp_CdpBrowserContext_Targets_"></a>
<a id="PuppeteerSharp_Cdp_CdpBrowserContext_Targets"></a>

### Targets()


```csharp
public override ITarget[] Targets()
```


Gets an array of all active targets inside the browser context.

**Returns:** `ITarget\[\]` — An array of all active targets inside the browser context.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpBrowserContext.cs#L46)
