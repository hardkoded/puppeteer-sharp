---
title: "PuppeteerSharp"
description: "Browser, page, frame, element, network, and launch APIs."
editUrl: false
sidebar:
  label: "PuppeteerSharp"
---

[API reference](/api/)

Browser, page, frame, element, network, and launch APIs.

## Classes

| Type | Summary |
| --- | --- |
| [AddScreenParams](/api/PuppeteerSharp/AddScreenParams/) | Parameters for adding a new screen. |
| [AddTagOptions](/api/PuppeteerSharp/AddTagOptions/) | Options used by AddScriptTagAsync(AddTagOptions) & AddStyleTagAsync(AddTagOptions). |
| [AutofillAddressData](/api/PuppeteerSharp/AutofillAddressData/) | Address data for autofilling. See https://chromedevtools.github.io/devtools-protocol/tot/Autofill/#type-Address. |
| [AutofillAddressField](/api/PuppeteerSharp/AutofillAddressField/) | Supported autofill address field names. See https://source.chromium.org/chromium/chromium/src/+/main:components/autofill/core/browser/fieldtypes.cc for the full list of supported fields. |
| [AutofillAddressFieldEntry](/api/PuppeteerSharp/AutofillAddressFieldEntry/) | Represents a single autofill address field name/value pair. |
| [AutofillData](/api/PuppeteerSharp/AutofillData/) | Data for autofilling form fields. Provide either CreditCard or Address, but not both. |
| [BluetoothManufacturerData](/api/PuppeteerSharp/BluetoothManufacturerData/) | Represents the simulated bluetooth peripheral's manufacturer data. |
| [BoundingBox](/api/PuppeteerSharp/BoundingBox/) | Bounding box data returned by BoundingBoxAsync(). |
| [BoxModel](/api/PuppeteerSharp/BoxModel/) | Represents boxes of the element. |
| [BoxModelPoint](/api/PuppeteerSharp/BoxModelPoint/) | Represents a point. |
| [Browser](/api/PuppeteerSharp/Browser/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [BrowserContext](/api/PuppeteerSharp/BrowserContext/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [BrowserContextOptions](/api/PuppeteerSharp/BrowserContextOptions/) | BrowserContext options. |
| [BrowserFetcher](/api/PuppeteerSharp/BrowserFetcher/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [BrowserFetcherOptions](/api/PuppeteerSharp/BrowserFetcherOptions/) | Browser fetcher options used to construct a BrowserFetcher. |
| [CallFrame](/api/PuppeteerSharp/CallFrame/) | Represents a call frame in a stack trace. |
| [CDPSession](/api/PuppeteerSharp/CDPSession/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [ChromeLauncher](/api/PuppeteerSharp/ChromeLauncher/) | Represents a Chromium process and any associated temporary user data directory that have created by Puppeteer and therefore must be cleaned up when no longer needed. |
| [CommandOptions](/api/PuppeteerSharp/CommandOptions/) | Command options. See CommandOptions). |
| [ConnectOptions](/api/PuppeteerSharp/ConnectOptions/) | Options for connecting to an existing browser. |
| [ConsoleEventArgs](/api/PuppeteerSharp/ConsoleEventArgs/) | Console data. |
| [ConsoleMessage](/api/PuppeteerSharp/ConsoleMessage/) | ConsoleMessage is part of ConsoleEventArgs used by Console. |
| [ConsoleMessageLocation](/api/PuppeteerSharp/ConsoleMessageLocation/) | Console message location. |
| [CookieData](/api/PuppeteerSharp/CookieData/) | Cookie parameter object used to set cookies in the browser-level cookies API. |
| [CookieParam](/api/PuppeteerSharp/CookieParam/) | Cookie data. |
| [CookiePartitionKey](/api/PuppeteerSharp/CookiePartitionKey/) | Cookie partition key. |
| [CreatePageOptions](/api/PuppeteerSharp/CreatePageOptions/) | Options for creating a new page. |
| [Credentials](/api/PuppeteerSharp/Credentials/) | Options to be used in AuthenticateAsync(Credentials). |
| [CreditCardData](/api/PuppeteerSharp/CreditCardData/) | Credit card data for autofilling. See https://chromedevtools.github.io/devtools-protocol/tot/Autofill/#type-CreditCard. |
| [CustomQueryHandler](/api/PuppeteerSharp/CustomQueryHandler/) | Contains two functions queryOne and queryAll to be used as custom query handlers The functions queryOne and queryAll are executed in the page context. |
| [DebugInfo](/api/PuppeteerSharp/DebugInfo/) | Debug information about the browser. Currently includes pending protocol calls. In the future, more info might be added. |
| [DeleteCookiesRequest](/api/PuppeteerSharp/DeleteCookiesRequest/) | Request to delete cookies matching specific filters. |
| [DeviceRequestPrompt](/api/PuppeteerSharp/DeviceRequestPrompt/) | Device request prompts let you respond to the page requesting for a device through an API like WebBluetooth. |
| [DeviceRequestPromptDevice](/api/PuppeteerSharp/DeviceRequestPromptDevice/) | Device in a request prompt. |
| [DeviceRequestPromptManager](/api/PuppeteerSharp/DeviceRequestPromptManager/) | Prompt manager. |
| [Dialog](/api/PuppeteerSharp/Dialog/) | Dialog objects are dispatched by page via the 'dialog' event. |
| [DialogEventArgs](/api/PuppeteerSharp/DialogEventArgs/) | Dialog arguments. |
| [DownloadBehavior](/api/PuppeteerSharp/DownloadBehavior/) | Behavior definition for when downloading a file. |
| [DragData](/api/PuppeteerSharp/DragData/) | Data returned by the drag methods. |
| [DragDataItem](/api/PuppeteerSharp/DragDataItem/) | Item returned by drag methods. |
| [ElementHandle](/api/PuppeteerSharp/ElementHandle/) | Inherits from IJSHandle. It represents an in-page DOM element. ElementHandles can be created by QuerySelectorAsync(string) or QuerySelectorAllAsync(string). |
| [ElementScreenshotOptions](/api/PuppeteerSharp/ElementScreenshotOptions/) | Options to be used in ElementScreenshotOptions), ScreenshotStreamAsync(ElementScreenshotOptions) and ScreenshotDataAsync(ElementScreenshotOptions). |
| [EmulateIdleOverrides](/api/PuppeteerSharp/EmulateIdleOverrides/) | Options for EmulateIdleStateAsync(EmulateIdleOverrides). |
| [EnableExtensionsOption](/api/PuppeteerSharp/EnableExtensionsOption/) | Represents the value for the EnableExtensions launch option. Can be implicitly converted from a bool or a string array. |
| [ErrorEventArgs](/api/PuppeteerSharp/ErrorEventArgs/) | Error arguments. |
| [EvaluationFailedException](/api/PuppeteerSharp/EvaluationFailedException/) | Exception thrown by Object). |
| [ExecutionContext](/api/PuppeteerSharp/ExecutionContext/) | The class represents a context for JavaScript execution. Examples of JavaScript contexts are: Each Frame has a separate IExecutionContext All kind of web workers have their own contexts. |
| [Extension](/api/PuppeteerSharp/Extension/) | Represents an Extension instance installed in the browser. |
| [ExtensionInstallOptions](/api/PuppeteerSharp/ExtensionInstallOptions/) | Options for ExtensionInstallOptions). |
| [FileChooser](/api/PuppeteerSharp/FileChooser/) | FileChooser objects are returned via the WaitForFileChooserAsync(WaitForOptions) method. File choosers let you react to the page requesting for a file. |
| [FirefoxLauncher](/api/PuppeteerSharp/FirefoxLauncher/) | Represents a Firefox process and any associated temporary user data directory that have created by Puppeteer and therefore must be cleaned up when no longer needed. |
| [Frame](/api/PuppeteerSharp/Frame/) | Provides methods to interact with a single page frame in Chromium. One IPage instance might have multiple IFrame instances. At every point of time, page exposes its current frame tree via the MainFrame and ChildFrames properties. IFrame object's lifecycle is controlled by three events, dispatched on the page object - FrameAttached - fires when the frame gets attached to the page. A Frame can be attached to the page only once - FrameNavigated - fired when the frame commits navigation to a different URL - FrameDetached - fired when the frame gets detached from the page. A Frame can be detached from the page only once. |
| [FrameEventArgs](/api/PuppeteerSharp/FrameEventArgs/) | FrameAttached, FrameDetached and FrameNavigated arguments. |
| [FrameNavigatedEventArgs](/api/PuppeteerSharp/FrameNavigatedEventArgs/) | FrameNavigated arguments. |
| [GeolocationOption](/api/PuppeteerSharp/GeolocationOption/) | Geolocation option. |
| [GetContentOptions](/api/PuppeteerSharp/GetContentOptions/) | Options for GetContentAsync(GetContentOptions) and GetContentAsync(GetContentOptions). |
| [GetPWAStateOptions](/api/PuppeteerSharp/GetPWAStateOptions/) | Options for GetPWAStateAsync(GetPWAStateOptions). |
| [HeapSnapshotOptions](/api/PuppeteerSharp/HeapSnapshotOptions/) | Options for CaptureHeapSnapshotAsync(HeapSnapshotOptions). |
| [Initiator](/api/PuppeteerSharp/Initiator/) | Information about the request initiator. |
| [InitiatorStackTrace](/api/PuppeteerSharp/InitiatorStackTrace/) | Stack trace information for the Initiator. |
| [InstallPWAOptions](/api/PuppeteerSharp/InstallPWAOptions/) | Options for InstallPWAAsync(InstallPWAOptions). |
| [InterceptResolutionState](/api/PuppeteerSharp/InterceptResolutionState/) | Represents the current state of request interception resolution. |
| [Issue](/api/PuppeteerSharp/Issue/) | Represents a DevTools issue. |
| [IssueEventArgs](/api/PuppeteerSharp/IssueEventArgs/) | Issue event arguments. |
| [JSHandle](/api/PuppeteerSharp/JSHandle/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [Launcher](/api/PuppeteerSharp/Launcher/) | Launcher controls the creation of processes or the connection remote ones. |
| [LauncherBase](/api/PuppeteerSharp/LauncherBase/) | Represents a Base process and any associated temporary user data directory that have created by Puppeteer and therefore must be cleaned up when no longer needed. |
| [LaunchOptions](/api/PuppeteerSharp/LaunchOptions/) | Options for launching the Chrome/ium browser. |
| [LaunchPWAOptions](/api/PuppeteerSharp/LaunchPWAOptions/) | Options for LaunchPWAAsync(LaunchPWAOptions). |
| [MediaFeatureValue](/api/PuppeteerSharp/MediaFeatureValue/) | Media Feature. MediaFeatureValue%7d). |
| [MessageEventArgs](/api/PuppeteerSharp/MessageEventArgs/) | MessageReceived arguments. |
| [MessageException](/api/PuppeteerSharp/MessageException/) | Exception thrown by. CommandOptions) |
| [MetricEventArgs](/api/PuppeteerSharp/MetricEventArgs/) | Metrics arguments. |
| [NavigationException](/api/PuppeteerSharp/NavigationException/) | Exception thrown when a IPage fails to navigate an URL. |
| [NavigationOptions](/api/PuppeteerSharp/NavigationOptions/) | Navigation options used by WaitForNavigationAsync(NavigationOptions) and NavigationOptions). |
| [NetworkConditions](/api/PuppeteerSharp/NetworkConditions/) | Options to be used in EmulateNetworkConditionsAsync(NetworkConditions). |
| [NetworkRestrictions](/api/PuppeteerSharp/NetworkRestrictions/) | Network restrictions that can be applied to a browser at runtime using RestrictNetworkAsync(NetworkRestrictions). |
| [NewDocumentScriptEvaluation](/api/PuppeteerSharp/NewDocumentScriptEvaluation/) | New document information. |
| [Page](/api/PuppeteerSharp/Page/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [PageCloseOptions](/api/PuppeteerSharp/PageCloseOptions/) | Page close options. |
| [PageErrorEventArgs](/api/PuppeteerSharp/PageErrorEventArgs/) | Page error event arguments. |
| [Payload](/api/PuppeteerSharp/Payload/) | Payload information. |
| [PdfOptions](/api/PuppeteerSharp/PdfOptions/) | Options to be used in PdfOptions), PdfStreamAsync(PdfOptions) and PdfDataAsync(PdfOptions). |
| [PermissionDescriptor](/api/PuppeteerSharp/PermissionDescriptor/) | Describes a permission to set via PermissionEntry%5b%5d). |
| [PermissionEntry](/api/PuppeteerSharp/PermissionEntry/) | Represents a permission entry to set via PermissionEntry%5b%5d). |
| [PopupEventArgs](/api/PuppeteerSharp/PopupEventArgs/) | Popup event arguments. Popup. |
| [PreconnectedPeripheral](/api/PuppeteerSharp/PreconnectedPeripheral/) | A bluetooth peripheral to be simulated. |
| [PredefinedNetworkConditions](/api/PuppeteerSharp/PredefinedNetworkConditions/) | Predefined network conditions. |
| [ProcessException](/api/PuppeteerSharp/ProcessException/) | process exception thrown by Launcher. |
| [Puppeteer](/api/PuppeteerSharp/Puppeteer/) | Provides a method to launch a Chromium instance. |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | Base exception used to identify any exception thrown by PuppeteerSharp. |
| [PuppeteerHandleExtensions](/api/PuppeteerSharp/PuppeteerHandleExtensions/) | IJSHandle and IElementHandle Extensions. |
| [PWAFileHandler](/api/PuppeteerSharp/PWAFileHandler/) | A file handler registered by a Progressive Web App (PWA) with the OS. |
| [PWAFileHandlerAccept](/api/PuppeteerSharp/PWAFileHandlerAccept/) | A media type and its associated file extensions accepted by a PWAFileHandler. |
| [PWAState](/api/PuppeteerSharp/PWAState/) | The OS-integration state of an installed web app, returned by GetPWAStateAsync(GetPWAStateOptions). |
| [QueryOptions](/api/PuppeteerSharp/QueryOptions/) | Options for query operations like QueryOptions). |
| [Realm](/api/PuppeteerSharp/Realm/) | Represents an execution context (realm) within a frame or worker. |
| [RecordOptions](/api/PuppeteerSharp/RecordOptions/) | Options for RecordAsync(RecordOptions). |
| [ReloadOptions](/api/PuppeteerSharp/ReloadOptions/) | Options for ReloadAsync(ReloadOptions). |
| [RemoteAddress](/api/PuppeteerSharp/RemoteAddress/) | Remote server address. |
| [Request&lt;TResponse&gt;](/api/PuppeteerSharp/Request-1/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [RequestEventArgs](/api/PuppeteerSharp/RequestEventArgs/) | Arguments used by IPage events. |
| [ResourceTiming](/api/PuppeteerSharp/ResourceTiming/) | Resource timing information as returned by the DevTools Network domain. |
| [Response&lt;TRequest&gt;](/api/PuppeteerSharp/Response-1/) | Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy. |
| [ResponseCreatedEventArgs](/api/PuppeteerSharp/ResponseCreatedEventArgs/) | Response arguments. |
| [ResponseData](/api/PuppeteerSharp/ResponseData/) | Response that will fulfill a request. |
| [ScreencastOptions](/api/PuppeteerSharp/ScreencastOptions/) | Options for ScreencastAsync(ScreencastOptions). |
| [ScreenInfo](/api/PuppeteerSharp/ScreenInfo/) | Contains information about a screen. |
| [ScreenOrientationInfo](/api/PuppeteerSharp/ScreenOrientationInfo/) | Represents screen orientation information. |
| [ScreenRecorder](/api/PuppeteerSharp/ScreenRecorder/) | Represents a screen recording session. |
| [ScreenRecording](/api/PuppeteerSharp/ScreenRecording/) | Represents a video stream-based screen recording session. |
| [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/) | Options to be used in ScreenshotOptions), ScreenshotStreamAsync(ScreenshotOptions) and ScreenshotDataAsync(ScreenshotOptions). |
| [SecurityDetails](/api/PuppeteerSharp/SecurityDetails/) | Represents the security details when response was received over the secure connection. |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | Exception thrown when an element selector returns null. |
| [SessionEventArgs](/api/PuppeteerSharp/SessionEventArgs/) | Session event arguments. |
| [SetContentOptions](/api/PuppeteerSharp/SetContentOptions/) | Options used by SetContentOptions) and SetContentOptions). |
| [SetUserAgentOptions](/api/PuppeteerSharp/SetUserAgentOptions/) | Options for SetUserAgentAsync(SetUserAgentOptions). |
| [TabInfo](/api/PuppeteerSharp/TabInfo/) | Information about the browser tab of a page. See TabInfoAsync(). |
| [Target](/api/PuppeteerSharp/Target/) | Target. |
| [TargetChangedArgs](/api/PuppeteerSharp/TargetChangedArgs/) | Event arguments used by target related events. |
| [TargetClosedException](/api/PuppeteerSharp/TargetClosedException/) | Exception thrown by the Connection when it detects that the target was closed. |
| [TargetInfo](/api/PuppeteerSharp/TargetInfo/) | Target info. |
| [TimeoutSettings](/api/PuppeteerSharp/TimeoutSettings/) | Timeout settings. |
| [Tracing](/api/PuppeteerSharp/Tracing/) | You can use StartAsync(TracingOptions) and StopAsync() to create a trace file which can be opened in Chrome DevTools or timeline viewer. |
| [TracingOptions](/api/PuppeteerSharp/TracingOptions/) | Tracing options used on StartAsync(TracingOptions). |
| [UninstallPWAOptions](/api/PuppeteerSharp/UninstallPWAOptions/) | Options for UninstallPWAAsync(UninstallPWAOptions). |
| [UserAgentBrandVersion](/api/PuppeteerSharp/UserAgentBrandVersion/) | Used to specify User Agent Cient Hints to emulate. See https://wicg.github.io/ua-client-hints. |
| [UserAgentMetadata](/api/PuppeteerSharp/UserAgentMetadata/) | Used to specify User Agent Cient Hints to emulate. See https://wicg.github.io/ua-client-hints Missing optional values will be filled in by the target with what it would normally use. |
| [ViewPortOptions](/api/PuppeteerSharp/ViewPortOptions/) | View port options used on SetViewportAsync(ViewPortOptions). |
| [WaitForFunctionOptions](/api/PuppeteerSharp/WaitForFunctionOptions/) | Optional waiting parameters. |
| [WaitForNetworkIdleOptions](/api/PuppeteerSharp/WaitForNetworkIdleOptions/) | Optional waiting parameters. |
| [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/) | Timeout options. |
| [WaitForSelectorOptions](/api/PuppeteerSharp/WaitForSelectorOptions/) | Optional waiting parameters. |
| [WaitTaskTimeoutException](/api/PuppeteerSharp/WaitTaskTimeoutException/) | Timeout exception that might be thrown by WaitFor methods in Frame. |
| [WebWorker](/api/PuppeteerSharp/WebWorker/) | The Worker class represents a WebWorker (<a href="https://developer.mozilla.org/en-US/docs/Web/API/WebWorkersAPI">https://developer.mozilla.org/en-US/docs/Web/API/WebWorkersAPI</a>). The events WorkerCreated and WorkerDestroyed are emitted on the page object to signal the worker lifecycle. |
| [WindowBounds](/api/PuppeteerSharp/WindowBounds/) | Bounds for a browser window. |
| [WorkAreaInsets](/api/PuppeteerSharp/WorkAreaInsets/) | Represents work area insets for a screen. |
| [WorkerEventArgs](/api/PuppeteerSharp/WorkerEventArgs/) | Workder created event arguments. |
| [WsOptions](/api/PuppeteerSharp/WsOptions/) | Options for the WebSocket connection to the browser. |

## Structs

| Type | Summary |
| --- | --- |
| [Offset](/api/PuppeteerSharp/Offset/) | Offset used in conjunction with Offset%7d). |
| [Point](/api/PuppeteerSharp/Point/) | Represents a Point. |

