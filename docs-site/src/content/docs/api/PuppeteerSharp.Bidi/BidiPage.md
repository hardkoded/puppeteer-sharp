---
title: "BidiPage"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "BidiPage"
  hidden: true
---

<a id="PuppeteerSharp_Bidi_BidiPage"></a>

Namespace: [PuppeteerSharp.Bidi](/api/PuppeteerSharp.Bidi/)


```csharp
public class BidiPage : Page, IPage, IDisposable, IAsyncDisposable
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Page](/api/PuppeteerSharp/Page/)

**Implements:** [IPage](/api/PuppeteerSharp/IPage/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L47)

## Properties

<a id="PuppeteerSharp_Bidi_BidiPage_Browser_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_Browser"></a>

### Browser


```csharp
protected override Browser Browser { get; }
```


Browser.

**Value:** [Browser](/api/PuppeteerSharp/Browser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L141)

<a id="PuppeteerSharp_Bidi_BidiPage_BrowserContext_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_BrowserContext"></a>

### BrowserContext


```csharp
public override IBrowserContext BrowserContext { get; }
```


Get the browser context that the page belongs to.

**Value:** [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L90)

<a id="PuppeteerSharp_Bidi_BidiPage_Client_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_Client"></a>

### Client


```csharp
public override CDPSession Client { get; }
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Value:** [CDPSession](/api/PuppeteerSharp/CDPSession/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L93)

<a id="PuppeteerSharp_Bidi_BidiPage_Frames_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_Frames"></a>

### Frames


```csharp
public override IFrame[] Frames { get; }
```


Gets all frames attached to the page.

**Value:** `IFrame\[\]` — An array of all frames attached to the page.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L99)

<a id="PuppeteerSharp_Bidi_BidiPage_IsJavaScriptEnabled_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_IsJavaScriptEnabled"></a>

### IsJavaScriptEnabled


```csharp
public override bool IsJavaScriptEnabled { get; }
```


If the page has JavaScript enabled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L117)

<a id="PuppeteerSharp_Bidi_BidiPage_MainFrame_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_MainFrame"></a>

### MainFrame


```csharp
public override IFrame MainFrame { get; }
```


Gets page's main frame.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/)

Page is guaranteed to have a main frame which persists during navigations.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L120)

<a id="PuppeteerSharp_Bidi_BidiPage_Target_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_Target"></a>

### Target


```csharp
public override Target Target { get; }
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Value:** [Target](/api/PuppeteerSharp/Target/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L96)

<a id="PuppeteerSharp_Bidi_BidiPage_Workers_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_Workers"></a>

### Workers


```csharp
public override WebWorker[] Workers { get; }
```


Gets all workers in the page.

**Value:** `WebWorker\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L114)

## Methods

<a id="PuppeteerSharp_Bidi_BidiPage_AuthenticateAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_AuthenticateAsync_PuppeteerSharp_Credentials_"></a>

### AuthenticateAsync(Credentials)


```csharp
public override Task AuthenticateAsync(Credentials credentials)
```


Provide credentials for http authentication &lt;a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication"&gt;https://developer.mozilla.org/en-US/docs/Web/HTTP/Authentication&lt;/a&gt;.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `credentials` | [Credentials](/api/PuppeteerSharp/Credentials/) | The credentials. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the message is sent to the browser.

To disable authentication, pass `null`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L186)

<a id="PuppeteerSharp_Bidi_BidiPage_BringToFrontAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_BringToFrontAsync"></a>

### BringToFrontAsync()


```csharp
public override Task BringToFrontAsync()
```


Brings page to front (activates tab).

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to Chromium.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L197)

<a id="PuppeteerSharp_Bidi_BidiPage_CaptureHeapSnapshotAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_CaptureHeapSnapshotAsync_PuppeteerSharp_HeapSnapshotOptions_"></a>

### CaptureHeapSnapshotAsync(HeapSnapshotOptions)


```csharp
public override Task CaptureHeapSnapshotAsync(HeapSnapshotOptions options)
```


Captures a snapshot of the JavaScript heap and writes it to a file.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [HeapSnapshotOptions](/api/PuppeteerSharp/HeapSnapshotOptions/) | Heap snapshot options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the heap snapshot is captured and written to disk.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L648)

<a id="PuppeteerSharp_Bidi_BidiPage_CloseAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_CloseAsync_PuppeteerSharp_PageCloseOptions_"></a>

### CloseAsync(PageCloseOptions)


