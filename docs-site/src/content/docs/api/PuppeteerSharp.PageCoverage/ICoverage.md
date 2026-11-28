---
title: "ICoverage"
description: "gathers information about parts of JavaScript and CSS that were used by the page."
editUrl: false
prev: false
next: false
sidebar:
  label: "ICoverage"
  hidden: true
---

<a id="PuppeteerSharp_PageCoverage_ICoverage"></a>

Namespace: [PuppeteerSharp.PageCoverage](/api/PuppeteerSharp.PageCoverage/)


```csharp
public interface ICoverage
```


gathers information about parts of JavaScript and CSS that were used by the page.

**Derived:** [Coverage](/api/PuppeteerSharp.PageCoverage/Coverage/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/ICoverage.cs#L8)

## Methods

<a id="PuppeteerSharp_PageCoverage_ICoverage_StartCSSCoverageAsync_"></a>
<a id="PuppeteerSharp_PageCoverage_ICoverage_StartCSSCoverageAsync_PuppeteerSharp_PageCoverage_CoverageStartOptions_"></a>

### StartCSSCoverageAsync(CoverageStartOptions)


```csharp
Task StartCSSCoverageAsync(CoverageStartOptions options = null)
```


Starts CSS coverage.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [CoverageStartOptions](/api/PuppeteerSharp.PageCoverage/CoverageStartOptions/) | Set of configurable options for coverage. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when coverage is started.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/ICoverage.cs#L15)

<a id="PuppeteerSharp_PageCoverage_ICoverage_StartJSCoverageAsync_"></a>
<a id="PuppeteerSharp_PageCoverage_ICoverage_StartJSCoverageAsync_PuppeteerSharp_PageCoverage_CoverageStartOptions_"></a>

### StartJSCoverageAsync(CoverageStartOptions)


```csharp
Task StartJSCoverageAsync(CoverageStartOptions options = null)
```


Starts JS coverage.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [CoverageStartOptions](/api/PuppeteerSharp.PageCoverage/CoverageStartOptions/) | Set of configurable options for coverage defaults to resetOnNavigation : true, reportAnonymousScripts : false, includeRawScriptCoverage : false, useBlockCoverage : true. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when coverage is started.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/ICoverage.cs#L24)

<a id="PuppeteerSharp_PageCoverage_ICoverage_StopCSSCoverageAsync_"></a>
<a id="PuppeteerSharp_PageCoverage_ICoverage_StopCSSCoverageAsync"></a>

### StopCSSCoverageAsync()


```csharp
Task<CoverageEntry[]> StopCSSCoverageAsync()
```


Stops JS coverage and returns coverage reports for all non-anonymous scripts.

**Returns:** [Task&lt;CoverageEntry\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task that resolves to the array of coverage reports for all stylesheets.

JavaScript Coverage doesn't include anonymous scripts; however, scripts with sourceURLs are reported.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/ICoverage.cs#L33)

<a id="PuppeteerSharp_PageCoverage_ICoverage_StopJSCoverageAsync_"></a>
<a id="PuppeteerSharp_PageCoverage_ICoverage_StopJSCoverageAsync"></a>

### StopJSCoverageAsync()


```csharp
Task<JSCoverageEntry[]> StopJSCoverageAsync()
```


Stops JS coverage and returns coverage reports for all scripts.

**Returns:** [Task&lt;JSCoverageEntry\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task that resolves to the array of coverage reports for all stylesheets.

JavaScript Coverage doesn't include anonymous scripts by default; however, scripts with sourceURLs are reported.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/ICoverage.cs#L42)
