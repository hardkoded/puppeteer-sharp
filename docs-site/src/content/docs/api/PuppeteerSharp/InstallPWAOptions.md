---
title: "InstallPWAOptions"
description: "Options for InstallPWAAsync(InstallPWAOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "InstallPWAOptions"
  hidden: true
---

<a id="PuppeteerSharp_InstallPWAOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class InstallPWAOptions
```


Options for [InstallPWAAsync(InstallPWAOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_InstallPWAAsync_PuppeteerSharp_InstallPWAOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InstallPWAOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_InstallPWAOptions_DisplayMode_"></a>
<a id="PuppeteerSharp_InstallPWAOptions_DisplayMode"></a>

### DisplayMode


```csharp
public PWADisplayMode? DisplayMode { get; set; }
```


Gets or sets whether the app should open in a standalone window or a browser tab.

**Value:** `PWADisplayMode?`

`PWA.install` alone leaves the app at Chromium's default display mode (`browser`);
setting this chains a `PWA.changeAppUserSettings` call to apply the preference.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InstallPWAOptions.cs#L30)

<a id="PuppeteerSharp_InstallPWAOptions_InstallUrlOrBundleUrl_"></a>
<a id="PuppeteerSharp_InstallPWAOptions_InstallUrlOrBundleUrl"></a>

### InstallUrlOrBundleUrl


```csharp
public string InstallUrlOrBundleUrl { get; set; }
```


Gets or sets the URL used to install the app, or the URL of its signed web bundle.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

This is required because the browser-scoped CDP session has no associated page from which Chromium
could derive an install URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InstallPWAOptions.cs#L21)

<a id="PuppeteerSharp_InstallPWAOptions_ManifestId_"></a>
<a id="PuppeteerSharp_InstallPWAOptions_ManifestId"></a>

### ManifestId


```csharp
public string ManifestId { get; set; }
```


Gets or sets the id from the web app's manifest file, commonly the URL of the site installing the web app.
See &lt;a href="https://web.dev/learn/pwa/web-app-manifest"&gt;Web app manifest&lt;/a&gt;.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InstallPWAOptions.cs#L12)
