---
title: "CookieParam"
description: "Cookie data."
editUrl: false
prev: false
next: false
sidebar:
  label: "CookieParam"
  hidden: true
---

<a id="PuppeteerSharp_CookieParam"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class CookieParam
```


Cookie data.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L12)

## Properties

<a id="PuppeteerSharp_CookieParam_Domain_"></a>
<a id="PuppeteerSharp_CookieParam_Domain"></a>

### Domain


```csharp
public string Domain { get; set; }
```


Gets or sets the domain.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The domain.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L36)

<a id="PuppeteerSharp_CookieParam_Expires_"></a>
<a id="PuppeteerSharp_CookieParam_Expires"></a>

### Expires


```csharp
public double? Expires { get; set; }
```


Gets or sets the expiration. Unix time in seconds.

**Value:** [double?](https://learn.microsoft.com/dotnet/api/system.double) — Expiration.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L65)

<a id="PuppeteerSharp_CookieParam_HttpOnly_"></a>
<a id="PuppeteerSharp_CookieParam_HttpOnly"></a>

### HttpOnly


```csharp
public bool? HttpOnly { get; set; }
```


Gets or sets if it's HTTP only.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean) — Whether it's http only or not.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L54)

<a id="PuppeteerSharp_CookieParam_Name_"></a>
<a id="PuppeteerSharp_CookieParam_Name"></a>

### Name


```csharp
public string Name { get; set; }
```


Gets or sets the name.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The name.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L18)

<a id="PuppeteerSharp_CookieParam_PartitionKey_"></a>
<a id="PuppeteerSharp_CookieParam_PartitionKey"></a>

### PartitionKey


```csharp
[JsonConverter(typeof(CookiePartitionKeyConverter))]
public CookiePartitionKey PartitionKey { get; set; }
```


Cookie partition key. In Chrome, it matches the top-level site the
partitioned cookie is available in. In Firefox, it matches the
source origin.

**Value:** [CookiePartitionKey](/api/PuppeteerSharp/CookiePartitionKey/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L94)

<a id="PuppeteerSharp_CookieParam_PartitionKeyOpaque_"></a>
<a id="PuppeteerSharp_CookieParam_PartitionKeyOpaque"></a>

### PartitionKeyOpaque


```csharp
public bool? PartitionKeyOpaque { get; set; }
```


True if cookie partition key is opaque. Supported only in Chrome.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L100)

<a id="PuppeteerSharp_CookieParam_Path_"></a>
<a id="PuppeteerSharp_CookieParam_Path"></a>

### Path


```csharp
public string Path { get; set; }
```


Gets or sets the path.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The path.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L42)

<a id="PuppeteerSharp_CookieParam_Priority_"></a>
<a id="PuppeteerSharp_CookieParam_Priority"></a>

### Priority


```csharp
public CookiePriority? Priority { get; set; }
```


Cookie Priority. Supported only in Chrome.

**Value:** `CookiePriority?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L82)

<a id="PuppeteerSharp_CookieParam_SameSite_"></a>
<a id="PuppeteerSharp_CookieParam_SameSite"></a>

### SameSite


```csharp
public SameSite? SameSite { get; set; }
```


Gets or sets the cookies SameSite value.

**Value:** `SameSite?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L59)

<a id="PuppeteerSharp_CookieParam_Secure_"></a>
<a id="PuppeteerSharp_CookieParam_Secure"></a>

### Secure


```csharp
public bool? Secure { get; set; }
```


Gets or sets if it's secure.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean) — Whether it's secure or not.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L48)

<a id="PuppeteerSharp_CookieParam_Session_"></a>
<a id="PuppeteerSharp_CookieParam_Session"></a>

### Session


```csharp
public bool? Session { get; set; }
```


Gets or sets if it's session only.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean) — Whether it's session only or not.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L77)

<a id="PuppeteerSharp_CookieParam_Size_"></a>
<a id="PuppeteerSharp_CookieParam_Size"></a>

### Size


```csharp
public int? Size { get; set; }
```


Gets or sets the size.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32) — The size.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L71)

<a id="PuppeteerSharp_CookieParam_SourceScheme_"></a>
<a id="PuppeteerSharp_CookieParam_SourceScheme"></a>

### SourceScheme


```csharp
public CookieSourceScheme? SourceScheme { get; set; }
```


Cookie source scheme type. Supported only in Chrome.

**Value:** `CookieSourceScheme?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L87)

<a id="PuppeteerSharp_CookieParam_Url_"></a>
<a id="PuppeteerSharp_CookieParam_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


Gets or sets the URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L30)

<a id="PuppeteerSharp_CookieParam_Value_"></a>
<a id="PuppeteerSharp_CookieParam_Value"></a>

### Value


```csharp
public string Value { get; set; }
```


Gets or sets the value.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieParam.cs#L24)