```csharp
public override Task CloseAsync(PageCloseOptions options = null)
```


Closes the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [PageCloseOptions](/api/PuppeteerSharp/PageCloseOptions/) | Close options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L588)

<a id="PuppeteerSharp_Bidi_BidiPage_CreateScreenRecording_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_CreateScreenRecording_PuppeteerSharp_RecordOptions_"></a>

### CreateScreenRecording(RecordOptions)


```csharp
protected override ScreenRecording CreateScreenRecording(RecordOptions options)
```


Creates a protocol-specific screen recording instance.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [RecordOptions](/api/PuppeteerSharp/RecordOptions/) | Recording options. |

**Returns:** [ScreenRecording](/api/PuppeteerSharp/ScreenRecording/) — A screen recording instance.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L1111)

<a id="PuppeteerSharp_Bidi_BidiPage_DeleteCookieAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_DeleteCookieAsync_PuppeteerSharp_CookieParam___"></a>

### DeleteCookieAsync(params CookieParam[])


```csharp
public override Task DeleteCookieAsync(params CookieParam[] cookies)
```


Deletes cookies from the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieParam\[\]` | Cookies to delete. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L604)

<a id="PuppeteerSharp_Bidi_BidiPage_Dispose_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_Dispose_System_Boolean_"></a>

### Dispose(bool)


```csharp
protected override void Dispose(bool disposing)
```


Dispose resources.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `disposing` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Indicates whether disposal was initiated by [Dispose()](/api/PuppeteerSharp/Page/#PuppeteerSharp_Page_Dispose) operation. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L1118)

<a id="PuppeteerSharp_Bidi_BidiPage_EmulateCPUThrottlingAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EmulateCPUThrottlingAsync_System_Nullable_System_Decimal__"></a>

### EmulateCPUThrottlingAsync(decimal?)


```csharp
public override Task EmulateCPUThrottlingAsync(decimal? factor = null)
```


Enables CPU throttling to emulate slow CPUs.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `factor` | [decimal?](https://learn.microsoft.com/dotnet/api/system.decimal) | Throttling rate as a slowdown factor (1 is no throttle, 2 is 2x slowdown, etc). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L265)

<a id="PuppeteerSharp_Bidi_BidiPage_EmulateFocusedPageAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EmulateFocusedPageAsync_System_Boolean_"></a>

### EmulateFocusedPageAsync(bool)


```csharp
public override Task EmulateFocusedPageAsync(bool enabled)
```


Emulates focus state of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether to emulate focus. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message has been sent to the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L269)

<a id="PuppeteerSharp_Bidi_BidiPage_EmulateIdleStateAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EmulateIdleStateAsync_PuppeteerSharp_EmulateIdleOverrides_"></a>

### EmulateIdleStateAsync(EmulateIdleOverrides)


```csharp
public override Task EmulateIdleStateAsync(EmulateIdleOverrides idleOverrides = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L261)

<a id="PuppeteerSharp_Bidi_BidiPage_EmulateLocaleAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EmulateLocaleAsync_System_String_"></a>

### EmulateLocaleAsync(string)


```csharp
public override Task EmulateLocaleAsync(string locale = null)
```


Emulates the locale of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `locale` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Locale to emulate on the page (e.g., "en-US", "de-DE"). Passing `null` disables locale emulation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the locale has been set.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L249)

<a id="PuppeteerSharp_Bidi_BidiPage_EmulateMediaFeaturesAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EmulateMediaFeaturesAsync_System_Collections_Generic_IEnumerable_PuppeteerSharp_MediaFeatureValue__"></a>

### `EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue>)`


```csharp
public override Task EmulateMediaFeaturesAsync(IEnumerable<MediaFeatureValue> features)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L430)

<a id="PuppeteerSharp_Bidi_BidiPage_EmulateMediaTypeAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EmulateMediaTypeAsync_PuppeteerSharp_Media_MediaType_"></a>

### EmulateMediaTypeAsync(MediaType)


```csharp
public override Task EmulateMediaTypeAsync(MediaType type)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L427)

<a id="PuppeteerSharp_Bidi_BidiPage_EmulateNetworkConditionsAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EmulateNetworkConditionsAsync_PuppeteerSharp_NetworkConditions_"></a>

### EmulateNetworkConditionsAsync(NetworkConditions)


```csharp
public override Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions)
```


Emulates network conditions.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `networkConditions` | [NetworkConditions](/api/PuppeteerSharp/NetworkConditions/) | Passing `null` disables network condition emulation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Result task.

