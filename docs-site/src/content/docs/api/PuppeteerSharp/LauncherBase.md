---
title: "LauncherBase"
description: "Represents a Base process and any associated temporary user data directory that have created by Puppeteer and therefore must be cleaned up when no longer nee..."
editUrl: false
prev: false
next: false
sidebar:
  label: "LauncherBase"
  hidden: true
---

<a id="PuppeteerSharp_LauncherBase"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class LauncherBase : IDisposable
```


Represents a Base process and any associated temporary user data directory that have created
by Puppeteer and therefore must be cleaned up when no longer needed.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

**Derived:** [ChromeLauncher](/api/PuppeteerSharp/ChromeLauncher/), [FirefoxLauncher](/api/PuppeteerSharp/FirefoxLauncher/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L17)

## Constructors

<a id="PuppeteerSharp_LauncherBase__ctor_"></a>
<a id="PuppeteerSharp_LauncherBase__ctor_System_String_PuppeteerSharp_LaunchOptions_"></a>

### LauncherBase(string, LaunchOptions)


```csharp
protected LauncherBase(string executable, LaunchOptions options)
```


Initializes a new instance of the [LauncherBase](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `executable` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Full path of executable. |
| `options` | [LaunchOptions](/api/PuppeteerSharp/LaunchOptions/) | Options for launching Base. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L27)

## Properties

<a id="PuppeteerSharp_LauncherBase_EndPoint_"></a>
<a id="PuppeteerSharp_LauncherBase_EndPoint"></a>

### EndPoint


```csharp
public string EndPoint { get; }
```


Gets Base endpoint.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L72)

<a id="PuppeteerSharp_LauncherBase_HasExited_"></a>
<a id="PuppeteerSharp_LauncherBase_HasExited"></a>

### HasExited


```csharp
public bool HasExited { get; }
```


Indicates whether Base process has exited.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L84)

<a id="PuppeteerSharp_LauncherBase_IsExiting_"></a>
<a id="PuppeteerSharp_LauncherBase_IsExiting"></a>

### IsExiting


```csharp
public bool IsExiting { get; }
```


Indicates whether Base process is exiting.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L79)

<a id="PuppeteerSharp_LauncherBase_Process_"></a>
<a id="PuppeteerSharp_LauncherBase_Process"></a>

### Process


```csharp
public Process Process { get; }
```


Gets Base process details.

**Value:** [Process](https://learn.microsoft.com/dotnet/api/system.diagnostics.process)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L67)

## Methods

<a id="PuppeteerSharp_LauncherBase_Dispose_"></a>
<a id="PuppeteerSharp_LauncherBase_Dispose"></a>

### Dispose()


```csharp
public void Dispose()
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L112)

<a id="PuppeteerSharp_LauncherBase_Dispose_System_Boolean_"></a>

### Dispose(bool)


```csharp
protected virtual void Dispose(bool disposing)
```


Disposes Base process and any temporary user directory.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `disposing` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Indicates whether disposal was initiated by [Dispose()](#PuppeteerSharp_LauncherBase_Dispose) operation. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L236)

<a id="PuppeteerSharp_LauncherBase_EnsureExitAsync_"></a>
<a id="PuppeteerSharp_LauncherBase_EnsureExitAsync_System_Nullable_System_TimeSpan__"></a>

### EnsureExitAsync(TimeSpan?)


```csharp
public Task EnsureExitAsync(TimeSpan? timeout)
```


Asynchronously waits for graceful Base process exit within a given timeout period.
Kills the Base process if it has not exited within this period.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timeout` | [TimeSpan?](https://learn.microsoft.com/dotnet/api/system.timespan) | The maximum waiting time for a graceful process exit. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the process is exited or killed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L130)

<a id="PuppeteerSharp_LauncherBase_Finalize_"></a>
<a id="PuppeteerSharp_LauncherBase_Finalize"></a>

### ~LauncherBase()


```csharp
protected ~LauncherBase()
```


Finalizes an instance of the [LauncherBase](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L59)

<a id="PuppeteerSharp_LauncherBase_KillAsync_"></a>
<a id="PuppeteerSharp_LauncherBase_KillAsync"></a>

### KillAsync()


```csharp
public Task KillAsync()
```


Asynchronously kills Base process.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the process is killed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L138)

<a id="PuppeteerSharp_LauncherBase_SetEnvVariables_"></a>
<a id="PuppeteerSharp_LauncherBase_SetEnvVariables_System_Collections_Generic_IDictionary_System_String_System_String__System_Collections_Generic_IDictionary_System_String_System_String__System_Collections_IDictionary_"></a>

### `SetEnvVariables(IDictionary<string, string>, IDictionary<string, string>, IDictionary)`


```csharp
protected static void SetEnvVariables(IDictionary<string, string> environment, IDictionary<string, string> customEnv, IDictionary realEnv)
```


Set Env Variables.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `environment` | [IDictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.idictionary-2) | The environment. |
| `customEnv` | [IDictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.idictionary-2) | The customEnv. |
| `realEnv` | [IDictionary](https://learn.microsoft.com/dotnet/api/system.collections.idictionary) | The realEnv. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L206)

<a id="PuppeteerSharp_LauncherBase_StartAsync_"></a>
<a id="PuppeteerSharp_LauncherBase_StartAsync"></a>

### StartAsync()


```csharp
public Task StartAsync()
```


Asynchronously starts Base process.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when after start process begins.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L122)

<a id="PuppeteerSharp_LauncherBase_WaitForExitAsync_"></a>
<a id="PuppeteerSharp_LauncherBase_WaitForExitAsync_System_Nullable_System_TimeSpan__"></a>

### WaitForExitAsync(TimeSpan?)


```csharp
public Task<bool> WaitForExitAsync(TimeSpan? timeout)
```


Waits for Base process exit within a given timeout.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timeout` | [TimeSpan?](https://learn.microsoft.com/dotnet/api/system.timespan) | The maximum wait period. |

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — `true` if Base process has exited within the given &lt;code class="paramref"&gt;timeout&lt;/code&gt;,
    or `false` otherwise.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LauncherBase.cs#L146)
