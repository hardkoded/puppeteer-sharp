---
title: "WaitUntilNavigation"
description: "Wait until navigation."
editUrl: false
prev: false
next: false
sidebar:
  label: "WaitUntilNavigation"
  hidden: true
---

<a id="PuppeteerSharp_WaitUntilNavigation"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<WaitUntilNavigation>))]
public enum WaitUntilNavigation
```


Wait until navigation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitUntilNavigation.cs#L9)

## Fields

<a id="PuppeteerSharp_WaitUntilNavigation_DOMContentLoaded"></a>

### DOMContentLoaded


```csharp
DOMContentLoaded = 1
```


Consider navigation to be finished when the `DOMContentLoaded` event is fired.

**Value:** [WaitUntilNavigation](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitUntilNavigation.cs#L20)

<a id="PuppeteerSharp_WaitUntilNavigation_Load"></a>

### Load


```csharp
Load = 0
```


Consider navigation to be finished when the `load` event is fired.

**Value:** [WaitUntilNavigation](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitUntilNavigation.cs#L15)

<a id="PuppeteerSharp_WaitUntilNavigation_Networkidle0"></a>

### Networkidle0


```csharp
Networkidle0 = 2
```


Consider navigation to be finished when there are no more than 0 network connections for at least `500` ms.

**Value:** [WaitUntilNavigation](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitUntilNavigation.cs#L25)

<a id="PuppeteerSharp_WaitUntilNavigation_Networkidle2"></a>

### Networkidle2


```csharp
Networkidle2 = 3
```


Consider navigation to be finished when there are no more than 2 network connections for at least `500` ms.

**Value:** [WaitUntilNavigation](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitUntilNavigation.cs#L30)