**NOTE** This does not affect WebSockets and WebRTC PeerConnections (see https://crbug.com/563644).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L736)

<a id="PuppeteerSharp_Bidi_BidiPage_EmulateTimezoneAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EmulateTimezoneAsync_System_String_"></a>

### EmulateTimezoneAsync(string)


```csharp
public override Task EmulateTimezoneAsync(string timezoneId)
```


Changes the timezone of the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timezoneId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Timezone to set. See &lt;a href="https://cs.chromium.org/chromium/src/third_party/icu/source/data/misc/metaZones.txt?rcl=faee8bc70570192d82d2978a71e2a615788597d1"&gt;ICU’s `metaZones.txt`&lt;/a&gt; for a list of supported timezone IDs. Passing `null` disables timezone emulation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The viewport task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L205)

<a id="PuppeteerSharp_Bidi_BidiPage_EmulateVisionDeficiencyAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EmulateVisionDeficiencyAsync_PuppeteerSharp_VisionDeficiency_"></a>

### EmulateVisionDeficiencyAsync(VisionDeficiency)


```csharp
public override Task EmulateVisionDeficiencyAsync(VisionDeficiency type)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L201)

<a id="PuppeteerSharp_Bidi_BidiPage_EvaluateExpressionOnNewDocumentAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EvaluateExpressionOnNewDocumentAsync_System_String_"></a>

### EvaluateExpressionOnNewDocumentAsync(string)


```csharp
public override Task<NewDocumentScriptEvaluation> EvaluateExpressionOnNewDocumentAsync(string expression)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L685)

<a id="PuppeteerSharp_Bidi_BidiPage_EvaluateFunctionOnNewDocumentAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_EvaluateFunctionOnNewDocumentAsync_System_String_System_Object___"></a>

### EvaluateFunctionOnNewDocumentAsync(string, params object[])


