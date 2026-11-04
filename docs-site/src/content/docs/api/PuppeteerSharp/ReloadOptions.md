---
title: "ReloadOptions"
description: "Options for ReloadAsync(ReloadOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "ReloadOptions"
  hidden: true
---

<a id="PuppeteerSharp_ReloadOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public record ReloadOptions : NavigationOptions, IEquatable<NavigationOptions>, IEquatable<ReloadOptions>
```


Options for [ReloadAsync(ReloadOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ReloadAsync_PuppeteerSharp_ReloadOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/)

**Implements:** [IEquatable&lt;NavigationOptions&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1), [IEquatable&lt;ReloadOptions&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ReloadOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_ReloadOptions_IgnoreCache_"></a>
<a id="PuppeteerSharp_ReloadOptions_IgnoreCache"></a>

### IgnoreCache


```csharp
public bool IgnoreCache { get; set; }
```


If set to true, the browser caches are ignored for the page reload.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — Defaults to `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ReloadOptions.cs#L12)
