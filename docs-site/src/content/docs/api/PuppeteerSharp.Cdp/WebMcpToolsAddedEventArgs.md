---
title: "WebMcpToolsAddedEventArgs"
description: "Event args for when WebMCP tools are added."
editUrl: false
prev: false
next: false
sidebar:
  label: "WebMcpToolsAddedEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_WebMcpToolsAddedEventArgs"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class WebMcpToolsAddedEventArgs : EventArgs
```


Event args for when WebMCP tools are added.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolsAddedEventArgs.cs#L30)

## Properties

<a id="PuppeteerSharp_Cdp_WebMcpToolsAddedEventArgs_Tools_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpToolsAddedEventArgs_Tools"></a>

### Tools


```csharp
public WebMcpTool[] Tools { get; init; }
```


Array of tools that were added.

**Value:** `WebMcpTool\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpToolsAddedEventArgs.cs#L35)
