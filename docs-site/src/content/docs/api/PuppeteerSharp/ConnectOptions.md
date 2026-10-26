---
title: "ConnectOptions"
description: "Options for connecting to an existing browser."
editUrl: false
prev: false
next: false
sidebar:
  label: "ConnectOptions"
  hidden: true
---

<a id="PuppeteerSharp_ConnectOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ConnectOptions : IBrowserOptions, IConnectionOptions
```


Options for connecting to an existing browser.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IBrowserOptions](/api/PuppeteerSharp/IBrowserOptions/), [IConnectionOptions](/api/PuppeteerSharp/IConnectionOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L12)

## Properties

<a id="PuppeteerSharp_ConnectOptions_AcceptInsecureCerts_"></a>
<a id="PuppeteerSharp_ConnectOptions_AcceptInsecureCerts"></a>

### AcceptInsecureCerts


```csharp
public bool AcceptInsecureCerts { get; set; }
```


Whether to ignore HTTPS errors during navigation. Defaults to false.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L17)

<a id="PuppeteerSharp_ConnectOptions_Allowlist_"></a>
<a id="PuppeteerSharp_ConnectOptions_Allowlist"></a>

### Allowlist


```csharp
public string[] Allowlist { get; set; }
```


A list of URL patterns to allow. When set, the browser can only access URLs matching these patterns.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

&lt;p&gt;
**Requires Chrome 149+.**
&lt;/p&gt;
&lt;p&gt;
This option allows you to restrict the browser from accessing any URLs
except for those that match the patterns in the allowlist.
It uses the standard &lt;a href="https://urlpattern.spec.whatwg.org/"&gt;URLPattern&lt;/a&gt; API to match URLs.
&lt;/p&gt;
&lt;p&gt;
When connecting to an existing browser, Puppeteer will silently detach from any
already open targets that violate the patterns.
&lt;/p&gt;
&lt;p&gt;
For any network requests made by the browser (including navigations and
subresources like images or scripts), the request will fail with an error
if the URL does not match any pattern in the allowlist.
&lt;/p&gt;
&lt;p&gt;
Currently only supported for CDP connections.
&lt;/p&gt;
&lt;p&gt;
Cannot be used along with [BlockList](#PuppeteerSharp_ConnectOptions_BlockList).
&lt;/p&gt;

**Example**

Pattern to allow a specific domain: `*://example.com/*`
Pattern to allow all subdomains: `*://*.example.com/*`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L183)

<a id="PuppeteerSharp_ConnectOptions_BlockList_"></a>
<a id="PuppeteerSharp_ConnectOptions_BlockList"></a>

### BlockList


```csharp
public string[] BlockList { get; set; }
```


A list of URL patterns to block.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

&lt;p&gt;
This option allows you to restrict the browser from accessing specific
URLs or origins. It uses the standard &lt;a href="https://urlpattern.spec.whatwg.org/"&gt;URLPattern&lt;/a&gt; API to match URLs.
&lt;/p&gt;
&lt;p&gt;
When connecting to an existing browser, Puppeteer will silently detach from any
already open targets that violate the patterns.
&lt;/p&gt;
&lt;p&gt;
For any network requests made by the browser (including navigations and
subresources like images or scripts), the request will fail with an error
if the URL matches a blocked pattern.
&lt;/p&gt;
&lt;p&gt;
Currently only supported for CDP connections.
&lt;/p&gt;
&lt;p&gt;
Cannot be used along with [Allowlist](#PuppeteerSharp_ConnectOptions_Allowlist).
&lt;/p&gt;

**Example**

Pattern to block a specific domain: `*://example.com/*`
Pattern to block all subdomains: `*://*.evil.com/*`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L149)

<a id="PuppeteerSharp_ConnectOptions_BrowserURL_"></a>
<a id="PuppeteerSharp_ConnectOptions_BrowserURL"></a>

### BrowserURL


```csharp
public string BrowserURL { get; set; }
```


