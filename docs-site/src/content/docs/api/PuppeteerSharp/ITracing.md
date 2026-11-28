---
title: "ITracing"
description: "You can use StartAsync(TracingOptions) and StopAsync() to create a trace file which can be opened in Chrome DevTools or timeline viewer."
editUrl: false
prev: false
next: false
sidebar:
  label: "ITracing"
  hidden: true
---

<a id="PuppeteerSharp_ITracing"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface ITracing
```


You can use [StartAsync(TracingOptions)](#PuppeteerSharp_ITracing_StartAsync_PuppeteerSharp_TracingOptions_) and [StopAsync()](#PuppeteerSharp_ITracing_StopAsync) to create a trace file which can be opened in Chrome DevTools or timeline viewer.

**Derived:** [Tracing](/api/PuppeteerSharp/Tracing/)

**Example**

```csharp
await Page.Tracing.StartAsync(new TracingOptions
{
    Screenshots = true,
    Path = _file
});
await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
await Page.Tracing.StopAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITracing.cs#L19)

## Methods

<a id="PuppeteerSharp_ITracing_StartAsync_"></a>
<a id="PuppeteerSharp_ITracing_StartAsync_PuppeteerSharp_TracingOptions_"></a>

### StartAsync(TracingOptions)


```csharp
Task StartAsync(TracingOptions options = null)
```


Starts tracing.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [TracingOptions](/api/PuppeteerSharp/TracingOptions/) | Tracing options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Start task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITracing.cs#L26)

<a id="PuppeteerSharp_ITracing_StopAsync_"></a>
<a id="PuppeteerSharp_ITracing_StopAsync"></a>

### StopAsync()


```csharp
Task<string> StopAsync()
```


Stops tracing.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Stop task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ITracing.cs#L32)