```csharp
public override Task<NewDocumentScriptEvaluation> EvaluateFunctionOnNewDocumentAsync(string pageFunction, params object[] args)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L652)

<a id="PuppeteerSharp_Bidi_BidiPage_ExposeFunctionAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_ExposeFunctionAsync_System_String_System_Delegate_"></a>

### ExposeFunctionAsync(string, Delegate)


```csharp
protected override Task ExposeFunctionAsync(string name, Delegate puppeteerFunction)
```


Internal add exposed functions.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function name. |
| `puppeteerFunction` | [Delegate](https://learn.microsoft.com/dotnet/api/system.delegate) | Puppeteer function. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) that completes when the function has been added.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L1107)

<a id="PuppeteerSharp_Bidi_BidiPage_ExtensionRealms_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_ExtensionRealms"></a>

### ExtensionRealms()


```csharp
public override IReadOnlyList<Realm> ExtensionRealms()
```


Retrieves the list of extension realms inside the main frame of this page.
Shortcut for `MainFrame.ExtensionRealms()`.

**Returns:** [IReadOnlyList&lt;Realm&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1) — A list of [Realm](/api/PuppeteerSharp/Realm/) instances representing extension execution contexts.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L681)

<a id="PuppeteerSharp_Bidi_BidiPage_GetCookiesAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_GetCookiesAsync_System_String___"></a>

### GetCookiesAsync(params string[])


```csharp
public override Task<CookieParam[]> GetCookiesAsync(params string[] urls)
```


Returns the page's cookies.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `urls` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Url's to return cookies for. |

**Returns:** [Task&lt;CookieParam\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Array of cookies.

If no URLs are specified, this method returns cookies for the current page URL.
If URLs are specified, only cookies for those URLs are returned.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L770)

<a id="PuppeteerSharp_Bidi_BidiPage_GoBackAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_GoBackAsync_PuppeteerSharp_NavigationOptions_"></a>

### GoBackAsync(NavigationOptions)


```csharp
public override Task<IResponse> GoBackAsync(NavigationOptions options = null)
```


Navigate to the previous page in history.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | Navigation parameters. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task that resolves to the main resource response. In case of multiple redirects,
    the navigation will resolve with the response of the last redirect. If can not go back, resolves to null.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L343)

<a id="PuppeteerSharp_Bidi_BidiPage_GoForwardAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_GoForwardAsync_PuppeteerSharp_NavigationOptions_"></a>

### GoForwardAsync(NavigationOptions)


```csharp
public override Task<IResponse> GoForwardAsync(NavigationOptions options = null)
```


Navigate to the next page in history.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | Navigation parameters. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task that resolves to the main resource response. In case of multiple redirects,
    the navigation will resolve with the response of the last redirect. If can not go forward, resolves to null.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L346)

<a id="PuppeteerSharp_Bidi_BidiPage_HasDevToolsAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_HasDevToolsAsync"></a>

### HasDevToolsAsync()


```csharp
public override Task<bool> HasDevToolsAsync()
```


Returns true if DevTools is attached to the current page.
Use [OpenDevToolsAsync()](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_OpenDevToolsAsync) to get the DevTools page.

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A `Task%601` that completes with true if DevTools is attached.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L703)

<a id="PuppeteerSharp_Bidi_BidiPage_MetricsAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_MetricsAsync"></a>

### MetricsAsync()


```csharp
public override Task<Dictionary<string, decimal>> MetricsAsync()
```


Returns metrics.

**Returns:** [Task&lt;Dictionary&lt;string, decimal&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves into a list of metrics.

All timestamps are in monotonic time: monotonically increasing time in seconds since an arbitrary point in the past.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L644)

<a id="PuppeteerSharp_Bidi_BidiPage_OpenDevToolsAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_OpenDevToolsAsync"></a>

### OpenDevToolsAsync()


```csharp
public override Task<IPage> OpenDevToolsAsync()
```


Opens DevTools for this page if not already open and returns the DevTools page.
This method is only available in Chrome.

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A `Task%601` that completes with the DevTools page.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L700)

<a id="PuppeteerSharp_Bidi_BidiPage_PdfInternalAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_PdfInternalAsync_System_IO_Stream_PuppeteerSharp_PdfOptions_"></a>

### PdfInternalAsync(Stream, PdfOptions)


```csharp
protected override Task PdfInternalAsync(Stream outputStream, PdfOptions options)
```


PDF implementation.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `outputStream` | [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) | Where to write the PDF bytes. |
| `options` | [PdfOptions](/api/PuppeteerSharp/PdfOptions/) | PDF options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — PDF data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L922)

<a id="PuppeteerSharp_Bidi_BidiPage_PerformScreenshotAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_PerformScreenshotAsync_PuppeteerSharp_ScreenshotType_PuppeteerSharp_ScreenshotOptions_"></a>

### PerformScreenshotAsync(ScreenshotType, ScreenshotOptions)


```csharp
protected override Task<string> PerformScreenshotAsync(ScreenshotType type, ScreenshotOptions options)
```


Screenshot implementation.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `type` | [ScreenshotType](/api/PuppeteerSharp/ScreenshotType/) | Screenshot type. |
| `options` | [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Options. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The screenshot as a base64 string.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L1009)

<a id="PuppeteerSharp_Bidi_BidiPage_QueryObjectsAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_QueryObjectsAsync_PuppeteerSharp_IJSHandle_"></a>

### QueryObjectsAsync(IJSHandle)


```csharp
public override Task<IJSHandle> QueryObjectsAsync(IJSHandle prototypeHandle)
```


The method iterates JavaScript heap and finds all the objects with the given prototype.
Shortcut for `page.MainFrame.GetExecutionContextAsync().QueryObjectsAsync(prototypeHandle)`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `prototypeHandle` | [IJSHandle](/api/PuppeteerSharp/IJSHandle/) | A handle to the object prototype. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves to a handle to an array of objects with this prototype.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L697)

<a id="PuppeteerSharp_Bidi_BidiPage_ReloadAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_ReloadAsync_PuppeteerSharp_ReloadOptions_"></a>

### ReloadAsync(ReloadOptions)


```csharp
public override Task<IResponse> ReloadAsync(ReloadOptions options)
```


Reloads the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ReloadOptions](/api/PuppeteerSharp/ReloadOptions/) | Navigation options. |

**Returns:** [Task&lt;IResponse&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the main resource response. In case of multiple redirects, the navigation will resolve with the response of the last redirect.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L273)

<a id="PuppeteerSharp_Bidi_BidiPage_RemoveExposedFunctionAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_RemoveExposedFunctionAsync_System_String_"></a>

### RemoveExposedFunctionAsync(string)


```csharp
public override Task RemoveExposedFunctionAsync(string name)
```


Removes a previously added function via `Action)`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of the function to remove. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L663)

<a id="PuppeteerSharp_Bidi_BidiPage_RemoveScriptToEvaluateOnNewDocumentAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_RemoveScriptToEvaluateOnNewDocumentAsync_System_String_"></a>

### RemoveScriptToEvaluateOnNewDocumentAsync(string)


```csharp
public override Task RemoveScriptToEvaluateOnNewDocumentAsync(string identifier)
```


Removes script that injected into page by [EvaluateExpressionOnNewDocumentAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EvaluateExpressionOnNewDocumentAsync_System_String_) or [EvaluateExpressionOnNewDocumentAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EvaluateExpressionOnNewDocumentAsync_System_String_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `identifier` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function identifier. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the script is removed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L667)

<a id="PuppeteerSharp_Bidi_BidiPage_ResizeAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_ResizeAsync_System_Int32_System_Int32_"></a>

### ResizeAsync(int, int)


```csharp
public override Task ResizeAsync(int contentWidth, int contentHeight)
```


Resizes the browser window of this page so that the content area (excluding browser UI) has the specified width and height.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `contentWidth` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | The desired width of the content area in pixels. |
| `contentHeight` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | The desired height of the content area in pixels. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the resize operation is complete.

This is an experimental API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L237)

<a id="PuppeteerSharp_Bidi_BidiPage_SetBurstModeOffAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetBurstModeOffAsync"></a>

### SetBurstModeOffAsync()


```csharp
public override Task SetBurstModeOffAsync()
```


Resets the background color and Viewport after taking Screenshots using BurstMode.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The burst mode off.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L349)

<a id="PuppeteerSharp_Bidi_BidiPage_SetBypassCSPAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetBypassCSPAsync_System_Boolean_"></a>

### SetBypassCSPAsync(bool)


```csharp
public override Task SetBypassCSPAsync(bool enabled)
```


Toggles bypassing page's Content-Security-Policy.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | sets bypassing of page's Content-Security-Policy. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task which resolves after the message is sent to the browser.

CSP bypassing happens at the moment of CSP initialization rather then evaluation.
Usually this means that [SetBypassCSPAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetBypassCSPAsync_System_Boolean_) should be called before navigating to the domain.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L409)

<a id="PuppeteerSharp_Bidi_BidiPage_SetBypassServiceWorkerAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetBypassServiceWorkerAsync_System_Boolean_"></a>

### SetBypassServiceWorkerAsync(bool)


```csharp
public override Task SetBypassServiceWorkerAsync(bool bypass)
```


Toggles ignoring of service worker for each request.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `bypass` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | When `true` bypasses service worker. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the message is sent to the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L674)

<a id="PuppeteerSharp_Bidi_BidiPage_SetCacheEnabledAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetCacheEnabledAsync_System_Boolean_"></a>

### SetCacheEnabledAsync(bool)


```csharp
public override Task SetCacheEnabledAsync(bool enabled = true)
```


Toggles ignoring cache for each request based on the enabled state. By default, caching is enabled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | sets the `enabled` state of the cache. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L418)

<a id="PuppeteerSharp_Bidi_BidiPage_SetCookieAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetCookieAsync_PuppeteerSharp_CookieParam___"></a>

### SetCookieAsync(params CookieParam[])


```csharp
public override Task SetCookieAsync(params CookieParam[] cookies)
```


Clears all of the current cookies and then sets the cookies for the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cookies` | `CookieParam\[\]` | Cookies to set. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L502)

