---
title: "PressOptions"
description: "options to use when pressing a key."
editUrl: false
prev: false
next: false
sidebar:
  label: "PressOptions"
  hidden: true
---

<a id="PuppeteerSharp_Input_PressOptions"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
public class PressOptions : DownOptions
```


options to use when pressing a key.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [DownOptions](/api/PuppeteerSharp.Input/DownOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/PressOptions.cs#L8)

## Properties

<a id="PuppeteerSharp_Input_PressOptions_Delay_"></a>
<a id="PuppeteerSharp_Input_PressOptions_Delay"></a>

### Delay


```csharp
public int? Delay { get; set; }
```


Time to wait between `keydown` and `keyup` in milliseconds. Defaults to 0.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/PressOptions.cs#L13)
