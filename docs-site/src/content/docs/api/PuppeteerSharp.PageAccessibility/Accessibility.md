---
title: "Accessibility"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Accessibility"
  hidden: true
---

<a id="PuppeteerSharp_PageAccessibility_Accessibility"></a>

Namespace: [PuppeteerSharp.PageAccessibility](/api/PuppeteerSharp.PageAccessibility/)


```csharp
public class Accessibility : IAccessibility
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IAccessibility](/api/PuppeteerSharp.PageAccessibility/IAccessibility/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/Accessibility.cs#L15)

## Methods

<a id="PuppeteerSharp_PageAccessibility_Accessibility_SnapshotAsync_"></a>
<a id="PuppeteerSharp_PageAccessibility_Accessibility_SnapshotAsync_PuppeteerSharp_PageAccessibility_AccessibilitySnapshotOptions_"></a>

### SnapshotAsync(AccessibilitySnapshotOptions)


```csharp
public Task<SerializedAXNode> SnapshotAsync(AccessibilitySnapshotOptions options = null)
```


Snapshots the async.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [AccessibilitySnapshotOptions](/api/PuppeteerSharp.PageAccessibility/AccessibilitySnapshotOptions/) | Options. |

**Returns:** [Task&lt;SerializedAXNode&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The async.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/Accessibility.cs#L29)
