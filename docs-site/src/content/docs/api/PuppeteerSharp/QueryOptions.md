---
title: "QueryOptions"
description: "Options for query operations like QueryOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "QueryOptions"
  hidden: true
---

<a id="PuppeteerSharp_QueryOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class QueryOptions
```


Options for query operations like `QueryOptions)`.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/QueryOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_QueryOptions_Isolate_"></a>
<a id="PuppeteerSharp_QueryOptions_Isolate"></a>

### Isolate


```csharp
public bool Isolate { get; set; }
```


Whether to run the query in isolation. When returning many elements
from `QueryOptions)` or similar methods,
it might be useful to turn off the isolation to improve performance.
By default, the querying code will be executed in a separate sandbox realm.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — Defaults to `true`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/QueryOptions.cs#L15)
