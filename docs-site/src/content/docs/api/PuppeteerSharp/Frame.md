---
title: "Frame"
description: "Provides methods to interact with a single page frame in Chromium. One IPage instance might have multiple IFrame instances. At every point of time, page expo..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Frame"
  hidden: true
---

<a id="PuppeteerSharp_Frame"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class Frame : IFrame
```


Provides methods to interact with a single page frame in Chromium. One [IPage](/api/PuppeteerSharp/IPage/) instance might have multiple [IFrame](/api/PuppeteerSharp/IFrame/) instances.
At every point of time, page exposes its current frame tree via the [MainFrame](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_MainFrame) and [ChildFrames](/api/PuppeteerSharp/IFrame/#PuppeteerSharp_IFrame_ChildFrames) properties.

[IFrame](/api/PuppeteerSharp/IFrame/) object's lifecycle is controlled by three events, dispatched on the page object
- [FrameAttached](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameAttached) - fires when the frame gets attached to the page. A Frame can be attached to the page only once
- [FrameNavigated](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameNavigated) - fired when the frame commits navigation to a different URL
- [FrameDetached](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameDetached) - fired when the frame gets detached from the page.  A Frame can be detached from the page only once.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IFrame](/api/PuppeteerSharp/IFrame/)

**Derived:** [BidiFrame](/api/PuppeteerSharp.Bidi/BidiFrame/), [CdpFrame](/api/PuppeteerSharp.Cdp/CdpFrame/)

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L14)

## Properties

<a id="PuppeteerSharp_Frame_ChildFrames_"></a>
<a id="PuppeteerSharp_Frame_ChildFrames"></a>

### ChildFrames


```csharp
public abstract IReadOnlyCollection<IFrame> ChildFrames { get; }
```


Gets the child frames of the this frame.

**Value:** [IReadOnlyCollection&lt;IFrame&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlycollection-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L32)

<a id="PuppeteerSharp_Frame_Client_"></a>
<a id="PuppeteerSharp_Frame_Client"></a>

### Client


```csharp
public abstract ICDPSession Client { get; protected set; }
```


Gets the CDPSession associated with this environment.

**Value:** [ICDPSession](/api/PuppeteerSharp/ICDPSession/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L53)

<a id="PuppeteerSharp_Frame_Detached_"></a>
<a id="PuppeteerSharp_Frame_Detached"></a>

### Detached


```csharp
public virtual bool Detached { get; }
```


Gets a value indicating if the frame is detached or not.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L41)

<a id="PuppeteerSharp_Frame_Id_"></a>
<a id="PuppeteerSharp_Frame_Id"></a>

### Id


```csharp
public string Id { get; }
```


Frame Id.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L50)

<a id="PuppeteerSharp_Frame_Logger_"></a>
<a id="PuppeteerSharp_Frame_Logger"></a>

### Logger


```csharp
protected ILogger Logger { get; init; }
```


Logger.

**Value:** [ILogger](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L79)

<a id="PuppeteerSharp_Frame_Name_"></a>
<a id="PuppeteerSharp_Frame_Name"></a>

### Name


```csharp
public string Name { get; }
```


Gets the frame's name attribute as specified in the tag
If the name is empty, returns the id attribute instead.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L35)

<a id="PuppeteerSharp_Frame_Page_"></a>
<a id="PuppeteerSharp_Frame_Page"></a>

### Page


```csharp
public abstract IPage Page { get; }
```


The [IPage](/api/PuppeteerSharp/IPage/) associated with the frame.

**Value:** [IPage](/api/PuppeteerSharp/IPage/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L44)

<a id="PuppeteerSharp_Frame_Url_"></a>
<a id="PuppeteerSharp_Frame_Url"></a>

### Url


```csharp
public virtual string Url { get; }
```


Gets the frame's url.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L38)

## Events

<a id="PuppeteerSharp_Frame_FrameSwappedByActivation"></a>

### FrameSwappedByActivation


```csharp
public event EventHandler FrameSwappedByActivation
```


Raised when the frame is swapped.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L19)

## Methods

<a id="PuppeteerSharp_Frame_AddScriptTagAsync_"></a>
<a id="PuppeteerSharp_Frame_AddScriptTagAsync_PuppeteerSharp_AddTagOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L223)

<a id="PuppeteerSharp_Frame_AddStyleTagAsync_"></a>
<a id="PuppeteerSharp_Frame_AddStyleTagAsync_PuppeteerSharp_AddTagOptions_"></a>

### AddStyleTagAsync(AddTagOptions)


```csharp
public abstract Task<IElementHandle> AddStyleTagAsync(AddTagOptions options)
```


Adds a `<link rel="stylesheet">` tag into the page with the desired url or a `<link rel="stylesheet">` tag with the content.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [AddTagOptions](/api/PuppeteerSharp/AddTagOptions/) | add style tag options. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the added tag when the stylesheet's onload fires or when the CSS content was injected into frame.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L220)

<a id="PuppeteerSharp_Frame_ClickAsync_"></a>
<a id="PuppeteerSharp_Frame_ClickAsync_System_String_PuppeteerSharp_Input_ClickOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L322)

<a id="PuppeteerSharp_Frame_EvaluateExpressionAsync_"></a>
<a id="PuppeteerSharp_Frame_EvaluateExpressionAsync_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L92)

<a id="PuppeteerSharp_Frame_EvaluateExpressionAsync__1_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L95)

<a id="PuppeteerSharp_Frame_EvaluateExpressionHandleAsync_"></a>
<a id="PuppeteerSharp_Frame_EvaluateExpressionHandleAsync_System_String_"></a>

### EvaluateExpressionHandleAsync(string)


```csharp
public Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
```


Passes an expression to the [EvaluateExpressionHandleAsync(string)](/api/PuppeteerSharp/ExecutionContext/#PuppeteerSharp_ExecutionContext_EvaluateExpressionHandleAsync_System_String_), returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), then [EvaluateExpressionHandleAsync(string)](/api/PuppeteerSharp/ExecutionContext/#PuppeteerSharp_ExecutionContext_EvaluateExpressionHandleAsync_System_String_) would wait for the [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) to resolve and return its value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Expression to be evaluated in the. [ExecutionContext](/api/PuppeteerSharp/ExecutionContext/) |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Resolves to the return value of &lt;code class="paramref"&gt;script&lt;/code&gt;.

**Example**

```csharp
var handle = await Page.MainFrame.EvaluateExpressionHandleAsync("1 + 2");
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L104)

