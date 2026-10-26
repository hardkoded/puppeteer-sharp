---
title: "GetContentOptions"
description: "Options for GetContentAsync(GetContentOptions) and GetContentAsync(GetContentOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "GetContentOptions"
  hidden: true
---

<a id="PuppeteerSharp_GetContentOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class GetContentOptions
```


Options for [GetContentAsync(GetContentOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_GetContentAsync_PuppeteerSharp_GetContentOptions_) and [GetContentAsync(GetContentOptions)](/api/PuppeteerSharp/IFrame/#PuppeteerSharp_IFrame_GetContentAsync_PuppeteerSharp_GetContentOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/GetContentOptions.cs#L28)

## Properties

<a id="PuppeteerSharp_GetContentOptions_ReplaceLoneSurrogates_"></a>
<a id="PuppeteerSharp_GetContentOptions_ReplaceLoneSurrogates"></a>

### ReplaceLoneSurrogates


```csharp
public bool ReplaceLoneSurrogates { get; set; }
```


Replace lone surrogates with U+FFFD replacement character.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

This functionality relies on the toWellFormed function. &lt;a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/toWellFormed"&gt;See.&lt;/a&gt;.
It's set to false by default to prevent extra processing when not needed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/GetContentOptions.cs#L37)
