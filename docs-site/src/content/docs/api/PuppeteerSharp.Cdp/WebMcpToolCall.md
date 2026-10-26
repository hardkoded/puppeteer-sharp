---
title: "WebMcpToolCall"
description: "Represents a pending or completed WebMCP tool invocation call."
editUrl: false
prev: false
next: false
sidebar:
  label: "WebMcpToolCall"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_WebMcpToolCall"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class WebMcpToolCall
```


Represents a pending or completed WebMCP tool invocation call.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCall.cs#L31)

## Properties

<a id="PuppeteerSharp_Cdp_WebMcpToolCall_Id_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolCall_Id"></a>

### Id


```csharp
public string Id { get; }
```


Tool invocation identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCall.cs#L48)

<a id="PuppeteerSharp_Cdp_WebMcpToolCall_Input_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolCall_Input"></a>

### Input


```csharp
public object Input { get; }
```


The input parameters used for the call.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCall.cs#L51)

<a id="PuppeteerSharp_Cdp_WebMcpToolCall_Tool_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolCall_Tool"></a>

### Tool


```csharp
public WebMcpTool Tool { get; }
```


Tool that was called.

**Value:** [WebMcpTool](/api/PuppeteerSharp.Cdp/WebMcpTool/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCall.cs#L54)