## Interfaces

| Type | Summary |
| --- | --- |
| [IBluetoothEmulation](/api/PuppeteerSharp/IBluetoothEmulation/) | Exposes the bluetooth emulation abilities. |
| [IBrowser](/api/PuppeteerSharp/IBrowser/) | Provides methods to interact with a browser. |
| [IBrowserContext](/api/PuppeteerSharp/IBrowserContext/) | BrowserContexts provide a way to operate multiple independent browser sessions. When a browser is launched, it has a single IBrowserContext used by default. The method NewPageAsync(CreatePageOptions) creates a IPage in the default IBrowserContext. |
| [IBrowserFetcher](/api/PuppeteerSharp/IBrowserFetcher/) | BrowserFetcher can download and manage different versions of Chromium. |
| [IBrowserOptions](/api/PuppeteerSharp/IBrowserOptions/) | Browser options. |
| [ICDPConnection](/api/PuppeteerSharp/ICDPConnection/) | An ICDPConnection is an object able to send and receive messages from the browser. |
| [ICDPSession](/api/PuppeteerSharp/ICDPSession/) | The CDPSession instances are used to talk raw Chrome Devtools Protocol: Protocol methods can be called with CommandOptions) method. Protocol events, using the MessageReceived event. Documentation on DevTools Protocol can be found here: <a href="https://chromedevtools.github.io/devtools-protocol/">https://chromedevtools.github.io/devtools-protocol/</a>. csharp var client = await Page.Target.CreateCDPSessionAsync(); await client.SendAsync("Animation.enable"); client.MessageReceived += (sender, e) => { if (e.MessageID == "Animation.animationCreated") { Console.WriteLine("Animation created!"); } }; JObject response = await client.SendAsync("Animation.getPlaybackRate"); Console.WriteLine("playback rate is " + response.playbackRate); await client.SendAsync("Animation.setPlaybackRate", new { playbackRate = Convert.ToInt32(response.playbackRate / 2) }); |
| [IConnectionOptions](/api/PuppeteerSharp/IConnectionOptions/) | Options for Connection creation. |
| [IElementHandle](/api/PuppeteerSharp/IElementHandle/) | Inherits from IJSHandle. It represents an in-page DOM element. ElementHandles can be created by QuerySelectorAsync(string) or QuerySelectorAllAsync(string). |
| [IExecutionContext](/api/PuppeteerSharp/IExecutionContext/) | The class represents a context for JavaScript execution. Examples of JavaScript contexts are: Each Frame has a separate IExecutionContext All kind of web workers have their own contexts. |
| [IFrame](/api/PuppeteerSharp/IFrame/) | Provides methods to interact with a single page frame in Chromium. One IPage instance might have multiple IFrame instances. At every point of time, page exposes its current frame tree via the MainFrame and ChildFrames properties. IFrame object's lifecycle is controlled by three events, dispatched on the page object - FrameAttached - fires when the frame gets attached to the page. A Frame can be attached to the page only once - FrameNavigated - fired when the frame commits navigation to a different URL - FrameDetached - fired when the frame gets detached from the page. A Frame can be detached from the page only once. |
| [IJSHandle](/api/PuppeteerSharp/IJSHandle/) | IJSHandle represents an in-page JavaScript object. JSHandles can be created with the EvaluateExpressionHandleAsync(string) and Object%5b%5d) methods. |
| [IPage](/api/PuppeteerSharp/IPage/) | Provides methods to interact with a single tab in Chromium. One IBrowser instance might have multiple IPage instances. |
| [IPuppeteerUtilWrapper](/api/PuppeteerSharp/IPuppeteerUtilWrapper/) | A class that contains a method to get the PuppeteerUtil handle. |
| [IRequest](/api/PuppeteerSharp/IRequest/) | Whenever the page sends a request, the following events are emitted by puppeteer's page: Request emitted when the request is issued by the page. Response emitted when/if the response is received for the request. RequestFinished emitted when the response body is downloaded and the request is complete. If request fails at some point, then instead of RequestFinished event (and possibly instead of Response event), the RequestFailed event is emitted. If request gets a 'redirect' response, the request is successfully finished with the RequestFinished event, and a new request is issued to a redirected url. |
| [IResponse](/api/PuppeteerSharp/IResponse/) | IResponse class represents responses which are received by page. |
| [ITarget](/api/PuppeteerSharp/ITarget/) | Target. |
| [ITracing](/api/PuppeteerSharp/ITracing/) | You can use StartAsync(TracingOptions) and StopAsync() to create a trace file which can be opened in Chrome DevTools or timeline viewer. |
| [IWritableDestination](/api/PuppeteerSharp/IWritableDestination/) | Represents a writable destination for a ScreenRecording. |

