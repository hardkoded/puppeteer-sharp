---
title: "DownOptions"
description: "options to use with DownOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "DownOptions"
  hidden: true
---

<a id="PuppeteerSharp_Input_DownOptions"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
public class DownOptions
```


options to use with `DownOptions)`.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Derived:** [PressOptions](/api/PuppeteerSharp.Input/PressOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/DownOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_Input_DownOptions_Commands_"></a>
<a id="PuppeteerSharp_Input_DownOptions_Commands"></a>

### Commands


```csharp
public string[] Commands { get; set; }
```


If specified, the commands of keyboard shortcuts.
See &lt;a href="https://source.chromium.org/chromium/chromium/src/+/main:third_party/blink/renderer/core/editing/commands/editor_command_names.h"&gt;Chromium Source Code&lt;/a&gt; for valid command names.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/DownOptions.cs#L17)

<a id="PuppeteerSharp_Input_DownOptions_Text_"></a>
<a id="PuppeteerSharp_Input_DownOptions_Text"></a>

### Text


```csharp
public string Text { get; set; }
```


If specified, generates an input event with this text.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/DownOptions.cs#L11)
