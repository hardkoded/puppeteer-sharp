---
title: "IRequest"
description: "Whenever the page sends a request, the following events are emitted by puppeteer's page: Request emitted when the request is issued by the page. Response emi..."
editUrl: false
prev: false
next: false
sidebar:
  label: "IRequest"
  hidden: true
---

<a id="PuppeteerSharp_IRequest"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IRequest
```


Whenever the page sends a request, the following events are emitted by puppeteer's page:
[Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request) emitted when the request is issued by the page.
[Response](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Response) emitted when/if the response is received for the request.
[RequestFinished](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_RequestFinished) emitted when the response body is downloaded and the request is complete.

If request fails at some point, then instead of [RequestFinished](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_RequestFinished) event (and possibly instead of [Response](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Response) event), the [RequestFailed](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_RequestFailed) event is emitted.

If request gets a 'redirect' response, the request is successfully finished with the [RequestFinished](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_RequestFinished) event, and a new request is issued to a redirected url.

**Derived:** [BidiHttpRequest](/api/PuppeteerSharp.Bidi/BidiHttpRequest/), [CdpHttpRequest](/api/PuppeteerSharp.Cdp/CdpHttpRequest/), [Request&lt;TResponse&gt;](/api/PuppeteerSharp/Request-1/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L17)