<a id="PuppeteerSharp_Frame_EvaluateFunctionAsync_"></a>
<a id="PuppeteerSharp_Frame_EvaluateFunctionAsync_System_String_System_Object___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L98)

<a id="PuppeteerSharp_Frame_EvaluateFunctionAsync__1_System_String_System_Object___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L101)

<a id="PuppeteerSharp_Frame_EvaluateFunctionHandleAsync_"></a>
<a id="PuppeteerSharp_Frame_EvaluateFunctionHandleAsync_System_String_System_Object___"></a>

### EvaluateFunctionHandleAsync(string, params object[])


```csharp
public Task<IJSHandle> EvaluateFunctionHandleAsync(string function, params object[] args)
```


Passes a function to the `Object%5b%5d)`, returns a [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task), then `Object%5b%5d)` would wait for the [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) to resolve and return its value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `function` | [string](https://learn.microsoft.com/dotnet/api/system.string) |  |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to &lt;code class="paramref"&gt;func&lt;/code&gt;. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Resolves to the return value of &lt;code class="paramref"&gt;func&lt;/code&gt;.

**Example**

```csharp
var handle = await Page.MainFrame.EvaluateFunctionHandleAsync("() => Promise.resolve(self)");
return handle; // Handle for the global object.
```


[IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances can be passed as arguments to the `Object%5b%5d)`:


```csharp
var handle = await Page.MainFrame.EvaluateExpressionHandleAsync("document.body");
var resultHandle = await Page.MainFrame.EvaluateFunctionHandleAsync("body => body.innerHTML", handle);
return await resultHandle.JsonValueAsync(); // prints body's innerHTML
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L107)

<a id="PuppeteerSharp_Frame_ExtensionRealms_"></a>
<a id="PuppeteerSharp_Frame_ExtensionRealms"></a>

### ExtensionRealms()


```csharp
public abstract IReadOnlyList<Realm> ExtensionRealms()
```


Retrieves the list of extension realms within this frame.

**Returns:** [IReadOnlyList&lt;Realm&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1) — A list of [Realm](/api/PuppeteerSharp/Realm/) instances representing extension execution contexts.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L425)

<a id="PuppeteerSharp_Frame_FocusAsync_"></a>
<a id="PuppeteerSharp_Frame_FocusAsync_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L349)

<a id="PuppeteerSharp_Frame_FrameElementAsync_"></a>
<a id="PuppeteerSharp_Frame_FrameElementAsync"></a>

### FrameElementAsync()


```csharp
public virtual Task<ElementHandle> FrameElementAsync()
```


The frame element associated with this frame (if any).

**Returns:** [Task&lt;ElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the frame element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L388)

<a id="PuppeteerSharp_Frame_GetContentAsync_"></a>
<a id="PuppeteerSharp_Frame_GetContentAsync_PuppeteerSharp_GetContentOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L289)

<a id="PuppeteerSharp_Frame_GetDeviceRequestPromptManager_"></a>
<a id="PuppeteerSharp_Frame_GetDeviceRequestPromptManager"></a>

### GetDeviceRequestPromptManager()


```csharp
protected abstract DeviceRequestPromptManager GetDeviceRequestPromptManager()
```


Gets the prompts manager for the current client.

**Returns:** [DeviceRequestPromptManager](/api/PuppeteerSharp/DeviceRequestPromptManager/) — The [DeviceRequestPromptManager](/api/PuppeteerSharp/DeviceRequestPromptManager/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L489)

<a id="PuppeteerSharp_Frame_GetTitleAsync_"></a>
<a id="PuppeteerSharp_Frame_GetTitleAsync"></a>

### GetTitleAsync()


```csharp
public Task<string> GetTitleAsync()
```


Returns page's title.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — page's title.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L319)

<a id="PuppeteerSharp_Frame_GoToAsync_"></a>
<a id="PuppeteerSharp_Frame_GoToAsync_System_String_PuppeteerSharp_NavigationOptions_"></a>

### GoToAsync(string, NavigationOptions)


```csharp
public abstract Task<IResponse> GoToAsync(string url, NavigationOptions options)
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
- the URL is blocked by blocklist/allowlist rules.

`WaitUntilNavigation%5b%5d)` will not throw an error when any valid HTTP status code is returned by the remote server,
including 404 "Not Found" and 500 "Internal Server Error".  The status code for such responses can be retrieved by calling [Status](/api/PuppeteerSharp/IResponse/#PuppeteerSharp_IResponse_Status)

&gt; **NOTE** `WaitUntilNavigation%5b%5d)` either throws an error or returns a main resource response.
The only exceptions are navigation to `about:blank` or navigation to the same URL with a different hash, which would succeed and return `null`.

&gt; **NOTE** Headless mode doesn't support navigation to a PDF document. See the &lt;see fref="https://bugs.chromium.org/p/chromium/issues/detail?id=761295"&gt;upstream issue&lt;/see&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L82)

<a id="PuppeteerSharp_Frame_GoToAsync_System_String_System_Nullable_System_Int32__PuppeteerSharp_WaitUntilNavigation___"></a>

### GoToAsync(string, int?, WaitUntilNavigation[])


```csharp
public Task<IResponse> GoToAsync(string url, int? timeout = null, WaitUntilNavigation[] waitUntil = null)
```


Navigates to an url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | URL to navigate page to. The url should include scheme, e.g. https://. |
| `timeout` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | maximum navigation time in milliseconds. Defaults to 30 seconds. Pass 0 to disable timeout. The default value can be changed by using the [DefaultNavigationTimeout](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_DefaultNavigationTimeout) property. |
| `waitUntil` | `WaitUntilNavigation\[\]` | When to consider navigation succeeded, defaults to [Load](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Load). Given an array of [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/), navigation is considered to be successful after all events have been fired. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L85)

<a id="PuppeteerSharp_Frame_HoverAsync_"></a>
<a id="PuppeteerSharp_Frame_HoverAsync_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L336)

<a id="PuppeteerSharp_Frame_Locator_"></a>
<a id="PuppeteerSharp_Frame_Locator_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L122)

<a id="PuppeteerSharp_Frame_LocatorFunction_"></a>
<a id="PuppeteerSharp_Frame_LocatorFunction_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L126)

<a id="PuppeteerSharp_Frame_OnDetach_"></a>
<a id="PuppeteerSharp_Frame_OnDetach"></a>

### OnDetach()


```csharp
protected virtual void OnDetach()
```


Called when this frame is being detached. Subclasses can override to perform additional cleanup.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L481)

<a id="PuppeteerSharp_Frame_QuerySelectorAllAsync_"></a>
<a id="PuppeteerSharp_Frame_QuerySelectorAllAsync_System_String_"></a>

### QuerySelectorAllAsync(string)


```csharp
public Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
```


Queries frame for the selector. If no elements match the selector, the return value resolve to `Empty%60%601`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query frame for. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L195)

