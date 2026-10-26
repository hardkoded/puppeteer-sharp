---
title: "WebMcpAnnotation"
description: "Tool annotations for a WebMCP tool."
editUrl: false
prev: false
next: false
sidebar:
  label: "WebMcpAnnotation"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_WebMcpAnnotation"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class WebMcpAnnotation
```


Tool annotations for a WebMCP tool.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpAnnotation.cs#L28)

## Properties

<a id="PuppeteerSharp_Cdp_WebMcpAnnotation_Autosubmit_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpAnnotation_Autosubmit"></a>

### Autosubmit


```csharp
public bool? Autosubmit { get; set; }
```


If the declarative tool was declared with the autosubmit attribute.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpAnnotation.cs#L43)

<a id="PuppeteerSharp_Cdp_WebMcpAnnotation_ReadOnly_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpAnnotation_ReadOnly"></a>

### ReadOnly


```csharp
public bool? ReadOnly { get; set; }
```


A hint indicating that the tool does not modify any state.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpAnnotation.cs#L33)

<a id="PuppeteerSharp_Cdp_WebMcpAnnotation_UntrustedContent_"></a>
<a id="PuppeteerSharp_Cdp_WebMcpAnnotation_UntrustedContent"></a>

### UntrustedContent


```csharp
public bool? UntrustedContent { get; set; }
```


A hint indicating that the tool output may contain untrusted content, ex: UGC, 3rd party data.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/WebMcpAnnotation.cs#L38)
