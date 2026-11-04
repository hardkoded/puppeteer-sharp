---
title: "PWAFileHandler"
description: "A file handler registered by a Progressive Web App (PWA) with the OS."
editUrl: false
prev: false
next: false
sidebar:
  label: "PWAFileHandler"
  hidden: true
---

<a id="PuppeteerSharp_PWAFileHandler"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class PWAFileHandler
```


A file handler registered by a Progressive Web App (PWA) with the OS.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWAFileHandler.cs#L6)

## Properties

<a id="PuppeteerSharp_PWAFileHandler_Accepts_"></a>
<a id="PuppeteerSharp_PWAFileHandler_Accepts"></a>

### Accepts


```csharp
public PWAFileHandlerAccept[] Accepts { get; set; }
```


Gets or sets the media types and file extensions accepted by this file handler.

**Value:** `PWAFileHandlerAccept\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWAFileHandler.cs#L16)

<a id="PuppeteerSharp_PWAFileHandler_Action_"></a>
<a id="PuppeteerSharp_PWAFileHandler_Action"></a>

### Action


```csharp
public string Action { get; set; }
```


Gets or sets the URL that opens when the OS invokes this file handler.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWAFileHandler.cs#L11)

<a id="PuppeteerSharp_PWAFileHandler_DisplayName_"></a>
<a id="PuppeteerSharp_PWAFileHandler_DisplayName"></a>

### DisplayName


```csharp
public string DisplayName { get; set; }
```


Gets or sets the display name of the file handler.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWAFileHandler.cs#L21)
