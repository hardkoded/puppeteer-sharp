---
title: "LaunchOptions"
description: "Options for launching the Chrome/ium browser."
editUrl: false
prev: false
next: false
sidebar:
  label: "LaunchOptions"
  hidden: true
---

<a id="PuppeteerSharp_LaunchOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class LaunchOptions : IBrowserOptions, IConnectionOptions
```


Options for launching the Chrome/ium browser.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IBrowserOptions](/api/PuppeteerSharp/IBrowserOptions/), [IConnectionOptions](/api/PuppeteerSharp/IConnectionOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L13)

## Properties

<a id="PuppeteerSharp_LaunchOptions_AcceptInsecureCerts_"></a>
<a id="PuppeteerSharp_LaunchOptions_AcceptInsecureCerts"></a>

### AcceptInsecureCerts


```csharp
public bool AcceptInsecureCerts { get; set; }
```


Whether to ignore HTTPS errors during navigation. Defaults to false.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L26)

<a id="PuppeteerSharp_LaunchOptions_Allowlist_"></a>
<a id="PuppeteerSharp_LaunchOptions_Allowlist"></a>

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
For any network requests made by the browser (including navigations and
subresources like images or scripts), the request will fail with an error
if the URL does not match any pattern in the allowlist.
&lt;/p&gt;
&lt;p&gt;
Currently only supported for CDP connections.
&lt;/p&gt;
&lt;p&gt;
Cannot be used along with [BlockList](#PuppeteerSharp_LaunchOptions_BlockList).
&lt;/p&gt;

**Example**

Pattern to allow a specific domain: `*://example.com/*`
Pattern to allow all subdomains: `*://*.example.com/*`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L320)

<a id="PuppeteerSharp_LaunchOptions_Args_"></a>
<a id="PuppeteerSharp_LaunchOptions_Args"></a>

### Args


```csharp
public string[] Args { get; set; }
```


Additional arguments to pass to the browser instance. List of Chromium flags can be found &lt;a href="http://peter.sh/experiments/chromium-command-line-switches/"&gt;here&lt;/a&gt;.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L56)

<a id="PuppeteerSharp_LaunchOptions_BlockList_"></a>
<a id="PuppeteerSharp_LaunchOptions_BlockList"></a>

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
For any network requests made by the browser (including navigations and
subresources like images or scripts), the request will fail with an error
if the URL matches a blocked pattern.
&lt;/p&gt;
&lt;p&gt;
Currently only supported for CDP connections.
&lt;/p&gt;
&lt;p&gt;
Cannot be used along with [Allowlist](#PuppeteerSharp_LaunchOptions_Allowlist).
&lt;/p&gt;

**Example**

Pattern to block a specific domain: `*://example.com/*`
Pattern to block all subdomains: `*://*.evil.com/*`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L290)

<a id="PuppeteerSharp_LaunchOptions_Browser_"></a>
<a id="PuppeteerSharp_LaunchOptions_Browser"></a>

### Browser


```csharp
public SupportedBrowser Browser { get; set; }
```


The browser to be used (Chrome, Chromium, Firefox).

**Value:** [SupportedBrowser](/api/PuppeteerSharp/SupportedBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L198)

<a id="PuppeteerSharp_LaunchOptions_Channel_"></a>
<a id="PuppeteerSharp_LaunchOptions_Channel"></a>

### Channel


```csharp
public ChromeReleaseChannel? Channel { get; set; }
```


Chrome Release Channel.

