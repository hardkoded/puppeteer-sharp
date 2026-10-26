---
title: "FrameEventArgs"
description: "FrameAttached, FrameDetached and FrameNavigated arguments."
editUrl: false
prev: false
next: false
sidebar:
  label: "FrameEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_FrameEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class FrameEventArgs
```


[FrameAttached](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameAttached), [FrameDetached](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameDetached) and [FrameNavigated](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_FrameNavigated) arguments.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FrameEventArgs.cs#L6)

## Constructors

<a id="PuppeteerSharp_FrameEventArgs__ctor_"></a>
<a id="PuppeteerSharp_FrameEventArgs__ctor_PuppeteerSharp_IFrame_"></a>

### FrameEventArgs(IFrame)


```csharp
public FrameEventArgs(IFrame frame)
```


Initializes a new instance of the [FrameEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `frame` | [IFrame](/api/PuppeteerSharp/IFrame/) | Frame. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FrameEventArgs.cs#L12)

## Properties

<a id="PuppeteerSharp_FrameEventArgs_Frame_"></a>
<a id="PuppeteerSharp_FrameEventArgs_Frame"></a>

### Frame


```csharp
public IFrame Frame { get; set; }
```


Gets or sets the frame.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/) — The frame.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FrameEventArgs.cs#L21)
