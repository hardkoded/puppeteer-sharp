---
title: "CookiePartitionKey"
description: "Cookie partition key."
editUrl: false
prev: false
next: false
sidebar:
  label: "CookiePartitionKey"
  hidden: true
---

<a id="PuppeteerSharp_CookiePartitionKey"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class CookiePartitionKey : IEquatable<CookiePartitionKey>
```


Cookie partition key.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;CookiePartitionKey&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookiePartitionKey.cs#L8)

## Properties

<a id="PuppeteerSharp_CookiePartitionKey_HasCrossSiteAncestor_"></a>
<a id="PuppeteerSharp_CookiePartitionKey_HasCrossSiteAncestor"></a>

### HasCrossSiteAncestor


```csharp
public bool? HasCrossSiteAncestor { get; set; }
```


Indicates if the cookie has any ancestors that are cross-site to the topLevelSite.
Supported only in Chrome.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookiePartitionKey.cs#L21)

<a id="PuppeteerSharp_CookiePartitionKey_SourceOrigin_"></a>
<a id="PuppeteerSharp_CookiePartitionKey_SourceOrigin"></a>

### SourceOrigin


```csharp
public string SourceOrigin { get; set; }
```


The site of the top-level URL the browser was visiting at the start of the request
to the endpoint that set the cookie.
In Chrome, maps to the CDP's topLevelSite partition key.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookiePartitionKey.cs#L15)

## Methods

<a id="PuppeteerSharp_CookiePartitionKey_Equals_"></a>
<a id="PuppeteerSharp_CookiePartitionKey_Equals_PuppeteerSharp_CookiePartitionKey_"></a>

### Equals(CookiePartitionKey)


```csharp
public bool Equals(CookiePartitionKey other)
```


Indicates whether the current object is equal to another object of the same type.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `other` | [CookiePartitionKey](./) | An object to compare with this object. |

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — &lt;a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool"&gt;true&lt;/a&gt; if the current object is equal to the &lt;code class="paramref"&gt;other&lt;/code&gt; parameter; otherwise, &lt;a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool"&gt;false&lt;/a&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookiePartitionKey.cs#L24)

<a id="PuppeteerSharp_CookiePartitionKey_Equals_System_Object_"></a>

### Equals(object)


```csharp
public override bool Equals(object obj)
```


Determines whether the specified object is equal to the current object.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `obj` | [object](https://learn.microsoft.com/dotnet/api/system.object) | The object to compare with the current object. |

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — &lt;a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool"&gt;true&lt;/a&gt; if the specified object  is equal to the current object; otherwise, &lt;a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool"&gt;false&lt;/a&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookiePartitionKey.cs#L35)

<a id="PuppeteerSharp_CookiePartitionKey_GetHashCode_"></a>
<a id="PuppeteerSharp_CookiePartitionKey_GetHashCode"></a>

### GetHashCode()


```csharp
public override int GetHashCode()
```


Serves as the default hash function.

**Returns:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — A hash code for the current object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CookiePartitionKey.cs#L38)
