---
title: "IJSHandle"
description: "IJSHandle represents an in-page JavaScript object. JSHandles can be created with the EvaluateExpressionHandleAsync(string) and Object%5b%5d) methods."
editUrl: false
prev: false
next: false
sidebar:
  label: "IJSHandle"
  hidden: true
---

<a id="PuppeteerSharp_IJSHandle"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IJSHandle : IAsyncDisposable
```


IJSHandle represents an in-page JavaScript object. JSHandles can be created with the [EvaluateExpressionHandleAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EvaluateExpressionHandleAsync_System_String_) and `Object%5b%5d)` methods.

**Derived:** [CdpElementHandle](/api/PuppeteerSharp.Cdp/CdpElementHandle/), [CdpJSHandle](/api/PuppeteerSharp.Cdp/CdpJSHandle/), [ElementHandle](/api/PuppeteerSharp/ElementHandle/), [JSHandle](/api/PuppeteerSharp/JSHandle/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L12)
