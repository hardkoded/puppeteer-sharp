---
title: "IKeyboard"
description: "Keyboard provides an api for managing a virtual keyboard. The high level api is TypeOptions), which takes raw characters and generates proper keydown, keypre..."
editUrl: false
prev: false
next: false
sidebar:
  label: "IKeyboard"
  hidden: true
---

<a id="PuppeteerSharp_Input_IKeyboard"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
public interface IKeyboard
```


Keyboard provides an api for managing a virtual keyboard. The high level api is `TypeOptions)`, which takes raw characters and generates proper keydown, keypress/input, and keyup events on your page.

For finer control, you can use `DownOptions)`, [UpAsync(string)](#PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_), and [SendCharacterAsync(string)](#PuppeteerSharp_Input_IKeyboard_SendCharacterAsync_System_String_) to manually fire events as if they were generated from a real keyboard.

**Derived:** [CdpKeyboard](/api/PuppeteerSharp.Cdp/CdpKeyboard/), [Keyboard](/api/PuppeteerSharp.Input/Keyboard/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IKeyboard.cs#L10)
