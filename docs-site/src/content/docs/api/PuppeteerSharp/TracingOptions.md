---
title: "TracingOptions"
description: "Tracing options used on StartAsync(TracingOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "TracingOptions"
  hidden: true
---

<a id="PuppeteerSharp_TracingOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class TracingOptions
```


Tracing options used on [StartAsync(TracingOptions)](/api/PuppeteerSharp/ITracing/#PuppeteerSharp_ITracing_StartAsync_PuppeteerSharp_TracingOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TracingOptions.cs#L8)

## Properties

<a id="PuppeteerSharp_TracingOptions_BufferSize_"></a>
<a id="PuppeteerSharp_TracingOptions_BufferSize"></a>

### BufferSize


```csharp
public int? BufferSize { get; set; }
```


Gets or sets the size of the trace buffer in kilobytes.
If not specified or zero is passed, the default value of 200 MB
(200,000 KB) is used by Chromium.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32) — The buffer size in kilobytes.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TracingOptions.cs#L37)

<a id="PuppeteerSharp_TracingOptions_Categories_"></a>
<a id="PuppeteerSharp_TracingOptions_Categories"></a>

### Categories


```csharp
public List<string> Categories { get; set; }
```


Gets or sets custom categories to use instead of default.
To exclude a category, prefix it with `-` (e.g., `-toplevel`).

**Value:** [List&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.list-1) — The categories.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TracingOptions.cs#L29)

<a id="PuppeteerSharp_TracingOptions_Path_"></a>
<a id="PuppeteerSharp_TracingOptions_Path"></a>

### Path


```csharp
public string Path { get; set; }
```


Gets or sets a path to write the trace file to.
If no path is specified, the trace will not be written to disk, but can
still be retrieved as a string from [StopAsync()](/api/PuppeteerSharp/ITracing/#PuppeteerSharp_ITracing_StopAsync).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The path.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TracingOptions.cs#L16)

<a id="PuppeteerSharp_TracingOptions_Screenshots_"></a>
<a id="PuppeteerSharp_TracingOptions_Screenshots"></a>

### Screenshots


```csharp
public bool Screenshots { get; set; }
```


Gets or sets a value indicating whether Tracing should capture screenshots in the trace.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — Screenshots option.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TracingOptions.cs#L22)
