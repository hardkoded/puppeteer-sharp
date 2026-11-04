---
title: "PermissionState"
description: "Permission state for PermissionEntry%5b%5d)."
editUrl: false
prev: false
next: false
sidebar:
  label: "PermissionState"
  hidden: true
---

<a id="PuppeteerSharp_PermissionState"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<PermissionState>))]
public enum PermissionState
```


Permission state for `PermissionEntry%5b%5d)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionState.cs#L10)

## Fields

<a id="PuppeteerSharp_PermissionState_Denied"></a>

### Denied


```csharp
[EnumMember(Value = "denied")]
Denied = 1
```


The permission is denied.

**Value:** [PermissionState](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionState.cs#L22)

<a id="PuppeteerSharp_PermissionState_Granted"></a>

### Granted


```csharp
[EnumMember(Value = "granted")]
Granted = 0
```


The permission is granted.

**Value:** [PermissionState](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionState.cs#L16)

<a id="PuppeteerSharp_PermissionState_Prompt"></a>

### Prompt


```csharp
[EnumMember(Value = "prompt")]
Prompt = 2
```


The permission is in the prompt state (default).

**Value:** [PermissionState](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionState.cs#L28)
