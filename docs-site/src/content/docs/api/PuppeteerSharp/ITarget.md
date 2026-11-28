---
title: "ITarget"
description: "Target."
editUrl: false
prev: false
next: false
sidebar:
  label: "ITarget"
  hidden: true
---

<a id="PuppeteerSharp_ITarget"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface ITarget
```


Target.

**Derived:** [CdpDevToolsTarget](/api/PuppeteerSharp.Cdp/CdpDevToolsTarget/), [CdpOtherTarget](/api/PuppeteerSharp.Cdp/CdpOtherTarget/), [CdpPageTarget](/api/PuppeteerSharp.Cdp/CdpPageTarget/), [CdpTarget](/api/PuppeteerSharp.Cdp/CdpTarget/), [CdpWorkerTarget](/api/PuppeteerSharp.Cdp/CdpWorkerTarget/), [Target](/api/PuppeteerSharp/Target/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L9)

## Properties

<a id="PuppeteerSharp_ITarget_Browser_"></a>
<a id="PuppeteerSharp_ITarget_Browser"></a>

### Browser


```csharp
IBrowser Browser { get; }
```


Get the browser the target belongs to.

**Value:** [IBrowser](/api/PuppeteerSharp/IBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L14)

<a id="PuppeteerSharp_ITarget_BrowserContext_"></a>
<a id="PuppeteerSharp_ITarget_BrowserContext"></a>

### BrowserContext


```csharp
IBrowserContext BrowserContext { get; }
```


Get the browser context the target belongs to.

**Value:** [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L19)

<a id="PuppeteerSharp_ITarget_ChildTargets_"></a>
<a id="PuppeteerSharp_ITarget_ChildTargets"></a>

### ChildTargets


```csharp
IEnumerable<ITarget> ChildTargets { get; }
```


Gets the target's child targets.

**Value:** [IEnumerable&lt;ITarget&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L45)

<a id="PuppeteerSharp_ITarget_Opener_"></a>
<a id="PuppeteerSharp_ITarget_Opener"></a>

### Opener


```csharp
ITarget Opener { get; }
```


Get the target that opened this target.

**Value:** [ITarget](./)

Top-level targets return `null`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L27)

<a id="PuppeteerSharp_ITarget_Type_"></a>
<a id="PuppeteerSharp_ITarget_Type"></a>

### Type


```csharp
TargetType Type { get; }
```


Gets the type. It will be [Type](/api/PuppeteerSharp/TargetInfo/#PuppeteerSharp_TargetInfo_Type).
Can be `"page"`, `"background_page"`, `"service_worker"`, `"shared_worker"`, `"browser"` or `"other"`.

**Value:** [TargetType](/api/PuppeteerSharp/TargetType/) — The type.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L34)

<a id="PuppeteerSharp_ITarget_Url_"></a>
<a id="PuppeteerSharp_ITarget_Url"></a>

### Url


```csharp
string Url { get; }
```


Gets the URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L40)

## Methods

<a id="PuppeteerSharp_ITarget_AsPageAsync_"></a>
<a id="PuppeteerSharp_ITarget_AsPageAsync"></a>

### AsPageAsync()


```csharp
Task<IPage> AsPageAsync()
```


Forcefully creates a page for a target of any type. It is useful if you
want to handle a CDP target of type `other` as a page. If you deal with a
regular page target, use [PageAsync()](#PuppeteerSharp_ITarget_PageAsync).

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [IPage](/api/PuppeteerSharp/IPage/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L71)

<a id="PuppeteerSharp_ITarget_CloseAsync_"></a>
<a id="PuppeteerSharp_ITarget_CloseAsync"></a>

### CloseAsync()


```csharp
Task CloseAsync()
```


Closes the target. This API is experimental.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the target is closed.

It does not create a [IPage](/api/PuppeteerSharp/IPage/) for the target, so it also closes a page target whose page
could not be created. It does not run `beforeunload` handlers.




Throws if the target does not support closing, for example the browser target, the target of a frame
or a dedicated worker.




Chrome destroys a service worker only after all CDP sessions attached to it detach. While a different
session, for example one from [CreateCDPSessionAsync()](#PuppeteerSharp_ITarget_CreateCDPSessionAsync), stays attached to a service worker,
the returned task might not complete.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L92)

<a id="PuppeteerSharp_ITarget_CreateCDPSessionAsync_"></a>
<a id="PuppeteerSharp_ITarget_CreateCDPSessionAsync"></a>

### CreateCDPSessionAsync()


```csharp
Task<ICDPSession> CreateCDPSessionAsync()
```


Creates a Chrome Devtools Protocol session attached to the target.

**Returns:** [Task&lt;ICDPSession&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [ICDPSession](/api/PuppeteerSharp/ICDPSession/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L51)

<a id="PuppeteerSharp_ITarget_PageAsync_"></a>
<a id="PuppeteerSharp_ITarget_PageAsync"></a>

### PageAsync()


```csharp
Task<IPage> PageAsync()
```


Returns the [IPage](/api/PuppeteerSharp/IPage/) associated with the target. If the target is not `"page"`, `"webview"` or `"background_page"` returns `null`.

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — a task that returns a [IPage](/api/PuppeteerSharp/IPage/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L57)

<a id="PuppeteerSharp_ITarget_WorkerAsync_"></a>
<a id="PuppeteerSharp_ITarget_WorkerAsync"></a>

### WorkerAsync()


```csharp
Task<WebWorker> WorkerAsync()
```


If the target is not of type `"service_worker"` or `"shared_worker"`, returns `null`.

**Returns:** [Task&lt;WebWorker&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [WebWorker](/api/PuppeteerSharp/WebWorker/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITarget.cs#L63)
