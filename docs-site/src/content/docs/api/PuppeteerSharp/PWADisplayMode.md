---
title: "PWADisplayMode"
description: "If the user prefers opening an installed web app in a standalone window or in a browser tab."
editUrl: false
prev: false
next: false
sidebar:
  label: "PWADisplayMode"
  hidden: true
---

<a id="PuppeteerSharp_PWADisplayMode"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<PWADisplayMode>))]
public enum PWADisplayMode
```


If the user prefers opening an installed web app in a standalone window or in a browser tab.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWADisplayMode.cs#L10)

## Fields

<a id="PuppeteerSharp_PWADisplayMode_Browser"></a>

### Browser


```csharp
[EnumMember(Value = "browser")]
Browser = 1
```


The app opens in a browser tab.

**Value:** [PWADisplayMode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWADisplayMode.cs#L22)

<a id="PuppeteerSharp_PWADisplayMode_Standalone"></a>

### Standalone


```csharp
[EnumMember(Value = "standalone")]
Standalone = 0
```


The app opens in a standalone window.

**Value:** [PWADisplayMode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PWADisplayMode.cs#L16)
