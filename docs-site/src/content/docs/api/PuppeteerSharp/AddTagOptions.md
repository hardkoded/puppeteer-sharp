---
title: "AddTagOptions"
description: "Options used by AddScriptTagAsync(AddTagOptions) & AddStyleTagAsync(AddTagOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "AddTagOptions"
  hidden: true
---

<a id="PuppeteerSharp_AddTagOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class AddTagOptions
```


Options used by [AddScriptTagAsync(AddTagOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_AddScriptTagAsync_PuppeteerSharp_AddTagOptions_) &amp; [AddStyleTagAsync(AddTagOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_AddStyleTagAsync_PuppeteerSharp_AddTagOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AddTagOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_AddTagOptions_Content_"></a>
<a id="PuppeteerSharp_AddTagOptions_Content"></a>

### Content


```csharp
public string Content { get; set; }
```


JavaScript to be injected into the frame.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AddTagOptions.cs#L21)

<a id="PuppeteerSharp_AddTagOptions_Id_"></a>
<a id="PuppeteerSharp_AddTagOptions_Id"></a>

### Id


```csharp
public object Id { get; set; }
```


Id of the script.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AddTagOptions.cs#L32)

<a id="PuppeteerSharp_AddTagOptions_Path_"></a>
<a id="PuppeteerSharp_AddTagOptions_Path"></a>

### Path


```csharp
public string Path { get; set; }
```


Path to the JavaScript file to be injected into frame. If its a relative path, then it is resolved relative to [GetCurrentDirectory()](https://learn.microsoft.com/dotnet/api/system.io.directory.getcurrentdirectory).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AddTagOptions.cs#L16)

<a id="PuppeteerSharp_AddTagOptions_Type_"></a>
<a id="PuppeteerSharp_AddTagOptions_Type"></a>

### Type


```csharp
public string Type { get; set; }
```


Script type. Use `module` in order to load a Javascript ES2015 module.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AddTagOptions.cs#L27)

<a id="PuppeteerSharp_AddTagOptions_Url_"></a>
<a id="PuppeteerSharp_AddTagOptions_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


Url of a script to be added.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/AddTagOptions.cs#L11)
