---
title: "AdapterState"
description: "Emulated bluetooth adapter state."
editUrl: false
prev: false
next: false
sidebar:
  label: "AdapterState"
  hidden: true
---

<a id="PuppeteerSharp_AdapterState"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<AdapterState>))]
public enum AdapterState
```


Emulated bluetooth adapter state.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AdapterState.cs#L10)

## Fields

<a id="PuppeteerSharp_AdapterState_Absent"></a>

### Absent


```csharp
[EnumMember(Value = "absent")]
Absent = 0
```


The Bluetooth adapter is absent.

**Value:** [AdapterState](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AdapterState.cs#L16)

<a id="PuppeteerSharp_AdapterState_PoweredOff"></a>

### PoweredOff


```csharp
[EnumMember(Value = "powered-off")]
PoweredOff = 1
```


The Bluetooth adapter is present, but powered off.

**Value:** [AdapterState](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AdapterState.cs#L22)

<a id="PuppeteerSharp_AdapterState_PoweredOn"></a>

### PoweredOn


```csharp
[EnumMember(Value = "powered-on")]
PoweredOn = 2
```


The Bluetooth adapter is present, and powered on.

**Value:** [AdapterState](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AdapterState.cs#L28)
