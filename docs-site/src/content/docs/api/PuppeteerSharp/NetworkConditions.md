---
title: "NetworkConditions"
description: "Options to be used in EmulateNetworkConditionsAsync(NetworkConditions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "NetworkConditions"
  hidden: true
---

<a id="PuppeteerSharp_NetworkConditions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class NetworkConditions
```


Options to be used in [EmulateNetworkConditionsAsync(NetworkConditions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EmulateNetworkConditionsAsync_PuppeteerSharp_NetworkConditions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkConditions.cs#L6)

## Fields

<a id="PuppeteerSharp_NetworkConditions_Fast3G"></a>

### Fast3G


```csharp
public const string Fast3G = "Fast 3G"
```


Key to be used with [NetworkConditions](/api/PuppeteerSharp/Puppeteer/#PuppeteerSharp_Puppeteer_NetworkConditions).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkConditions.cs#L16)

<a id="PuppeteerSharp_NetworkConditions_Fast4G"></a>

### Fast4G


```csharp
public const string Fast4G = "Fast 4G"
```


Key to be used with [NetworkConditions](/api/PuppeteerSharp/Puppeteer/#PuppeteerSharp_Puppeteer_NetworkConditions).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkConditions.cs#L26)

<a id="PuppeteerSharp_NetworkConditions_Slow3G"></a>

### Slow3G


```csharp
public const string Slow3G = "Slow 3G"
```


Key to be used with [NetworkConditions](/api/PuppeteerSharp/Puppeteer/#PuppeteerSharp_Puppeteer_NetworkConditions).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkConditions.cs#L11)

<a id="PuppeteerSharp_NetworkConditions_Slow4G"></a>

### Slow4G


```csharp
public const string Slow4G = "Slow 4G"
```


Key to be used with [NetworkConditions](/api/PuppeteerSharp/Puppeteer/#PuppeteerSharp_Puppeteer_NetworkConditions).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkConditions.cs#L21)

## Properties

<a id="PuppeteerSharp_NetworkConditions_Download_"></a>
<a id="PuppeteerSharp_NetworkConditions_Download"></a>

### Download


```csharp
public double? Download { get; set; }
```


Download speed (bytes/s). If not provided, download throttling is disabled and only the offline mode is emulated.

**Value:** [double?](https://learn.microsoft.com/dotnet/api/system.double)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkConditions.cs#L36)

<a id="PuppeteerSharp_NetworkConditions_Latency_"></a>
<a id="PuppeteerSharp_NetworkConditions_Latency"></a>

### Latency


```csharp
public double? Latency { get; set; }
```


Latency (ms). If not provided, no latency is emulated.

**Value:** [double?](https://learn.microsoft.com/dotnet/api/system.double)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkConditions.cs#L46)

<a id="PuppeteerSharp_NetworkConditions_Offline_"></a>
<a id="PuppeteerSharp_NetworkConditions_Offline"></a>

### Offline


```csharp
public bool Offline { get; set; }
```


Emulates the offline mode. Shortcut for [SetOfflineModeAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetOfflineModeAsync_System_Boolean_).

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkConditions.cs#L31)

<a id="PuppeteerSharp_NetworkConditions_Upload_"></a>
<a id="PuppeteerSharp_NetworkConditions_Upload"></a>

### Upload


```csharp
public double? Upload { get; set; }
```


Upload speed (bytes/s). If not provided, upload throttling is disabled and only the offline mode is emulated.

**Value:** [double?](https://learn.microsoft.com/dotnet/api/system.double)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkConditions.cs#L41)
