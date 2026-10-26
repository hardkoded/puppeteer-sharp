---
title: "TabInfo"
description: "Information about the browser tab of a page. See TabInfoAsync()."
editUrl: false
prev: false
next: false
sidebar:
  label: "TabInfo"
  hidden: true
---

<a id="PuppeteerSharp_TabInfo"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class TabInfo
```


Information about the browser tab of a page. See [TabInfoAsync()](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_TabInfoAsync).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

This is an experimental API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TabInfo.cs#L9)

## Properties

<a id="PuppeteerSharp_TabInfo_Active_"></a>
<a id="PuppeteerSharp_TabInfo_Active"></a>

### Active


```csharp
public bool Active { get; set; }
```


Gets or sets a value indicating whether the tab is the active tab in its window.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TabInfo.cs#L19)

<a id="PuppeteerSharp_TabInfo_GroupId_"></a>
<a id="PuppeteerSharp_TabInfo_GroupId"></a>

### GroupId


```csharp
public string GroupId { get; set; }
```


Gets or sets the ID of the tab group the tab belongs to, if any.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TabInfo.cs#L29)

<a id="PuppeteerSharp_TabInfo_Index_"></a>
<a id="PuppeteerSharp_TabInfo_Index"></a>

### Index


```csharp
public int Index { get; set; }
```


Gets or sets the zero-based index of the tab in its window's tab strip.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TabInfo.cs#L14)

<a id="PuppeteerSharp_TabInfo_Pinned_"></a>
<a id="PuppeteerSharp_TabInfo_Pinned"></a>

### Pinned


```csharp
public bool Pinned { get; set; }
```


Gets or sets a value indicating whether the tab is pinned.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TabInfo.cs#L24)
