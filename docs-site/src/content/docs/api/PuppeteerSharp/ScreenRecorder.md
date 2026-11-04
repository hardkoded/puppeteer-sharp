---
title: "ScreenRecorder"
description: "Represents a screen recording session."
editUrl: false
prev: false
next: false
sidebar:
  label: "ScreenRecorder"
  hidden: true
---

<a id="PuppeteerSharp_ScreenRecorder"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ScreenRecorder : IAsyncDisposable
```


Represents a screen recording session.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecorder.cs#L17)

## Methods

<a id="PuppeteerSharp_ScreenRecorder_DisposeAsync_"></a>
<a id="PuppeteerSharp_ScreenRecorder_DisposeAsync"></a>

### DisposeAsync()


```csharp
public ValueTask DisposeAsync()
```


Performs application-defined tasks associated with freeing, releasing, or
resetting unmanaged resources asynchronously.

**Returns:** [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask) — A task that represents the asynchronous dispose operation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecorder.cs#L229)

<a id="PuppeteerSharp_ScreenRecorder_StopAsync_"></a>
<a id="PuppeteerSharp_ScreenRecorder_StopAsync"></a>

### StopAsync()


```csharp
public Task StopAsync()
```


Stops the recorder.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) that completes when the recorder has stopped.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecorder.cs#L165)
