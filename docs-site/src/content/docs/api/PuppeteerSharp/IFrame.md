---
title: "IFrame"
description: "Provides methods to interact with a single page frame in Chromium. One IPage instance might have multiple IFrame instances. At every point of time, page expo..."
editUrl: false
prev: false
next: false
sidebar:
  label: "IFrame"
  hidden: true
---

<a id="PuppeteerSharp_IFrame"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IFrame
```


Provides methods to interact with a single page frame in Chromium. One [IPage](/api/PuppeteerSharp/IPage/) instance might have multiple [IFrame](./) instances.
At every point of time, page exposes its current frame tree via the [MainFrame](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_MainFrame) and [ChildFrames](#PuppeteerSharp_IFrame_ChildFrames) properties.

[IFrame](./) object's lifecycle is controlled by three events, dispatched on the page object
- [FrameAttached](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameAttached) - fires when the frame gets attached to the page. A Frame can be attached to the page only once
- [FrameNavigated](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameNavigated) - fired when the frame commits navigation to a different URL
- [FrameDetached](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameDetached) - fired when the frame gets detached from the page.  A Frame can be detached from the page only once.

**Derived:** [BidiFrame](/api/PuppeteerSharp.Bidi/BidiFrame/), [CdpFrame](/api/PuppeteerSharp.Cdp/CdpFrame/), [Frame](/api/PuppeteerSharp/Frame/)

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L39)

## Properties

<a id="PuppeteerSharp_IFrame_ChildFrames_"></a>
<a id="PuppeteerSharp_IFrame_ChildFrames"></a>

### ChildFrames


```csharp
IReadOnlyCollection<IFrame> ChildFrames { get; }
```


Gets the child frames of the this frame.

**Value:** [IReadOnlyCollection&lt;IFrame&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlycollection-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L49)

<a id="PuppeteerSharp_IFrame_Detached_"></a>
<a id="PuppeteerSharp_IFrame_Detached"></a>

### Detached


```csharp
bool Detached { get; }
```


Gets a value indicating if the frame is detached or not.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L54)

<a id="PuppeteerSharp_IFrame_Id_"></a>
<a id="PuppeteerSharp_IFrame_Id"></a>

### Id


```csharp
string Id { get; }
```


Frame Id.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L59)

<a id="PuppeteerSharp_IFrame_Name_"></a>
<a id="PuppeteerSharp_IFrame_Name"></a>

### Name


```csharp
[Obsolete("Use (await frame.FrameElementAsync()).EvaluateFunctionAsync<string>(\"frame => frame.name\") instead.")]
string Name { get; }
```


Gets the frame's name attribute as specified in the tag
If the name is empty, returns the id attribute instead.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L65)

<a id="PuppeteerSharp_IFrame_Page_"></a>
<a id="PuppeteerSharp_IFrame_Page"></a>

### Page


```csharp
IPage Page { get; }
```


The [IPage](/api/PuppeteerSharp/IPage/) associated with the frame.

**Value:** [IPage](/api/PuppeteerSharp/IPage/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L73)

<a id="PuppeteerSharp_IFrame_ParentFrame_"></a>
<a id="PuppeteerSharp_IFrame_ParentFrame"></a>

### ParentFrame


```csharp
IFrame ParentFrame { get; }
```


Gets the parent frame, if any. Detached frames and main frames return `null`.

**Value:** [IFrame](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L78)

<a id="PuppeteerSharp_IFrame_Url_"></a>
<a id="PuppeteerSharp_IFrame_Url"></a>

### Url


```csharp
string Url { get; }
```


Gets the frame's url.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L83)

## Events

<a id="PuppeteerSharp_IFrame_FrameSwappedByActivation"></a>

### FrameSwappedByActivation


```csharp
event EventHandler FrameSwappedByActivation
```


Raised when the frame is swapped.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L44)

## Methods

<a id="PuppeteerSharp_IFrame_AddScriptTagAsync_"></a>
<a id="PuppeteerSharp_IFrame_AddScriptTagAsync_PuppeteerSharp_AddTagOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L92)

<a id="PuppeteerSharp_IFrame_AddStyleTagAsync_"></a>
<a id="PuppeteerSharp_IFrame_AddStyleTagAsync_PuppeteerSharp_AddTagOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L101)

<a id="PuppeteerSharp_IFrame_ClickAsync_"></a>
<a id="PuppeteerSharp_IFrame_ClickAsync_System_String_PuppeteerSharp_Input_ClickOptions_"></a>

### ClickAsync(string, ClickOptions)


```csharp
Task ClickAsync(string selector, ClickOptions options = null)
```


Fetches an element with `selector`, scrolls it into view if needed, and then uses [Mouse](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Mouse) to click in the center of the element.

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L110)

<a id="PuppeteerSharp_IFrame_EvaluateExpressionAsync_"></a>
<a id="PuppeteerSharp_IFrame_EvaluateExpressionAsync_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L122)

<a id="PuppeteerSharp_IFrame_EvaluateExpressionAsync__1_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L135)

<a id="PuppeteerSharp_IFrame_EvaluateExpressionHandleAsync_"></a>
<a id="PuppeteerSharp_IFrame_EvaluateExpressionHandleAsync_System_String_"></a>

### EvaluateExpressionHandleAsync(string)


```csharp
Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
```


Passes an expression to the [EvaluateExpressionHandleAsync(string)](/api/PuppeteerSharp/ExecutionContext/#PuppeteerSharp_ExecutionContext_EvaluateExpressionHandleAsync_System_String_), returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), then [EvaluateExpressionHandleAsync(string)](/api/PuppeteerSharp/ExecutionContext/#PuppeteerSharp_ExecutionContext_EvaluateExpressionHandleAsync_System_String_) would wait for the [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) to resolve and return its value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Expression to be evaluated in the. [ExecutionContext](/api/PuppeteerSharp/ExecutionContext/) |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Resolves to the return value of `script`.

**Example**

```csharp
var handle = await Page.MainFrame.EvaluateExpressionHandleAsync("1 + 2");
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L149)

