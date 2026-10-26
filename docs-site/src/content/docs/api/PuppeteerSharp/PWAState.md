---
title: "PWAState"
description: "The OS-integration state of an installed web app, returned by GetPWAStateAsync(GetPWAStateOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "PWAState"
  hidden: true
---

<a id="PuppeteerSharp_PWAState"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class PWAState
```


The OS-integration state of an installed web app, returned by [GetPWAStateAsync(GetPWAStateOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_GetPWAStateAsync_PuppeteerSharp_GetPWAStateOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWAState.cs#L6)

## Properties

<a id="PuppeteerSharp_PWAState_BadgeCount_"></a>
<a id="PuppeteerSharp_PWAState_BadgeCount"></a>

### BadgeCount


```csharp
public int BadgeCount { get; set; }
```


Gets or sets the current badge count shown on the app icon.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWAState.cs#L11)

<a id="PuppeteerSharp_PWAState_FileHandlers_"></a>
<a id="PuppeteerSharp_PWAState_FileHandlers"></a>

### FileHandlers


```csharp
public PWAFileHandler[] FileHandlers { get; set; }
```


Gets or sets the file handlers registered by the app with the OS.

**Value:** `PWAFileHandler\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWAState.cs#L16)