<a id="PuppeteerSharp_Bidi_BidiPage_SetDragInterceptionAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetDragInterceptionAsync_System_Boolean_"></a>

### SetDragInterceptionAsync(bool)


```csharp
public override Task SetDragInterceptionAsync(bool enabled)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L641)

<a id="PuppeteerSharp_Bidi_BidiPage_SetExtraHttpHeadersAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetExtraHttpHeadersAsync_System_Collections_Generic_Dictionary_System_String_System_String__"></a>

### `SetExtraHttpHeadersAsync(Dictionary<string, string>)`


```csharp
public override Task SetExtraHttpHeadersAsync(Dictionary<string, string> headers)
```


Sets extra HTTP headers that will be sent with every request the page initiates.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `headers` | [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) | Additional http headers to be sent with every request. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L164)

<a id="PuppeteerSharp_Bidi_BidiPage_SetGeolocationAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetGeolocationAsync_PuppeteerSharp_GeolocationOption_"></a>

### SetGeolocationAsync(GeolocationOption)


```csharp
public override Task SetGeolocationAsync(GeolocationOption options)
```


Sets the page's geolocation.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [GeolocationOption](/api/PuppeteerSharp/GeolocationOption/) | Geolocation options. Pass `null` to remove the geolocation override. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

Consider using `OverridePermission%7d)` to grant permissions for the page to read its geolocation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L352)

<a id="PuppeteerSharp_Bidi_BidiPage_SetJavaScriptEnabledAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetJavaScriptEnabledAsync_System_Boolean_"></a>

### SetJavaScriptEnabledAsync(bool)


```csharp
public override Task SetJavaScriptEnabledAsync(bool enabled)
```


Enables/Disables Javascript on the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether or not to enable JavaScript on the page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L396)

<a id="PuppeteerSharp_Bidi_BidiPage_SetOfflineModeAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetOfflineModeAsync_System_Boolean_"></a>

### SetOfflineModeAsync(bool)


```csharp
public override Task SetOfflineModeAsync(bool value)
```


Set offline mode for the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `value` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | When `true` enables offline mode for the page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Result task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L715)

<a id="PuppeteerSharp_Bidi_BidiPage_SetRequestInterceptionAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetRequestInterceptionAsync_System_Boolean_"></a>

### SetRequestInterceptionAsync(bool)


```csharp
public override Task SetRequestInterceptionAsync(bool value)
```


Activating request interception enables `AbortAsync`,
`ContinueAsync` and `RespondAsync` methods.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `value` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether to enable request interception.. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The request interception task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L706)

<a id="PuppeteerSharp_Bidi_BidiPage_SetUserAgentAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetUserAgentAsync_PuppeteerSharp_SetUserAgentOptions_"></a>

### SetUserAgentAsync(SetUserAgentOptions)


```csharp
public override Task SetUserAgentAsync(SetUserAgentOptions options)
```


Sets the user agent, user agent metadata, and platform.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [SetUserAgentOptions](/api/PuppeteerSharp/SetUserAgentOptions/) | User agent options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L447)

<a id="PuppeteerSharp_Bidi_BidiPage_SetUserAgentAsync_System_String_PuppeteerSharp_UserAgentMetadata_"></a>

### SetUserAgentAsync(string, UserAgentMetadata)


```csharp
public override Task SetUserAgentAsync(string userAgent, UserAgentMetadata userAgentData = null)
```


Sets the user agent to be used in this page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `userAgent` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Specific user agent to use in this page. |
| `userAgentData` | [UserAgentMetadata](/api/PuppeteerSharp/UserAgentMetadata/) | Specific user agent client hint data to use in this page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L434)

<a id="PuppeteerSharp_Bidi_BidiPage_SetViewportAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_SetViewportAsync_PuppeteerSharp_ViewPortOptions_"></a>

### SetViewportAsync(ViewPortOptions)


```csharp
public override Task SetViewportAsync(ViewPortOptions viewport)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L461)

