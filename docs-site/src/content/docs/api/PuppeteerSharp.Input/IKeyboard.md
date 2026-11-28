---
title: "IKeyboard"
description: "Keyboard provides an api for managing a virtual keyboard. The high level api is TypeAsync(), which takes raw characters and generates proper keydown, keypres..."
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


Keyboard provides an api for managing a virtual keyboard. The high level api is `TypeAsync()`, which takes raw characters and generates proper keydown, keypress/input, and keyup events on your page.

For finer control, you can use `DownAsync()`, [UpAsync(string)](#PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_), and [SendCharacterAsync(string)](#PuppeteerSharp_Input_IKeyboard_SendCharacterAsync_System_String_) to manually fire events as if they were generated from a real keyboard.

**Derived:** [CdpKeyboard](/api/PuppeteerSharp.Cdp/CdpKeyboard/), [Keyboard](/api/PuppeteerSharp.Input/Keyboard/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IKeyboard.cs#L10)

## Methods

<a id="PuppeteerSharp_Input_IKeyboard_DownAsync_"></a>
<a id="PuppeteerSharp_Input_IKeyboard_DownAsync_System_String_PuppeteerSharp_Input_DownOptions_"></a>

### DownAsync(string, DownOptions)


```csharp
Task DownAsync(string key, DownOptions options = null)
```


Dispatches a `keydown` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `key` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of key to press, such as `ArrowLeft`. `KeyDefinitions` for a list of all key names. |
| `options` | [DownOptions](/api/PuppeteerSharp.Input/DownOptions/) | down options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If `key` is a single character and no modifier keys besides `Shift` are being held down, a `keypress`/`input` event will also generated. The `text` option can be specified to force an input event to be generated.
If `key` is a modifier key, `Shift`, `Meta`, `Control`, or `Alt`, subsequent key presses will be sent with that modifier active. To release the modifier key, use [UpAsync(string)](#PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_)
After the key is pressed once, subsequent calls to `DownAsync()` will have [repeat](https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/repeat) set to `true`. To release the key, use [UpAsync(string)](#PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IKeyboard.cs#L23)

<a id="PuppeteerSharp_Input_IKeyboard_PressAsync_"></a>
<a id="PuppeteerSharp_Input_IKeyboard_PressAsync_System_String_PuppeteerSharp_Input_PressOptions_"></a>

### PressAsync(string, PressOptions)


```csharp
Task PressAsync(string key, PressOptions options = null)
```


Shortcut for `DownAsync()` and [UpAsync(string)](#PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `key` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of key to press, such as `ArrowLeft`. `KeyDefinitions` for a list of all key names. |
| `options` | [PressOptions](/api/PuppeteerSharp.Input/PressOptions/) | press options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If `key` is a single character and no modifier keys besides `Shift` are being held down, a `keypress`/`input` event will also generated. The [Text](/api/PuppeteerSharp.Input/DownOptions/#PuppeteerSharp_Input_DownOptions_Text) option can be specified to force an input event to be generated.
Modifier keys DO effect `PressAsync()`. Holding down `Shift` will type the text in upper case.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IKeyboard.cs#L35)

<a id="PuppeteerSharp_Input_IKeyboard_SendCharacterAsync_"></a>
<a id="PuppeteerSharp_Input_IKeyboard_SendCharacterAsync_System_String_"></a>

### SendCharacterAsync(string)


```csharp
Task SendCharacterAsync(string charText)
```


Dispatches a `keypress` and `input` event. This does not send a `keydown` or `keyup` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `charText` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Character to send into the page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IKeyboard.cs#L42)

<a id="PuppeteerSharp_Input_IKeyboard_TypeAsync_"></a>
<a id="PuppeteerSharp_Input_IKeyboard_TypeAsync_System_String_PuppeteerSharp_Input_TypeOptions_"></a>

### TypeAsync(string, TypeOptions)


```csharp
Task TypeAsync(string text, TypeOptions options = null)
```


Sends a `keydown`, `keypress`/`input`, and `keyup` event for each character in the text.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `text` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A text to type into a focused element. |
| `options` | [TypeOptions](/api/PuppeteerSharp.Input/TypeOptions/) | type options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

To press a special key, like `Control` or `ArrowDown`, use `PressAsync()`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IKeyboard.cs#L53)

<a id="PuppeteerSharp_Input_IKeyboard_UpAsync_"></a>
<a id="PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_"></a>

### UpAsync(string)


```csharp
Task UpAsync(string key)
```


Dispatches a `keyup` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `key` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of key to release, such as `ArrowLeft`. See `KeyDefinitions` for a list of all key names. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IKeyboard.cs#L60)
