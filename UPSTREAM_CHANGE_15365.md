# Upstream PR #15365 → PuppeteerSharp

## Upstream change

[puppeteer/puppeteer#15365](https://github.com/puppeteer/puppeteer/pull/15365) — **feat(webmcp): support canceling tool execution**

Adds AbortSignal-based cancellation for WebMCP tool execution so long-running or obsolete tool calls can be aborted cleanly via CDP `WebMCP.cancelInvocation`.

## PuppeteerSharp mapping

| Upstream | PuppeteerSharp |
| --- | --- |
| `WebMCPToolExecuteOptions.signal` (`AbortSignal`) | `WebMcpToolExecuteOptions.CancellationToken` |
| `WebMCPTool.execute(input, options)` | `WebMcpTool.ExecuteAsync(input, options)` |
| `WebMCP.cancelInvocation` | `CdpWebMcp.CancelInvocationAsync` |
| Tests in `webmcp.test.ts` | `PageWebMcpTests` with `[PuppeteerTest("webmcp.spec", ...)]` |

Cancellation resolves with `WebMcpInvocationStatus.Canceled` (matching upstream) rather than throwing `OperationCanceledException`.

## Files changed

- `lib/PuppeteerSharp/Cdp/WebMcpToolExecuteOptions.cs` (new)
- `lib/PuppeteerSharp/Cdp/WebMcpTool.cs` — accept options; register cancel callback
- `lib/PuppeteerSharp/Cdp/CdpWebMcp.cs` — `CancelInvocationAsync`; update feature-flag docs
- `lib/PuppeteerSharp.Tests/WebMcpTests/PageWebMcpTests.cs` — cancel tests + align with `document.modelContext` / `--enable-features=WebMCP`
- `lib/PuppeteerSharp.Nunit/TestExpectations/TestExpectations.upstream.json` — remove outdated Chrome 149+ FAIL so WebMCP runs on Chrome 152
- `lib/PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj` — fix `ProjectReferenc1e` typo blocking builds

## Verification

`BROWSER=CHROME PROTOCOL=cdp` — all 9 `PageWebMcpTests` passed.
