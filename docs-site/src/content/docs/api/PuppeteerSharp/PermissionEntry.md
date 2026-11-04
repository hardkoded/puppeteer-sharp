---
title: "PermissionEntry"
description: "Represents a permission entry to set via PermissionEntry%5b%5d)."
editUrl: false
prev: false
next: false
sidebar:
  label: "PermissionEntry"
  hidden: true
---

<a id="PuppeteerSharp_PermissionEntry"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class PermissionEntry
```


Represents a permission entry to set via `PermissionEntry%5b%5d)`.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionEntry.cs#L6)

## Properties

<a id="PuppeteerSharp_PermissionEntry_Permission_"></a>
<a id="PuppeteerSharp_PermissionEntry_Permission"></a>

### Permission


```csharp
public PermissionDescriptor Permission { get; set; }
```


Gets or sets the permission descriptor.

**Value:** [PermissionDescriptor](/api/PuppeteerSharp/PermissionDescriptor/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionEntry.cs#L11)

<a id="PuppeteerSharp_PermissionEntry_State_"></a>
<a id="PuppeteerSharp_PermissionEntry_State"></a>

### State


```csharp
public PermissionState State { get; set; }
```


Gets or sets the state to set the permission to.

**Value:** [PermissionState](/api/PuppeteerSharp/PermissionState/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PermissionEntry.cs#L16)