<a id="PuppeteerSharp_IFrame_EvaluateFunctionAsync_"></a>
<a id="PuppeteerSharp_IFrame_EvaluateFunctionAsync_System_String_System_Object___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L163)

<a id="PuppeteerSharp_IFrame_EvaluateFunctionAsync__1_System_String_System_Object___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L178)

<a id="PuppeteerSharp_IFrame_EvaluateFunctionHandleAsync_"></a>
<a id="PuppeteerSharp_IFrame_EvaluateFunctionHandleAsync_System_String_System_Object___"></a>

### EvaluateFunctionHandleAsync(string, params object[])


```csharp
Task<IJSHandle> EvaluateFunctionHandleAsync(string func, params object[] args)
```


Passes a function to the `IExecutionContext.EvaluateFunctionAsync()`, returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), then `ExecutionContext.EvaluateFunctionHandleAsync()` would wait for the [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) to resolve and return its value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `func` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in the [ExecutionContext](/api/PuppeteerSharp/ExecutionContext/). |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `func`. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Resolves to the return value of `func`.

**Example**

```csharp
var handle = await Page.MainFrame.EvaluateFunctionHandleAsync("() => Promise.resolve(self)");
return handle; // Handle for the global object.
```


[IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances can be passed as arguments to the `IExecutionContext.EvaluateFunctionAsync()`:


```csharp
var handle = await Page.MainFrame.EvaluateExpressionHandleAsync("document.body");
var resultHandle = await Page.MainFrame.EvaluateFunctionHandleAsync("body => body.innerHTML", handle);
return await resultHandle.JsonValueAsync(); // prints body's innerHTML
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L202)

<a id="PuppeteerSharp_IFrame_FocusAsync_"></a>
<a id="PuppeteerSharp_IFrame_FocusAsync_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L210)

<a id="PuppeteerSharp_IFrame_FrameElementAsync_"></a>
<a id="PuppeteerSharp_IFrame_FrameElementAsync"></a>

### FrameElementAsync()


```csharp
Task<ElementHandle> FrameElementAsync()
```


The frame element associated with this frame (if any).

**Returns:** [Task&lt;ElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the frame element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L507)

<a id="PuppeteerSharp_IFrame_GetContentAsync_"></a>
<a id="PuppeteerSharp_IFrame_GetContentAsync_PuppeteerSharp_GetContentOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L218)

<a id="PuppeteerSharp_IFrame_GetTitleAsync_"></a>
<a id="PuppeteerSharp_IFrame_GetTitleAsync"></a>

### GetTitleAsync()


```csharp
Task<string> GetTitleAsync()
```


Returns page's title.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — page's title.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L225)

<a id="PuppeteerSharp_IFrame_GoToAsync_"></a>
<a id="PuppeteerSharp_IFrame_GoToAsync_System_String_PuppeteerSharp_NavigationOptions_"></a>

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

`IFrame.GoToAsync()` will throw an error if:
- there's an SSL error (e.g. in case of self-signed certificates).
- target URL is invalid.
- the `timeout` is exceeded during navigation.
- the remote server does not respond or is unreachable.
- the main resource failed to load.
- the URL is blocked by blocklist/allowlist rules.

`IFrame.GoToAsync()` will not throw an error when any valid HTTP status code is returned by the remote server,
including 404 "Not Found" and 500 "Internal Server Error".  The status code for such responses can be retrieved by calling [Status](/api/PuppeteerSharp/IResponse/#PuppeteerSharp_IResponse_Status)

&gt; **NOTE** `IFrame.GoToAsync()` either throws an error or returns a main resource response.
The only exceptions are navigation to `about:blank` or navigation to the same URL with a different hash, which would succeed and return `null`.

&gt; **NOTE** Headless mode doesn't support navigation to a PDF document. See the &lt;see fref="https://bugs.chromium.org/p/chromium/issues/detail?id=761295"&gt;upstream issue&lt;/see&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L266)

<a id="PuppeteerSharp_IFrame_GoToAsync_System_String_System_Nullable_System_Int32__PuppeteerSharp_WaitUntilNavigation___"></a>

### GoToAsync(string, int?, WaitUntilNavigation[])


```csharp
Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null)
```


Navigates to an url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | URL to navigate page to. The url should include scheme, e.g. https://. |
| `timeout` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | maximum navigation time in milliseconds. Defaults to 30 seconds. Pass 0 to disable timeout. The default value can be changed by using the [DefaultNavigationTimeout](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_DefaultNavigationTimeout) property. |
| `waitUntil` | `WaitUntilNavigation[]` | When to consider navigation succeeded, defaults to [Load](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Load). Given an array of [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/), navigation is considered to be successful after all events have been fired. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L236)

<a id="PuppeteerSharp_IFrame_HoverAsync_"></a>
<a id="PuppeteerSharp_IFrame_HoverAsync_System_String_"></a>

### HoverAsync(string)


```csharp
Task HoverAsync(string selector)
```


Fetches an element with `selector`, scrolls it into view if needed, and then uses [Mouse](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Mouse) to hover over the center of the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to hover. If there are multiple elements satisfying the selector, the first will be hovered. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching `selector` is successfully hovered.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching `selector`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L274)

<a id="PuppeteerSharp_IFrame_Locator_"></a>
<a id="PuppeteerSharp_IFrame_Locator_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L431)

<a id="PuppeteerSharp_IFrame_LocatorFunction_"></a>
<a id="PuppeteerSharp_IFrame_LocatorFunction_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L439)

<a id="PuppeteerSharp_IFrame_QuerySelectorAllAsync_"></a>
<a id="PuppeteerSharp_IFrame_QuerySelectorAllAsync_System_String_"></a>

### QuerySelectorAllAsync(string)


```csharp
Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
```


Queries frame for the selector. If no elements match the selector, the return value resolve to `Empty<T>`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query frame for. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L282)

<a id="PuppeteerSharp_IFrame_QuerySelectorAllAsync_System_String_PuppeteerSharp_QueryOptions_"></a>

### QuerySelectorAllAsync(string, QueryOptions)


```csharp
Task<IElementHandle[]> QuerySelectorAllAsync(string selector, QueryOptions options)
```


Queries frame for the selector. If no elements match the selector, the return value resolve to `Empty<T>`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query frame for. |
| `options` | [QueryOptions](/api/PuppeteerSharp/QueryOptions/) | Optional query options. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L291)

<a id="PuppeteerSharp_IFrame_QuerySelectorAllHandleAsync_"></a>
<a id="PuppeteerSharp_IFrame_QuerySelectorAllHandleAsync_System_String_"></a>

### QuerySelectorAllHandleAsync(string)


```csharp
Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
```


A utility function to be used with `PuppeteerHandleExtensions.EvaluateFunctionAsync<T>()`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query page for. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [IJSHandle](/api/PuppeteerSharp/IJSHandle/) of `document.querySelectorAll` result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L298)

<a id="PuppeteerSharp_IFrame_QuerySelectorAsync_"></a>
<a id="PuppeteerSharp_IFrame_QuerySelectorAsync_System_String_"></a>

### QuerySelectorAsync(string)


```csharp
Task<IElementHandle> QuerySelectorAsync(string selector)
```


Queries frame for the selector. If there's no such element within the frame, the method will resolve to `null`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Selector to query frame for. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to [IElementHandle](/api/PuppeteerSharp/IElementHandle/) pointing to the frame element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L306)

<a id="PuppeteerSharp_IFrame_SelectAsync_"></a>
<a id="PuppeteerSharp_IFrame_SelectAsync_System_String_System_String___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L318)

<a id="PuppeteerSharp_IFrame_SetContentAsync_"></a>
<a id="PuppeteerSharp_IFrame_SetContentAsync_System_String_PuppeteerSharp_NavigationOptions_"></a>

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
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | The options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L327)

<a id="PuppeteerSharp_IFrame_SetContentAsync_System_String_PuppeteerSharp_SetContentOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L340)

<a id="PuppeteerSharp_IFrame_TapAsync_"></a>
<a id="PuppeteerSharp_IFrame_TapAsync_System_String_"></a>

### TapAsync(string)


```csharp
Task TapAsync(string selector)
```


Fetches an element with `selector`, scrolls it into view if needed, and then uses [Touchscreen](/api/PuppeteerSharp.Input/Touchscreen/) to tap in the center of the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to tap. If there are multiple elements satisfying the selector, the first will be clicked. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching `selector` is successfully tapped.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching `selector`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L501)

<a id="PuppeteerSharp_IFrame_TypeAsync_"></a>
<a id="PuppeteerSharp_IFrame_TypeAsync_System_String_System_String_PuppeteerSharp_Input_TypeOptions_"></a>

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

To press a special key, like `Control` or `ArrowDown` use `IKeyboard.PressAsync()`.

**Example**

```csharp
await frame.TypeAsync("#mytextarea", "Hello"); // Types instantly
await frame.TypeAsync("#mytextarea", "World", new TypeOptions { Delay = 100 }); // Types slower, like a user
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L361)

