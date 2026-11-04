---
title: "ConsoleMessageLocation"
description: "Console message location."
editUrl: false
prev: false
next: false
sidebar:
  label: "ConsoleMessageLocation"
  hidden: true
---

<a id="PuppeteerSharp_ConsoleMessageLocation"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ConsoleMessageLocation : IEquatable<ConsoleMessageLocation>
```


Console message location.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;ConsoleMessageLocation&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessageLocation.cs#L9)

## Properties

<a id="PuppeteerSharp_ConsoleMessageLocation_ColumnNumber_"></a>
<a id="PuppeteerSharp_ConsoleMessageLocation_ColumnNumber"></a>

### ColumnNumber


```csharp
public int? ColumnNumber { get; set; }
```


0-based column number in the resource if known.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessageLocation.cs#L24)

<a id="PuppeteerSharp_ConsoleMessageLocation_FunctionName_"></a>
<a id="PuppeteerSharp_ConsoleMessageLocation_FunctionName"></a>

### FunctionName


```csharp
public string FunctionName { get; set; }
```


Name of the function if known.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessageLocation.cs#L29)

<a id="PuppeteerSharp_ConsoleMessageLocation_LineNumber_"></a>
<a id="PuppeteerSharp_ConsoleMessageLocation_LineNumber"></a>

### LineNumber


```csharp
public int? LineNumber { get; set; }
```


0-based line number in the resource if known.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessageLocation.cs#L19)

<a id="PuppeteerSharp_ConsoleMessageLocation_URL_"></a>
<a id="PuppeteerSharp_ConsoleMessageLocation_URL"></a>

### URL


```csharp
public string URL { get; set; }
```


URL of the resource if known.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessageLocation.cs#L14)

## Methods

<a id="PuppeteerSharp_ConsoleMessageLocation_Equals_"></a>
<a id="PuppeteerSharp_ConsoleMessageLocation_Equals_PuppeteerSharp_ConsoleMessageLocation_"></a>

### Equals(ConsoleMessageLocation)


```csharp
public bool Equals(ConsoleMessageLocation other)
```


Indicates whether the current object is equal to another object of the same type.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `other` | [ConsoleMessageLocation](./) | An object to compare with this object. |

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — &lt;a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool"&gt;true&lt;/a&gt; if the current object is equal to the &lt;code class="paramref"&gt;other&lt;/code&gt; parameter; otherwise, &lt;a href="https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/bool"&gt;false&lt;/a&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessageLocation.cs#L46)

<a id="PuppeteerSharp_ConsoleMessageLocation_Equals_System_Object_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessageLocation.cs#L50)

<a id="PuppeteerSharp_ConsoleMessageLocation_GetHashCode_"></a>
<a id="PuppeteerSharp_ConsoleMessageLocation_GetHashCode"></a>

### GetHashCode()


```csharp
public override int GetHashCode()
```


Serves as the default hash function.

**Returns:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — A hash code for the current object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessageLocation.cs#L53)

## Operators

<a id="PuppeteerSharp_ConsoleMessageLocation_op_Equality_"></a>
<a id="PuppeteerSharp_ConsoleMessageLocation_op_Equality_PuppeteerSharp_ConsoleMessageLocation_PuppeteerSharp_ConsoleMessageLocation_"></a>

### operator ==(ConsoleMessageLocation, ConsoleMessageLocation)


```csharp
public static bool operator ==(ConsoleMessageLocation location1, ConsoleMessageLocation location2)
```


Overriding == operator for [ConsoleMessageLocation](./).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `location1` | [ConsoleMessageLocation](./) | the value to compare against &lt;code class="paramref"&gt;location2&lt;/code&gt;. |
| `location2` | [ConsoleMessageLocation](./) | the value to compare against &lt;code class="paramref"&gt;location1&lt;/code&gt;. |

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the two instances are equal to the same value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessageLocation.cs#L35)

<a id="PuppeteerSharp_ConsoleMessageLocation_op_Inequality_"></a>
<a id="PuppeteerSharp_ConsoleMessageLocation_op_Inequality_PuppeteerSharp_ConsoleMessageLocation_PuppeteerSharp_ConsoleMessageLocation_"></a>

### operator !=(ConsoleMessageLocation, ConsoleMessageLocation)


```csharp
public static bool operator !=(ConsoleMessageLocation location1, ConsoleMessageLocation location2)
```


Overriding != operator for [ConsoleMessageLocation](./).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `location1` | [ConsoleMessageLocation](./) | the value to compare against &lt;code class="paramref"&gt;location2&lt;/code&gt;. |
| `location2` | [ConsoleMessageLocation](./) | the value to compare against &lt;code class="paramref"&gt;location1&lt;/code&gt;. |

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the two instances are not equal to the same value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessageLocation.cs#L42)