A browser url to connect to, in format `http://${host}:${port}`.
Use interchangeably with `browserWSEndpoint` to let Puppeteer fetch it from &lt;a href="https://chromedevtools.github.io/devtools-protocol/#how-do-i-access-the-browser-target"&gt;metadata endpoin&lt;/a&gt;.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L28)

<a id="PuppeteerSharp_ConnectOptions_BrowserWSEndpoint_"></a>
<a id="PuppeteerSharp_ConnectOptions_BrowserWSEndpoint"></a>

### BrowserWSEndpoint


```csharp
public string BrowserWSEndpoint { get; set; }
```


A browser websocket endpoint to connect to.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L22)

<a id="PuppeteerSharp_ConnectOptions_DefaultViewport_"></a>
<a id="PuppeteerSharp_ConnectOptions_DefaultViewport"></a>

### DefaultViewport


```csharp
public ViewPortOptions DefaultViewport { get; set; }
```


Gets or sets the default Viewport.

**Value:** [ViewPortOptions](/api/PuppeteerSharp/ViewPortOptions/) — The default Viewport.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L68)

<a id="PuppeteerSharp_ConnectOptions_EnqueueAsyncMessages_"></a>
<a id="PuppeteerSharp_ConnectOptions_EnqueueAsyncMessages"></a>

### EnqueueAsyncMessages


```csharp
public bool EnqueueAsyncMessages { get; set; }
```


Affects how responses to `CommandOptions)` are returned to the caller. If `true` (default), the
response is delivered to the caller on its own thread; otherwise, the response is delivered the same way [MessageReceived](/api/PuppeteerSharp/CDPSession/#PuppeteerSharp_CDPSession_MessageReceived)
events are raised.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

This should normally be set to `true` to support applications that aren't `async` "all the way up"; i.e., the application
has legacy code that is not async which makes calls into PuppeteerSharp. If you experience issues, or your application is not mixed sync/async use, you
can set this to `false` (default).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L94)

<a id="PuppeteerSharp_ConnectOptions_EnqueueTransportMessages_"></a>
<a id="PuppeteerSharp_ConnectOptions_EnqueueTransportMessages"></a>

### EnqueueTransportMessages


```csharp
public bool EnqueueTransportMessages { get; set; }
```


If not [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/) is set this will be use to determine is the default [WebSocketTransport](/api/PuppeteerSharp.Transport/WebSocketTransport/) will enqueue messages.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

It's set to `true` by default because it's the safest way to send commands to Chromium.
Setting this to `false` proved to work in .NET Core but it tends to fail on .NET Framework.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L82)

<a id="PuppeteerSharp_ConnectOptions_HandleDevToolsAsPage_"></a>
<a id="PuppeteerSharp_ConnectOptions_HandleDevToolsAsPage"></a>

### HandleDevToolsAsPage


```csharp
public bool HandleDevToolsAsPage { get; set; }
```


Whether to handle the DevTools windows as pages in Puppeteer. Supported only in Chrome with CDP.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L112)

<a id="PuppeteerSharp_ConnectOptions_Headers_"></a>
<a id="PuppeteerSharp_ConnectOptions_Headers"></a>

### Headers


```csharp
[Obsolete("Use WsOptions.Headers via ConnectOptions.WsOptions instead. When both are set, WsOptions.Headers wins.")]
public Dictionary<string, string> Headers { get; set; }
```


Headers that should be sent with connection-related HTTP and WebSocket requests.

**Value:** [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2)

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L49)

<a id="PuppeteerSharp_ConnectOptions_InitAction_"></a>
<a id="PuppeteerSharp_ConnectOptions_InitAction"></a>

### InitAction


```csharp
public Action<IBrowser> InitAction { get; set; }
```


Optional callback to initialize properties as soon as the [IBrowser](/api/PuppeteerSharp/IBrowser/) instance is created, i.e., set up event handlers.

**Value:** [Action&lt;IBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.action-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L107)

<a id="PuppeteerSharp_ConnectOptions_IssuesEnabled_"></a>
<a id="PuppeteerSharp_ConnectOptions_IssuesEnabled"></a>