**Value:** `ChromeReleaseChannel?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L21)

<a id="PuppeteerSharp_LaunchOptions_DefaultViewport_"></a>
<a id="PuppeteerSharp_LaunchOptions_DefaultViewport"></a>

### DefaultViewport


```csharp
public ViewPortOptions DefaultViewport { get; set; }
```


Gets or sets the default Viewport.

**Value:** [ViewPortOptions](/api/PuppeteerSharp/ViewPortOptions/) — The default Viewport.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L184)

<a id="PuppeteerSharp_LaunchOptions_Devtools_"></a>
<a id="PuppeteerSharp_LaunchOptions_Devtools"></a>

### Devtools


```csharp
public bool Devtools { get; set; }
```


Whether to auto-open DevTools panel for each tab. If this option is true, the headless option will be set false.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L81)

<a id="PuppeteerSharp_LaunchOptions_DumpIO_"></a>
<a id="PuppeteerSharp_LaunchOptions_DumpIO"></a>

### DumpIO


```csharp
public bool DumpIO { get; set; }
```


Whether to pipe browser process stdout and stderr into process stdout and process stderr. Defaults to false.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L66)

<a id="PuppeteerSharp_LaunchOptions_EnableExtensions_"></a>
<a id="PuppeteerSharp_LaunchOptions_EnableExtensions"></a>

### EnableExtensions


```csharp
public EnableExtensionsOption EnableExtensions { get; set; }
```


If `true`, avoids passing default arguments to the browser that would
prevent extensions from being enabled. Passing a list of strings will
load the provided paths as unpacked extensions.

**Value:** [EnableExtensionsOption](/api/PuppeteerSharp/EnableExtensionsOption/)

&lt;p&gt;In Chrome, setting this to `true` or a `string[]` replaces the
`--disable-extensions` flag with `--enable-unsafe-extension-debugging`.&lt;/p&gt;
&lt;p&gt;Passing a `string[]` is only supported with Firefox (BiDi) at this time.&lt;/p&gt;
&lt;p&gt;In Firefox, there is no known command line flag to disable addons so they are always enabled.&lt;/p&gt;

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L241)

<a id="PuppeteerSharp_LaunchOptions_EnqueueAsyncMessages_"></a>
<a id="PuppeteerSharp_LaunchOptions_EnqueueAsyncMessages"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L210)

<a id="PuppeteerSharp_LaunchOptions_EnqueueTransportMessages_"></a>
<a id="PuppeteerSharp_LaunchOptions_EnqueueTransportMessages"></a>

### EnqueueTransportMessages


```csharp
public bool EnqueueTransportMessages { get; set; }
```


If not [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/) is set this will be use to determine is the default [WebSocketTransport](/api/PuppeteerSharp.Transport/WebSocketTransport/) will enqueue messages.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

It's set to `true` by default because it's the safest way to send commands to Chromium.
Setting this to `false` proved to work in .NET Core, but it tends to fail on .NET Framework.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L193)

<a id="PuppeteerSharp_LaunchOptions_Env_"></a>
<a id="PuppeteerSharp_LaunchOptions_Env"></a>

### Env


```csharp
public IDictionary<string, string> Env { get; }
```


Specify environment variables that will be visible to browser. Defaults to Environment variables.

**Value:** [IDictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.idictionary-2)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L76)

<a id="PuppeteerSharp_LaunchOptions_ExecutablePath_"></a>
<a id="PuppeteerSharp_LaunchOptions_ExecutablePath"></a>

### ExecutablePath


```csharp
public string ExecutablePath { get; set; }
```


Path to a Chromium or Chrome executable to run instead of bundled Chromium. If executablePath is a relative path, then it is resolved relative to current working directory.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L46)

<a id="PuppeteerSharp_LaunchOptions_ExtensionsEnabledInIncognito_"></a>
<a id="PuppeteerSharp_LaunchOptions_ExtensionsEnabledInIncognito"></a>

### ExtensionsEnabledInIncognito


```csharp
public string[] ExtensionsEnabledInIncognito { get; set; }
```


Gets or sets the list of extension paths that will be enabled in Incognito and off-the-record profiles.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L246)

<a id="PuppeteerSharp_LaunchOptions_ExtraPrefsFirefox_"></a>
<a id="PuppeteerSharp_LaunchOptions_ExtraPrefsFirefox"></a>

### ExtraPrefsFirefox


```csharp
public Dictionary<string, object> ExtraPrefsFirefox { get; set; }
```


Additional preferences that can be passed when launching with Firefox. &lt;see fref="https://searchfox.org/mozilla-release/source/modules/libpref/init/all.js"&gt;See&lt;/see&gt;.

**Value:** [Dictionary&lt;string, object&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L223)

<a id="PuppeteerSharp_LaunchOptions_HandleDevToolsAsPage_"></a>
<a id="PuppeteerSharp_LaunchOptions_HandleDevToolsAsPage"></a>

### HandleDevToolsAsPage


```csharp
public bool HandleDevToolsAsPage { get; set; }
```


Whether to handle the DevTools windows as pages in Puppeteer. Supported only in Chrome with CDP.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L251)

<a id="PuppeteerSharp_LaunchOptions_Headers_"></a>
<a id="PuppeteerSharp_LaunchOptions_Headers"></a>

### Headers


```csharp
[Obsolete("Use WsOptions.Headers via LaunchOptions.WsOptions instead. When both are set, WsOptions.Headers wins.")]
public Dictionary<string, string> Headers { get; set; }
```


Headers that should be sent with connection-related HTTP and WebSocket requests.

**Value:** [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2)

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L160)

<a id="PuppeteerSharp_LaunchOptions_Headless_"></a>
<a id="PuppeteerSharp_LaunchOptions_Headless"></a>

### Headless


```csharp
public bool Headless { get; set; }
```


Whether to run browser in headless mode. Defaults to true unless the devtools option is true.
If you need to run using the old headless mode, set [HeadlessMode](#PuppeteerSharp_LaunchOptions_HeadlessMode) this to [Shell](/api/PuppeteerSharp/HeadlessMode/#PuppeteerSharp_HeadlessMode_Shell).

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L32)

<a id="PuppeteerSharp_LaunchOptions_HeadlessMode_"></a>
<a id="PuppeteerSharp_LaunchOptions_HeadlessMode"></a>

### HeadlessMode


```csharp
public HeadlessMode HeadlessMode { get; set; }
```


Whether to run browser in headless mode. Defaults to true unless the devtools option is true.

**Value:** [HeadlessMode](/api/PuppeteerSharp/HeadlessMode/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L41)

<a id="PuppeteerSharp_LaunchOptions_IgnoreDefaultArgs_"></a>
<a id="PuppeteerSharp_LaunchOptions_IgnoreDefaultArgs"></a>

### IgnoreDefaultArgs


```csharp
public bool IgnoreDefaultArgs { get; set; }
```


If `true`, then do not use `GetDefaultArgs(LaunchOptions)`.
Dangerous option; use with care. Defaults to `false`.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L107)

<a id="PuppeteerSharp_LaunchOptions_IgnoredDefaultArgs_"></a>
<a id="PuppeteerSharp_LaunchOptions_IgnoredDefaultArgs"></a>

### IgnoredDefaultArgs


```csharp
public string[] IgnoredDefaultArgs { get; set; }
```


if [IgnoreDefaultArgs](#PuppeteerSharp_LaunchOptions_IgnoreDefaultArgs) is set to `false` this list will be used to filter `GetDefaultArgs(LaunchOptions)`.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L112)

<a id="PuppeteerSharp_LaunchOptions_IssuesEnabled_"></a>
<a id="PuppeteerSharp_LaunchOptions_IssuesEnabled"></a>

### IssuesEnabled


```csharp
public bool IssuesEnabled { get; set; }
```


Experimental setting to disable monitoring issue events by default.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L178)

<a id="PuppeteerSharp_LaunchOptions_LogProcess_"></a>
<a id="PuppeteerSharp_LaunchOptions_LogProcess"></a>

### LogProcess


```csharp
[Obsolete("This property has no effect and will be removed in a future version.")]
public bool LogProcess { get; set; }
```


Logs process counts after launching chrome and after exiting.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

:::caution[Obsolete]
This API is obsolete.
:::

This property is obsolete and has no effect. It will be removed in a future version.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L100)

<a id="PuppeteerSharp_LaunchOptions_NetworkEnabled_"></a>
<a id="PuppeteerSharp_LaunchOptions_NetworkEnabled"></a>

### NetworkEnabled


```csharp
public bool NetworkEnabled { get; set; }
```


Experimental setting to disable monitoring network events by default. When
set to `false`, parts of Puppeteer that depend on network events would not
work such as [IRequest](/api/PuppeteerSharp/IRequest/) and [IResponse](/api/PuppeteerSharp/IResponse/).

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L173)

<a id="PuppeteerSharp_LaunchOptions_Pipe_"></a>
<a id="PuppeteerSharp_LaunchOptions_Pipe"></a>

### Pipe


```csharp
public bool Pipe { get; set; }
```


Connects to the browser over a pipe instead of a WebSocket.
Only supported for Chrome/Chromium with CDP protocol.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L257)

<a id="PuppeteerSharp_LaunchOptions_Protocol_"></a>
<a id="PuppeteerSharp_LaunchOptions_Protocol"></a>

### Protocol


```csharp
public ProtocolType Protocol { get; set; }
```


The protocol to use for browser communication.
Defaults to [Cdp](/api/PuppeteerSharp/ProtocolType/#PuppeteerSharp_ProtocolType_Cdp). Set to [WebdriverBiDi](/api/PuppeteerSharp/ProtocolType/#PuppeteerSharp_ProtocolType_WebdriverBiDi) to use WebDriver BiDi.
Firefox always uses WebDriver BiDi regardless of this setting.

**Value:** [ProtocolType](/api/PuppeteerSharp/ProtocolType/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L264)

<a id="PuppeteerSharp_LaunchOptions_ProtocolTimeout_"></a>
<a id="PuppeteerSharp_LaunchOptions_ProtocolTimeout"></a>

### ProtocolTimeout


```csharp
public int ProtocolTimeout { get; set; }
```


Timeout setting for individual protocol (CDP) calls.
Defaults to 180_000.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L218)

<a id="PuppeteerSharp_LaunchOptions_SlowMo_"></a>
<a id="PuppeteerSharp_LaunchOptions_SlowMo"></a>

### SlowMo


```csharp
public int SlowMo { get; set; }
```


Slows down Puppeteer operations by the specified amount of milliseconds. Useful so that you can see what is going on.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L51)

<a id="PuppeteerSharp_LaunchOptions_TargetFilter_"></a>
<a id="PuppeteerSharp_LaunchOptions_TargetFilter"></a>

### TargetFilter


```csharp
public Func<Target, bool> TargetFilter { get; set; }
```


Callback to decide if Puppeteer should connect to a given target or not.

**Value:** [Func&lt;Target, bool&gt;](https://learn.microsoft.com/dotnet/api/system.func-2)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L215)

<a id="PuppeteerSharp_LaunchOptions_Timeout_"></a>
<a id="PuppeteerSharp_LaunchOptions_Timeout"></a>

### Timeout


```csharp
public int Timeout { get; set; }
```


Maximum time in milliseconds to wait for the browser instance to start. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L61)

<a id="PuppeteerSharp_LaunchOptions_TransportFactory_"></a>
<a id="PuppeteerSharp_LaunchOptions_TransportFactory"></a>

### TransportFactory


```csharp
public TransportFactory TransportFactory { get; set; }
```


Optional factory for [IConnectionTransport](/api/PuppeteerSharp.Transport/IConnectionTransport/) implementations.

**Value:** [TransportFactory](/api/PuppeteerSharp.Transport/TransportFactory/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L166)

<a id="PuppeteerSharp_LaunchOptions_UserDataDir_"></a>
<a id="PuppeteerSharp_LaunchOptions_UserDataDir"></a>

### UserDataDir


```csharp
public string UserDataDir { get; set; }
```


Path to a User Data Directory.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L71)

<a id="PuppeteerSharp_LaunchOptions_WaitForInitialPage_"></a>
<a id="PuppeteerSharp_LaunchOptions_WaitForInitialPage"></a>

### WaitForInitialPage


```csharp
public bool WaitForInitialPage { get; set; }
```


Wait for the initial page before initializing.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L228)

<a id="PuppeteerSharp_LaunchOptions_WebSocketFactory_"></a>
<a id="PuppeteerSharp_LaunchOptions_WebSocketFactory"></a>

### WebSocketFactory


```csharp
public WebSocketFactory WebSocketFactory { get; set; }
```


Optional factory for [WebSocket](https://learn.microsoft.com/dotnet/api/system.net.websockets.websocket) implementations.
If [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/) is set this property will be ignored.

**Value:** [WebSocketFactory](/api/PuppeteerSharp.Transport/WebSocketFactory/)

If you need to run Puppeteer-Sharp on Windows 7, you can use [WebSocketFactory](#PuppeteerSharp_LaunchOptions_WebSocketFactory) to inject &lt;a href="https://www.nuget.org/packages/System.Net.WebSockets.Client.Managed/"&gt;System.Net.WebSockets.Client.Managed&lt;/a&gt;.

&lt;example&gt;

WebSocketFactory = async (uri, socketOptions, cancellationToken) =&gt;
{
    var client = SystemClientWebSocket.CreateClientWebSocket();
    if (client is System.Net.WebSockets.Managed.ClientWebSocket managed)
    {
       managed.Options.KeepAliveInterval = TimeSpan.FromSeconds(0);
        await managed.ConnectAsync(uri, cancellationToken);
    }
    else
    {
        var coreSocket = client as ClientWebSocket;
        coreSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(0);
        await coreSocket.ConnectAsync(uri, cancellationToken);
    }

    return client;
},

&lt;/example&gt;

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L150)

<a id="PuppeteerSharp_LaunchOptions_WsOptions_"></a>
<a id="PuppeteerSharp_LaunchOptions_WsOptions"></a>

### WsOptions


```csharp
public WsOptions WsOptions { get; set; }
```


Options for the WebSocket connection to the browser.

**Value:** [WsOptions](/api/PuppeteerSharp/WsOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchOptions.cs#L155)
