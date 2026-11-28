# PuppeteerSharp Codebase Overview

PuppeteerSharp is a C# library for automating Chromium-based browsers and Firefox. It provides a high-level API to control browsers via the DevTools Protocol (CDP) and WebDriver BiDi protocol.

## Goal

The goal of this project is to be a port of the popular Node.js Puppeteer library to .NET.
Everything in PuppeteerSharp is inspired by the original Puppeteer library, but adapted to C# idioms and .NET practices.

## C# Code Style

When editing C# files, always place static members before non-static members (SA1204/SA1202 style rules). Never place a static method after instance methods.

StyleCop enforces a strict member ordering inside a class, applied in this priority: (1) member *kind* - fields, then constructors, then properties/indexers/events, then methods (SA1201: "a method should not follow a field/property/event"); (2) within each kind, *accessibility* - public, then internal, then protected internal, then protected, then private (SA1202); (3) within each accessibility group, *static before instance* (SA1204). When adding a new private helper method, it does not belong near the top of the class or next to the fields/properties it supports logically - it belongs in the private-methods section, after all public/internal/protected methods, grouped with other private statics before private instance methods.

## Git Workflow

Always verify which git branch you are on before making commits or creating PRs. Never commit to main/master. If the task requires a new branch, create it immediately and confirm with `git branch --show-current` before proceeding.

## Formatting

CSharpier owns formatting. Run `dotnet csharpier format .` from the repo root before every commit/push. The Husky.Net pre-push hook runs `dotnet csharpier check .` and blocks unformatted pushes. Do not run `dotnet format whitespace` or a plain `dotnet format`, because their whitespace pass fights CSharpier. Use `dotnet format style` / `dotnet format analyzers` only.

- CSharpier and Husky.Net are local tools pinned in `.config/dotnet-tools.json`. Run `dotnet tool restore` once to get them.
- The hook is installed automatically by the `Husky` MSBuild target in `lib/PuppeteerSharp/PuppeteerSharp.csproj` on restore/build. It is skipped when `CI=true`, when `HUSKY=0`, or when there is no `.git`.
- CI runs `dotnet csharpier check .` in the `csharpier` job of `.github/workflows/dotnet.yml`.
- Files CSharpier must not touch are listed in `.csharpierignore`.

## Upstream Porting Conventions

When porting upstream TypeScript/Puppeteer changes to PuppeteerSharp (.NET), translate API naming conventions: camelCase → PascalCase, Promise<T> → Task<T>, interfaces prefixed with I, events use C# event patterns. Use `ReloadOptions`-style single parameter objects rather than creating redundant overloads.

## External Sources:
These external sources are referenced or used as inspiration in the codebase. Feel free to explore them for deeper understanding.
You are allowed to run git commands to update these repositories locally.

- Original Puppeteer repository: ../../puppeteer/puppeteer. Every time "upstream" is mentioned we are referring to this code.
- Bidi Driver: ../../webdriverbidi-net/webdriverbidi-net

## chromium-bidi Mapper Version

Chrome BiDi support uses the chromium-bidi mapper (downloaded at build time from npm). The version is tracked in `lib/PuppeteerSharp/Bidi/chromium-bidi.version`. When syncing with upstream Puppeteer, check if the chromium-bidi version has changed in `packages/puppeteer-core/package.json` (`"chromium-bidi": "X.Y.Z"`) and update our version file accordingly.

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
- `TestExpectations.canary.json`: Mirrors upstream's `CanaryTestExpectations.json`. Contains expectations for known browser bugs in newer/canary versions that also affect our current browser versions. Sync this file with upstream's canary expectations when tests fail due to browser-level bugs (not PuppeteerSharp code issues).
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

### StyleCop rules turned off because of CSharpier

CSharpier owns layout, so these StyleCop layout rules are set to `none` in `lib/.editorconfig`. Do not turn them back on, and do not hand-format code to satisfy them; run `dotnet csharpier format .` instead.

- SA1003: Symbols should be spaced correctly. CSharpier breaks a cast before a wrapped operand.
- SA1009: Closing parenthesis should be spaced correctly. CSharpier puts the closing parenthesis of a wrapped argument or parameter list on its own line.
- SA1111: Closing parenthesis should be on the line of the last parameter. Same cause as SA1009.
- SA1118: Parameter should not span multiple lines. CSharpier wraps long arguments, such as lambdas and multi-line strings.
- SA1500: Braces for multi-line statements should not share a line. CSharpier writes `} while (...);`.
- SA1501: Statement should not be on a single line. CSharpier writes empty blocks such as `catch { }` on one line.
- SA1502: Element should not be on a single line. CSharpier writes empty bodies as `{ }`.
- SA1504: All accessors should be single-line or multi-line. CSharpier collapses short accessors.
- SA1513: Closing brace should be followed by a blank line. CSharpier controls the blank lines around wrapped expressions and patterns.
- SA1515: Single-line comment should be preceded by a blank line. CSharpier removes blank lines inside attribute lists.
- SA1516: Elements should be separated by a blank line. CSharpier removes blank lines between accessors.

Rules that stay on and that CSharpier output still meets:

- SA1507: Code should not contain multiple blank lines in a row. CSharpier collapses them, so a double blank line means the file was not formatted.
- SA1116: Parameters should begin on the line after the declaration when they span multiple lines. Do not put a `#pragma` inside a parameter list, because CSharpier then keeps the list as written. Put the `#pragma warning disable` before the member instead.

### Analyzer rules

- CA2000: Dispose objects before losing scope. It also applies to test code. Declare disposable test doubles with `using`. When ownership moves to another object, wrap the allocation in `#pragma warning disable CA2000 // Ownership is transferred to ...`, in one shared helper if several tests need it.
