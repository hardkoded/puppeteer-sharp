---
title: "IPuppeteerUtilWrapper"
description: "A class that contains a method to get the PuppeteerUtil handle."
editUrl: false
prev: false
next: false
sidebar:
  label: "IPuppeteerUtilWrapper"
  hidden: true
---

<a id="PuppeteerSharp_IPuppeteerUtilWrapper"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IPuppeteerUtilWrapper
```


A class that contains a method to get the PuppeteerUtil handle.

**Derived:** [ExecutionContext](/api/PuppeteerSharp/ExecutionContext/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPuppeteerUtilWrapper.cs#L30)

## Methods

<a id="PuppeteerSharp_IPuppeteerUtilWrapper_GetPuppeteerUtilAsync_"></a>
<a id="PuppeteerSharp_IPuppeteerUtilWrapper_GetPuppeteerUtilAsync"></a>

### GetPuppeteerUtilAsync()


```csharp
Task<IJSHandle> GetPuppeteerUtilAsync()
```


Gets the PuppeteerUtil handle.

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The PuppeteerUtil handle.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPuppeteerUtilWrapper.cs#L36)
