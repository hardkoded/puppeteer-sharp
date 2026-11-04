---
title: "ScreenRecording"
description: "Represents a video stream-based screen recording session."
editUrl: false
prev: false
next: false
sidebar:
  label: "ScreenRecording"
  hidden: true
---

<a id="PuppeteerSharp_ScreenRecording"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class ScreenRecording : IAsyncEnumerable<byte[]>, IAsyncDisposable
```


Represents a video stream-based screen recording session.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IAsyncEnumerable&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.iasyncenumerable-1), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L14)

## Constructors

<a id="PuppeteerSharp_ScreenRecording__ctor_"></a>
<a id="PuppeteerSharp_ScreenRecording__ctor_PuppeteerSharp_Page_PuppeteerSharp_RecordOptions_Microsoft_Extensions_Logging_ILogger_"></a>

### ScreenRecording(Page, RecordOptions, ILogger)


```csharp
protected ScreenRecording(Page page, RecordOptions options, ILogger logger)
```


Initializes a new instance of the [ScreenRecording](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `page` | [Page](/api/PuppeteerSharp/Page/) | The page being recorded. |
| `options` | [RecordOptions](/api/PuppeteerSharp/RecordOptions/) | Recording options. |
| `logger` | [ILogger](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger) | Logger instance. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L29)

## Properties

<a id="PuppeteerSharp_ScreenRecording_Destinations_"></a>
<a id="PuppeteerSharp_ScreenRecording_Destinations"></a>

### Destinations


```csharp
protected HashSet<IWritableDestination> Destinations { get; }
```


Gets the destinations the recording is piped to.

**Value:** [HashSet&lt;IWritableDestination&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.hashset-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L54)

<a id="PuppeteerSharp_ScreenRecording_Logger_"></a>
<a id="PuppeteerSharp_ScreenRecording_Logger"></a>

### Logger


```csharp
protected ILogger Logger { get; }
```


Gets the logger instance.

**Value:** [ILogger](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.ilogger)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L49)

<a id="PuppeteerSharp_ScreenRecording_Options_"></a>
<a id="PuppeteerSharp_ScreenRecording_Options"></a>

### Options


```csharp
protected RecordOptions Options { get; }
```


Gets the recording options.

**Value:** [RecordOptions](/api/PuppeteerSharp/RecordOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L44)

<a id="PuppeteerSharp_ScreenRecording_Page_"></a>
<a id="PuppeteerSharp_ScreenRecording_Page"></a>

### Page


```csharp
protected Page Page { get; }
```


Gets the page being recorded.

**Value:** [Page](/api/PuppeteerSharp/Page/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L39)

<a id="PuppeteerSharp_ScreenRecording_Stopped_"></a>
<a id="PuppeteerSharp_ScreenRecording_Stopped"></a>

### Stopped


```csharp
protected bool Stopped { get; set; }
```


Gets a value indicating whether the recording has been stopped.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L59)

## Methods

<a id="PuppeteerSharp_ScreenRecording_CloseDestinationsAsync_"></a>
<a id="PuppeteerSharp_ScreenRecording_CloseDestinationsAsync"></a>

### CloseDestinationsAsync()


```csharp
protected Task CloseDestinationsAsync()
```


Closes all destinations and completes the readable stream.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when destinations are closed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L144)

<a id="PuppeteerSharp_ScreenRecording_DisposeAsync_"></a>
<a id="PuppeteerSharp_ScreenRecording_DisposeAsync"></a>

### DisposeAsync()


```csharp
public ValueTask DisposeAsync()
```


Performs application-defined tasks associated with freeing, releasing, or
resetting unmanaged resources asynchronously.

**Returns:** [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask) — A task that represents the asynchronous dispose operation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L114)

<a id="PuppeteerSharp_ScreenRecording_EnqueueChunk_"></a>
<a id="PuppeteerSharp_ScreenRecording_EnqueueChunk_System_Byte___"></a>

### EnqueueChunk(byte[])


```csharp
protected void EnqueueChunk(byte[] buffer)
```


Enqueues a chunk to stream consumers and destinations.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `buffer` | [byte\[\]](https://learn.microsoft.com/dotnet/api/system.byte) | The chunk to enqueue. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L130)

<a id="PuppeteerSharp_ScreenRecording_GetAsyncEnumerator_"></a>
<a id="PuppeteerSharp_ScreenRecording_GetAsyncEnumerator_System_Threading_CancellationToken_"></a>

### GetAsyncEnumerator(CancellationToken)


```csharp
public IAsyncEnumerator<byte[]> GetAsyncEnumerator(CancellationToken cancellationToken = default)
```


Returns an enumerator that iterates asynchronously through the collection.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `cancellationToken` | [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) | A [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) that may be used to cancel the asynchronous iteration. |

**Returns:** [IAsyncEnumerator&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.iasyncenumerator-1) — An enumerator that can be used to iterate asynchronously through the collection.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L110)

<a id="PuppeteerSharp_ScreenRecording_Pipe_"></a>
<a id="PuppeteerSharp_ScreenRecording_Pipe_PuppeteerSharp_IWritableDestination_"></a>

### Pipe(IWritableDestination)


```csharp
public IWritableDestination Pipe(IWritableDestination destination)
```


Pipes the recorded stream to a writable destination and returns it for chaining.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `destination` | [IWritableDestination](/api/PuppeteerSharp/IWritableDestination/) | Writable target for MP4 chunks. |

**Returns:** [IWritableDestination](/api/PuppeteerSharp/IWritableDestination/) — The same destination instance.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L87)

<a id="PuppeteerSharp_ScreenRecording_PipeAsync_"></a>
<a id="PuppeteerSharp_ScreenRecording_PipeAsync_PuppeteerSharp_IWritableDestination_"></a>

### PipeAsync(IWritableDestination)


```csharp
public Task PipeAsync(IWritableDestination destination)
```


Registers a writable destination to receive recording chunks asynchronously.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `destination` | [IWritableDestination](/api/PuppeteerSharp/IWritableDestination/) | The destination that will receive chunks. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the destination has been registered.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L103)

<a id="PuppeteerSharp_ScreenRecording_PipeAsync_System_IO_Stream_"></a>

### PipeAsync(Stream)


```csharp
public Task PipeAsync(Stream destination)
```


Pipes the recorded stream to a destination stream.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `destination` | [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) | The destination stream. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when piping finishes.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L72)

<a id="PuppeteerSharp_ScreenRecording_StopAsync_"></a>
<a id="PuppeteerSharp_ScreenRecording_StopAsync"></a>

### StopAsync()


```csharp
public abstract Task StopAsync()
```


Stops the screen recording.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when recording has stopped.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenRecording.cs#L65)
