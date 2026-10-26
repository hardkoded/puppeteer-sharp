---
title: "CdpDialog"
description: "Dialog objects are dispatched by page via the 'dialog' event."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpDialog"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpDialog"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpDialog : Dialog
```


[Dialog](/api/PuppeteerSharp/Dialog/) objects are dispatched by page via the 'dialog' event.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Dialog](/api/PuppeteerSharp/Dialog/)

**Example**

An example of using Dialog class:


```csharp
Page.Dialog += async (sender, e) =>
{
    await e.Dialog.Accept();
}
await Page.EvaluateExpressionAsync("alert('yo');");
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpDialog.cs#L29)

## Constructors

<a id="PuppeteerSharp_Cdp_CdpDialog__ctor_"></a>
<a id="PuppeteerSharp_Cdp_CdpDialog__ctor_PuppeteerSharp_CDPSession_PuppeteerSharp_DialogType_System_String_System_String_"></a>

### CdpDialog(CDPSession, DialogType, string, string)


```csharp
public CdpDialog(CDPSession client, DialogType type, string message, string defaultValue)
```


Initializes a new instance of the [CdpDialog](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `client` | [CDPSession](/api/PuppeteerSharp/CDPSession/) | Client. |
| `type` | [DialogType](/api/PuppeteerSharp/DialogType/) | Type. |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `defaultValue` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Default value. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpDialog.cs#L40)