<a id="PuppeteerSharp_Bidi_BidiPage_TabInfoAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_TabInfoAsync"></a>

### TabInfoAsync()


```csharp
public override Task<TabInfo> TabInfoAsync()
```


Returns information about the browser tab of this page. The result is a snapshot and is not updated when the tab changes.

**Returns:** [Task&lt;TabInfo&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the tab info, or `null` if the page is not in a tab strip (for example, in chrome-headless-shell) or the browser does not report tab information.

This is an experimental API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L245)

<a id="PuppeteerSharp_Bidi_BidiPage_TriggerExtensionActionAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_TriggerExtensionActionAsync_PuppeteerSharp_Extension_"></a>

### TriggerExtensionActionAsync(Extension)


```csharp
public override Task TriggerExtensionActionAsync(Extension extension)
```


Triggers an extension action for the given extension.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `extension` | [Extension](/api/PuppeteerSharp/Extension/) | The extension to trigger the action for. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the extension action has been triggered.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L677)

<a id="PuppeteerSharp_Bidi_BidiPage_WaitForFileChooserAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_WaitForFileChooserAsync_PuppeteerSharp_WaitForOptions_"></a>

### WaitForFileChooserAsync(WaitForOptions)


```csharp
public override Task<FileChooser> WaitForFileChooserAsync(WaitForOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L305)

<a id="PuppeteerSharp_Bidi_BidiPage_WindowIdAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiPage_WindowIdAsync"></a>

### WindowIdAsync()


```csharp
public override Task<string> WindowIdAsync()
```


Returns the page's window ID.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the window ID string.

This is an experimental API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiPage.cs#L241)
