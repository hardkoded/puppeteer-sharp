---
name: Bidi migration
description: Implement bidi code
---

# Context
You are a software developer integrating changes from a project written in TypeScript into a .NET project.
We call upstream to the original typescript project.
This task should use the **v21 branch** as base branch and to create pull requests.
Use the puppeteer-sharp/lib/CLAUDE.md for project context.

## Upstream Repositories
- Upstream puppeteer: https://github.com/puppeteer/puppeteer/
- Upstream webdriver bidi: https://github.com/webdriverbidi-net/webdriverbidi-net

Clone these if needed to reference implementation:
```bash
cd /home/runner/work
git clone --depth 1 https://github.com/puppeteer/puppeteer.git
git clone --depth 1 https://github.com/webdriverbidi-net/webdriverbidi-net.git
```

# Task

You are going to implement the changes to make $ARGUMENTS pass.

## Step-by-Step Process

1. **Find and analyze the test expectation**
   - Location: `lib/PuppeteerSharp.Nunit/TestExpectations/TestExpectations.local.json`
   - Find the entry with `testIdPattern` matching $ARGUMENTS
   - Note: The pattern may not exist exactly as specified. Check for broader patterns that include it.
   - Example: If looking for `[jshandle.spec] *.evaluateHandle*`, you might find `[jshandle.spec] *` instead

2. **Locate matching tests**
   - Tests are in `lib/PuppeteerSharp.Tests/`
   - Look for `[Test, PuppeteerTest("spec-file", "category", "test-name")]` attributes
   - Pattern matching examples:
     - `[jshandle.spec] *.evaluateHandle*` matches: `[Test, PuppeteerTest("jshandle.spec", "JSHandle Page.evaluateHandle", "should work")]`
     - `[chromiumonly.spec] Chromium-Specific *` matches: `[Test, PuppeteerTest("chromiumonly.spec", "Chromium-Specific Page Tests", "...")]`

3. **Check if implementation already exists**
   - **IMPORTANT**: Many features may already be implemented! Check the code BEFORE making changes.
   - BiDi implementations are in `lib/PuppeteerSharp/Bidi/`
   - Key files to check:
     - `BidiRealm.cs` - Main realm implementation  
     - `BidiJSHandle.cs` - JSHandle for BiDi
     - `BidiElementHandle.cs` - ElementHandle for BiDi
     - `BidiFrame.cs` - Frame for BiDi
   - Base implementations are in `lib/PuppeteerSharp/`
   - If the feature is already implemented, document this in the PR description

4. **Reference upstream implementation** (if code changes needed)
   - Puppeteer TypeScript: `/home/runner/work/puppeteer/packages/puppeteer-core/src/`
   - BiDi-specific code: `/home/runner/work/puppeteer/packages/puppeteer-core/src/bidi/`
   - Test files: `/home/runner/work/puppeteer/test/src/`
   - WebDriver BiDi .NET: `/home/runner/work/webdriverbidi-net/`

5. **Implement or verify the code**
   - Port TypeScript to C# using .NET idioms
   - Match the upstream logic as closely as possible
   - Common patterns:
     - TypeScript `async methodName(...args): Promise<Type>` → C# `Task<Type> MethodNameAsync(params object[] args)`
     - TypeScript `using` (explicit resource management) → C# `await using` / `IAsyncDisposable`
     - Handle delegation: JSHandle methods often delegate to Realm with handle as first argument

6. **Update test expectations**
   - If feature is working, remove or refine the test expectation entry
   - Be careful with broad patterns like `[jshandle.spec] *` - only remove if ALL tests pass
   - Consider creating more specific patterns if only some tests work

7. **Document in PR description** (NOT in separate files)
   - **Do NOT create IMPLEMENTATION_NOTES.md, SUMMARY.md, or similar documentation files**
   - Put ALL implementation details, analysis, and findings in the PR description
   - Include:
     - What tests are affected
     - Implementation status (already working, new code added, or partially working)
     - Key code components involved
     - Comparison with upstream TypeScript if relevant

8. **Test verification**
   - Run tests with: `BROWSER=FIREFOX PROTOCOL=webDriverBiDi dotnet test --filter "TestName"`
   - Build first if needed: `dotnet build lib/PuppeteerSharp.sln`
   - Note: May need self-signed certificate for test server (see setup below)

## Important Notes

- **Start from v21 branch**: Always checkout v21 first before creating your feature branch
- **Check existing code**: Many features are already implemented - verify before making changes
- **No separate docs**: All documentation goes in the PR description, not in separate markdown files
- **Test patterns**: Use wildcards to match multiple tests, be careful with broad patterns
- **BiDi vs CDP**: This is for WebDriver BiDi protocol (Firefox), not Chrome DevTools Protocol

## Setup Requirements

If building/testing is needed:

**Self-signed certificate for test server:**
```bash
cd lib/PuppeteerSharp.TestServer
openssl req -x509 -newkey rsa:2048 -keyout testKey.pem -out testCert.cer -days 365 -nodes -subj "/CN=localhost"
```

**Update .gitignore** to exclude test certificates:
```
lib/PuppeteerSharp.TestServer/testKey.pem
lib/PuppeteerSharp.TestServer/testCert.cer
```

## Code Structure Reference

```
lib/PuppeteerSharp/
├── Bidi/                      # BiDi protocol implementations
│   ├── BidiRealm.cs          # Main realm - handles evaluation
│   ├── BidiJSHandle.cs       # JSHandle for BiDi
│   ├── BidiElementHandle.cs  # ElementHandle for BiDi
│   ├── BidiFrame.cs          # Frame for BiDi
│   └── Core/                 # Core BiDi protocol classes
├── JSHandle.cs               # Base JSHandle class
├── Realm.cs                  # Base Realm class
└── IJSHandle.cs              # JSHandle interface

lib/PuppeteerSharp.Tests/
├── JSHandleTests/            # JSHandle tests
├── FrameTests/               # Frame tests
└── ...                       # Other test directories

lib/PuppeteerSharp.Nunit/TestExpectations/
└── TestExpectations.local.json  # Test expectations to modify
```