<a id="PuppeteerSharp_IFrame_WaitForDevicePromptAsync_"></a>
<a id="PuppeteerSharp_IFrame_WaitForDevicePromptAsync_PuppeteerSharp_WaitForOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L493)

<a id="PuppeteerSharp_IFrame_WaitForExpressionAsync_"></a>
<a id="PuppeteerSharp_IFrame_WaitForExpressionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_"></a>

### WaitForExpressionAsync(string, WaitForFunctionOptions)


```csharp
Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options)
```


Waits for an expression to be evaluated to a truthy value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Expression to be evaluated in browser context. |
| `options` | [WaitForFunctionOptions](/api/PuppeteerSharp/WaitForFunctionOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when the `script` returns a truthy value.

**Exceptions**

| Type | Condition |
| --- | --- |
| [WaitTaskTimeoutException](/api/PuppeteerSharp/WaitTaskTimeoutException/) | If timeout occurred. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L371)

<a id="PuppeteerSharp_IFrame_WaitForFunctionAsync_"></a>
<a id="PuppeteerSharp_IFrame_WaitForFunctionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_System_Object___"></a>

### WaitForFunctionAsync(string, WaitForFunctionOptions, params object[])


```csharp
Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args)
```


Waits for a function to be evaluated to a truthy value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `options` | [WaitForFunctionOptions](/api/PuppeteerSharp/WaitForFunctionOptions/) | Optional waiting parameters. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `script`. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when the `script` returns a truthy value.

