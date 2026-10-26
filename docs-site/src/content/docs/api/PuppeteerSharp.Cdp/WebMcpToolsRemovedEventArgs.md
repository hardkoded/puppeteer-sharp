---
title: "WebMcpToolsRemovedEventArgs"
description: "Event args for when WebMCP tools are removed."
editUrl: false
prev: false
next: false
sidebar:
  label: "WebMcpToolsRemovedEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_WebMcpToolsRemovedEventArgs"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class WebMcpToolsRemovedEventArgs : EventArgs
```


Event args for when WebMCP tools are removed.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolsRemovedEventArgs.cs#L30)

## Properties

<a id="PuppeteerSharp_Cdp_WebMcpToolsRemovedEventArgs_Tools_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolsRemovedEventArgs_Tools"></a>

### Tools


```csharp
public WebMcpTool[] Tools { get; init; }
```


Array of tools that were removed.

**Value:** `WebMcpTool\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolsRemovedEventArgs.cs#L35)
