---
title: "RemoteObject"
description: "Remote object is a mirror object referencing original JavaScript object."
editUrl: false
prev: false
next: false
sidebar:
  label: "RemoteObject"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject"></a>

Namespace: [PuppeteerSharp.Cdp.Messaging](/api/PuppeteerSharp.Cdp.Messaging/)


```csharp
public class RemoteObject
```


Remote object is a mirror object referencing original JavaScript object.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Messaging/RemoteObject.cs#L8)

## Properties

<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_ClassName_"></a>
<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_ClassName"></a>

### ClassName


```csharp
public string ClassName { get; set; }
```


Gets or sets the Object class (constructor) name. Specified for object type values only. (Optional).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Messaging/RemoteObject.cs#L18)

<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_Description_"></a>
<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_Description"></a>

### Description


```csharp
public string Description { get; set; }
```


Gets or sets String representation of the object. (Optional).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Messaging/RemoteObject.cs#L13)

<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_ObjectId_"></a>
<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_ObjectId"></a>

### ObjectId


```csharp
public string ObjectId { get; set; }
```


Gets or sets the object identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Messaging/RemoteObject.cs#L33)

<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_Subtype_"></a>
<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_Subtype"></a>

### Subtype


```csharp
public RemoteObjectSubtype Subtype { get; set; }
```


Gets or sets the subtype.

**Value:** [RemoteObjectSubtype](/api/PuppeteerSharp.Cdp.Messaging/RemoteObjectSubtype/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Messaging/RemoteObject.cs#L28)

<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_Type_"></a>
<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_Type"></a>

### Type


```csharp
public RemoteObjectType Type { get; set; }
```


Gets or sets the type.

**Value:** [RemoteObjectType](/api/PuppeteerSharp.Cdp.Messaging/RemoteObjectType/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Messaging/RemoteObject.cs#L23)

<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_UnserializableValue_"></a>
<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_UnserializableValue"></a>

### UnserializableValue


```csharp
public string UnserializableValue { get; set; }
```


Gets or sets the unserializable value.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Messaging/RemoteObject.cs#L38)

<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_Value_"></a>
<a id="PuppeteerSharp_Cdp_Messaging_RemoteObject_Value"></a>

### Value


```csharp
public JsonElement? Value { get; set; }
```


Gets or sets the value.

**Value:** [JsonElement?](https://learn.microsoft.com/dotnet/api/system.text.json.jsonelement)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Messaging/RemoteObject.cs#L43)