**Exceptions**

| Type | Condition |
| --- | --- |
| [WaitTaskTimeoutException](/api/PuppeteerSharp/WaitTaskTimeoutException/) | If timeout occurred. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L382)

<a id="PuppeteerSharp_IFrame_WaitForNavigationAsync_"></a>
<a id="PuppeteerSharp_IFrame_WaitForNavigationAsync_PuppeteerSharp_NavigationOptions_"></a>

### WaitForNavigationAsync(NavigationOptions)


```csharp
Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
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

Usage of the `History API` [https://developer.mozilla.org/en-US/docs/Web/API/History_API](https://developer.mozilla.org/en-US/docs/Web/API/History_API) to change the URL is considered a navigation.

**Example**

```csharp
var navigationTask = Page.WaitForNavigationAsync();
await Page.MainFrame.ClickAsync("a.my-link");
await navigationTask;
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L409)

<a id="PuppeteerSharp_IFrame_WaitForSelectorAsync_"></a>
<a id="PuppeteerSharp_IFrame_WaitForSelectorAsync_System_String_PuppeteerSharp_WaitForSelectorOptions_"></a>

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

**Exceptions**

| Type | Condition |
| --- | --- |
| [WaitTaskTimeoutException](/api/PuppeteerSharp/WaitTaskTimeoutException/) | If timeout occurred. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L420)

