---
title: "MediaFeatureValue"
description: "Media Feature. MediaFeatureValue%7d)."
editUrl: false
prev: false
next: false
sidebar:
  label: "MediaFeatureValue"
  hidden: true
---

<a id="PuppeteerSharp_MediaFeatureValue"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class MediaFeatureValue
```


Media Feature. `MediaFeatureValue%7d)`.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MediaFeatureValue.cs#L8)

## Properties

<a id="PuppeteerSharp_MediaFeatureValue_MediaFeature_"></a>
<a id="PuppeteerSharp_MediaFeatureValue_MediaFeature"></a>

### MediaFeature


```csharp
[JsonPropertyName("name")]
public MediaFeature MediaFeature { get; set; }
```


The CSS media feature name. Supported names are `'prefers-color-scheme'`, `'prefers-reduced-motion'`, `'prefers-contrast'`, `'prefers-reduced-transparency'` and `'forced-colors'`.

**Value:** [MediaFeature](/api/PuppeteerSharp/MediaFeature/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MediaFeatureValue.cs#L13)

<a id="PuppeteerSharp_MediaFeatureValue_Value_"></a>
<a id="PuppeteerSharp_MediaFeatureValue_Value"></a>

### Value


```csharp
public string Value { get; set; }
```


The value for the given CSS media feature.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MediaFeatureValue.cs#L19)
