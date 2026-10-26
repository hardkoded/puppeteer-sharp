---
title: "DeviceRequestPrompt"
description: "Device request prompts let you respond to the page requesting for a device through an API like WebBluetooth."
editUrl: false
prev: false
next: false
sidebar:
  label: "DeviceRequestPrompt"
  hidden: true
---

<a id="PuppeteerSharp_DeviceRequestPrompt"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Listener subscriptions are disposed in SelectAsync/CancelAsync via DisposableActionsStack.")]
public class DeviceRequestPrompt
```


Device request prompts let you respond to the page requesting for a device
through an API like WebBluetooth.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[DeviceRequestPrompt](./) instances are returned via the [WaitForDevicePromptAsync(WaitForOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_WaitForDevicePromptAsync_PuppeteerSharp_WaitForOptions_).

**Example**

```cs

```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeviceRequestPrompt.cs#L20)

## Properties

<a id="PuppeteerSharp_DeviceRequestPrompt_Devices_"></a>
<a id="PuppeteerSharp_DeviceRequestPrompt_Devices"></a>

### Devices


```csharp
public List<DeviceRequestPromptDevice> Devices { get; }
```


Current list of selectable devices.

**Value:** [List&lt;DeviceRequestPromptDevice&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.list-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeviceRequestPrompt.cs#L51)

## Methods

<a id="PuppeteerSharp_DeviceRequestPrompt_CancelAsync_"></a>
<a id="PuppeteerSharp_DeviceRequestPrompt_CancelAsync"></a>

### CancelAsync()


```csharp
public Task CancelAsync()
```


Cancel the prompt.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves after the cancel message is processed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeviceRequestPrompt.cs#L119)

<a id="PuppeteerSharp_DeviceRequestPrompt_SelectAsync_"></a>
<a id="PuppeteerSharp_DeviceRequestPrompt_SelectAsync_PuppeteerSharp_DeviceRequestPromptDevice_"></a>

### SelectAsync(DeviceRequestPromptDevice)


```csharp
public Task SelectAsync(DeviceRequestPromptDevice device)
```


Select a device in the prompt's list.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `device` | [DeviceRequestPromptDevice](/api/PuppeteerSharp/DeviceRequestPromptDevice/) | The device to select. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves after the select message is processed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeviceRequestPrompt.cs#L58)

<a id="PuppeteerSharp_DeviceRequestPrompt_WaitForDeviceAsync_"></a>
<a id="PuppeteerSharp_DeviceRequestPrompt_WaitForDeviceAsync_System_Func_PuppeteerSharp_DeviceRequestPromptDevice_System_Boolean__PuppeteerSharp_WaitForOptions_"></a>

### `WaitForDeviceAsync(Func<DeviceRequestPromptDevice, bool>, WaitForOptions)`


```csharp
public Task<DeviceRequestPromptDevice> WaitForDeviceAsync(Func<DeviceRequestPromptDevice, bool> filter, WaitForOptions options = null)
```


Resolve to the first device in the prompt matching a filter.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `filter` | [Func&lt;DeviceRequestPromptDevice, bool&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | The filter to apply. |
| `options` | [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | The options. |

**Returns:** [Task&lt;DeviceRequestPromptDevice&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the first device matching the filter.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/DeviceRequestPrompt.cs#L91)
