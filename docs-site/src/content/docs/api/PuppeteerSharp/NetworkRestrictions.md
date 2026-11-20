---
title: "NetworkRestrictions"
description: "Network restrictions that can be applied to a browser at runtime using RestrictNetworkAsync(NetworkRestrictions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "NetworkRestrictions"
  hidden: true
---

<a id="PuppeteerSharp_NetworkRestrictions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class NetworkRestrictions
```


Network restrictions that can be applied to a browser at runtime using [RestrictNetworkAsync(NetworkRestrictions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_RestrictNetworkAsync_PuppeteerSharp_NetworkRestrictions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkRestrictions.cs#L6)

## Properties

<a id="PuppeteerSharp_NetworkRestrictions_Allowlist_"></a>
<a id="PuppeteerSharp_NetworkRestrictions_Allowlist"></a>

### Allowlist


```csharp
public string[] Allowlist { get; set; }
```


A list of &lt;a href="https://urlpattern.spec.whatwg.org/"&gt;URLPattern&lt;/a&gt; strings to allow.
Mutually exclusive with [BlockList](#PuppeteerSharp_NetworkRestrictions_BlockList).
Passing an empty array or `null` means no restriction.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

The allowlist does not apply to WebTransport and to TURN over TCP at
this time. It blocks all WebRTC traffic over UDP.
See [Allowlist](/api/PuppeteerSharp/ConnectOptions/#PuppeteerSharp_ConnectOptions_Allowlist).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkRestrictions.cs#L29)

<a id="PuppeteerSharp_NetworkRestrictions_BlockList_"></a>
<a id="PuppeteerSharp_NetworkRestrictions_BlockList"></a>

### BlockList


```csharp
public string[] BlockList { get; set; }
```


A list of &lt;a href="https://urlpattern.spec.whatwg.org/"&gt;URLPattern&lt;/a&gt; strings to block.
Mutually exclusive with [Allowlist](#PuppeteerSharp_NetworkRestrictions_Allowlist).
Passing an empty array or `null` means no restriction.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

The blocklist does not apply to WebTransport and WebRTC at this time.
See [BlockList](/api/PuppeteerSharp/ConnectOptions/#PuppeteerSharp_ConnectOptions_BlockList).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NetworkRestrictions.cs#L17)
