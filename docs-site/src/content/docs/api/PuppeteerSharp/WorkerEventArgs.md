---
title: "WorkerEventArgs"
description: "Workder created event arguments."
editUrl: false
prev: false
next: false
sidebar:
  label: "WorkerEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_WorkerEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class WorkerEventArgs : EventArgs
```


Workder created event arguments.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WorkerEventArgs.cs#L8)

## Constructors

<a id="PuppeteerSharp_WorkerEventArgs__ctor_"></a>
<a id="PuppeteerSharp_WorkerEventArgs__ctor_PuppeteerSharp_WebWorker_"></a>

### WorkerEventArgs(WebWorker)


```csharp
public WorkerEventArgs(WebWorker worker)
```


Initializes a new instance of the [WorkerEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `worker` | [WebWorker](/api/PuppeteerSharp/WebWorker/) | Worker. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WorkerEventArgs.cs#L14)

## Properties

<a id="PuppeteerSharp_WorkerEventArgs_Worker_"></a>
<a id="PuppeteerSharp_WorkerEventArgs_Worker"></a>

### Worker


```csharp
public WebWorker Worker { get; set; }
```


Worker.

**Value:** [WebWorker](/api/PuppeteerSharp/WebWorker/) — The worker.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WorkerEventArgs.cs#L20)
