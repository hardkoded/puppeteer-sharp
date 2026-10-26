---
title: "WaitForFunctionPollingOption"
description: "An interval at which the pageFunction is executed."
editUrl: false
prev: false
next: false
sidebar:
  label: "WaitForFunctionPollingOption"
  hidden: true
---

<a id="PuppeteerSharp_WaitForFunctionPollingOption"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<WaitForFunctionPollingOption>))]
public enum WaitForFunctionPollingOption
```


An interval at which the `pageFunction` is executed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForFunctionPollingOption.cs#L9)

## Fields

<a id="PuppeteerSharp_WaitForFunctionPollingOption_Mutation"></a>

### Mutation


```csharp
Mutation = 1
```


To execute `pageFunction` on every DOM mutation.

**Value:** [WaitForFunctionPollingOption](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForFunctionPollingOption.cs#L21)

<a id="PuppeteerSharp_WaitForFunctionPollingOption_Raf"></a>

### Raf


```csharp
Raf = 0
```


To constantly execute `pageFunction` in `requestAnimationFrame` callback.
This is the tightest polling mode which is suitable to observe styling changes.

**Value:** [WaitForFunctionPollingOption](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForFunctionPollingOption.cs#L16)
