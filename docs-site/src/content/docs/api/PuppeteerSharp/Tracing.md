---
title: "Tracing"
description: "You can use StartAsync(TracingOptions) and StopAsync() to create a trace file which can be opened in Chrome DevTools or timeline viewer."
editUrl: false
prev: false
next: false
sidebar:
  label: "Tracing"
  hidden: true
---

<a id="PuppeteerSharp_Tracing"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class Tracing : ITracing
```


You can use [StartAsync(TracingOptions)](/api/PuppeteerSharp/ITracing/#PuppeteerSharp_ITracing_StartAsync_PuppeteerSharp_TracingOptions_) and [StopAsync()](/api/PuppeteerSharp/ITracing/#PuppeteerSharp_ITracing_StopAsync) to create a trace file which can be opened in Chrome DevTools or timeline viewer.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [ITracing](/api/PuppeteerSharp/ITracing/)

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Tracing.cs#L27)

## Methods

<a id="PuppeteerSharp_Tracing_StartAsync_"></a>
<a id="PuppeteerSharp_Tracing_StartAsync_PuppeteerSharp_TracingOptions_"></a>

### StartAsync(TracingOptions)


```csharp
public Task StartAsync(TracingOptions options = null)
```


Starts tracing.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [TracingOptions](/api/PuppeteerSharp/TracingOptions/) | Tracing options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Start task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Tracing.cs#L62)

<a id="PuppeteerSharp_Tracing_StopAsync_"></a>
<a id="PuppeteerSharp_Tracing_StopAsync"></a>

### StopAsync()


```csharp
public Task<string> StopAsync()
```


Stops tracing.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Stop task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Tracing.cs#L101)
