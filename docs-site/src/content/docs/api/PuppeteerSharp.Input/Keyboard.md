---
title: "Keyboard"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Keyboard"
  hidden: true
---

<a id="PuppeteerSharp_Input_Keyboard"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
public abstract class Keyboard : IKeyboard
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IKeyboard](/api/PuppeteerSharp.Input/IKeyboard/)

**Derived:** [CdpKeyboard](/api/PuppeteerSharp.Cdp/CdpKeyboard/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Keyboard.cs#L6)

## Methods

<a id="PuppeteerSharp_Input_Keyboard_DownAsync_"></a>
<a id="PuppeteerSharp_Input_Keyboard_DownAsync_System_String_PuppeteerSharp_Input_DownOptions_"></a>

### DownAsync(string, DownOptions)


```csharp
public abstract Task DownAsync(string key, DownOptions options = null)
```


Dispatches a `keydown` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `key` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of key to press, such as `ArrowLeft`. `KeyDefinitions` for a list of all key names. |
| `options` | [DownOptions](/api/PuppeteerSharp.Input/DownOptions/) | down options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If `key` is a single character and no modifier keys besides `Shift` are being held down, a `keypress`/`input` event will also generated. The `text` option can be specified to force an input event to be generated.
If `key` is a modifier key, `Shift`, `Meta`, `Control`, or `Alt`, subsequent key presses will be sent with that modifier active. To release the modifier key, use [UpAsync(string)](/api/PuppeteerSharp.Input/IKeyboard/#PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_)
After the key is pressed once, subsequent calls to `DownOptions)` will have &lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/repeat"&gt;repeat&lt;/a&gt; set to `true`. To release the key, use [UpAsync(string)](/api/PuppeteerSharp.Input/IKeyboard/#PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Keyboard.cs#L11)

<a id="PuppeteerSharp_Input_Keyboard_PressAsync_"></a>
<a id="PuppeteerSharp_Input_Keyboard_PressAsync_System_String_PuppeteerSharp_Input_PressOptions_"></a>

### PressAsync(string, PressOptions)


```csharp
public abstract Task PressAsync(string key, PressOptions options = null)
```


Shortcut for `DownOptions)` and [UpAsync(string)](/api/PuppeteerSharp.Input/IKeyboard/#PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `key` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of key to press, such as `ArrowLeft`. `KeyDefinitions` for a list of all key names. |
| `options` | [PressOptions](/api/PuppeteerSharp.Input/PressOptions/) | press options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

If &lt;code class="paramref"&gt;key&lt;/code&gt; is a single character and no modifier keys besides `Shift` are being held down, a `keypress`/`input` event will also generated. The [Text](/api/PuppeteerSharp.Input/DownOptions/#PuppeteerSharp_Input_DownOptions_Text) option can be specified to force an input event to be generated.
Modifier keys DO effect `PressOptions)`. Holding down `Shift` will type the text in upper case.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Keyboard.cs#L23)

<a id="PuppeteerSharp_Input_Keyboard_SendCharacterAsync_"></a>
<a id="PuppeteerSharp_Input_Keyboard_SendCharacterAsync_System_String_"></a>

### SendCharacterAsync(string)


```csharp
public abstract Task SendCharacterAsync(string charText)
```


Dispatches a `keypress` and `input` event. This does not send a `keydown` or `keyup` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `charText` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Character to send into the page. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Keyboard.cs#L17)

<a id="PuppeteerSharp_Input_Keyboard_TypeAsync_"></a>
<a id="PuppeteerSharp_Input_Keyboard_TypeAsync_System_String_PuppeteerSharp_Input_TypeOptions_"></a>

### TypeAsync(string, TypeOptions)


```csharp
public abstract Task TypeAsync(string text, TypeOptions options = null)
```


Sends a `keydown`, `keypress`/`input`, and `keyup` event for each character in the text.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `text` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A text to type into a focused element. |
| `options` | [TypeOptions](/api/PuppeteerSharp.Input/TypeOptions/) | type options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

To press a special key, like `Control` or `ArrowDown`, use `PressOptions)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Keyboard.cs#L20)

<a id="PuppeteerSharp_Input_Keyboard_UpAsync_"></a>
<a id="PuppeteerSharp_Input_Keyboard_UpAsync_System_String_"></a>

### UpAsync(string)


```csharp
public abstract Task UpAsync(string key)
```


Dispatches a `keyup` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `key` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of key to release, such as `ArrowLeft`. See `KeyDefinitions` for a list of all key names. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Keyboard.cs#L14)
