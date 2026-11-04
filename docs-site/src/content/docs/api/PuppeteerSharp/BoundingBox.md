---
title: "BoundingBox"
description: "Bounding box data returned by BoundingBoxAsync()."
editUrl: false
prev: false
next: false
sidebar:
  label: "BoundingBox"
  hidden: true
---

<a id="PuppeteerSharp_BoundingBox"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class BoundingBox : IEquatable<BoundingBox>
```


Bounding box data returned by [BoundingBoxAsync()](/api/PuppeteerSharp/IElementHandle/#PuppeteerSharp_IElementHandle_BoundingBoxAsync).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;BoundingBox&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

**Derived:** [Clip](/api/PuppeteerSharp.Media/Clip/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BoundingBox.cs#L9)

## Constructors

<a id="PuppeteerSharp_BoundingBox__ctor_"></a>
<a id="PuppeteerSharp_BoundingBox__ctor"></a>

### BoundingBox()


```csharp
public BoundingBox()
```


Initializes a new instance of the [BoundingBox](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BoundingBox.cs#L14)

<a id="PuppeteerSharp_BoundingBox__ctor_System_Decimal_System_Decimal_System_Decimal_System_Decimal_"></a>

### BoundingBox(decimal, decimal, decimal, decimal)


```csharp
public BoundingBox(decimal x, decimal y, decimal width, decimal height)
```


Initializes a new instance of the [BoundingBox](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The x coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The y coordinate. |
| `width` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Width. |
| `height` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Height. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BoundingBox.cs#L25)

## Properties

<a id="PuppeteerSharp_BoundingBox_Height_"></a>
<a id="PuppeteerSharp_BoundingBox_Height"></a>

### Height


```csharp
public decimal Height { get; set; }
```


The height of the element in pixels.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) — The height.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BoundingBox.cs#L55)

<a id="PuppeteerSharp_BoundingBox_Width_"></a>
<a id="PuppeteerSharp_BoundingBox_Width"></a>

### Width


```csharp
public decimal Width { get; set; }
```


The width of the element in pixels.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) — The width.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BoundingBox.cs#L49)

<a id="PuppeteerSharp_BoundingBox_X_"></a>
<a id="PuppeteerSharp_BoundingBox_X"></a>

### X


```csharp
public decimal X { get; set; }
```


The x coordinate of the element in pixels.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) — The x.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BoundingBox.cs#L37)

<a id="PuppeteerSharp_BoundingBox_Y_"></a>
<a id="PuppeteerSharp_BoundingBox_Y"></a>

### Y


```csharp
public decimal Y { get; set; }
```


The y coordinate of the element in pixels.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) — The y.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BoundingBox.cs#L43)

## Methods

<a id="PuppeteerSharp_BoundingBox_Equals_"></a>
<a id="PuppeteerSharp_BoundingBox_Equals_PuppeteerSharp_BoundingBox_"></a>

### Equals(BoundingBox)


```csharp
public bool Equals(BoundingBox obj)
```


Determines whether the specified [BoundingBox](./) is equal to the current [BoundingBox](./).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `obj` | [BoundingBox](./) | The [BoundingBox](./) to compare with the current [BoundingBox](./). |

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the specified [BoundingBox](./) is equal to the current
    [BoundingBox](./); otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BoundingBox.cs#L74)

<a id="PuppeteerSharp_BoundingBox_Equals_System_Object_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BoundingBox.cs#L58)

<a id="PuppeteerSharp_BoundingBox_GetHashCode_"></a>
<a id="PuppeteerSharp_BoundingBox_GetHashCode"></a>

### GetHashCode()


```csharp
public override int GetHashCode()
```


Serves as the default hash function.

**Returns:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — A hash code for the current object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BoundingBox.cs#L82)
