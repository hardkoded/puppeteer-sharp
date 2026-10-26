---
title: "DownloadBehavior"
description: "Behavior definition for when downloading a file."
editUrl: false
prev: false
next: false
sidebar:
  label: "DownloadBehavior"
  hidden: true
---

<a id="PuppeteerSharp_DownloadBehavior"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class DownloadBehavior
```


Behavior definition for when downloading a file.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DownloadBehavior.cs#L6)

## Properties

<a id="PuppeteerSharp_DownloadBehavior_DownloadPath_"></a>
<a id="PuppeteerSharp_DownloadBehavior_DownloadPath"></a>

### DownloadPath


```csharp
public string DownloadPath { get; set; }
```


The default path to save downloaded files to.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

Setting this is required if [Policy](#PuppeteerSharp_DownloadBehavior_Policy) is set to [Allow](/api/PuppeteerSharp/DownloadPolicy/#PuppeteerSharp_DownloadPolicy_Allow) or [AllowAndName](/api/PuppeteerSharp/DownloadPolicy/#PuppeteerSharp_DownloadPolicy_AllowAndName).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DownloadBehavior.cs#L22)

<a id="PuppeteerSharp_DownloadBehavior_Policy_"></a>
<a id="PuppeteerSharp_DownloadBehavior_Policy"></a>

### Policy


```csharp
public DownloadPolicy Policy { get; set; }
```


Whether to allow all or deny all download requests, or use default behavior if available.

**Value:** [DownloadPolicy](/api/PuppeteerSharp/DownloadPolicy/)

Setting this to [AllowAndName](/api/PuppeteerSharp/DownloadPolicy/#PuppeteerSharp_DownloadPolicy_AllowAndName) will name all files according to their download GUIDs.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DownloadBehavior.cs#L14)