<a id="PuppeteerSharp_Frame_QuerySelectorAllAsync_System_String_PuppeteerSharp_QueryOptions_"></a>

### QuerySelectorAllAsync(string, QueryOptions)


```csharp
public Task<IElementHandle[]> QuerySelectorAllAsync(string selector, QueryOptions options)
```


Queries frame for the selector. If no elements match the selector, the return value resolve to `Empty%60%601`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query frame for. |
| `options` | [QueryOptions](/api/PuppeteerSharp/QueryOptions/) | Optional query options. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L202)

<a id="PuppeteerSharp_Frame_QuerySelectorAllHandleAsync_"></a>
<a id="PuppeteerSharp_Frame_QuerySelectorAllHandleAsync_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L181)

<a id="PuppeteerSharp_Frame_QuerySelectorAsync_"></a>
<a id="PuppeteerSharp_Frame_QuerySelectorAsync_System_String_"></a>

### QuerySelectorAsync(string)


```csharp
public Task<IElementHandle> QuerySelectorAsync(string selector)
```


Queries frame for the selector. If there's no such element within the frame, the method will resolve to `null`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Selector to query frame for. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to [IElementHandle](/api/PuppeteerSharp/IElementHandle/) pointing to the frame element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L188)

<a id="PuppeteerSharp_Frame_SelectAsync_"></a>
<a id="PuppeteerSharp_Frame_SelectAsync_System_String_System_String___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L168)

