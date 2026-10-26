---
title: "InterceptResolutionState"
description: "Represents the current state of request interception resolution."
editUrl: false
prev: false
next: false
sidebar:
  label: "InterceptResolutionState"
  hidden: true
---

<a id="PuppeteerSharp_InterceptResolutionState"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class InterceptResolutionState
```


Represents the current state of request interception resolution.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionState.cs#L30)

## Constructors

<a id="PuppeteerSharp_InterceptResolutionState__ctor_"></a>
<a id="PuppeteerSharp_InterceptResolutionState__ctor_PuppeteerSharp_InterceptResolutionAction_System_Nullable_System_Int32__"></a>

### InterceptResolutionState(InterceptResolutionAction, int?)


```csharp
public InterceptResolutionState(InterceptResolutionAction action, int? priority = null)
```


Represents the current state of request interception resolution.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `action` | [InterceptResolutionAction](/api/PuppeteerSharp/InterceptResolutionAction/) | The interception action. |
| `priority` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | The interception priority. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionState.cs#L30)

## Properties

<a id="PuppeteerSharp_InterceptResolutionState_Action_"></a>
<a id="PuppeteerSharp_InterceptResolutionState_Action"></a>

### Action


```csharp
public InterceptResolutionAction Action { get; set; }
```


Gets or sets the intercept resolution action.

**Value:** [InterceptResolutionAction](/api/PuppeteerSharp/InterceptResolutionAction/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionState.cs#L35)

<a id="PuppeteerSharp_InterceptResolutionState_Priority_"></a>
<a id="PuppeteerSharp_InterceptResolutionState_Priority"></a>

### Priority


```csharp
public int? Priority { get; set; }
```


Gets or sets the intercept resolution priority.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/InterceptResolutionState.cs#L40)
