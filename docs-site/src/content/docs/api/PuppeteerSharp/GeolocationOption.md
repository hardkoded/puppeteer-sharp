---
title: "GeolocationOption"
description: "Geolocation option."
editUrl: false
prev: false
next: false
sidebar:
  label: "GeolocationOption"
  hidden: true
---

<a id="PuppeteerSharp_GeolocationOption"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class GeolocationOption : IEquatable<GeolocationOption>
```


Geolocation option.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;GeolocationOption&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/GeolocationOption.cs#L9)

## Properties

<a id="PuppeteerSharp_GeolocationOption_Accuracy_"></a>
<a id="PuppeteerSharp_GeolocationOption_Accuracy"></a>

### Accuracy


```csharp
public decimal Accuracy { get; set; }
```


Optional non-negative accuracy value.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) — The accuracy.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/GeolocationOption.cs#L27)

<a id="PuppeteerSharp_GeolocationOption_Latitude_"></a>
<a id="PuppeteerSharp_GeolocationOption_Latitude"></a>

### Latitude


```csharp
public decimal Latitude { get; set; }
```


Latitude between -90 and 90.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) — The latitude.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/GeolocationOption.cs#L15)

<a id="PuppeteerSharp_GeolocationOption_Longitude_"></a>
<a id="PuppeteerSharp_GeolocationOption_Longitude"></a>

### Longitude


```csharp
public decimal Longitude { get; set; }
```


Longitude between -180 and 180.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) — The longitude.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/GeolocationOption.cs#L21)

## Methods

<a id="PuppeteerSharp_GeolocationOption_Equals_"></a>
<a id="PuppeteerSharp_GeolocationOption_Equals_PuppeteerSharp_GeolocationOption_"></a>

### Equals(GeolocationOption)


```csharp
public bool Equals(GeolocationOption other)
```


Determines whether the specified [GeolocationOption](./) is equal to the current [GeolocationOption](./).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `other` | [GeolocationOption](./) | The [GeolocationOption](./) to compare with the current [GeolocationOption](./). |

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the specified [GeolocationOption](./) is equal to the current
    [GeolocationOption](./); otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/GeolocationOption.cs#L35)

<a id="PuppeteerSharp_GeolocationOption_Equals_System_Object_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/GeolocationOption.cs#L42)

<a id="PuppeteerSharp_GeolocationOption_GetHashCode_"></a>
<a id="PuppeteerSharp_GeolocationOption_GetHashCode"></a>

### GetHashCode()


```csharp
public override int GetHashCode()
```


Serves as the default hash function.

**Returns:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — A hash code for the current object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/GeolocationOption.cs#L45)