<a id="PuppeteerSharp_Frame_SetContentAsync_"></a>
<a id="PuppeteerSharp_Frame_SetContentAsync_System_String_PuppeteerSharp_NavigationOptions_"></a>

### SetContentAsync(string, NavigationOptions)


```csharp
[Obsolete("Use SetContentAsync(string, SetContentOptions) instead. The networkidle0 and networkidle2 wait conditions never worked reliably with SetContent.")]
public abstract Task SetContentAsync(string html, NavigationOptions options)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L309)

<a id="PuppeteerSharp_Frame_SetContentAsync_System_String_PuppeteerSharp_SetContentOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L313)

<a id="PuppeteerSharp_Frame_TapAsync_"></a>
<a id="PuppeteerSharp_Frame_TapAsync_System_String_"></a>

### TapAsync(string)


```csharp
public Task TapAsync(string selector)
```


Fetches an element with &lt;code class="paramref"&gt;selector&lt;/code&gt;, scrolls it into view if needed, and then uses [Touchscreen](/api/PuppeteerSharp.Input/Touchscreen/) to tap in the center of the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to search for element to tap. If there are multiple elements satisfying the selector, the first will be clicked. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element matching &lt;code class="paramref"&gt;selector&lt;/code&gt; is successfully tapped.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If there's no element matching &lt;code class="paramref"&gt;selector&lt;/code&gt;. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L362)

<a id="PuppeteerSharp_Frame_TypeAsync_"></a>
<a id="PuppeteerSharp_Frame_TypeAsync_System_String_System_String_PuppeteerSharp_Input_TypeOptions_"></a>

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
await frame.TypeAsync("#mytextarea", "Hello"); // Types instantly
await frame.TypeAsync("#mytextarea", "World", new TypeOptions { Delay = 100 }); // Types slower, like a user
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L375)

<a id="PuppeteerSharp_Frame_WaitForDevicePromptAsync_"></a>
<a id="PuppeteerSharp_Frame_WaitForDevicePromptAsync_PuppeteerSharp_WaitForOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L216)

<a id="PuppeteerSharp_Frame_WaitForExpressionAsync_"></a>
<a id="PuppeteerSharp_Frame_WaitForExpressionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_"></a>

### WaitForExpressionAsync(string, WaitForFunctionOptions)


```csharp
public Task<IJSHandle> WaitForExpressionAsync(string script, WaitForFunctionOptions options)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L157)

