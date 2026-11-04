---
title: "DeviceRequestPromptDevice"
description: "Device in a request prompt."
editUrl: false
prev: false
next: false
sidebar:
  label: "DeviceRequestPromptDevice"
  hidden: true
---

<a id="PuppeteerSharp_DeviceRequestPromptDevice"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public record DeviceRequestPromptDevice : IEquatable<DeviceRequestPromptDevice>
```


Device in a request prompt.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;DeviceRequestPromptDevice&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeviceRequestPromptDevice.cs#L6)

## Constructors

<a id="PuppeteerSharp_DeviceRequestPromptDevice__ctor_"></a>
<a id="PuppeteerSharp_DeviceRequestPromptDevice__ctor_System_String_System_String_"></a>

### DeviceRequestPromptDevice(string, string)


```csharp
public DeviceRequestPromptDevice(string Name, string Id)
```


Device in a request prompt.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `Name` | [string](https://learn.microsoft.com/dotnet/api/system.string) |  |
| `Id` | [string](https://learn.microsoft.com/dotnet/api/system.string) |  |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeviceRequestPromptDevice.cs#L6)

## Properties

<a id="PuppeteerSharp_DeviceRequestPromptDevice_Id_"></a>
<a id="PuppeteerSharp_DeviceRequestPromptDevice_Id"></a>

### Id


```csharp
public string Id { get; init; }
```


**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeviceRequestPromptDevice.cs#L6)

<a id="PuppeteerSharp_DeviceRequestPromptDevice_Name_"></a>
<a id="PuppeteerSharp_DeviceRequestPromptDevice_Name"></a>

### Name


```csharp
public string Name { get; init; }
```


**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeviceRequestPromptDevice.cs#L6)
