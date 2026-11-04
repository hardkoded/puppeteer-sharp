---
title: "DialogEventArgs"
description: "Dialog arguments."
editUrl: false
prev: false
next: false
sidebar:
  label: "DialogEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_DialogEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class DialogEventArgs : EventArgs
```


[Dialog](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Dialog) arguments.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DialogEventArgs.cs#L8)

## Constructors

<a id="PuppeteerSharp_DialogEventArgs__ctor_"></a>
<a id="PuppeteerSharp_DialogEventArgs__ctor_PuppeteerSharp_Dialog_"></a>

### DialogEventArgs(Dialog)


```csharp
public DialogEventArgs(Dialog dialog)
```


Initializes a new instance of the [DialogEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `dialog` | [Dialog](/api/PuppeteerSharp/Dialog/) | Dialog. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DialogEventArgs.cs#L14)

## Properties

<a id="PuppeteerSharp_DialogEventArgs_Dialog_"></a>
<a id="PuppeteerSharp_DialogEventArgs_Dialog"></a>

### Dialog


```csharp
public Dialog Dialog { get; }
```


Dialog data.

**Value:** [Dialog](/api/PuppeteerSharp/Dialog/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DialogEventArgs.cs#L19)
