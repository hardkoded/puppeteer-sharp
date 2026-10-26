---
title: "PuppeteerSharp.Cdp"
description: "Chrome DevTools Protocol browser and page implementation."
editUrl: false
sidebar:
  label: "PuppeteerSharp.Cdp"
---

[API reference](/api/)

Chrome DevTools Protocol browser and page implementation.

## Classes

| Type | Summary |
| --- | --- |
| [CdpBrowser](/api/PuppeteerSharp.Cdp/CdpBrowser/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [CdpBrowserContext](/api/PuppeteerSharp.Cdp/CdpBrowserContext/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [CdpCDPSession](/api/PuppeteerSharp.Cdp/CdpCDPSession/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [CdpDevToolsTarget](/api/PuppeteerSharp.Cdp/CdpDevToolsTarget/) | DevTools target. |
| [CdpDialog](/api/PuppeteerSharp.Cdp/CdpDialog/) | Dialog objects are dispatched by page via the 'dialog' event. |
| [CdpElementHandle](/api/PuppeteerSharp.Cdp/CdpElementHandle/) | Inherits from IJSHandle. It represents an in-page DOM element. ElementHandles can be created by QuerySelectorAsync(string) or QuerySelectorAllAsync(string). |
| [CdpFrame](/api/PuppeteerSharp.Cdp/CdpFrame/) | Provides methods to interact with a single page frame in Chromium. One IPage instance might have multiple IFrame instances. At every point of time, page exposes its current frame tree via the MainFrame and ChildFrames properties. IFrame object's lifecycle is controlled by three events, dispatched on the page object - FrameAttached - fires when the frame gets attached to the page. A Frame can be attached to the page only once - FrameNavigated - fired when the frame commits navigation to a different URL - FrameDetached - fired when the frame gets detached from the page. A Frame can be detached from the page only once. |
| [CdpHttpRequest](/api/PuppeteerSharp.Cdp/CdpHttpRequest/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [CdpHttpResponse](/api/PuppeteerSharp.Cdp/CdpHttpResponse/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [CdpJSHandle](/api/PuppeteerSharp.Cdp/CdpJSHandle/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [CdpKeyboard](/api/PuppeteerSharp.Cdp/CdpKeyboard/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [CdpMouse](/api/PuppeteerSharp.Cdp/CdpMouse/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [CdpOtherTarget](/api/PuppeteerSharp.Cdp/CdpOtherTarget/) | Other target. |
| [CdpPage](/api/PuppeteerSharp.Cdp/CdpPage/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [CdpPageTarget](/api/PuppeteerSharp.Cdp/CdpPageTarget/) | Page target. |
| [CdpTarget](/api/PuppeteerSharp.Cdp/CdpTarget/) | Target. |
| [CdpTouchscreen](/api/PuppeteerSharp.Cdp/CdpTouchscreen/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [CdpWebMcp](/api/PuppeteerSharp.Cdp/CdpWebMcp/) | Experimental WebMCP API. Requires Chrome 149+ with --enable-features=WebMCP flag. |
| [CdpWebWorker](/api/PuppeteerSharp.Cdp/CdpWebWorker/) | The Worker class represents a WebWorker (<a href="https://developer.mozilla.org/en-US/docs/Web/API/WebWorkersAPI">https://developer.mozilla.org/en-US/docs/Web/API/WebWorkersAPI</a>). The events WorkerCreated and WorkerDestroyed are emitted on the page object to signal the worker lifecycle. |
| [CdpWorkerTarget](/api/PuppeteerSharp.Cdp/CdpWorkerTarget/) | Worker target. |
| [Connection](/api/PuppeteerSharp.Cdp/Connection/) | A connection handles the communication with a Chromium browser. |
| [WebMcpAnnotation](/api/PuppeteerSharp.Cdp/WebMcpAnnotation/) | Tool annotations for a WebMCP tool. |
| [WebMcpTool](/api/PuppeteerSharp.Cdp/WebMcpTool/) | Represents a registered WebMCP tool available on the page. |
| [WebMcpToolCall](/api/PuppeteerSharp.Cdp/WebMcpToolCall/) | Represents a pending or completed WebMCP tool invocation call. |
| [WebMcpToolCallResult](/api/PuppeteerSharp.Cdp/WebMcpToolCallResult/) | Result of a WebMCP tool invocation. |
| [WebMcpToolExecuteOptions](/api/PuppeteerSharp.Cdp/WebMcpToolExecuteOptions/) | Options for WebMcpToolExecuteOptions). |
| [WebMcpToolsAddedEventArgs](/api/PuppeteerSharp.Cdp/WebMcpToolsAddedEventArgs/) | Event args for when WebMCP tools are added. |
| [WebMcpToolsRemovedEventArgs](/api/PuppeteerSharp.Cdp/WebMcpToolsRemovedEventArgs/) | Event args for when WebMCP tools are removed. |

## Interfaces

| Type | Summary |
| --- | --- |
| [ICdpHandle](/api/PuppeteerSharp.Cdp/ICdpHandle/) | CDP Specific handle info. |

## Enums

| Type | Summary |
| --- | --- |
| [WebMcpInvocationStatus](/api/PuppeteerSharp.Cdp/WebMcpInvocationStatus/) | Represents the status of a WebMCP tool invocation. |
