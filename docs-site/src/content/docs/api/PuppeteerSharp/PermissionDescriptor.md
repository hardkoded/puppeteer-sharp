---
title: "PermissionDescriptor"
description: "Describes a permission to set via PermissionEntry%5b%5d)."
editUrl: false
prev: false
next: false
sidebar:
  label: "PermissionDescriptor"
  hidden: true
---

<a id="PuppeteerSharp_PermissionDescriptor"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class PermissionDescriptor
```


Describes a permission to set via `PermissionEntry%5b%5d)`.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionDescriptor.cs#L6)

## Properties

<a id="PuppeteerSharp_PermissionDescriptor_AllowWithoutSanitization_"></a>
<a id="PuppeteerSharp_PermissionDescriptor_AllowWithoutSanitization"></a>

### AllowWithoutSanitization


```csharp
public bool? AllowWithoutSanitization { get; set; }
```


Gets or sets a value indicating whether to allow writing without sanitization (clipboard).

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionDescriptor.cs#L31)

<a id="PuppeteerSharp_PermissionDescriptor_Name_"></a>
<a id="PuppeteerSharp_PermissionDescriptor_Name"></a>

### Name


```csharp
public string Name { get; set; }
```


Gets or sets the permission name (e.g. "geolocation", "midi", "notifications").

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionDescriptor.cs#L11)

<a id="PuppeteerSharp_PermissionDescriptor_PanTiltZoom_"></a>
<a id="PuppeteerSharp_PermissionDescriptor_PanTiltZoom"></a>

### PanTiltZoom


```csharp
public bool? PanTiltZoom { get; set; }
```


Gets or sets a value indicating whether to enable pan-tilt-zoom controls (camera).

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionDescriptor.cs#L26)

<a id="PuppeteerSharp_PermissionDescriptor_Sysex_"></a>
<a id="PuppeteerSharp_PermissionDescriptor_Sysex"></a>

### Sysex


```csharp
public bool? Sysex { get; set; }
```


Gets or sets a value indicating whether to enable system exclusive access (MIDI).

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionDescriptor.cs#L21)

<a id="PuppeteerSharp_PermissionDescriptor_UserVisibleOnly_"></a>
<a id="PuppeteerSharp_PermissionDescriptor_UserVisibleOnly"></a>

### UserVisibleOnly


```csharp
public bool? UserVisibleOnly { get; set; }
```


Gets or sets a value indicating whether the permission should only apply to user-visible operations.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionDescriptor.cs#L16)
