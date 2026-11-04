---
title: "Target"
description: "Target."
editUrl: false
prev: false
next: false
sidebar:
  label: "Target"
  hidden: true
---

<a id="PuppeteerSharp_Target"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class Target : ITarget
```


Target.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [ITarget](/api/PuppeteerSharp/ITarget/)

**Derived:** [CdpTarget](/api/PuppeteerSharp.Cdp/CdpTarget/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Target.cs#L13)

## Properties

<a id="PuppeteerSharp_Target_Opener_"></a>
<a id="PuppeteerSharp_Target_Opener"></a>

### Opener


```csharp
public abstract ITarget Opener { get; }
```


Get the target that opened this target.

**Value:** [ITarget](/api/PuppeteerSharp/ITarget/)

Top-level targets return `null`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Target.cs#L29)

<a id="PuppeteerSharp_Target_Type_"></a>
<a id="PuppeteerSharp_Target_Type"></a>

### Type


```csharp
public abstract TargetType Type { get; }
```


Gets the type. It will be [Type](/api/PuppeteerSharp/TargetInfo/#PuppeteerSharp_TargetInfo_Type).
Can be `"page"`, `"background_page"`, `"service_worker"`, `"shared_worker"`, `"browser"` or `"other"`.

**Value:** [TargetType](/api/PuppeteerSharp/TargetType/) — The type.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Target.cs#L26)

<a id="PuppeteerSharp_Target_Url_"></a>
<a id="PuppeteerSharp_Target_Url"></a>

### Url


```csharp
public abstract string Url { get; }
```


Gets the URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Target.cs#L23)

## Methods

<a id="PuppeteerSharp_Target_AsPageAsync_"></a>
<a id="PuppeteerSharp_Target_AsPageAsync"></a>

### AsPageAsync()


```csharp
public abstract Task<IPage> AsPageAsync()
```


Forcefully creates a page for a target of any type. It is useful if you
want to handle a CDP target of type `other` as a page. If you deal with a
regular page target, use [PageAsync()](/api/PuppeteerSharp/ITarget/#PuppeteerSharp_ITarget_PageAsync).

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [IPage](/api/PuppeteerSharp/IPage/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Target.cs#L51)

<a id="PuppeteerSharp_Target_CreateCDPSessionAsync_"></a>
<a id="PuppeteerSharp_Target_CreateCDPSessionAsync"></a>

### CreateCDPSessionAsync()


```csharp
public abstract Task<ICDPSession> CreateCDPSessionAsync()
```


Creates a Chrome Devtools Protocol session attached to the target.

**Returns:** [Task&lt;ICDPSession&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [ICDPSession](/api/PuppeteerSharp/ICDPSession/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Target.cs#L54)

<a id="PuppeteerSharp_Target_PageAsync_"></a>
<a id="PuppeteerSharp_Target_PageAsync"></a>

### PageAsync()


```csharp
public virtual Task<IPage> PageAsync()
```


Returns the [IPage](/api/PuppeteerSharp/IPage/) associated with the target. If the target is not `"page"`, `"webview"` or `"background_page"` returns `null`.

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — a task that returns a [IPage](/api/PuppeteerSharp/IPage/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Target.cs#L45)

<a id="PuppeteerSharp_Target_WorkerAsync_"></a>
<a id="PuppeteerSharp_Target_WorkerAsync"></a>

### WorkerAsync()


```csharp
public virtual Task<WebWorker> WorkerAsync()
```


If the target is not of type `"service_worker"` or `"shared_worker"`, returns `null`.

**Returns:** [Task&lt;WebWorker&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [WebWorker](/api/PuppeteerSharp/WebWorker/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Target.cs#L48)
