---
title: "CookieData"
description: "Cookie parameter object used to set cookies in the browser-level cookies API."
editUrl: false
prev: false
next: false
sidebar:
  label: "CookieData"
  hidden: true
---

<a id="PuppeteerSharp_CookieData"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class CookieData
```


Cookie parameter object used to set cookies in the browser-level cookies API.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L9)

## Properties

<a id="PuppeteerSharp_CookieData_Domain_"></a>
<a id="PuppeteerSharp_CookieData_Domain"></a>

### Domain


```csharp
public string Domain { get; set; }
```


Gets or sets the domain.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L24)

<a id="PuppeteerSharp_CookieData_Expires_"></a>
<a id="PuppeteerSharp_CookieData_Expires"></a>

### Expires


```csharp
public double? Expires { get; set; }
```


Gets or sets the expiration. Unix time in seconds.

**Value:** [double?](https://learn.microsoft.com/dotnet/api/system.double)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L49)

<a id="PuppeteerSharp_CookieData_HttpOnly_"></a>
<a id="PuppeteerSharp_CookieData_HttpOnly"></a>

### HttpOnly


```csharp
public bool? HttpOnly { get; set; }
```


Gets or sets if it's HTTP only.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L39)

<a id="PuppeteerSharp_CookieData_Name_"></a>
<a id="PuppeteerSharp_CookieData_Name"></a>

### Name


```csharp
public string Name { get; set; }
```


Gets or sets the name.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L14)

<a id="PuppeteerSharp_CookieData_PartitionKey_"></a>
<a id="PuppeteerSharp_CookieData_PartitionKey"></a>

### PartitionKey


```csharp
[JsonConverter(typeof(CookiePartitionKeyConverter))]
public CookiePartitionKey PartitionKey { get; set; }
```


Cookie partition key. In Chrome, it matches the top-level site the
partitioned cookie is available in. In Firefox, it matches the
source origin.

**Value:** [CookiePartitionKey](/api/PuppeteerSharp/CookiePartitionKey/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L66)

<a id="PuppeteerSharp_CookieData_Path_"></a>
<a id="PuppeteerSharp_CookieData_Path"></a>

### Path


```csharp
public string Path { get; set; }
```


Gets or sets the path.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L29)

<a id="PuppeteerSharp_CookieData_Priority_"></a>
<a id="PuppeteerSharp_CookieData_Priority"></a>

### Priority


```csharp
public CookiePriority? Priority { get; set; }
```


Cookie Priority. Supported only in Chrome.

**Value:** `CookiePriority?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L54)

<a id="PuppeteerSharp_CookieData_SameSite_"></a>
<a id="PuppeteerSharp_CookieData_SameSite"></a>

### SameSite


```csharp
public SameSite? SameSite { get; set; }
```


Gets or sets the cookies SameSite value.

**Value:** `SameSite?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L44)

<a id="PuppeteerSharp_CookieData_Secure_"></a>
<a id="PuppeteerSharp_CookieData_Secure"></a>

### Secure


```csharp
public bool? Secure { get; set; }
```


Gets or sets if it's secure.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L34)

<a id="PuppeteerSharp_CookieData_SourceScheme_"></a>
<a id="PuppeteerSharp_CookieData_SourceScheme"></a>

### SourceScheme


```csharp
public CookieSourceScheme? SourceScheme { get; set; }
```


Cookie source scheme type. Supported only in Chrome.

**Value:** `CookieSourceScheme?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L59)

<a id="PuppeteerSharp_CookieData_Value_"></a>
<a id="PuppeteerSharp_CookieData_Value"></a>

### Value


```csharp
public string Value { get; set; }
```


Gets or sets the value.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookieData.cs#L19)
