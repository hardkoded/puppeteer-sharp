---
title: "IBrowserOptions"
description: "Browser options."
editUrl: false
prev: false
next: false
sidebar:
  label: "IBrowserOptions"
  hidden: true
---

<a id="PuppeteerSharp_IBrowserOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IBrowserOptions
```


Browser options.

**Derived:** [ConnectOptions](/api/PuppeteerSharp/ConnectOptions/), [LaunchOptions](/api/PuppeteerSharp/LaunchOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_IBrowserOptions_AcceptInsecureCerts_"></a>
<a id="PuppeteerSharp_IBrowserOptions_AcceptInsecureCerts"></a>

### AcceptInsecureCerts


```csharp
bool AcceptInsecureCerts { get; }
```


Whether to ignore HTTPS errors during navigation. Defaults to false.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserOptions.cs#L11)

<a id="PuppeteerSharp_IBrowserOptions_DefaultViewport_"></a>
<a id="PuppeteerSharp_IBrowserOptions_DefaultViewport"></a>

### DefaultViewport


```csharp
ViewPortOptions DefaultViewport { get; set; }
```


Gets or sets the default Viewport.

**Value:** [ViewPortOptions](/api/PuppeteerSharp/ViewPortOptions/) — The default Viewport.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserOptions.cs#L29)

<a id="PuppeteerSharp_IBrowserOptions_DownloadBehavior_"></a>
<a id="PuppeteerSharp_IBrowserOptions_DownloadBehavior"></a>

### DownloadBehavior


```csharp
DownloadBehavior DownloadBehavior { get; set; }
```


Gets or sets the download behavior applied to the default browser context.

**Value:** [DownloadBehavior](/api/PuppeteerSharp/DownloadBehavior/) — The download behavior.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserOptions.cs#L35)

<a id="PuppeteerSharp_IBrowserOptions_IssuesEnabled_"></a>
<a id="PuppeteerSharp_IBrowserOptions_IssuesEnabled"></a>

### IssuesEnabled


```csharp
bool IssuesEnabled { get; set; }
```


Experimental setting to disable monitoring issue events by default.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserOptions.cs#L23)

<a id="PuppeteerSharp_IBrowserOptions_NetworkEnabled_"></a>
<a id="PuppeteerSharp_IBrowserOptions_NetworkEnabled"></a>

### NetworkEnabled


```csharp
bool NetworkEnabled { get; set; }
```


Experimental setting to disable monitoring network events by default. When
set to `false`, parts of Puppeteer that depend on network events would not
work such as [IRequest](/api/PuppeteerSharp/IRequest/) and [IResponse](/api/PuppeteerSharp/IResponse/).

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserOptions.cs#L18)
