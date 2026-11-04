---
title: "ICDPSession"
description: "The CDPSession instances are used to talk raw Chrome Devtools Protocol: Protocol methods can be called with CommandOptions) method. Protocol events, using th..."
editUrl: false
prev: false
next: false
sidebar:
  label: "ICDPSession"
  hidden: true
---

<a id="PuppeteerSharp_ICDPSession"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface ICDPSession : ICDPConnection
```


The CDPSession instances are used to talk raw Chrome Devtools Protocol:
 * Protocol methods can be called with `CommandOptions)` method.
 * Protocol events, using the [MessageReceived](/api/PuppeteerSharp/ICDPConnection/#PuppeteerSharp_ICDPConnection_MessageReceived) event.

Documentation on DevTools Protocol can be found here: &lt;a href="https://chromedevtools.github.io/devtools-protocol/"&gt;https://chromedevtools.github.io/devtools-protocol/&lt;/a&gt;.


```csharp
var client = await Page.Target.CreateCDPSessionAsync();
await client.SendAsync("Animation.enable");
client.MessageReceived += (sender, e) =>
{
     if (e.MessageID == "Animation.animationCreated")
     {
         Console.WriteLine("Animation created!");
     }
};
JObject response = await client.SendAsync("Animation.getPlaybackRate");
Console.WriteLine("playback rate is " + response.playbackRate);
await client.SendAsync("Animation.setPlaybackRate", new
{
    playbackRate = Convert.ToInt32(response.playbackRate / 2)
});
```

**Derived:** [BidiCdpSession](/api/PuppeteerSharp.Bidi/BidiCdpSession/), [CDPSession](/api/PuppeteerSharp/CDPSession/), [CdpCDPSession](/api/PuppeteerSharp.Cdp/CdpCDPSession/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L34)
