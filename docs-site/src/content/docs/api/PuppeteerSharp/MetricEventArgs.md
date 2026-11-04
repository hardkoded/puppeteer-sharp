---
title: "MetricEventArgs"
description: "Metrics arguments."
editUrl: false
prev: false
next: false
sidebar:
  label: "MetricEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_MetricEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class MetricEventArgs : EventArgs
```


[Metrics](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Metrics) arguments.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MetricEventArgs.cs#L9)

## Constructors

<a id="PuppeteerSharp_MetricEventArgs__ctor_"></a>
<a id="PuppeteerSharp_MetricEventArgs__ctor_System_String_System_Collections_Generic_Dictionary_System_String_System_Decimal__"></a>

### `MetricEventArgs(string, Dictionary<string, decimal>)`


```csharp
public MetricEventArgs(string title, Dictionary<string, decimal> metrics)
```


Initializes a new instance of the [MetricEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `title` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Title. |
| `metrics` | [Dictionary&lt;string, decimal&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) | Metrics. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MetricEventArgs.cs#L16)

## Properties

<a id="PuppeteerSharp_MetricEventArgs_Metrics_"></a>
<a id="PuppeteerSharp_MetricEventArgs_Metrics"></a>

### Metrics


```csharp
public Dictionary<string, decimal> Metrics { get; }
```


Gets the metrics.

**Value:** [Dictionary&lt;string, decimal&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) — The metrics.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MetricEventArgs.cs#L32)

<a id="PuppeteerSharp_MetricEventArgs_Title_"></a>
<a id="PuppeteerSharp_MetricEventArgs_Title"></a>

### Title


```csharp
public string Title { get; }
```


Gets the title.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The title.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MetricEventArgs.cs#L26)
