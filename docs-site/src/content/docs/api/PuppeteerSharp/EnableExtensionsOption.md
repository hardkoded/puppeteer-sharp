---
title: "EnableExtensionsOption"
description: "Represents the value for the EnableExtensions launch option. Can be implicitly converted from a bool or a string array."
editUrl: false
prev: false
next: false
sidebar:
  label: "EnableExtensionsOption"
  hidden: true
---

<a id="PuppeteerSharp_EnableExtensionsOption"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class EnableExtensionsOption
```


Represents the value for the EnableExtensions launch option.
Can be implicitly converted from a [bool](https://learn.microsoft.com/dotnet/api/system.boolean) or a [string](https://learn.microsoft.com/dotnet/api/system.string) array.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EnableExtensionsOption.cs#L7)

## Constructors

<a id="PuppeteerSharp_EnableExtensionsOption__ctor_"></a>
<a id="PuppeteerSharp_EnableExtensionsOption__ctor_System_Boolean_"></a>

### EnableExtensionsOption(bool)


```csharp
public EnableExtensionsOption(bool enabled)
```


Initializes a new instance of the [EnableExtensionsOption](./) class with a boolean value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether extensions are enabled. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EnableExtensionsOption.cs#L13)

<a id="PuppeteerSharp_EnableExtensionsOption__ctor_System_String___"></a>

### EnableExtensionsOption(string[])


```csharp
public EnableExtensionsOption(string[] paths)
```


Initializes a new instance of the [EnableExtensionsOption](./) class with an array of extension paths.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `paths` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | The paths to unpacked extensions to load. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EnableExtensionsOption.cs#L22)

## Properties

<a id="PuppeteerSharp_EnableExtensionsOption_Enabled_"></a>
<a id="PuppeteerSharp_EnableExtensionsOption_Enabled"></a>

### Enabled


```csharp
public bool Enabled { get; }
```


Gets a value indicating whether extensions should be enabled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EnableExtensionsOption.cs#L31)

<a id="PuppeteerSharp_EnableExtensionsOption_Paths_"></a>
<a id="PuppeteerSharp_EnableExtensionsOption_Paths"></a>

### Paths


```csharp
public string[] Paths { get; }
```


Gets the paths to unpacked extensions to load.
When `null`, no specific extensions are loaded, but extensions may still be enabled
if [Enabled](#PuppeteerSharp_EnableExtensionsOption_Enabled) is `true`.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EnableExtensionsOption.cs#L38)

## Operators

<a id="PuppeteerSharp_EnableExtensionsOption_op_Implicit_"></a>
<a id="PuppeteerSharp_EnableExtensionsOption_op_Implicit_System_Boolean__PuppeteerSharp_EnableExtensionsOption"></a>

### implicit operator EnableExtensionsOption(bool)


```csharp
public static implicit operator EnableExtensionsOption(bool enabled)
```


Implicitly converts a [bool](https://learn.microsoft.com/dotnet/api/system.boolean) to an [EnableExtensionsOption](./).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | The boolean value. |

**Returns:** [EnableExtensionsOption](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EnableExtensionsOption.cs#L44)

<a id="PuppeteerSharp_EnableExtensionsOption_op_Implicit_System_String____PuppeteerSharp_EnableExtensionsOption"></a>

### implicit operator EnableExtensionsOption(string[])


```csharp
public static implicit operator EnableExtensionsOption(string[] paths)
```


Implicitly converts a [string](https://learn.microsoft.com/dotnet/api/system.string) array to an [EnableExtensionsOption](./).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `paths` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | The array of extension paths. |

**Returns:** [EnableExtensionsOption](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EnableExtensionsOption.cs#L50)
