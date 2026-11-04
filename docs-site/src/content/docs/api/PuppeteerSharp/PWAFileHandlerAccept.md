---
title: "PWAFileHandlerAccept"
description: "A media type and its associated file extensions accepted by a PWAFileHandler."
editUrl: false
prev: false
next: false
sidebar:
  label: "PWAFileHandlerAccept"
  hidden: true
---

<a id="PuppeteerSharp_PWAFileHandlerAccept"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class PWAFileHandlerAccept
```


A media type and its associated file extensions accepted by a [PWAFileHandler](/api/PuppeteerSharp/PWAFileHandler/).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWAFileHandlerAccept.cs#L6)

## Properties

<a id="PuppeteerSharp_PWAFileHandlerAccept_FileExtensions_"></a>
<a id="PuppeteerSharp_PWAFileHandlerAccept_FileExtensions"></a>

### FileExtensions


```csharp
public string[] FileExtensions { get; set; }
```


Gets or sets the file extensions associated with [MediaType](#PuppeteerSharp_PWAFileHandlerAccept_MediaType).

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWAFileHandlerAccept.cs#L17)

<a id="PuppeteerSharp_PWAFileHandlerAccept_MediaType_"></a>
<a id="PuppeteerSharp_PWAFileHandlerAccept_MediaType"></a>

### MediaType


```csharp
public string MediaType { get; set; }
```


Gets or sets the mime type, as per
&lt;a href="https://www.iana.org/assignments/media-types/media-types.xhtml"&gt;the IANA media types registry&lt;/a&gt;.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWAFileHandlerAccept.cs#L12)
