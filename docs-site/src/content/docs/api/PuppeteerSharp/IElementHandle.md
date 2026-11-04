---
title: "IElementHandle"
description: "Inherits from IJSHandle. It represents an in-page DOM element. ElementHandles can be created by QuerySelectorAsync(string) or QuerySelectorAllAsync(string)."
editUrl: false
prev: false
next: false
sidebar:
  label: "IElementHandle"
  hidden: true
---

<a id="PuppeteerSharp_IElementHandle"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IElementHandle : IJSHandle, IAsyncDisposable
```


Inherits from [IJSHandle](/api/PuppeteerSharp/IJSHandle/). It represents an in-page DOM element.
ElementHandles can be created by [QuerySelectorAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAsync_System_String_) or [QuerySelectorAllAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAllAsync_System_String_).

**Derived:** [CdpElementHandle](/api/PuppeteerSharp.Cdp/CdpElementHandle/), [ElementHandle](/api/PuppeteerSharp/ElementHandle/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L12)
