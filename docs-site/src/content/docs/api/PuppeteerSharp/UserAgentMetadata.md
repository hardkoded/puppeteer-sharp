---
title: "UserAgentMetadata"
description: "Used to specify User Agent Cient Hints to emulate. See https://wicg.github.io/ua-client-hints Missing optional values will be filled in by the target with wh..."
editUrl: false
prev: false
next: false
sidebar:
  label: "UserAgentMetadata"
  hidden: true
---

<a id="PuppeteerSharp_UserAgentMetadata"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class UserAgentMetadata
```


Used to specify User Agent Cient Hints to emulate. See https://wicg.github.io/ua-client-hints
Missing optional values will be filled in by the target with what it would normally use.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/UserAgentMetadata.cs#L7)

## Properties

<a id="PuppeteerSharp_UserAgentMetadata_Architecture_"></a>
<a id="PuppeteerSharp_UserAgentMetadata_Architecture"></a>

### Architecture


```csharp
public string Architecture { get; set; }
```


Architecture.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/UserAgentMetadata.cs#L32)

<a id="PuppeteerSharp_UserAgentMetadata_Bitness_"></a>
<a id="PuppeteerSharp_UserAgentMetadata_Bitness"></a>

### Bitness


```csharp
public string Bitness { get; set; }
```


Bitness (e.g., "32" or "64").

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/UserAgentMetadata.cs#L47)

<a id="PuppeteerSharp_UserAgentMetadata_Brands_"></a>
<a id="PuppeteerSharp_UserAgentMetadata_Brands"></a>

### Brands


```csharp
public UserAgentBrandVersion[] Brands { get; set; }
```


Brands.

**Value:** `UserAgentBrandVersion\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/UserAgentMetadata.cs#L12)

<a id="PuppeteerSharp_UserAgentMetadata_FullVersion_"></a>
<a id="PuppeteerSharp_UserAgentMetadata_FullVersion"></a>

### FullVersion


```csharp
public string FullVersion { get; set; }
```


Full version.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/UserAgentMetadata.cs#L17)

<a id="PuppeteerSharp_UserAgentMetadata_Mobile_"></a>
<a id="PuppeteerSharp_UserAgentMetadata_Mobile"></a>

### Mobile


```csharp
public bool Mobile { get; set; }
```


Mobile.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/UserAgentMetadata.cs#L42)

<a id="PuppeteerSharp_UserAgentMetadata_Model_"></a>
<a id="PuppeteerSharp_UserAgentMetadata_Model"></a>

### Model


```csharp
public string Model { get; set; }
```


Model.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/UserAgentMetadata.cs#L37)

<a id="PuppeteerSharp_UserAgentMetadata_Platform_"></a>
<a id="PuppeteerSharp_UserAgentMetadata_Platform"></a>

### Platform


```csharp
public string Platform { get; set; }
```


Platform.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/UserAgentMetadata.cs#L22)

<a id="PuppeteerSharp_UserAgentMetadata_PlatformVersion_"></a>
<a id="PuppeteerSharp_UserAgentMetadata_PlatformVersion"></a>

### PlatformVersion


```csharp
public string PlatformVersion { get; set; }
```


Platform version.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/UserAgentMetadata.cs#L27)

<a id="PuppeteerSharp_UserAgentMetadata_Wow64_"></a>
<a id="PuppeteerSharp_UserAgentMetadata_Wow64"></a>

### Wow64


```csharp
public bool? Wow64 { get; set; }
```


True if on Windows and WOW64 (Windows 32-bit on Windows 64-bit).

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/UserAgentMetadata.cs#L52)
