---
title: "CdpWebMcp"
description: "Experimental WebMCP API. Requires Chrome 149+ with --enable-features=WebMCP flag."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpWebMcp"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpWebMcp"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Listener subscriptions are disposed in UpdateClient via DisposableActionsStack.")]
public class CdpWebMcp
```


Experimental WebMCP API. Requires Chrome 149+ with
--enable-features=WebMCP flag.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebMcp.cs#L40)

## Events

<a id="PuppeteerSharp_Cdp_CdpWebMcp_ToolInvoked"></a>

### ToolInvoked


```csharp
public event EventHandler<WebMcpToolCall> ToolInvoked
```


Emitted when a tool invocation starts.

**Value:** [EventHandler&lt;WebMcpToolCall&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebMcp.cs#L66)

<a id="PuppeteerSharp_Cdp_CdpWebMcp_ToolResponded"></a>

### ToolResponded


```csharp
public event EventHandler<WebMcpToolCallResult> ToolResponded
```


Emitted when a tool invocation completes or fails.

**Value:** [EventHandler&lt;WebMcpToolCallResult&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebMcp.cs#L69)

<a id="PuppeteerSharp_Cdp_CdpWebMcp_ToolsAdded"></a>

### ToolsAdded


```csharp
public event EventHandler<WebMcpToolsAddedEventArgs> ToolsAdded
```


Emitted when tools are added to the page.

**Value:** [EventHandler&lt;WebMcpToolsAddedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebMcp.cs#L60)

<a id="PuppeteerSharp_Cdp_CdpWebMcp_ToolsRemoved"></a>

### ToolsRemoved


```csharp
public event EventHandler<WebMcpToolsRemovedEventArgs> ToolsRemoved
```


Emitted when tools are removed from the page.

**Value:** [EventHandler&lt;WebMcpToolsRemovedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebMcp.cs#L63)

## Methods

<a id="PuppeteerSharp_Cdp_CdpWebMcp_Tools_"></a>
<a id="PuppeteerSharp_Cdp_CdpWebMcp_Tools"></a>

### Tools()


```csharp
public WebMcpTool[] Tools()
```


Gets all WebMCP tools registered on the page.

**Returns:** `WebMcpTool\[\]` — Array of registered tools.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebMcp.cs#L75)
