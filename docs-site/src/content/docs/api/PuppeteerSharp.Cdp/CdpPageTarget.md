---
title: "CdpPageTarget"
description: "Page target."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpPageTarget"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpPageTarget"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpPageTarget : CdpTarget, ITarget
```


Page target.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Target](/api/PuppeteerSharp/Target/) → [CdpTarget](/api/PuppeteerSharp.Cdp/CdpTarget/)

**Implements:** [ITarget](/api/PuppeteerSharp/ITarget/)

**Derived:** [CdpDevToolsTarget](/api/PuppeteerSharp.Cdp/CdpDevToolsTarget/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpPageTarget.cs#L10)

## Methods

<a id="PuppeteerSharp_Cdp_CdpPageTarget_AsPageAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpPageTarget_AsPageAsync"></a>

### AsPageAsync()


```csharp
public override Task<IPage> AsPageAsync()
```


Forcefully creates a page for a target of any type. It is useful if you
want to handle a CDP target of type `other` as a page. If you deal with a
regular page target, use [PageAsync()](/api/PuppeteerSharp/ITarget/#PuppeteerSharp_ITarget_PageAsync).

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that returns a [IPage](/api/PuppeteerSharp/IPage/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpPageTarget.cs#L33)

<a id="PuppeteerSharp_Cdp_CdpPageTarget_CheckIfInitialized_"></a>
<a id="PuppeteerSharp_Cdp_CdpPageTarget_CheckIfInitialized"></a>

### CheckIfInitialized()


```csharp
protected override void CheckIfInitialized()
```


Check is the target is not initialized.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpPageTarget.cs#L97)

<a id="PuppeteerSharp_Cdp_CdpPageTarget_PageAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpPageTarget_PageAsync"></a>

### PageAsync()


```csharp
public override Task<IPage> PageAsync()
```


Returns the [IPage](/api/PuppeteerSharp/IPage/) associated with the target. If the target is not `"page"`, `"webview"` or `"background_page"` returns `null`.

**Returns:** [Task&lt;IPage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — a task that returns a [IPage](/api/PuppeteerSharp/IPage/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpPageTarget.cs#L48)
