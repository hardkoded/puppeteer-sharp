---
title: "WebMcpToolCallResult"
description: "Result of a WebMCP tool invocation."
editUrl: false
prev: false
next: false
sidebar:
  label: "WebMcpToolCallResult"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class WebMcpToolCallResult
```


Result of a WebMCP tool invocation.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCallResult.cs#L30)

## Properties

<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_Call_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_Call"></a>

### Call


```csharp
public WebMcpToolCall Call { get; init; }
```


The corresponding tool call if available.

**Value:** [WebMcpToolCall](/api/PuppeteerSharp.Cdp/WebMcpToolCall/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCallResult.cs#L36)

<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_ErrorText_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_ErrorText"></a>

### ErrorText


```csharp
public string ErrorText { get; init; }
```


Error text.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCallResult.cs#L39)

<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_Exception_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_Exception"></a>

### Exception


```csharp
public RemoteObject Exception { get; init; }
```


The exception object, if the JavaScript tool threw an error.

**Value:** [RemoteObject](/api/PuppeteerSharp.Cdp.Messaging/RemoteObject/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCallResult.cs#L42)

<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_Id_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_Id"></a>

### Id


```csharp
public string Id { get; init; }
```


Tool invocation identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCallResult.cs#L33)

<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_Output_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_Output"></a>

### Output


```csharp
public object Output { get; init; }
```


Output delivered to the agent. Present only when Status is Completed.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCallResult.cs#L45)

<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_Status_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolCallResult_Status"></a>

### Status


```csharp
public WebMcpInvocationStatus Status { get; init; }
```


Status of the invocation.

**Value:** [WebMcpInvocationStatus](/api/PuppeteerSharp.Cdp/WebMcpInvocationStatus/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolCallResult.cs#L48)
