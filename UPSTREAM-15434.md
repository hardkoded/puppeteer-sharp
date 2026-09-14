# Upstream PR #15434 — Roll to Chrome 152.0.7977.82

## Upstream

- PR: https://github.com/puppeteer/puppeteer/pull/15434
- Title: fix: roll to Chrome 152.0.7977.82
- Intent: bump pinned Chrome / chrome-headless-shell build from `152.0.7977.75` to `152.0.7977.82`.

## Mapping to PuppeteerSharp

| Upstream | PuppeteerSharp |
| --- | --- |
| `packages/puppeteer-core/src/revisions.ts` (`chrome`, `chrome-headless-shell`) | `lib/PuppeteerSharp/BrowserData/Chrome.cs` (`DefaultBuildId`) |
| `versions.json` | N/A (no equivalent pin file) |
| `package-lock.json` peer/schematics noise | N/A (Node-only) |

## Implementation notes

- Both Chrome and ChromeHeadlessShell downloads resolve through `Chrome.DefaultBuildId` in `BrowserFetcher` / `Launcher`, so a single constant update covers both pins from upstream.
- Did not advance to 153.x; that belongs to a later upstream roll.

## Verification

```bash
BROWSER=CHROME PROTOCOL=cdp dotnet build lib/PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj
BROWSER=CHROME PROTOCOL=cdp dotnet test lib/PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj \
  --filter "FullyQualifiedName~BrowserFetcher" --no-build \
  -- NUnit.TestOutputXml=TestResults
```
