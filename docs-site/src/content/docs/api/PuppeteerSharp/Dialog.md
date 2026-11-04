---
title: "Dialog"
description: "Dialog objects are dispatched by page via the 'dialog' event."
editUrl: false
prev: false
next: false
sidebar:
  label: "Dialog"
  hidden: true
---

<a id="PuppeteerSharp_Dialog"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class Dialog
```


[Dialog](./) objects are dispatched by page via the 'dialog' event.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Derived:** [BidiDialog](/api/PuppeteerSharp.Bidi/BidiDialog/), [CdpDialog](/api/PuppeteerSharp.Cdp/CdpDialog/)

**Example**

An example of using Dialog class:


```csharp
Page.Dialog += async (sender, e) =>
{
    await e.Dialog.Accept();
}
await Page.EvaluateExpressionAsync("alert('yo');");
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Dialog.cs#L21)

## Constructors

<a id="PuppeteerSharp_Dialog__ctor_"></a>
<a id="PuppeteerSharp_Dialog__ctor_PuppeteerSharp_DialogType_System_String_System_String_"></a>

### Dialog(DialogType, string, string)


```csharp
public Dialog(DialogType type, string message, string defaultValue)
```


Initializes a new instance of the [Dialog](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `type` | [DialogType](/api/PuppeteerSharp/DialogType/) | Type. |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `defaultValue` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Default value. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Dialog.cs#L31)

## Properties

<a id="PuppeteerSharp_Dialog_DefaultValue_"></a>
<a id="PuppeteerSharp_Dialog_DefaultValue"></a>

### DefaultValue


```csharp
public string DefaultValue { get; set; }
```


If dialog is prompt, returns default prompt value. Otherwise, returns empty string.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The default value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Dialog.cs#L48)

<a id="PuppeteerSharp_Dialog_DialogType_"></a>
<a id="PuppeteerSharp_Dialog_DialogType"></a>

### DialogType


```csharp
public DialogType DialogType { get; set; }
```


Dialog's type, can be one of alert, beforeunload, confirm or prompt.

**Value:** [DialogType](/api/PuppeteerSharp/DialogType/) — The type of the dialog.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Dialog.cs#L42)

<a id="PuppeteerSharp_Dialog_Handled_"></a>
<a id="PuppeteerSharp_Dialog_Handled"></a>

### Handled


```csharp
public bool Handled { get; }
```


Gets a value indicating whether the dialog has been handled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the dialog has already been accepted or dismissed; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Dialog.cs#L60)

<a id="PuppeteerSharp_Dialog_Message_"></a>
<a id="PuppeteerSharp_Dialog_Message"></a>

### Message


```csharp
public string Message { get; set; }
```


A message displayed in the dialog.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The message.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Dialog.cs#L54)

## Methods

<a id="PuppeteerSharp_Dialog_Accept_"></a>
<a id="PuppeteerSharp_Dialog_Accept_System_String_"></a>

### Accept(string)


```csharp
public Task Accept(string promptText = "")
```


Accept the Dialog.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `promptText` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A text to enter in prompt. Does not cause any effects if the dialog's type is not prompt. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the dialog has been accepted.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Dialog.cs#L73)

<a id="PuppeteerSharp_Dialog_Dismiss_"></a>
<a id="PuppeteerSharp_Dialog_Dismiss"></a>

### Dismiss()


```csharp
public Task Dismiss()
```


Dismiss the dialog.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the dialog has been dismissed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Dialog.cs#L88)
