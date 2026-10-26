---
title: "FirefoxLauncher"
description: "Represents a Firefox process and any associated temporary user data directory that have created by Puppeteer and therefore must be cleaned up when no longer..."
editUrl: false
prev: false
next: false
sidebar:
  label: "FirefoxLauncher"
  hidden: true
---

<a id="PuppeteerSharp_FirefoxLauncher"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class FirefoxLauncher : LauncherBase, IDisposable
```


Represents a Firefox process and any associated temporary user data directory that have created
by Puppeteer and therefore must be cleaned up when no longer needed.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [LauncherBase](/api/PuppeteerSharp/LauncherBase/)

**Implements:** [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FirefoxLauncher.cs#L16)

## Constructors

<a id="PuppeteerSharp_FirefoxLauncher__ctor_"></a>
<a id="PuppeteerSharp_FirefoxLauncher__ctor_System_String_PuppeteerSharp_LaunchOptions_"></a>

### FirefoxLauncher(string, LaunchOptions)


```csharp
public FirefoxLauncher(string executable, LaunchOptions options)
```


Initializes a new instance of the [FirefoxLauncher](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `executable` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Full path of executable. |
| `options` | [LaunchOptions](/api/PuppeteerSharp/LaunchOptions/) | Options for launching Firefox. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FirefoxLauncher.cs#L31)

## Methods

<a id="PuppeteerSharp_FirefoxLauncher_ToString_"></a>
<a id="PuppeteerSharp_FirefoxLauncher_ToString"></a>

### ToString()


```csharp
public override string ToString()
```


Returns a string that represents the current object.

**Returns:** [string](https://learn.microsoft.com/dotnet/api/system.string) — A string that represents the current object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/FirefoxLauncher.cs#L40)
