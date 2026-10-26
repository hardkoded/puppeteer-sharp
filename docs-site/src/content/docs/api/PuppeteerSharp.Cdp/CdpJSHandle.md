---
title: "CdpJSHandle"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpJSHandle"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpJSHandle"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpJSHandle : JSHandle, ICdpHandle, IJSHandle, IAsyncDisposable
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [JSHandle](/api/PuppeteerSharp/JSHandle/)

**Implements:** [ICdpHandle](/api/PuppeteerSharp.Cdp/ICdpHandle/), [IJSHandle](/api/PuppeteerSharp/IJSHandle/), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpJSHandle.cs#L33)

## Properties

<a id="PuppeteerSharp_Cdp_CdpJSHandle_RemoteObject_"></a>
<a id="PuppeteerSharp_Cdp_CdpJSHandle_RemoteObject"></a>

### RemoteObject


```csharp
public RemoteObject RemoteObject { get; }
```


CDP remote object.

**Value:** [RemoteObject](/api/PuppeteerSharp.Cdp.Messaging/RemoteObject/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpJSHandle.cs#L45)

## Methods

<a id="PuppeteerSharp_Cdp_CdpJSHandle_DisposeAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpJSHandle_DisposeAsync"></a>

### DisposeAsync()


```csharp
public override ValueTask DisposeAsync()
```


Performs application-defined tasks associated with freeing, releasing, or
resetting unmanaged resources asynchronously.

**Returns:** [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask) — A task that represents the asynchronous dispose operation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpJSHandle.cs#L105)

<a id="PuppeteerSharp_Cdp_CdpJSHandle_GetPropertiesAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpJSHandle_GetPropertiesAsync"></a>

### GetPropertiesAsync()


```csharp
public override Task<Dictionary<string, IJSHandle>> GetPropertiesAsync()
```


Returns a `Dictionary%602` with property names as keys and [IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances for the property values.

**Returns:** [Task&lt;Dictionary&lt;string, IJSHandle&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a `Dictionary%602`.

**Example**

```csharp
var handle = await page.EvaluateExpressionHandle("({window, document})");
var properties = await handle.GetPropertiesAsync();
var windowHandle = properties["window"];
var documentHandle = properties["document"];
await handle.DisposeAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpJSHandle.cs#L75)

<a id="PuppeteerSharp_Cdp_CdpJSHandle_JsonValueAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpJSHandle_JsonValueAsync__1"></a>

### `JsonValueAsync<T>()`


```csharp
public override Task<T> JsonValueAsync<T>()
```


Returns a JSON representation of the object.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | A strongly typed object to parse to. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task.

The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpJSHandle.cs#L60)

<a id="PuppeteerSharp_Cdp_CdpJSHandle_ToString_"></a>
<a id="PuppeteerSharp_Cdp_CdpJSHandle_ToString"></a>

### ToString()


```csharp
public override string ToString()
```


Returns a string that represents the current object.

**Returns:** [string](https://learn.microsoft.com/dotnet/api/system.string) — A string that represents the current object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpJSHandle.cs#L124)
