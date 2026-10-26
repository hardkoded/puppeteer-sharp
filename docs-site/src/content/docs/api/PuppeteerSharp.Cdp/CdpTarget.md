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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L30)

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L68)

<a id="PuppeteerSharp_Cdp_CdpTarget_TargetId_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_TargetId"></a>

### TargetId


```csharp
public string TargetId { get; }
```


Gets the target identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The target identifier.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L65)

<a id="PuppeteerSharp_Cdp_CdpTarget_Type_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_Type"></a>

### Type


```csharp
public override TargetType Type { get; }
```


Gets the type. It will be [Type](/api/PuppeteerSharp/TargetInfo/#PuppeteerSharp_TargetInfo_Type).
Can be `"page"`, `"background_page"`, `"service_worker"`, `"shared_worker"`, `"browser"` or `"other"`.

**Value:** [TargetType](/api/PuppeteerSharp/TargetType/) — The type.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L59)

<a id="PuppeteerSharp_Cdp_CdpTarget_Url_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_Url"></a>

### Url


```csharp
public override string Url { get; }
```


Gets the URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L56)

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L98)

<a id="PuppeteerSharp_Cdp_CdpTarget_CheckIfInitialized_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_CheckIfInitialized"></a>

### CheckIfInitialized()


```csharp
protected virtual void CheckIfInitialized()
```


Check is the target is not initialized.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L150)

<a id="PuppeteerSharp_Cdp_CdpTarget_CreateCDPSessionAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpTarget_CreateCDPSessionAsync"></a>

### CreateCDPSessionAsync()


```csharp
public override Task<ICDPSession> CreateCDPSessionAsync()
```


Creates a Chrome Devtools Protocol session attached to the target.

**Returns:** [Task&lt;ICDPSession&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [ICDPSession](/api/PuppeteerSharp/ICDPSession/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTarget.cs#L110)
