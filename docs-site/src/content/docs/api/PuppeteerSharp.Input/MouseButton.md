---
title: "MouseButton"
description: "The type of button click to use with DownAsync(ClickOptions), UpAsync(ClickOptions) and ClickOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "MouseButton"
  hidden: true
---

<a id="PuppeteerSharp_Input_MouseButton"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<MouseButton>))]
[Flags]
public enum MouseButton
```


The type of button click to use with [DownAsync(ClickOptions)](/api/PuppeteerSharp.Input/IMouse/#PuppeteerSharp_Input_IMouse_DownAsync_PuppeteerSharp_Input_ClickOptions_), [UpAsync(ClickOptions)](/api/PuppeteerSharp.Input/IMouse/#PuppeteerSharp_Input_IMouse_UpAsync_PuppeteerSharp_Input_ClickOptions_) and `ClickOptions)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/MouseButton.cs#L11)

## Fields

<a id="PuppeteerSharp_Input_MouseButton_Back"></a>

### Back


```csharp
[EnumMember(Value = "back")]
Back = 8
```


The back mouse button.

**Value:** [MouseButton](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/MouseButton.cs#L44)

<a id="PuppeteerSharp_Input_MouseButton_Forward"></a>

### Forward


```csharp
[EnumMember(Value = "forward")]
Forward = 16
```


The forward mouse button.

**Value:** [MouseButton](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/MouseButton.cs#L50)

<a id="PuppeteerSharp_Input_MouseButton_Left"></a>

### Left


```csharp
[EnumMember(Value = "left")]
Left = 1
```


The left mouse button.

**Value:** [MouseButton](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/MouseButton.cs#L26)

<a id="PuppeteerSharp_Input_MouseButton_Middle"></a>

### Middle


```csharp
[EnumMember(Value = "middle")]
Middle = 4
```


The middle mouse button.

**Value:** [MouseButton](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/MouseButton.cs#L38)

<a id="PuppeteerSharp_Input_MouseButton_None"></a>

### None


```csharp
[EnumMember(Value = "none")]
None = 0
```


Non specified.

**Value:** [MouseButton](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/MouseButton.cs#L20)

<a id="PuppeteerSharp_Input_MouseButton_Right"></a>

### Right


```csharp
[EnumMember(Value = "right")]
Right = 2
```


The right mouse button.

**Value:** [MouseButton](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/MouseButton.cs#L32)