<a id="PuppeteerSharp_Frame_WaitForFunctionAsync_"></a>
<a id="PuppeteerSharp_Frame_WaitForFunctionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_System_Object___"></a>

### WaitForFunctionAsync(string, WaitForFunctionOptions, params object[])


```csharp
public Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options, params object[] args)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L146)

<a id="PuppeteerSharp_Frame_WaitForNavigationAsync_"></a>
<a id="PuppeteerSharp_Frame_WaitForNavigationAsync_PuppeteerSharp_NavigationOptions_"></a>

### WaitForNavigationAsync(NavigationOptions)


```csharp
public abstract Task<IResponse> WaitForNavigationAsync(NavigationOptions options = null)
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

Usage of the `History API` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/History_API"&gt;https://developer.mozilla.org/en-US/docs/Web/API/History_API&lt;/a&gt; to change the URL is considered a navigation.

**Example**

```csharp
var navigationTask = Page.WaitForNavigationAsync();
await Page.MainFrame.ClickAsync("a.my-link");
await navigationTask;
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L89)

<a id="PuppeteerSharp_Frame_WaitForSelectorAsync_"></a>
<a id="PuppeteerSharp_Frame_WaitForSelectorAsync_System_String_PuppeteerSharp_WaitForSelectorOptions_"></a>

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

**Exceptions**

| Type | Condition |
| --- | --- |
| [WaitTaskTimeoutException](/api/PuppeteerSharp/WaitTaskTimeoutException/) | If timeout occurred. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L110)

<a id="PuppeteerSharp_Frame_WaitForXPathAsync_"></a>
<a id="PuppeteerSharp_Frame_WaitForXPathAsync_System_String_PuppeteerSharp_WaitForSelectorOptions_"></a>

### WaitForXPathAsync(string, WaitForSelectorOptions)


```csharp
public Task<IElementHandle> WaitForXPathAsync(string xpath, WaitForSelectorOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L130)

<a id="PuppeteerSharp_Frame_XPathAsync_"></a>
<a id="PuppeteerSharp_Frame_XPathAsync_System_String_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Frame.cs#L209)
