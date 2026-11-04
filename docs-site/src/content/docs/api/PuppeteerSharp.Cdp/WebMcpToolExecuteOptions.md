---
title: "WebMcpToolExecuteOptions"
description: "Options for WebMcpToolExecuteOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "WebMcpToolExecuteOptions"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_WebMcpToolExecuteOptions"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class WebMcpToolExecuteOptions
```


Options for `WebMcpToolExecuteOptions)`.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolExecuteOptions.cs#L30)

## Properties

<a id="PuppeteerSharp_Cdp_WebMcpToolExecuteOptions_CancellationToken_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolExecuteOptions_CancellationToken"></a>

### CancellationToken


```csharp
public CancellationToken CancellationToken { get; set; }
```


A [CancellationToken](#PuppeteerSharp_Cdp_WebMcpToolExecuteOptions_CancellationToken) that allows you to cancel the tool execution.

**Value:** [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

This is the .NET equivalent of the upstream AbortController/AbortSignal pattern.
When the token is cancelled, PuppeteerSharp requests cancellation via CDP and the
execution resolves with [Canceled](/api/PuppeteerSharp.Cdp/WebMcpInvocationStatus/#PuppeteerSharp_Cdp_WebMcpInvocationStatus_Canceled) rather than
throwing [OperationCanceledException](https://learn.microsoft.com/dotnet/api/system.operationcanceledexception).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolExecuteOptions.cs#L41)
