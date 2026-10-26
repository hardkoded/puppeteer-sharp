---
title: "CreatePageType"
description: "Specifies the type of page to create."
editUrl: false
prev: false
next: false
sidebar:
  label: "CreatePageType"
  hidden: true
---

<a id="PuppeteerSharp_CreatePageType"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<CreatePageType>))]
public enum CreatePageType
```


Specifies the type of page to create.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CreatePageType.cs#L10)

## Fields

<a id="PuppeteerSharp_CreatePageType_Tab"></a>

### Tab


```csharp
[EnumMember(Value = "tab")]
Tab = 0
```


Create a new tab.

**Value:** [CreatePageType](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CreatePageType.cs#L16)

<a id="PuppeteerSharp_CreatePageType_Window"></a>

### Window


```csharp
[EnumMember(Value = "window")]
Window = 1
```


Create a new window.

**Value:** [CreatePageType](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CreatePageType.cs#L22)
