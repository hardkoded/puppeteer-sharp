---
title: "DialogType"
description: "Dialog type."
editUrl: false
prev: false
next: false
sidebar:
  label: "DialogType"
  hidden: true
---

<a id="PuppeteerSharp_DialogType"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<DialogType>))]
public enum DialogType
```


Dialog type.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DialogType.cs#L11)

## Fields

<a id="PuppeteerSharp_DialogType_Alert"></a>

### Alert


```csharp
Alert = 0
```


Alert dialog.

**Value:** [DialogType](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DialogType.cs#L17)

<a id="PuppeteerSharp_DialogType_BeforeUnload"></a>

### BeforeUnload


```csharp
[EnumMember(Value = "beforeunload")]
BeforeUnload = 3
```


Before unload dialog.

**Value:** [DialogType](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DialogType.cs#L32)

<a id="PuppeteerSharp_DialogType_Confirm"></a>

### Confirm


```csharp
Confirm = 2
```


Confirm dialog.

**Value:** [DialogType](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DialogType.cs#L27)

<a id="PuppeteerSharp_DialogType_Prompt"></a>

### Prompt


```csharp
Prompt = 1
```


Prompt dialog.

**Value:** [DialogType](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DialogType.cs#L22)