<a id="PuppeteerSharp_IFrame_WaitForXPathAsync_"></a>
<a id="PuppeteerSharp_IFrame_WaitForXPathAsync_System_String_PuppeteerSharp_WaitForSelectorOptions_"></a>

### WaitForXPathAsync(string, WaitForSelectorOptions)


```csharp
[Obsolete("Use WaitForSelectorAsync instead")]
Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
```


Waits for a selector to be added to the DOM.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `xpath` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A xpath selector of an element to wait for. |
| `options` | [WaitForSelectorOptions](/api/PuppeteerSharp/WaitForSelectorOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves when element specified by xpath string is added to DOM.
    Resolves to `null` if waiting for `hidden: true` and xpath is not found in DOM.

**Exceptions**

| Type | Condition |
| --- | --- |
| [WaitTaskTimeoutException](/api/PuppeteerSharp/WaitTaskTimeoutException/) | If timeout occurred. |

:::caution[Obsolete]
This API is obsolete.
:::

**Example**

```csharp
var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();
string currentURL = null;
page.MainFrame
    .WaitForXPathAsync("//img")
    .ContinueWith(_ => Console.WriteLine("First URL with image: " + currentURL));
foreach (var current in new[] { "https://example.com", "https://google.com", "https://bbc.com" })
{
    currentURL = current;
    await page.GoToAsync(currentURL);
}
await browser.CloseAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L468)

<a id="PuppeteerSharp_IFrame_XPathAsync_"></a>
<a id="PuppeteerSharp_IFrame_XPathAsync_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IFrame.cs#L476)
