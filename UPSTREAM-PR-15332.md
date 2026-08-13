# Upstream PR #15332 → PuppeteerSharp

## Upstream

- **PR**: [puppeteer/puppeteer#15332](https://github.com/puppeteer/puppeteer/pull/15332)
- **Title**: refactor: use Desposable stacks to track listeners
- **Summary**: Replace manual `on`/`off` event listener cleanup with `DisposableStack` + `EventEmitter` wrappers across CDP Browser, DeviceRequestPrompt, FrameManager, Page (heap snapshot), TargetManager, and WebMCP.

## PuppeteerSharp mapping

| Upstream | PuppeteerSharp |
|----------|----------------|
| `DisposableStack` | Existing `DisposableActionsStack` |
| `EventEmitter` wrapper auto-unsubscribe | `+=` handler + `Defer(() => -= handler)` |
| `cdp/Browser.ts` | `Cdp/CdpBrowser.cs` |
| `cdp/DeviceRequestPrompt.ts` | `DeviceRequestPrompt.cs` |
| `cdp/FrameManager.ts` | `Cdp/FrameManager.cs` |
| `cdp/Page.ts` (heap snapshot) | `Cdp/CdpPage.cs` |
| `cdp/TargetManager.ts` | `Cdp/ChromeTargetManager.cs`, `Cdp/FirefoxTargetManager.cs`, `ITargetManager` |
| `cdp/WebMCP.ts` | `Cdp/CdpWebMcp.cs` |

## Changes

1. **CdpBrowser** – Track connection/target-manager listeners in `_subscriptions`; `Detach()` disposes the stack (and the target manager).
2. **DeviceRequestPrompt** – Track `MessageReceived` via `DisposableActionsStack`; dispose on select/cancel; drop detached-session nulling (matches upstream).
3. **FrameManager** – Use `using` + `DisposableActionsStack` for temporary frame-swap / page-close listeners.
4. **CdpPage.CaptureHeapSnapshotAsync** – Same stack pattern for heap snapshot chunk listener.
5. **ChromeTargetManager / FirefoxTargetManager** – Connection-level `_subscriptions` and per-session `_attachmentSubscriptions`; `ITargetManager : IDisposable`.
6. **CdpWebMcp** – Bind/unbind `MessageReceived` through a recreatable `DisposableActionsStack` on `UpdateClient`.

## Verification

- `BROWSER=CHROME PROTOCOL=cdp` library + tests build: success
- DeviceRequestPrompt tests: 22 passed
- Browser / BrowserContext / PageClose related tests: passed (one flaky `ShouldCloseAServiceWorker` reference-equality assertion; passed on re-run)
