---
title: "InterceptResolutionAction"
description: "Represents the action taken for an intercepted request."
editUrl: false
prev: false
next: false
sidebar:
  label: "InterceptResolutionAction"
  hidden: true
---

<a id="PuppeteerSharp_InterceptResolutionAction"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<InterceptResolutionAction>))]
public enum InterceptResolutionAction
```


Represents the action taken for an intercepted request.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionAction.cs#L31)

## Fields

<a id="PuppeteerSharp_InterceptResolutionAction_Abort"></a>

### Abort


```csharp
Abort = 0
```


The request will be aborted.

**Value:** [InterceptResolutionAction](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionAction.cs#L37)

<a id="PuppeteerSharp_InterceptResolutionAction_AlreadyHandled"></a>

### AlreadyHandled


```csharp
AlreadyHandled = 5
```


The interception has already been handled.

**Value:** [InterceptResolutionAction](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionAction.cs#L62)

<a id="PuppeteerSharp_InterceptResolutionAction_Continue"></a>

### Continue


```csharp
Continue = 2
```


The request will continue normally.

**Value:** [InterceptResolutionAction](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionAction.cs#L47)

<a id="PuppeteerSharp_InterceptResolutionAction_Disabled"></a>

### Disabled


```csharp
Disabled = 3
```


Request interception is disabled.

**Value:** [InterceptResolutionAction](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionAction.cs#L52)

<a id="PuppeteerSharp_InterceptResolutionAction_None"></a>

### None


```csharp
None = 4
```


No interception action has been set yet.

**Value:** [InterceptResolutionAction](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionAction.cs#L57)

<a id="PuppeteerSharp_InterceptResolutionAction_Respond"></a>

### Respond


```csharp
Respond = 1
```


The request will be fulfilled with a custom response.

**Value:** [InterceptResolutionAction](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionAction.cs#L42)
