---
title: "Realm"
description: "Represents an execution context (realm) within a frame or worker."
editUrl: false
prev: false
next: false
sidebar:
  label: "Realm"
  hidden: true
---

<a id="PuppeteerSharp_Realm"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class Realm
```


Represents an execution context (realm) within a frame or worker.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Realm.cs#L9)

## Constructors

<a id="PuppeteerSharp_Realm__ctor_"></a>
<a id="PuppeteerSharp_Realm__ctor_PuppeteerSharp_TimeoutSettings_"></a>

### Realm(TimeoutSettings)


```csharp
protected Realm(TimeoutSettings timeoutSettings)
```


Represents an execution context (realm) within a frame or worker.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timeoutSettings` | [TimeoutSettings](/api/PuppeteerSharp/TimeoutSettings/) |  |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Realm.cs#L9)

## Properties

<a id="PuppeteerSharp_Realm_Origin_"></a>
<a id="PuppeteerSharp_Realm_Origin"></a>

### Origin


```csharp
public abstract string Origin { get; }
```


Gets the origin that created this Realm.
For example, a Chrome extension content script would have an origin like
`chrome-extension://<extension-id>`.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

This API is experimental.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Realm.cs#L17)

## Methods

<a id="PuppeteerSharp_Realm_ExtensionAsync_"></a>
<a id="PuppeteerSharp_Realm_ExtensionAsync"></a>

### ExtensionAsync()


```csharp
public abstract Task<Extension> ExtensionAsync()
```


Returns the extension that created this realm, if the realm was created from an Extension.
An example of this is an extension content script running on a page.

**Returns:** [Task&lt;Extension&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the [Extension](/api/PuppeteerSharp/Extension/) that created this realm, or `null`.

This API is experimental.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Realm.cs#L31)
