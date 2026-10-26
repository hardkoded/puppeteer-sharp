---
title: "CustomQueryHandler"
description: "Contains two functions queryOne and queryAll to be used as custom query handlers The functions queryOne and queryAll are executed in the page context."
editUrl: false
prev: false
next: false
sidebar:
  label: "CustomQueryHandler"
  hidden: true
---

<a id="PuppeteerSharp_CustomQueryHandler"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class CustomQueryHandler
```


Contains two functions `queryOne` and `queryAll` to be used as custom query handlers
The functions `queryOne` and `queryAll` are executed in the page context.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CustomQueryHandler.cs#L9)

## Properties

<a id="PuppeteerSharp_CustomQueryHandler_QueryAll_"></a>
<a id="PuppeteerSharp_CustomQueryHandler_QueryAll"></a>

### QueryAll


```csharp
public string QueryAll { get; set; }
```


`queryAll` takes the same arguments but should instead return a `NodeListOf of Element` or `Array of Element` with all the elements that match the given query selector.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CustomQueryHandler.cs#L19)

<a id="PuppeteerSharp_CustomQueryHandler_QueryOne_"></a>
<a id="PuppeteerSharp_CustomQueryHandler_QueryOne"></a>

### QueryOne


```csharp
public string QueryOne { get; set; }
```


`queryOne` should take an `Element` and a selector string as argument and return a single `Element` or `null` if no element is found.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CustomQueryHandler.cs#L14)
