---
title: "DragDataItem"
description: "Item returned by drag methods."
editUrl: false
prev: false
next: false
sidebar:
  label: "DragDataItem"
  hidden: true
---

<a id="PuppeteerSharp_DragDataItem"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class DragDataItem
```


Item returned by drag methods.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DragDataItem.cs#L6)

## Properties

<a id="PuppeteerSharp_DragDataItem_BaseURL_"></a>
<a id="PuppeteerSharp_DragDataItem_BaseURL"></a>

### BaseURL


```csharp
public string BaseURL { get; set; }
```


Stores the base URL for the contained markup. Only valid when `mimeType` == "text/html".

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DragDataItem.cs#L26)

<a id="PuppeteerSharp_DragDataItem_Data_"></a>
<a id="PuppeteerSharp_DragDataItem_Data"></a>

### Data


```csharp
public string Data { get; set; }
```


Depending of the value of `mimeType`, it contains the dragged link, text, HTML markup or any other data.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DragDataItem.cs#L16)

<a id="PuppeteerSharp_DragDataItem_MimeType_"></a>
<a id="PuppeteerSharp_DragDataItem_MimeType"></a>

### MimeType


```csharp
public string MimeType { get; set; }
```


Mime type of the dragged data.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DragDataItem.cs#L11)

<a id="PuppeteerSharp_DragDataItem_Title_"></a>
<a id="PuppeteerSharp_DragDataItem_Title"></a>

### Title


```csharp
public string Title { get; set; }
```


Title associated with a link. Only valid when `mimeType` == "text/uri-list".

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DragDataItem.cs#L21)