## Enums

| Type | Summary |
| --- | --- |
| [AdapterState](/api/PuppeteerSharp/AdapterState/) | Emulated bluetooth adapter state. |
| [BrowserTag](/api/PuppeteerSharp/BrowserTag/) | Browser tag. |
| [ConsoleType](/api/PuppeteerSharp/ConsoleType/) | Console type used on ConsoleMessage. |
| [CookiePriority](/api/PuppeteerSharp/CookiePriority/) | Represents the cookie's 'Priority' status. |
| [CookieSourceScheme](/api/PuppeteerSharp/CookieSourceScheme/) | Represents the source scheme of the origin that originally set the cookie. A value of "Unset" allows protocol clients to emulate legacy cookie scope for the scheme. This is a temporary ability and it will be removed in the future. |
| [CreatePageType](/api/PuppeteerSharp/CreatePageType/) | Specifies the type of page to create. |
| [DialogType](/api/PuppeteerSharp/DialogType/) | Dialog type. |
| [DownloadPolicy](/api/PuppeteerSharp/DownloadPolicy/) | Download policy for controlling download behavior. |
| [DragOperation](/api/PuppeteerSharp/DragOperation/) | Drag operations available on DragData. |
| [HeadlessMode](/api/PuppeteerSharp/HeadlessMode/) | Headless mode. |
| [InitiatorType](/api/PuppeteerSharp/InitiatorType/) | Type of the Initiator. |
| [InterceptResolutionAction](/api/PuppeteerSharp/InterceptResolutionAction/) | Represents the action taken for an intercepted request. |
| [MediaFeature](/api/PuppeteerSharp/MediaFeature/) | Media Feature. See MediaFeatureValue%7d). |
| [OverridePermission](/api/PuppeteerSharp/OverridePermission/) | Override permission. |
| [PermissionState](/api/PuppeteerSharp/PermissionState/) | Permission state for PermissionEntry%5b%5d). |
| [Platform](/api/PuppeteerSharp/Platform/) | Platform used by a BrowserFetcher. |
| [ProtocolType](/api/PuppeteerSharp/ProtocolType/) | Protocol used to communicate with the browser. |
| [PWADisplayMode](/api/PuppeteerSharp/PWADisplayMode/) | If the user prefers opening an installed web app in a standalone window or in a browser tab. |
| [RequestAbortErrorCode](/api/PuppeteerSharp/RequestAbortErrorCode/) | Abort error codes. used by Int32%7d). |
| [ResourceType](/api/PuppeteerSharp/ResourceType/) | Resource type. |
| [SameSite](/api/PuppeteerSharp/SameSite/) | SameSite values in cookies. |
| [ScreenshotType](/api/PuppeteerSharp/ScreenshotType/) | Screenshot file type. |
| [SupportedBrowser](/api/PuppeteerSharp/SupportedBrowser/) | Supported Browser. |
| [TargetType](/api/PuppeteerSharp/TargetType/) | Target type. |
| [VisionDeficiency](/api/PuppeteerSharp/VisionDeficiency/) | Types of vision deficiency to emulate using EmulateVisionDeficiencyAsync(VisionDeficiency). |
| [WaitForFunctionPollingOption](/api/PuppeteerSharp/WaitForFunctionPollingOption/) | An interval at which the pageFunction is executed. |
| [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/) | Wait until navigation. |
| [WindowState](/api/PuppeteerSharp/WindowState/) | Window state for WindowBounds. |

## Delegates

| Type | Summary |
| --- | --- |
| [BrowserFetcherOptions.CustomFileDownloadAction](/api/PuppeteerSharp/BrowserFetcherOptions.CustomFileDownloadAction/) | A custom download delegate. |
| [LazyArg](/api/PuppeteerSharp/LazyArg/) | A LazyArg is an evaluation argument that will be resolved when the CDP call is built. |
