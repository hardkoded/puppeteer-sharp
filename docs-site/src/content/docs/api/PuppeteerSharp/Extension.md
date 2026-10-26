---
title: "Extension"
description: "Represents an Extension instance installed in the browser."
editUrl: false
prev: false
next: false
sidebar:
  label: "Extension"
  hidden: true
---

<a id="PuppeteerSharp_Extension"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class Extension
```


Represents an Extension instance installed in the browser.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

&lt;p&gt;This API is experimental.&lt;/p&gt;

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Extension.cs#L12)

## Constructors

<a id="PuppeteerSharp_Extension__ctor_"></a>
<a id="PuppeteerSharp_Extension__ctor_System_String_System_String_System_String_System_String_System_Boolean_"></a>

### Extension(string, string, string, string, bool)


```csharp
protected Extension(string id, string version, string name, string path, bool enabled)
```


Initializes a new instance of the [Extension](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `id` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The extension ID. |
| `version` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The extension version. |
| `name` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The extension name. |
| `path` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file system path of the extension. |
| `enabled` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether the extension is enabled. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Extension.cs#L22)

## Properties

<a id="PuppeteerSharp_Extension_Enabled_"></a>
<a id="PuppeteerSharp_Extension_Enabled"></a>

### Enabled


```csharp
public bool Enabled { get; }
```


Gets a value indicating whether the extension is enabled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Extension.cs#L54)

<a id="PuppeteerSharp_Extension_Id_"></a>
<a id="PuppeteerSharp_Extension_Id"></a>

### Id


```csharp
public string Id { get; }
```


Gets the ID of the extension.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Extension.cs#L34)

<a id="PuppeteerSharp_Extension_Name_"></a>
<a id="PuppeteerSharp_Extension_Name"></a>

### Name


```csharp
public string Name { get; }
```


Gets the name of the extension.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Extension.cs#L44)

<a id="PuppeteerSharp_Extension_Path_"></a>
<a id="PuppeteerSharp_Extension_Path"></a>

### Path


```csharp
public string Path { get; }
```


Gets the path in the file system where the extension is located.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Extension.cs#L49)

<a id="PuppeteerSharp_Extension_Version_"></a>
<a id="PuppeteerSharp_Extension_Version"></a>

### Version


```csharp
public string Version { get; }
```


Gets the version of the extension.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Extension.cs#L39)

## Methods

<a id="PuppeteerSharp_Extension_PagesAsync_"></a>
<a id="PuppeteerSharp_Extension_PagesAsync"></a>

### PagesAsync()


```csharp
public abstract Task<IReadOnlyList<IPage>> PagesAsync()
```


Returns the list of the visible pages of the extension.

**Returns:** [Task&lt;IReadOnlyList&lt;IPage&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the list of pages.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Extension.cs#L66)

<a id="PuppeteerSharp_Extension_TriggerActionAsync_"></a>
<a id="PuppeteerSharp_Extension_TriggerActionAsync_PuppeteerSharp_IPage_"></a>

### TriggerActionAsync(IPage)


```csharp
public abstract Task TriggerActionAsync(IPage page)
```


Triggers an extension default action on the given page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `page` | [IPage](/api/PuppeteerSharp/IPage/) | The page to trigger the action on. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the action is triggered.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Extension.cs#L73)

<a id="PuppeteerSharp_Extension_WorkersAsync_"></a>
<a id="PuppeteerSharp_Extension_WorkersAsync"></a>

### WorkersAsync()


```csharp
public abstract Task<IReadOnlyList<WebWorker>> WorkersAsync()
```


Returns the list of the currently active service workers of the extension.

**Returns:** [Task&lt;IReadOnlyList&lt;WebWorker&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the list of web workers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Extension.cs#L60)
