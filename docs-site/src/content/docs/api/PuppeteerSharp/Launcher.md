---
title: "Launcher"
description: "Launcher controls the creation of processes or the connection remote ones."
editUrl: false
prev: false
next: false
sidebar:
  label: "Launcher"
  hidden: true
---

<a id="PuppeteerSharp_Launcher"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class Launcher
```


Launcher controls the creation of processes or the connection remote ones.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Launcher.cs#L32)

## Constructors

<a id="PuppeteerSharp_Launcher__ctor_"></a>
<a id="PuppeteerSharp_Launcher__ctor_Microsoft_Extensions_Logging_ILoggerFactory_"></a>

### Launcher(ILoggerFactory)


```csharp
public Launcher(ILoggerFactory loggerFactory = null)
```


Initializes a new instance of the [Launcher](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `loggerFactory` | [ILoggerFactory](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.iloggerfactory) | Logger factory. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Launcher.cs#L42)

## Properties

<a id="PuppeteerSharp_Launcher_Process_"></a>
<a id="PuppeteerSharp_Launcher_Process"></a>

### Process


```csharp
public LauncherBase Process { get; }
```


Gets the process, if any was created by this launcher.

**Value:** [LauncherBase](/api/PuppeteerSharp/LauncherBase/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Launcher.cs#L47)

## Methods

<a id="PuppeteerSharp_Launcher_ConnectAsync_"></a>
<a id="PuppeteerSharp_Launcher_ConnectAsync_PuppeteerSharp_ConnectOptions_"></a>

### ConnectAsync(ConnectOptions)


```csharp
public Task<IBrowser> ConnectAsync(ConnectOptions options)
```


Attaches Puppeteer to an existing process instance. The browser will be closed when the Browser is disposed.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ConnectOptions](/api/PuppeteerSharp/ConnectOptions/) | Options for connecting. |

**Returns:** [Task&lt;IBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A connected browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Launcher.cs#L264)

<a id="PuppeteerSharp_Launcher_LaunchAsync_"></a>
<a id="PuppeteerSharp_Launcher_LaunchAsync_PuppeteerSharp_LaunchOptions_"></a>

### LaunchAsync(LaunchOptions)


```csharp
[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The caller is responsible for disposing the returned object.")]
public Task<IBrowser> LaunchAsync(LaunchOptions options)
```


The method launches a browser instance with given arguments. The browser will be closed when the Browser is disposed.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LaunchOptions](/api/PuppeteerSharp/LaunchOptions/) | Options for launching the browser. |

**Returns:** [Task&lt;IBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A connected browser.

See &lt;a href="https://www.howtogeek.com/202825/what%E2%80%99s-the-difference-between-chromium-and-chrome/"&gt;this article&lt;/a&gt;
for a description of the differences between Chromium and Chrome.
&lt;a href="https://chromium.googlesource.com/chromium/src/+/lkcr/docs/chromium_browser_vs_google_chrome.md"&gt;This article&lt;/a&gt; describes some differences for Linux users.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Launcher.cs#L59)
