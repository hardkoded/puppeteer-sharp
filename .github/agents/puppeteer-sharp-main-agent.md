---
name: Puppeteer-Sharp main agent
description: Puppeteer-sharp expert
---

# PuppeteerSharp Codebase Overview

PuppeteerSharp is a C# library for automating Chromium-based browsers and Firefox. It provides a high-level API to control browsers via the DevTools Protocol (CDP) and WebDriver BiDi protocol.

## Goal

The goal of this project is to port the popular Node.js Puppeteer library to .NET.
Everything in PuppeteerSharp is inspired by the original Puppeteer library, but adapted to C# idioms and .NET practices.

## External Sources:
These external sources are referenced or used as inspiration in the codebase. Feel free to explore them for a deeper understanding.
You may run git commands to update these repositories locally.

- Original Puppeteer repository: https://github.com/puppeteer/puppeteer/. Every time "upstream" is mentioned, we are referring to this code.
- Bidi Driver: https://github.com/webdriverbidi-net/webdriverbidi-net

## Upstream code structure

- Code in upstream puppeteer-core/src/api/* are our abstract class. For instanc,e our public abstract class Frame.
- Code in upstream puppeteer-core/src/bidi/* are our Bidi* classes.
- Code in upstream puppeteer-core/src/cdp/* are our Cdp* classes.

## Project Structure

```
lib/
├── PuppeteerSharp/                 # Main library
│   ├── Bidi/                       # WebDriver BiDi protocol implementation (45 files)
│   ├── Cdp/                        # Chrome DevTools Protocol implementation (204 files)
│   ├── Transport/                  # Communication layer (WebSocket, etc.)
│   ├── Helpers/                    # Utility functions and extensions
│   ├── Input/                      # Keyboard, Mouse, Touchscreen input handling
│   ├── Media/                      # Screenshot, PDF, viewport options
│   ├── Mobile/                     # Device descriptors and emulation
│   ├── PageAccessibility/          # Accessibility tree and ARIA handling
│   ├── PageCoverage/               # Code coverage tracking
│   ├── QueryHandlers/              # CSS, XPath, Pierce, Text selectors
│   ├── BrowserData/                # Browser version and channel management
│   ├── States/                     # Process state machine
│   ├── Injected/                   # JavaScript to be injected into pages
│   └── *.cs                        # Core classes (Browser, Page, Frame, etc.)
├── PuppeteerSharp.Tests/           # 58+ test categories (~45k+ test files)
├── PuppeteerSharp.Nunit/           # NUnit test framework integration
├── PuppeteerSharp.TestServer/      # Local HTTP server for testing
├── PuppeteerSharp.TestServer/wwwroot/ # Test fixtures and assets
├── demo/                           # Demo application
└── PuppeteerSharp.sln              # Solution file
```

## Key Architecture Patterns

### Dual Protocol Support: CDP vs BiDi

PuppeteerSharp supports two browser automation protocols:

1. **Chrome DevTools Protocol (CDP)**:
   - Implemented in: `CdpBrowser`, `CdpPage`, `CdpFrame`, `CdpTarget`, etc.
   - CDP-specific implementation classes
   - Used for Chromium-based browsers and Firefox
   - Classes: `CdpJSHandle`, `CdpElementHandle`, `CdpKeyboard`, `CdpMouse`, etc.

2. **WebDriver BiDi Protocol**:
   - Implemented in: `BidiBrowser`, `BidiPage`, `BidiFrame`, `BidiTarget`, etc.
   - Newer, more standardized protocol
   - Lower-level core implementations in `Bidi/Core/` directory
   - Classes: `BidiJSHandle`, `BidiElementHandle`, `BidiMouse`, etc.
   - Events: `BidiBrowsingContextEventArgs`, `UserPromptEventArgs`, `ResponseEventArgs`, etc.

### Protocol Abstraction Pattern

The library uses a base class pattern with protocol-specific implementations:

- **Interface Level**: `IBrowser`, `IPage`, `IFrame`, `IRequest`, `IResponse`
- **Abstract Base Classes**: `Browser`, `Page`, `Frame`, `Request<TResponse>`, `Response<TRequest>`
- **Protocol Implementations**: `CdpBrowser`/`BidiBrowser`, `CdpPage`/`BidiPage`, etc.

This design allows code to work with either protocol transparently through interfaces.

## Core Components

### 1. Entry Point: Puppeteer Static Class
- **File**: `/PuppeteerSharp/Puppeteer.cs`
- **Key Methods**:
  - `LaunchAsync(LaunchOptions)`: Launch a browser instance
  - `ConnectAsync(ConnectOptions)`: Connect to existing browser
  - `GetDefaultArgs()`: Get default browser launch arguments
  - Static properties: `Devices`, `NetworkConditions`, `ExtraJsonSerializerContext`

### 2. Browser Management

#### `Launcher` Class
- Determines which browser to launch (Chrome, Chromium, Firefox)
- Resolves executable path
- Creates appropriate launcher (`ChromeLauncher` or `FirefoxLauncher`)
- Handles process lifecycle

#### `IBrowser` Interface & `Browser` Abstract Class
- **Key Properties**:
  - `WebSocketEndpoint`: Connection URL
  - `BrowserType`: Chrome, Chromium, or Firefox
  - `DefaultContext`: Default browser context
  - `DefaultWaitForTimeout`: Global timeout setting
- **Key Methods**:
  - `NewPageAsync()`: Create new page
  - `CreateBrowserContextAsync()`: Create isolated context
  - `Targets()`: Get all targets (pages, workers, etc.)
  - `PagesAsync()`: Get all pages across contexts
  - `GetVersionAsync()`: Browser version
  - `Disconnect()`: Disconnect from browser

### 3. Page Model

#### `IPage` Interface & `Page` Abstract Class
- **Key Properties**:
  - `MainFrame`: Primary frame
  - `Frames`: All frames in page
  - `Url`: Current URL
  - `Workers`: Web workers
  - `Keyboard`, `Mouse`, `Touchscreen`: Input devices
- **Key Events**: (40+ events)
  - `Close`, `Load`, `DOMContentLoaded`
  - `Console`, `Dialog`, `Error`, `PageError`
  - `Request`, `Response`, `RequestFailed`, `RequestFinished`
  - `FrameAttached`, `FrameDetached`, `FrameNavigated`
- **Key Methods**:
  - `GoToAsync()`: Navigate to URL
  - `ScreenshotAsync()`: Capture page screenshot
  - `PdfAsync()`: Generate PDF
  - `EvaluateAsync()`: Execute JavaScript
  - `WaitForNavigationAsync()`: Wait for navigation
  - `WaitForSelectorAsync()`: Wait for element
  - `InterceptRequestAsync()`: Intercept network requests

### 4. Frame Model

#### `IFrame` Interface & `Frame` Abstract Class
- Represents an `<iframe>` or main document
- Lifecycle events: attached → navigated → detached
- Similar evaluation and waiting methods as Page
- **Key Methods**:
  - `ChildFrames`: Get nested frames
  - `ParentFrame`: Get parent frame
  - `EvaluateAsync()`, `EvaluateFunctionAsync()`: Execute code
  - `WaitForSelectorAsync()`: Wait for elements
  - `QuerySelectorAsync()`, `QuerySelectorAllAsync()`: Find elements

### 5. Element Handling

#### Handle Hierarchy
- `IJSHandle` (Interface): Represents object in browser
- `JSHandle` (Abstract): Base implementation
- `CdpJSHandle` / `BidiJSHandle`: Protocol-specific implementations
- `IElementHandle` (Interface): Represents DOM element
- `ElementHandle` (Abstract): Element-specific operations
- `CdpElementHandle` / `BidiElementHandle`: Protocol-specific implementations

#### `ElementHandle` Capabilities
- **Selection**: `QuerySelectorAsync()`, `QuerySelectorAllAsync()`, `EvaluateAsync()`
- **Interaction**: `ClickAsync()`, `TypeAsync()`, `PressAsync()`
- **Properties**: `BoundingBoxAsync()`, `BoxModelAsync()`, `ScreenshotAsync()`
- **Forms**: `UploadFileAsync()`, `SelectAsync()`
- **Drag & Drop**: `DragAsync()`, `DragAndDropAsync()`

### 6. Network Management

#### Request/Response Model
- **Request**:
  - Abstract base: `Request<TResponse>`
  - Implementations: `CdpHttpRequest`, `BidiHttpRequest`
  - Properties: URL, headers, method, post data, resource type
  - Methods: `ContinueAsync()`, `AbortAsync()`, `RespondAsync()`

- **Response**:
  - Abstract base: `Response<TRequest>`
  - Implementations: `CdpHttpResponse`, `BidiHttpResponse`
  - Methods: `TextAsync()`, `JsonAsync()`, `BufferAsync()`
  - Properties: Status code, headers, URL

#### Interception
- `InterceptRequestAsync()`: Register request handler
- Request properties and methods for control
- Redirect chain tracking
- Initiator information

### 7. Communication Layer

#### Transport (`Transport/` directory)
- `IConnectionTransport`: Abstract transport interface
- `WebSocketTransport`: WebSocket implementation
- `MessageReceivedEventArgs`: Message event data
- Handles binary message sending/receiving

#### Connection (`Cdp/Connection.cs`)
- `Connection` class: Manages CDP communication
- Session management via `CdpCDPSession`
- Message routing and callback handling
- Error handling and protocol timeout

#### CDPSession (`ICDPSession` interface)
- Low-level protocol session
- `SendAsync()`: Send CDP commands
- Event subscriptions for CDP events
- Methods for direct protocol access

### 8. Execution Context & Script Evaluation

#### Realm (Base Class)
- Abstract: `Realm` (core evaluation base)
- Manages execution contexts and script evaluation
- Task manager for tracking ongoing operations

#### IsolatedWorld
- CDP-specific execution context implementation
- Binding management for JavaScript callbacks
- Context lifecycle handling
- Frame and Worker association

#### ExecutionContext
- Represents a JavaScript execution environment
- Methods for evaluation and handle adoption
- Binding utilities for calling .NET from JS

### 9. Input Handling

#### Keyboard (`Input/`)
- `IKeyboard` interface
- `Keyboard` base class
- `CdpKeyboard`, `BidiKeyboard` implementations
- **Methods**: `PressAsync()`, `TypeAsync()`, `DownAsync()`, `UpAsync()`
- Key definitions and special key handling

#### Mouse (`Input/`)
- `IMouse` interface
- `Mouse` base class
- `CdpMouse`, `BidiMouse` implementations
- **Methods**: `MoveAsync()`, `ClickAsync()`, `DragAsync()`, `WheelAsync()`
- Button types: Left, Right, Middle
- Mouse state tracking

#### Touchscreen (`Input/`)
- `ITouchscreen` interface
- `Touchscreen` base class
- `CdpTouchscreen` implementation
- **Methods**: `TapAsync()`, `TouchAsync()`

### 10. Dialog Handling

#### Dialog Classes
- Abstract `Dialog` base class
- `CdpDialog` (CDP implementation)
- `BidiDialog` (BiDi implementation)
- **Types**: alert, confirm, prompt, beforeunload
- **Methods**: `Accept(string text)`, `Dismiss()`
- Properties: `DialogType`, `Message`, `DefaultValue`

### 11. File Handling

#### FileChooser
- Handles file input selection
- `AcceptAsync()`: Select files for upload
- `CancelAsync()`: Cancel file selection
- Triggers on file input element interaction

#### DeviceRequestPrompt
- Handles device permission requests
- Device info: name, model, type
- Accept/reject operations

### 12. Target Management

#### ITarget Interface
- Represents browser entity (page, iframe, worker, etc.)
- Types: Page, WebWorker, Other
- **Methods**: `PageAsync()`, `WorkerAsync()`, `AsPageAsync()`, `CreateCDPSessionAsync()`
- URL, type, parent/child relationships

#### Target Manager
- `ITargetManager` interface
- `ChromeTargetManager`: Chrome-specific target tracking
- `FirefoxTargetManager`: Firefox-specific target tracking
- Target lifecycle: discovered → created → destroyed

### 13. Error Handling

#### Exception Hierarchy
- `PuppeteerException`: Base exception for all Puppeteer errors
- `ProcessException`: Browser process issues
- `TargetClosedException`: Target closed during operation
- `TargetCrashedException`: Target crashed
- `MessageException`: Protocol message errors
- `SelectorException`: Selector parsing errors
- `BufferException`: Buffer operation errors
- `EvaluationFailedException`: Script evaluation errors
- `WaitTaskTimeoutException`: Wait operation timeout

### 14. Feature-Specific Modules

#### PageAccessibility (`PageAccessibility/`)
- `IAccessibility` interface
- Accessibility tree snapshot
- AXNode serialization
- `SerializedAXNode`: Serialized accessibility data
- Supports ARIA attributes

#### PageCoverage (`PageCoverage/`)
- `ICoverage` interface
- JavaScript coverage tracking
- CSS coverage tracking
- Coverage entry with ranges
- Function-level coverage support

#### Media (`Media/`)
- Viewport options, margins, clips
- Screen orientation
- Print media emulation
- Screenshot options (type, quality, clip)
- Print-to-PDF options

#### Mobile (`Mobile/`)
- Device descriptors (iPhone, iPad, etc.)
- Pre-configured device profiles
- Emulation settings: viewport, user agent, device scale factor
- Touch support configuration

### 15. Query Handlers (`QueryHandlers/`)
- `CssQueryHandler`: CSS selector support
- `XPathQueryHandler`: XPath expression support
- `TextQueryHandler`: Text content matching
- `PierceQueryHandler`: Shadow DOM piercing
- `CustomQueryHandler`: User-defined handlers
- Support for combined/nested selectors

### 16. Browser Data Management

#### BrowserData (`BrowserData/`)
- Browser version tracking
- Release channel information
- Build ID management
- Installed browser cache
- Chrome and Firefox specifics
- Version resolution and downloading

### 17. Process State Management (`States/`)
- State machine for browser process lifecycle
- States: Initial → ProcessStarting → Started → Exiting → Exited → Disposed → Killing
- `State` base class
- `StateManager`: State transition handling
- Proper cleanup and resource management

## Testing Infrastructure

### Test Organization (58+ categories)

Test directory structure demonstrates comprehensive coverage:

- **AccessibilityTests/**: Accessibility tree and AXNode testing
- **BrowserContextTests/**: Context isolation and management
- **BrowserTests/**: Browser lifecycle, context creation
- **ClickTests/**: Element click operations
- **CookiesTests/**: Cookie management
- **CoverageTests/**: Code coverage tracking
- **DeviceRequestPromptTests/**: Device permissions
- **DialogTests/**: Alert, confirm, prompt dialogs
- **DragAndDropTests/**: Drag and drop operations
- **ElementHandleTests/**: Element interaction
- **EmulationTests/**: Device and network emulation
- **EvaluationTests/**: JavaScript evaluation
- **FrameTests/**: Frame lifecycle and operations
- **InputTests/**: Keyboard, mouse, touchscreen
- **NetworkTests/**: Request/response interception
- **PageTests/**: Page lifecycle, navigation, properties
- **PrerenderTests/**: Prerendering capabilities
- **RequestInterceptionTests/**: Request modification
- **ScreenshotTests/**: Screenshot and PDF generation
- **TracingTests/**: Performance tracing
- Plus many more...

### Test Infrastructure

#### PuppeteerSharp.Nunit (`PuppeteerSharp.Nunit/`)
- `PuppeteerTestAttribute`: Decorates tests with upstream metadata
- Test expectations system (skip/fail/timeout conditions)
- Platform-specific expectations (Win32, Linux, Darwin)
- Browser-specific expectations (Chrome, Firefox)
- Protocol-specific expectations (CDP, WebDriver BiDi)
- Headless mode variations (headless, headful, headless-shell)
- Local and upstream expectation merging
- Tests should always match the code in upstream. Tests should never be changed to match the code local code.

#### Upstream matching

All our tests have this attribute `[Test, PuppeteerTest("screenshot.spec", "Screenshots Page.screenshot", "should take fullPage screenshots")]`
This helps us link a test with the upstream test:
 * The first argument is the file upstream without the extension.
 * The second argument is the concatenation of `describe`s in upstream. In the example above it means that the test is inside two describes `Screenshots` and `Page.screenshot`.
 * The third argument is the test name.

You shouldn't create tests that don't match upstream unless explicitly requested. In that case, the first argument should be `puppeteer-sharp`.

**IMPORTANT: Test Expectations Files Rules:**
- `TestExpectations.upstream.json`: This file should NEVER be edited unless syncing with the upstream Puppeteer project. It contains expectations that match the upstream Puppeteer test expectations.
- `TestExpectations.local.json`: Use this file for local overrides and PuppeteerSharp-specific test expectations. Add entries here to skip or mark tests that fail due to .NET-specific issues or features not yet implemented.

#### Test Server (`PuppeteerSharp.TestServer/`)
- ASP.NET Core server for hosting test pages
- wwwroot directory with test fixtures
- Asset serving for test scenarios

## Development Best Practices

### Building and Running Tests

When running tests, always build first and then use the `--no-build` flag to avoid rebuilding during test execution. This provides faster and more reliable test runs:
Always be explicit with the browser and protocol you want to test using ENV variables BROWSER=FIREFOX|CHROME and PROTOCOL=bidi|cdp

**IMPORTANT: Chrome should ALWAYS use CDP protocol. Firefox can use either CDP or BiDi.**

```bash
# Build the test project first with Firefox and BiDi
BROWSER=FIREFOX PROTOCOL=bidi dotnet build PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj

# Then run tests with --no-build flag
BROWSER=FIREFOX PROTOCOL=bidi dotnet test PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj --filter "FullyQualifiedName~TestName" --no-build -- NUnit.TestOutputXml=TestResults

# Chrome should ALWAYS use CDP protocol
BROWSER=CHROME PROTOCOL=cdp dotnet build PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj && BROWSER=CHROME PROTOCOL=cdp dotnet test PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj --filter "FullyQualifiedName~TestName" --no-build -- NUnit.TestOutputXml=TestResults

# Firefox should ALWAYS use bidi protocol
BROWSER=FIREFOX PROTOCOL=bidi dotnet build PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj && BROWSER=FIREFOX PROTOCOL=cdp dotnet test PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj --filter "FullyQualifiedName~TestName" --no-build -- NUnit.TestOutputXml=TestResults
```

You can switch between CDP and Bidi by changing the PuppeteerTestAttribute.IsCdp property.
You can switch between Chrome and Firefox by changing the PuppeteerTestAttribute.IsChrome property.
If you are fixing one single test, then you must run the entire test suite to confirm that you didn't break other tests.

## Utilities and Helpers

### Helper Classes (`Helpers/`)
- `AsyncFileHelper`: File I/O operations
- `AsyncMessageQueue`: Message queue for async processing
- `ConcurrentSet<T>`: Thread-safe set
- `DeferredTaskQueue`: Deferred task execution
- `EnumHelper`: Enum utilities
- `MultiMap<K, V>`: Multi-value dictionary
- `RemoteObjectHelper`: Remote object handling
- `TaskHelper`: Task utility functions
- `TaskQueue`: Sequential task execution
- `StringExtensions`: String utilities
- `TempDirectory`: Temporary file management
- `ProtocolStreamReader`: Binary stream reading

### JSON Helpers (`Helpers/Json/`)
- Custom JSON converters
- JSHandle serialization/deserialization
- Protocol message serialization
- Supports System.Text.Json

## Key Design Patterns

### 1. Abstract Factory Pattern
- Protocol-specific implementations behind abstract base classes
- Browser, Page, Frame, etc. have Cdp and Bidi variants
- Factories create appropriate implementation based on protocol

### 2. Template Method Pattern
- Base Page/Frame classes define algorithm structure
- Abstract methods for protocol-specific steps
- Subclasses implement protocol details

### 3. Task-Based Concurrency
- Async/await throughout
- TaskQueue for sequential operation ordering
- TaskManager for tracking concurrent operations

## Continuous improvement

Every time you find a code style error during builds, for instance rules failures like "SA1648". Update this document to include the new rule and a brief description of how the codebase adheres to it. This will help maintain a high standard of code quality and consistency throughout the project.
