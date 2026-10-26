---
title: "TransportTaskScheduler"
description: "Delegate for scheduling of long-running transport tasks."
editUrl: false
prev: false
next: false
sidebar:
  label: "TransportTaskScheduler"
  hidden: true
---

<a id="PuppeteerSharp_Transport_TransportTaskScheduler"></a>

Namespace: [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/)


```csharp
public delegate void TransportTaskScheduler(Func<CancellationToken, Task> taskFactory, CancellationToken cancellationToken)
```


Delegate for scheduling of long-running transport tasks.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `taskFactory` | [Func&lt;CancellationToken, Task&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Delegate that creates the task to be scheduled. |
| `cancellationToken` | [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) | Cancellation token for the task to be scheduled. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/TransportTaskScheduler.cs#L12)
