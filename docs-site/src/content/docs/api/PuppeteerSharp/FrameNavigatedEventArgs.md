---
title: "FrameNavigatedEventArgs"
description: "FrameNavigated arguments."
editUrl: false
prev: false
next: false
sidebar:
  label: "FrameNavigatedEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_FrameNavigatedEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public record FrameNavigatedEventArgs : IEquatable<FrameNavigatedEventArgs>
```


[FrameNavigated](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameNavigated) arguments.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;FrameNavigatedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FrameNavigatedEventArgs.cs#L8)

## Properties

<a id="PuppeteerSharp_FrameNavigatedEventArgs_Frame_"></a>
<a id="PuppeteerSharp_FrameNavigatedEventArgs_Frame"></a>

### Frame


```csharp
public IFrame Frame { get; set; }
```


Gets or sets the frame.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/) — The frame.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FrameNavigatedEventArgs.cs#L27)

<a id="PuppeteerSharp_FrameNavigatedEventArgs_Type_"></a>
<a id="PuppeteerSharp_FrameNavigatedEventArgs_Type"></a>

### Type


```csharp
public NavigationType Type { get; }
```


Navigation type.

**Value:** [NavigationType](/api/PuppeteerSharp.Cdp.Messaging/NavigationType/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FrameNavigatedEventArgs.cs#L32)
