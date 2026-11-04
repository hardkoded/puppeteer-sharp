---
title: "WebMcpTool"
description: "Represents a registered WebMCP tool available on the page."
editUrl: false
prev: false
next: false
sidebar:
  label: "WebMcpTool"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_WebMcpTool"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class WebMcpTool
```


Represents a registered WebMCP tool available on the page.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpTool.cs#L32)

## Properties

<a id="PuppeteerSharp_Cdp_WebMcpTool_Annotations_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpTool_Annotations"></a>

### Annotations


```csharp
public WebMcpAnnotation Annotations { get; init; }
```


Optional annotations for the tool.

**Value:** [WebMcpAnnotation](/api/PuppeteerSharp.Cdp/WebMcpAnnotation/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpTool.cs#L44)

<a id="PuppeteerSharp_Cdp_WebMcpTool_Description_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpTool_Description"></a>

### Description


```csharp
public string Description { get; init; }
```


Tool description.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpTool.cs#L38)

<a id="PuppeteerSharp_Cdp_WebMcpTool_Frame_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpTool_Frame"></a>

### Frame


```csharp
public IFrame Frame { get; init; }
```


Frame the tool was defined for.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpTool.cs#L47)

<a id="PuppeteerSharp_Cdp_WebMcpTool_InputSchema_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpTool_InputSchema"></a>

### InputSchema


```csharp
public object InputSchema { get; init; }
```


Schema for the tool's input parameters.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpTool.cs#L41)

<a id="PuppeteerSharp_Cdp_WebMcpTool_Location_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpTool_Location"></a>

### Location


```csharp
public ConsoleMessageLocation Location { get; init; }
```


Source location that defined the tool (if available).

**Value:** [ConsoleMessageLocation](/api/PuppeteerSharp/ConsoleMessageLocation/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpTool.cs#L50)

<a id="PuppeteerSharp_Cdp_WebMcpTool_Name_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpTool_Name"></a>

### Name


```csharp
public string Name { get; init; }
```


Tool name.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpTool.cs#L35)

## Methods

<a id="PuppeteerSharp_Cdp_WebMcpTool_ExecuteAsync_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpTool_ExecuteAsync_System_Object_PuppeteerSharp_Cdp_WebMcpToolExecuteOptions_"></a>

### ExecuteAsync(object, WebMcpToolExecuteOptions)


```csharp
public Task<WebMcpToolCallResult> ExecuteAsync(object input = null, WebMcpToolExecuteOptions options = null)
```


Executes the tool with the given input parameters.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `input` | [object](https://learn.microsoft.com/dotnet/api/system.object) | Input object matching the tool's `inputSchema`. Defaults to empty object. |
| `options` | [WebMcpToolExecuteOptions](/api/PuppeteerSharp.Cdp/WebMcpToolExecuteOptions/) | Optional execution options, including a cancellation token. |

**Returns:** [Task&lt;WebMcpToolCallResult&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task resolving to the tool call result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpTool.cs#L60)
