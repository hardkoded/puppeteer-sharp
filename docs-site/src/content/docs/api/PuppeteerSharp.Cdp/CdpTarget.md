---
title: "CdpTarget"
description: "Target."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpTarget"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpTarget"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpTarget : Target, ITarget
```


Target.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Target](/api/PuppeteerSharp/Target/)

**Implements:** [ITarget](/api/PuppeteerSharp/ITarget/)

**Derived:** [CdpOtherTarget](/api/PuppeteerSharp.Cdp/CdpOtherTarget/), [CdpPageTarget](/api/PuppeteerSharp.Cdp/CdpPageTarget/), [CdpWorkerTarget](/api/PuppeteerSharp.Cdp/CdpWorkerTarget/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L33)

## Properties

<a id="PuppeteerSharp_Cdp_CdpTarget_Opener_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_Opener"></a>

### Opener


```csharp
public override ITarget Opener { get; }
```


Get the target that opened this target.

**Value:** [ITarget](/api/PuppeteerSharp/ITarget/)

Top-level targets return `null`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L72)

<a id="PuppeteerSharp_Cdp_CdpTarget_TargetId_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_TargetId"></a>

### TargetId


```csharp
public string TargetId { get; }
```


Gets the target identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The target identifier.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L69)

<a id="PuppeteerSharp_Cdp_CdpTarget_Type_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_Type"></a>

### Type


```csharp
public override TargetType Type { get; }
```


Gets the type. It will be [Type](/api/PuppeteerSharp/TargetInfo/#PuppeteerSharp_TargetInfo_Type).
Can be `"page"`, `"background_page"`, `"service_worker"`, `"shared_worker"`, `"browser"` or `"other"`.

**Value:** [TargetType](/api/PuppeteerSharp/TargetType/) — The type.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L63)

<a id="PuppeteerSharp_Cdp_CdpTarget_Url_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_Url"></a>

### Url


```csharp
public override string Url { get; }
```


Gets the URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L60)

## Methods

<a id="PuppeteerSharp_Cdp_CdpTarget_AsPageAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_AsPageAsync"></a>

### AsPageAsync()


```csharp
public override Task<IPage> AsPageAsync()
```


Forcefully creates a page for a target of any type. It is useful if you
want to handle a CDP target of type `other` as a page. If you deal with a
regular page target, use [PageAsync()](/api/PuppeteerSharp/ITarget/#PuppeteerSharp_ITarget_PageAsync).

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [IPage](/api/PuppeteerSharp/IPage/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L114)

<a id="PuppeteerSharp_Cdp_CdpTarget_CheckIfInitialized_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_CheckIfInitialized"></a>

### CheckIfInitialized()


```csharp
protected virtual void CheckIfInitialized()
```


Check is the target is not initialized.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L214)

<a id="PuppeteerSharp_Cdp_CdpTarget_CloseAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_CloseAsync"></a>

### CloseAsync()


```csharp
public override Task CloseAsync()
```


Closes the target. This API is experimental.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the target is closed.

&lt;p&gt;
It does not create a [IPage](/api/PuppeteerSharp/IPage/) for the target, so it also closes a page target whose page
could not be created. It does not run `beforeunload` handlers.
&lt;/p&gt;
&lt;p&gt;
Throws if the target does not support closing, for example the browser target, the target of a frame
or a dedicated worker.
&lt;/p&gt;
&lt;p&gt;
Chrome destroys a service worker only after all CDP sessions attached to it detach. While a different
session, for example one from [CreateCDPSessionAsync()](/api/PuppeteerSharp/ITarget/#PuppeteerSharp_ITarget_CreateCDPSessionAsync), stays attached to a service worker,
the returned task might not complete.
&lt;/p&gt;

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L143)

<a id="PuppeteerSharp_Cdp_CdpTarget_CreateCDPSessionAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_CreateCDPSessionAsync"></a>

### CreateCDPSessionAsync()


```csharp
public override Task<ICDPSession> CreateCDPSessionAsync()
```


Creates a Chrome Devtools Protocol session attached to the target.

**Returns:** [Task&lt;ICDPSession&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [ICDPSession](/api/PuppeteerSharp/ICDPSession/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L126)
