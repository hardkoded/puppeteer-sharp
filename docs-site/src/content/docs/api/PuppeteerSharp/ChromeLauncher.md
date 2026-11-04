---
title: "ChromeLauncher"
description: "Represents a Chromium process and any associated temporary user data directory that have created by Puppeteer and therefore must be cleaned up when no longer..."
editUrl: false
prev: false
next: false
sidebar:
  label: "ChromeLauncher"
  hidden: true
---

<a id="PuppeteerSharp_ChromeLauncher"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ChromeLauncher : LauncherBase, IDisposable
```


Represents a Chromium process and any associated temporary user data directory that have created
by Puppeteer and therefore must be cleaned up when no longer needed.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [LauncherBase](/api/PuppeteerSharp/LauncherBase/)

**Implements:** [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ChromeLauncher.cs#L16)

## Constructors

<a id="PuppeteerSharp_ChromeLauncher__ctor_"></a>
<a id="PuppeteerSharp_ChromeLauncher__ctor_System_String_PuppeteerSharp_LaunchOptions_"></a>

### ChromeLauncher(string, LaunchOptions)


```csharp
public ChromeLauncher(string executable, LaunchOptions options)
```


Initializes a new instance of the [ChromeLauncher](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `executable` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Full path of executable. |
| `options` | [LaunchOptions](/api/PuppeteerSharp/LaunchOptions/) | Options for launching Chromium. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ChromeLauncher.cs#L29)

## Methods

<a id="PuppeteerSharp_ChromeLauncher_Dispose_"></a>
<a id="PuppeteerSharp_ChromeLauncher_Dispose_System_Boolean_"></a>

### Dispose(bool)


```csharp
protected override void Dispose(bool disposing)
```


Disposes Base process and any temporary user directory.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `disposing` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Indicates whether disposal was initiated by [Dispose()](/api/PuppeteerSharp/LauncherBase/#PuppeteerSharp_LauncherBase_Dispose) operation. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ChromeLauncher.cs#L211)

<a id="PuppeteerSharp_ChromeLauncher_ToString_"></a>
<a id="PuppeteerSharp_ChromeLauncher_ToString"></a>

### ToString()


```csharp
public override string ToString()
```


Returns a string that represents the current object.

**Returns:** [string](https://learn.microsoft.com/dotnet/api/system.string) — A string that represents the current object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ChromeLauncher.cs#L48)
