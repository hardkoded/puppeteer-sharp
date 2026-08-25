# Upstream PR #15292 — Shadow roots in MutationPoller

## Upstream

- PR: https://github.com/puppeteer/puppeteer/pull/15292
- Title: feat: support shadow roots in MutationPoller
- Fix intent: `MutationPoller` did not observe shadow roots, so piercing
  selectors (`div >>> h1`) and other mutation-polled handlers timed out when
  matching nodes were added inside open shadow DOM.

## Mapping to PuppeteerSharp

| Upstream | PuppeteerSharp |
| --- | --- |
| `packages/puppeteer-core/src/injected/Poller.ts` | `lib/PuppeteerSharp/Injected/injected.js` (bundled MutationPoller) |
| `test/src/waittask.test.ts` | `lib/PuppeteerSharp.Tests/WaitTaskTests/FrameWaitForSelectorTests.cs` |
| `test/TestExpectations.json` | `lib/PuppeteerSharp.Nunit/TestExpectations/TestExpectations.upstream.json` |

## Implementation notes

- Kept PuppeteerSharp’s existing `createDeferredPromise` / `#promise` style in
  `injected.js` rather than switching the whole poller stack to upstream’s
  `Deferred` helper.
- Ported helper logic: `MUTATION_OBSERVER_OPTIONS`, `canHostShadowRoots`,
  `parentOf`, `hasAncestorIn`, plus `#observe` / `#observeAddedShadowRoots` /
  `#observeShadowRoots`.
- Attaching a shadow root to a node already in the DOM still produces no
  mutation (WHATWG DOM #1287); that test remains SKIP’d, matching upstream.

## Verification

```bash
BROWSER=CHROME PROTOCOL=cdp dotnet build lib/PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj
BROWSER=CHROME PROTOCOL=cdp dotnet test lib/PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj \
  --filter "FullyQualifiedName~FrameWaitForSelectorTests" --no-build \
  -- NUnit.TestOutputXml=TestResults
```

Result: 38 passed, 1 skipped (`ShouldWorkWhenAShadowRootIsAttachedToAnExistingNode`).
