---
title: "BidiBrowserContext"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "BidiBrowserContext"
  hidden: true
---

<a id="PuppeteerSharp_Bidi_BidiBrowserContext"></a>

Namespace: [PuppeteerSharp.Bidi](/api/PuppeteerSharp.Bidi/)


```csharp
public class BidiBrowserContext : BrowserContext, IBrowserContext, IDisposable, IAsyncDisposable
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [BrowserContext](/api/PuppeteerSharp/BrowserContext/)

**Implements:** [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowserContext.cs#L38)

## Methods

<a id="PuppeteerSharp_Bidi_BidiBrowserContext_ClearPermissionOverridesAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowserContext_ClearPermissionOverridesAsync"></a>

### ClearPermissionOverridesAsync()


```csharp
public override Task ClearPermissionOverridesAsync()
```


Clears all permission overrides for the browser context.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowserContext.cs#L145)

<a id="PuppeteerSharp_Bidi_BidiBrowserContext_CloseAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowserContext_CloseAsync"></a>

### CloseAsync()


```csharp
public override Task CloseAsync()
```


Closes the browser context. All the targets that belong to the browser context will be closed.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowserContext.cs#L244)

<a id="PuppeteerSharp_Bidi_BidiBrowserContext_GetCookiesAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowserContext_GetCookiesAsync"></a>

### GetCookiesAsync()


```csharp
public override Task<CookieParam[]> GetCookiesAsync()
```


Gets all cookies in the browser context.

**Returns:** [Task&lt;CookieParam\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of [CookieParam](/api/PuppeteerSharp/CookieParam/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowserContext.cs#L170)

<a id="PuppeteerSharp_Bidi_BidiBrowserContext_NewPageAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowserContext_NewPageAsync_PuppeteerSharp_CreatePageOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowserContext.cs#L195)

<a id="PuppeteerSharp_Bidi_BidiBrowserContext_OverridePermissionsAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowserContext_OverridePermissionsAsync_System_String_System_Collections_Generic_IEnumerable_PuppeteerSharp_OverridePermission__"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowserContext.cs#L60)

<a id="PuppeteerSharp_Bidi_BidiBrowserContext_PagesAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowserContext_PagesAsync_System_Boolean_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowserContext.cs#L192)

<a id="PuppeteerSharp_Bidi_BidiBrowserContext_SetCookieAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowserContext_SetCookieAsync_PuppeteerSharp_CookieData___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowserContext.cs#L177)

<a id="PuppeteerSharp_Bidi_BidiBrowserContext_SetPermissionAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowserContext_SetPermissionAsync_System_String_PuppeteerSharp_PermissionEntry___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowserContext.cs#L108)

<a id="PuppeteerSharp_Bidi_BidiBrowserContext_Targets_"></a>
<a id="PuppeteerSharp_Bidi_BidiBrowserContext_Targets"></a>

### Targets()


```csharp
public override ITarget[] Targets()
```


Gets an array of all active targets inside the browser context.

**Returns:** `ITarget\[\]` — An array of all active targets inside the browser context.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiBrowserContext.cs#L265)
