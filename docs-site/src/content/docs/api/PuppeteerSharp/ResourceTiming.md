---
title: "ResourceTiming"
description: "Resource timing information as returned by the DevTools Network domain."
editUrl: false
prev: false
next: false
sidebar:
  label: "ResourceTiming"
  hidden: true
---

<a id="PuppeteerSharp_ResourceTiming"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ResourceTiming
```


Resource timing information as returned by the DevTools Network domain.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L28)

## Properties

<a id="PuppeteerSharp_ResourceTiming_ConnectEnd_"></a>
<a id="PuppeteerSharp_ResourceTiming_ConnectEnd"></a>

### ConnectEnd


```csharp
public decimal ConnectEnd { get; set; }
```


Connected to the remote host.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L63)

<a id="PuppeteerSharp_ResourceTiming_ConnectStart_"></a>
<a id="PuppeteerSharp_ResourceTiming_ConnectStart"></a>

### ConnectStart


```csharp
public decimal ConnectStart { get; set; }
```


Started connecting to the remote host.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L58)

<a id="PuppeteerSharp_ResourceTiming_DnsEnd_"></a>
<a id="PuppeteerSharp_ResourceTiming_DnsEnd"></a>

### DnsEnd


```csharp
public decimal DnsEnd { get; set; }
```


Finished DNS address resolve.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L53)

<a id="PuppeteerSharp_ResourceTiming_DnsStart_"></a>
<a id="PuppeteerSharp_ResourceTiming_DnsStart"></a>

### DnsStart


```csharp
public decimal DnsStart { get; set; }
```


Started DNS address resolve.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L48)

<a id="PuppeteerSharp_ResourceTiming_ProxyEnd_"></a>
<a id="PuppeteerSharp_ResourceTiming_ProxyEnd"></a>

### ProxyEnd


```csharp
public decimal ProxyEnd { get; set; }
```


Finished resolving proxy.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L43)

<a id="PuppeteerSharp_ResourceTiming_ProxyStart_"></a>
<a id="PuppeteerSharp_ResourceTiming_ProxyStart"></a>

### ProxyStart


```csharp
public decimal ProxyStart { get; set; }
```


Started resolving proxy.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L38)

<a id="PuppeteerSharp_ResourceTiming_PushEnd_"></a>
<a id="PuppeteerSharp_ResourceTiming_PushEnd"></a>

### PushEnd


```csharp
public decimal PushEnd { get; set; }
```


Time the server finished pushing request.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L113)

<a id="PuppeteerSharp_ResourceTiming_PushStart_"></a>
<a id="PuppeteerSharp_ResourceTiming_PushStart"></a>

### PushStart


```csharp
public decimal PushStart { get; set; }
```


Time the server started pushing request.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L108)

<a id="PuppeteerSharp_ResourceTiming_ReceiveHeadersEnd_"></a>
<a id="PuppeteerSharp_ResourceTiming_ReceiveHeadersEnd"></a>

### ReceiveHeadersEnd


```csharp
public decimal ReceiveHeadersEnd { get; set; }
```


Finished receiving response headers.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L123)

<a id="PuppeteerSharp_ResourceTiming_ReceiveHeadersStart_"></a>
<a id="PuppeteerSharp_ResourceTiming_ReceiveHeadersStart"></a>

### ReceiveHeadersStart


```csharp
public decimal ReceiveHeadersStart { get; set; }
```


Started receiving response headers.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L118)

<a id="PuppeteerSharp_ResourceTiming_RequestTime_"></a>
<a id="PuppeteerSharp_ResourceTiming_RequestTime"></a>

### RequestTime


```csharp
public decimal RequestTime { get; set; }
```


Timing's requestTime is a baseline in seconds, while the other numbers are ticks in milliseconds relatively to this requestTime.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L33)

<a id="PuppeteerSharp_ResourceTiming_SendEnd_"></a>
<a id="PuppeteerSharp_ResourceTiming_SendEnd"></a>

### SendEnd


```csharp
public decimal SendEnd { get; set; }
```


Finished sending request.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L103)

<a id="PuppeteerSharp_ResourceTiming_SendStart_"></a>
<a id="PuppeteerSharp_ResourceTiming_SendStart"></a>

### SendStart


```csharp
public decimal SendStart { get; set; }
```


Started sending request.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L98)

<a id="PuppeteerSharp_ResourceTiming_SslEnd_"></a>
<a id="PuppeteerSharp_ResourceTiming_SslEnd"></a>

### SslEnd


```csharp
public decimal SslEnd { get; set; }
```


Finished SSL handshake.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L73)

<a id="PuppeteerSharp_ResourceTiming_SslStart_"></a>
<a id="PuppeteerSharp_ResourceTiming_SslStart"></a>

### SslStart


```csharp
public decimal SslStart { get; set; }
```


Started SSL handshake.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L68)

<a id="PuppeteerSharp_ResourceTiming_WorkerFetchStart_"></a>
<a id="PuppeteerSharp_ResourceTiming_WorkerFetchStart"></a>

### WorkerFetchStart


```csharp
public decimal WorkerFetchStart { get; set; }
```


Started fetch event.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L88)

<a id="PuppeteerSharp_ResourceTiming_WorkerReady_"></a>
<a id="PuppeteerSharp_ResourceTiming_WorkerReady"></a>

### WorkerReady


```csharp
public decimal WorkerReady { get; set; }
```


Finished Starting ServiceWorker.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L83)

<a id="PuppeteerSharp_ResourceTiming_WorkerRespondWithSettled_"></a>
<a id="PuppeteerSharp_ResourceTiming_WorkerRespondWithSettled"></a>

### WorkerRespondWithSettled


```csharp
public decimal WorkerRespondWithSettled { get; set; }
```


Settled fetch event respondWith promise.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L93)

<a id="PuppeteerSharp_ResourceTiming_WorkerStart_"></a>
<a id="PuppeteerSharp_ResourceTiming_WorkerStart"></a>

### WorkerStart


```csharp
public decimal WorkerStart { get; set; }
```


Started running ServiceWorker.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResourceTiming.cs#L78)
