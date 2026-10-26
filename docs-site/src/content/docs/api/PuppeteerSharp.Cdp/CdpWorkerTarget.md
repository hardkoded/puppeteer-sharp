---
title: "CdpWorkerTarget"
description: "Worker target."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpWorkerTarget"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpWorkerTarget"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpWorkerTarget : CdpTarget, ITarget
```


Worker target.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Target](/api/PuppeteerSharp/Target/) → [CdpTarget](/api/PuppeteerSharp.Cdp/CdpTarget/)

**Implements:** [ITarget](/api/PuppeteerSharp/ITarget/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWorkerTarget.cs#L10)

## Methods

<a id="PuppeteerSharp_Cdp_CdpWorkerTarget_WorkerAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpWorkerTarget_WorkerAsync"></a>

### WorkerAsync()


```csharp
public override Task<WebWorker> WorkerAsync()
```


If the target is not of type `"service_worker"` or `"shared_worker"`, returns `null`.

**Returns:** [Task&lt;WebWorker&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [WebWorker](/api/PuppeteerSharp/WebWorker/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWorkerTarget.cs#L26)
