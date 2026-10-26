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
