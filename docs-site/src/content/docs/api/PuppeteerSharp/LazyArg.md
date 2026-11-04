---
title: "LazyArg"
description: "A LazyArg is an evaluation argument that will be resolved when the CDP call is built."
editUrl: false
prev: false
next: false
sidebar:
  label: "LazyArg"
  hidden: true
---

<a id="PuppeteerSharp_LazyArg"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public delegate Task<object> LazyArg(IPuppeteerUtilWrapper context)
```


A LazyArg is an evaluation argument that will be resolved when the CDP call is built.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `context` | [IPuppeteerUtilWrapper](/api/PuppeteerSharp/IPuppeteerUtilWrapper/) | Execution context. |

**Returns:** [Task&lt;object&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Resolved argument.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IsolatedWorld.cs#L20)
