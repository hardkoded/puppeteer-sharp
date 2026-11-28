---
title: "IBluetoothEmulation"
description: "Exposes the bluetooth emulation abilities."
editUrl: false
prev: false
next: false
sidebar:
  label: "IBluetoothEmulation"
  hidden: true
---

<a id="PuppeteerSharp_IBluetoothEmulation"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IBluetoothEmulation
```


Exposes the bluetooth emulation abilities.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBluetoothEmulation.cs#L8)

## Methods

<a id="PuppeteerSharp_IBluetoothEmulation_DisableEmulationAsync_"></a>
<a id="PuppeteerSharp_IBluetoothEmulation_DisableEmulationAsync"></a>

### DisableEmulationAsync()


```csharp
Task DisableEmulationAsync()
```


Disable emulated bluetooth adapter.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) that completes when the emulation is disabled.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBluetoothEmulation.cs#L22)

<a id="PuppeteerSharp_IBluetoothEmulation_EmulateAdapterAsync_"></a>
<a id="PuppeteerSharp_IBluetoothEmulation_EmulateAdapterAsync_PuppeteerSharp_AdapterState_System_Boolean_"></a>

### EmulateAdapterAsync(AdapterState, bool)


```csharp
Task EmulateAdapterAsync(AdapterState state, bool leSupported = true)
```


Emulate Bluetooth adapter. Required for bluetooth simulations.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `state` | [AdapterState](/api/PuppeteerSharp/AdapterState/) | The desired bluetooth adapter state. |
| `leSupported` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Mark if the adapter supports low-energy bluetooth. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) that completes when the adapter is emulated.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBluetoothEmulation.cs#L16)

<a id="PuppeteerSharp_IBluetoothEmulation_SimulatePreconnectedPeripheralAsync_"></a>
<a id="PuppeteerSharp_IBluetoothEmulation_SimulatePreconnectedPeripheralAsync_PuppeteerSharp_PreconnectedPeripheral_"></a>

### SimulatePreconnectedPeripheralAsync(PreconnectedPeripheral)


```csharp
Task SimulatePreconnectedPeripheralAsync(PreconnectedPeripheral preconnectedPeripheral)
```


Simulated preconnected Bluetooth Peripheral.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `preconnectedPeripheral` | [PreconnectedPeripheral](/api/PuppeteerSharp/PreconnectedPeripheral/) | The peripheral to simulate. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) that completes when the peripheral is simulated.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBluetoothEmulation.cs#L29)
