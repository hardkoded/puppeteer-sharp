---
title: "RequestAbortErrorCode"
description: "Abort error codes. used by Int32%7d)."
editUrl: false
prev: false
next: false
sidebar:
  label: "RequestAbortErrorCode"
  hidden: true
---

<a id="PuppeteerSharp_RequestAbortErrorCode"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[JsonConverter(typeof(JsonStringEnumMemberConverter<RequestAbortErrorCode>))]
public enum RequestAbortErrorCode
```


Abort error codes. used by `Int32%7d)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L9)

## Fields

<a id="PuppeteerSharp_RequestAbortErrorCode_Aborted"></a>

### Aborted


```csharp
Aborted = 0
```


An operation was aborted (due to user action).

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L15)

<a id="PuppeteerSharp_RequestAbortErrorCode_AccessDenied"></a>

### AccessDenied


```csharp
AccessDenied = 1
```


Permission to access a resource, other than the network, was denied.

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L20)

<a id="PuppeteerSharp_RequestAbortErrorCode_AddressUnreachable"></a>

### AddressUnreachable


```csharp
AddressUnreachable = 2
```


The IP address is unreachable. This usually means that there is no route to the specified host or network.

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L25)

<a id="PuppeteerSharp_RequestAbortErrorCode_BlockedByClient"></a>

### BlockedByClient


```csharp
BlockedByClient = 3
```


The client chose to block the request.

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L30)

<a id="PuppeteerSharp_RequestAbortErrorCode_BlockedByResponse"></a>

### BlockedByResponse


```csharp
BlockedByResponse = 4
```


The request failed because the response was delivered along with requirements which are not met
('X-Frame-Options' and 'Content-Security-Policy' ancestor checks, for instance).

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L36)

<a id="PuppeteerSharp_RequestAbortErrorCode_ConnectionAborted"></a>

### ConnectionAborted


```csharp
ConnectionAborted = 5
```


A connection timed out as a result of not receiving an ACK for data sent.

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L41)

<a id="PuppeteerSharp_RequestAbortErrorCode_ConnectionClosed"></a>

### ConnectionClosed


```csharp
ConnectionClosed = 6
```


A connection was closed (corresponding to a TCP FIN).

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L46)

<a id="PuppeteerSharp_RequestAbortErrorCode_ConnectionFailed"></a>

### ConnectionFailed


```csharp
ConnectionFailed = 7
```


A connection attempt failed.

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L51)

<a id="PuppeteerSharp_RequestAbortErrorCode_ConnectionRefused"></a>

### ConnectionRefused


```csharp
ConnectionRefused = 8
```


A connection attempt was refused.

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L56)

<a id="PuppeteerSharp_RequestAbortErrorCode_ConnectionReset"></a>

### ConnectionReset


```csharp
ConnectionReset = 9
```


A connection was reset (corresponding to a TCP RST).

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L61)

<a id="PuppeteerSharp_RequestAbortErrorCode_Failed"></a>

### Failed


```csharp
Failed = 13
```


A generic failure occurred.

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L81)

<a id="PuppeteerSharp_RequestAbortErrorCode_InternetDisconnected"></a>

### InternetDisconnected


```csharp
InternetDisconnected = 10
```


The Internet connection has been lost.

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L66)

<a id="PuppeteerSharp_RequestAbortErrorCode_NameNotResolved"></a>

### NameNotResolved


```csharp
NameNotResolved = 11
```


The host name could not be resolved.

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L71)

<a id="PuppeteerSharp_RequestAbortErrorCode_TimedOut"></a>

### TimedOut


```csharp
TimedOut = 12
```


An operation timed out.

**Value:** [RequestAbortErrorCode](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestAbortErrorCode.cs#L76)
