---
title: "DebugInfo"
description: "Debug information about the browser. Currently includes pending protocol calls. In the future, more info might be added."
editUrl: false
prev: false
next: false
sidebar:
  label: "DebugInfo"
  hidden: true
---

<a id="PuppeteerSharp_DebugInfo"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class DebugInfo
```


Debug information about the browser.
Currently includes pending protocol calls. In the future, more info might be added.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DebugInfo.cs#L10)

## Properties

<a id="PuppeteerSharp_DebugInfo_PendingProtocolErrors_"></a>
<a id="PuppeteerSharp_DebugInfo_PendingProtocolErrors"></a>

### PendingProtocolErrors


```csharp
public IReadOnlyList<string> PendingProtocolErrors { get; init; }
```


Gets the list of pending protocol errors.

**Value:** [IReadOnlyList&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DebugInfo.cs#L15)
