---
title: "CreatePageOptions"
description: "Options for creating a new page."
editUrl: false
prev: false
next: false
sidebar:
  label: "CreatePageOptions"
  hidden: true
---

<a id="PuppeteerSharp_CreatePageOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class CreatePageOptions
```


Options for creating a new page.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CreatePageOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_CreatePageOptions_Background_"></a>
<a id="PuppeteerSharp_CreatePageOptions_Background"></a>

### Background


```csharp
public bool? Background { get; set; }
```


Gets or sets a value indicating whether to create the page in the background.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean) — `false` by default.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CreatePageOptions.cs#L22)

<a id="PuppeteerSharp_CreatePageOptions_Type_"></a>
<a id="PuppeteerSharp_CreatePageOptions_Type"></a>

### Type


```csharp
public CreatePageType? Type { get; set; }
```


Gets or sets the type of page to create.

**Value:** `CreatePageType?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CreatePageOptions.cs#L11)

<a id="PuppeteerSharp_CreatePageOptions_WindowBounds_"></a>
<a id="PuppeteerSharp_CreatePageOptions_WindowBounds"></a>

### WindowBounds


```csharp
public WindowBounds WindowBounds { get; set; }
```


Gets or sets the window bounds when [Type](#PuppeteerSharp_CreatePageOptions_Type) is [Window](/api/PuppeteerSharp/CreatePageType/#PuppeteerSharp_CreatePageType_Window).

**Value:** [WindowBounds](/api/PuppeteerSharp/WindowBounds/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CreatePageOptions.cs#L16)
