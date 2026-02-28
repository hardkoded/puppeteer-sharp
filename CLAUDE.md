# PuppeteerSharp Codebase Overview

PuppeteerSharp is a C# library for automating Chromium-based browsers and Firefox. It provides a high-level API to control browsers via the DevTools Protocol (CDP) and WebDriver BiDi protocol.

## Goal

The goal of this project is to be a port of the popular Node.js Puppeteer library to .NET.
Everything in PuppeteerSharp is inspired by the original Puppeteer library, but adapted to C# idioms and .NET practices.

## C# Code Style

When editing C# files, always place static members before non-static members (SA1204/SA1202 style rules). Never place a static method after instance methods.

## Git Workflow

Always verify which git branch you are on before making commits or creating PRs. Never commit to main/master. If the task requires a new branch, create it immediately and confirm with `git branch --show-current` before proceeding.

## Upstream Porting Conventions

When porting upstream TypeScript/Puppeteer changes to PuppeteerSharp (.NET), translate API naming conventions: camelCase → PascalCase, Promise<T> → Task<T>, interfaces prefixed with I, events use C# event patterns. Use `ReloadOptions`-style single parameter objects rather than creating redundant overloads.

## External Sources:
These external sources are referenced or used as inspiration in the codebase. Feel free to explore them for deeper understanding.
You are allowed to run git commands to update these repositories locally.

- Original Puppeteer repository: ../../puppeteer/puppeteer. Every time "upstream" is mentioned we are referring to this code.
- Bidi Driver: ../../webdriverbidi-net/webdriverbidi-net

## Upstream code structure

- Code in upstream puppeteer-core/src/api/* are our abstract class. For instance our public abstract class Frame.
- Code in upstream puppeteer-core/src/bidi/* are our Bidi* classes.
- Code in upstream puppeteer-core/src/cdp/* are our Cdp* classes.

## Project Structure

```
lib/
├── PuppeteerSharp/                 # Main library
│   ├── Bidi/                       # WebDriver BiDi protocol implementation
│   ├── Cdp/                        # Chrome DevTools Protocol implementation
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
├── PuppeteerSharp.Tests/           # Test categories
├── PuppeteerSharp.Nunit/           # NUnit test framework integration
├── PuppeteerSharp.TestServer/      # Local HTTP server for testing
├── PuppeteerSharp.TestServer/wwwroot/ # Test fixtures and assets
├── demo/                           # Demo application
└── PuppeteerSharp.sln              # Solution file
```

### Protocol Abstraction Pattern

The library uses a base class pattern with protocol-specific implementations:

- **Interface Level**: `IBrowser`, `IPage`, `IFrame`, `IRequest`, `IResponse`
- **Abstract Base Classes**: `Browser`, `Page`, `Frame`, `Request<TResponse>`, `Response<TRequest>`
- **Protocol Implementations**: `CdpBrowser`/`BidiBrowser`, `CdpPage`/`BidiPage`, etc.

## Testing Infrastructure

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

**IMPORTANT: Test Expectations Files Rules:**
- `TestExpectations.upstream.json`: This file should NEVER be edited unless syncing with the upstream Puppeteer project. It contains expectations that match the upstream Puppeteer test expectations.
- `TestExpectations.local.json`: Use this file for local overrides and PuppeteerSharp-specific test expectations. Add entries here to skip or mark tests that fail due to .NET-specific issues or features not yet implemented. Never add entries to this file that are meant to match upstream expectations and never add entries without explicit confirmation.

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

## Testing & Debugging

When investigating flaky tests, always look for root causes (race conditions, non-deterministic ordering like ConcurrentDictionary enumeration, frame ordering assumptions) rather than skipping the test or adding it to expected failures.

## Continuous improvement

Every time you find a code style error during builds, for instance rules failures like "SA1648". Update this document to include the new rule and a brief description of how the codebase adheres to it. This will help maintain a high standard of code quality and consistency throughout the project.