### IssuesEnabled


```csharp
public bool IssuesEnabled { get; set; }
```


Experimental setting to disable monitoring issue events by default.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L62)

<a id="PuppeteerSharp_ConnectOptions_NetworkEnabled_"></a>
<a id="PuppeteerSharp_ConnectOptions_NetworkEnabled"></a>

### NetworkEnabled


```csharp
public bool NetworkEnabled { get; set; }
```


Experimental setting to disable monitoring network events by default. When
set to `false`, parts of Puppeteer that depend on network events would not
work such as [IRequest](/api/PuppeteerSharp/IRequest/) and [IResponse](/api/PuppeteerSharp/IResponse/).

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L57)

<a id="PuppeteerSharp_ConnectOptions_Protocol_"></a>
<a id="PuppeteerSharp_ConnectOptions_Protocol"></a>

### Protocol


```csharp
public ProtocolType Protocol { get; set; }
```


The protocol to use for browser communication.
Defaults to [Cdp](/api/PuppeteerSharp/ProtocolType/#PuppeteerSharp_ProtocolType_Cdp). Set to [WebdriverBiDi](/api/PuppeteerSharp/ProtocolType/#PuppeteerSharp_ProtocolType_WebdriverBiDi) to use WebDriver BiDi.
Firefox always uses WebDriver BiDi regardless of this setting.

**Value:** [ProtocolType](/api/PuppeteerSharp/ProtocolType/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L119)

<a id="PuppeteerSharp_ConnectOptions_ProtocolTimeout_"></a>
<a id="PuppeteerSharp_ConnectOptions_ProtocolTimeout"></a>

### ProtocolTimeout


```csharp
public int ProtocolTimeout { get; set; }
```


Timeout setting for individual protocol (CDP) calls.
Defaults to 180_000.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L102)

<a id="PuppeteerSharp_ConnectOptions_SlowMo_"></a>
<a id="PuppeteerSharp_ConnectOptions_SlowMo"></a>

### SlowMo


```csharp
public int SlowMo { get; set; }
```


Slows down Puppeteer operations by the specified amount of milliseconds. Useful so that you can see what is going on.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L33)

<a id="PuppeteerSharp_ConnectOptions_TargetFilter_"></a>
<a id="PuppeteerSharp_ConnectOptions_TargetFilter"></a>

### TargetFilter


```csharp
public Func<Target, bool> TargetFilter { get; set; }
```


Callback to decide if Puppeteer should connect to a given target or not.

**Value:** [Func&lt;Target, bool&gt;](https://learn.microsoft.com/dotnet/api/system.func-2)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L99)

<a id="PuppeteerSharp_ConnectOptions_TransportFactory_"></a>
<a id="PuppeteerSharp_ConnectOptions_TransportFactory"></a>

### TransportFactory


```csharp
public TransportFactory TransportFactory { get; set; }
```


Optional factory for [IConnectionTransport](/api/PuppeteerSharp.Transport/IConnectionTransport/) implementations.

**Value:** [TransportFactory](/api/PuppeteerSharp.Transport/TransportFactory/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L73)

<a id="PuppeteerSharp_ConnectOptions_WebSocketFactory_"></a>
<a id="PuppeteerSharp_ConnectOptions_WebSocketFactory"></a>

### WebSocketFactory


```csharp
public WebSocketFactory WebSocketFactory { get; set; }
```


Optional factory for [WebSocket](https://learn.microsoft.com/dotnet/api/system.net.websockets.websocket) implementations.
If [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/) is set this property will be ignored.

**Value:** [WebSocketFactory](/api/PuppeteerSharp.Transport/WebSocketFactory/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L39)

<a id="PuppeteerSharp_ConnectOptions_WsOptions_"></a>
<a id="PuppeteerSharp_ConnectOptions_WsOptions"></a>

### WsOptions


```csharp
public WsOptions WsOptions { get; set; }
```


Options for the WebSocket connection to the browser.

**Value:** [WsOptions](/api/PuppeteerSharp/WsOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConnectOptions.cs#L44)
