---
title: "TargetInfo"
description: "Target info."
editUrl: false
prev: false
next: false
sidebar:
  label: "TargetInfo"
  hidden: true
---

<a id="PuppeteerSharp_TargetInfo"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class TargetInfo
```


Target info.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetInfo.cs#L9)

## Properties

<a id="PuppeteerSharp_TargetInfo_Attached_"></a>
<a id="PuppeteerSharp_TargetInfo_Attached"></a>

### Attached


```csharp
public bool Attached { get; set; }
```


Gets whether the target is attached.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetInfo.cs#L43)

<a id="PuppeteerSharp_TargetInfo_BrowserContextId_"></a>
<a id="PuppeteerSharp_TargetInfo_BrowserContextId"></a>

### BrowserContextId


```csharp
[JsonConverter(typeof(AnyTypeToStringConverter))]
public string BrowserContextId { get; set; }
```


Gets the target browser contextId.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetInfo.cs#L32)

<a id="PuppeteerSharp_TargetInfo_OpenerId_"></a>
<a id="PuppeteerSharp_TargetInfo_OpenerId"></a>

### OpenerId


```csharp
public string OpenerId { get; set; }
```


Get the target that opened this target.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetInfo.cs#L38)

<a id="PuppeteerSharp_TargetInfo_Subtype_"></a>
<a id="PuppeteerSharp_TargetInfo_Subtype"></a>

### Subtype


```csharp
public string Subtype { get; set; }
```


Provides additional details for specific target types. For example, for
the type of "page", this may be set to "portal" or "prerender".

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetInfo.cs#L49)

<a id="PuppeteerSharp_TargetInfo_TargetId_"></a>
<a id="PuppeteerSharp_TargetInfo_TargetId"></a>

### TargetId


```csharp
public string TargetId { get; set; }
```


Gets the target identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The target identifier.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetInfo.cs#L27)

<a id="PuppeteerSharp_TargetInfo_Type_"></a>
<a id="PuppeteerSharp_TargetInfo_Type"></a>

### Type


```csharp
public TargetType Type { get; set; }
```


Gets the type.

**Value:** [TargetType](/api/PuppeteerSharp/TargetType/) — The type.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetInfo.cs#L15)

<a id="PuppeteerSharp_TargetInfo_Url_"></a>
<a id="PuppeteerSharp_TargetInfo_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


Gets the URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetInfo.cs#L21)
