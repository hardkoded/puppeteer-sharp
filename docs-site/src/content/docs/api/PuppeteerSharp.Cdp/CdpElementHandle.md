---
title: "CdpElementHandle"
description: "Inherits from IJSHandle. It represents an in-page DOM element. ElementHandles can be created by QuerySelectorAsync(string) or QuerySelectorAllAsync(string)."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpElementHandle"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpElementHandle"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpElementHandle : ElementHandle, IElementHandle, ICdpHandle, IJSHandle, IAsyncDisposable
```


Inherits from [IJSHandle](/api/PuppeteerSharp/IJSHandle/). It represents an in-page DOM element.
ElementHandles can be created by [QuerySelectorAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAsync_System_String_) or [QuerySelectorAllAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAllAsync_System_String_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [JSHandle](/api/PuppeteerSharp/JSHandle/) → [ElementHandle](/api/PuppeteerSharp/ElementHandle/)

**Implements:** [IElementHandle](/api/PuppeteerSharp/IElementHandle/), [ICdpHandle](/api/PuppeteerSharp.Cdp/ICdpHandle/), [IJSHandle](/api/PuppeteerSharp/IJSHandle/), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpElementHandle.cs#L35)

## Properties

<a id="PuppeteerSharp_Cdp_CdpElementHandle_Page_"></a>
<a id="PuppeteerSharp_Cdp_CdpElementHandle_Page"></a>

### Page


```csharp
protected override Page Page { get; }
```


Element's page.

**Value:** [Page](/api/PuppeteerSharp/Page/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpElementHandle.cs#L69)

<a id="PuppeteerSharp_Cdp_CdpElementHandle_RemoteObject_"></a>
<a id="PuppeteerSharp_Cdp_CdpElementHandle_RemoteObject"></a>

### RemoteObject


```csharp
public RemoteObject RemoteObject { get; }
```


CDP Remote object.

**Value:** [RemoteObject](/api/PuppeteerSharp.Cdp.Messaging/RemoteObject/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpElementHandle.cs#L54)

## Methods

<a id="PuppeteerSharp_Cdp_CdpElementHandle_AutofillAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpElementHandle_AutofillAsync_PuppeteerSharp_AutofillData_"></a>

### AutofillAsync(AutofillData)


```csharp
public override Task AutofillAsync(AutofillData data)
```


If the element is a form input, you can use [AutofillAsync(AutofillData)](/api/PuppeteerSharp/IElementHandle/#PuppeteerSharp_IElementHandle_AutofillAsync_PuppeteerSharp_AutofillData_)
to test if the form is compatible with the browser's autofill implementation.
Throws an error if the form cannot be autofilled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `data` | [AutofillData](/api/PuppeteerSharp/AutofillData/) | The autofill data. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the autofill is complete.

Currently, Puppeteer supports auto-filling credit card information only and
in Chrome in the new headless and headful modes only.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpElementHandle.cs#L182)

<a id="PuppeteerSharp_Cdp_CdpElementHandle_BackendNodeIdAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpElementHandle_BackendNodeIdAsync"></a>

### BackendNodeIdAsync()


```csharp
public override Task<int> BackendNodeIdAsync()
```


When connected using Chrome DevTools Protocol, it returns a DOM.BackendNodeId for the element.

**Returns:** [Task&lt;int&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the backend node ID.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpElementHandle.cs#L166)

<a id="PuppeteerSharp_Cdp_CdpElementHandle_ContentFrameAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpElementHandle_ContentFrameAsync"></a>

### ContentFrameAsync()


```csharp
public override Task<IFrame> ContentFrameAsync()
```


Resolves the frame associated with the element..

**Returns:** [Task&lt;IFrame&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the frame associated with the element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpElementHandle.cs#L83)

<a id="PuppeteerSharp_Cdp_CdpElementHandle_ScrollIntoViewAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpElementHandle_ScrollIntoViewAsync"></a>

### ScrollIntoViewAsync()


```csharp
public override Task ScrollIntoViewAsync()
```


Scrolls the element into view using either the automation protocol client or by calling element.scrollIntoView.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpElementHandle.cs#L95)

<a id="PuppeteerSharp_Cdp_CdpElementHandle_ToString_"></a>
<a id="PuppeteerSharp_Cdp_CdpElementHandle_ToString"></a>

### ToString()


```csharp
public override string ToString()
```


Returns a string that represents the current object.

**Returns:** [string](https://learn.microsoft.com/dotnet/api/system.string) — A string that represents the current object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpElementHandle.cs#L206)

<a id="PuppeteerSharp_Cdp_CdpElementHandle_UploadFileAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpElementHandle_UploadFileAsync_System_Boolean_System_String___"></a>

### UploadFileAsync(bool, params string[])


```csharp
public override Task UploadFileAsync(bool resolveFilePaths, params string[] filePaths)
```


Uploads files.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `resolveFilePaths` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Set to true to resolve paths using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |
| `filePaths` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Sets the value of the file input to these paths. Paths are resolved using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

This method expects `elementHandle` to point to an `input element` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input"&gt;https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input&lt;/a&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpElementHandle.cs#L117)
