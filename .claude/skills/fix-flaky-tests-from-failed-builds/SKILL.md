# Fix Flaky Tests from Failed Builds

## Context
You are automating flaky test triage and remediation for PuppeteerSharp CI failures.

## Task
Given `$ARGUMENTS` (a workflow run URL/ID, PR URL/number, or empty), identify failed builds, detect flaky tests, implement a real code fix for flaky behavior, and create a PR.

## Workflow

### Step 1: Select the Failed Build

1. If `$ARGUMENTS` contains a workflow run URL/ID, use that run.
2. If `$ARGUMENTS` contains a PR, locate its most recent failed `build` workflow run.
3. If `$ARGUMENTS` is empty, pick the latest failed `build` workflow run on this repository.

Use:
```bash
gh run list --workflow dotnet.yml --status failure --limit 20
gh run view <RUN_ID> --json databaseId,url,displayTitle,headBranch,headSha,event,conclusion,jobs
```

### Step 2: Inspect Failed Jobs and Extract Candidate Tests

For each failed job in the run, fetch logs and extract failing test names and failure signatures.

Use:
```bash
gh run view <RUN_ID> --job <JOB_ID> --log
```

Capture:
- Test full name(s)
- Browser/protocol/mode from the failed matrix job name
- Failure type (timeout, assertion mismatch, detached frame, navigation race, etc.)
- Any rerun/retry evidence from `dotnet retest`

### Step 3: Determine if the Failure is Flaky

For each candidate test:
1. Check recent failed runs for recurrence.
2. Determine whether the same test alternates between pass/fail across runs and environments.
3. Exclude deterministic regressions where failure is consistent.

Use:
```bash
gh run list --workflow dotnet.yml --limit 50
```

Classify each candidate as:
- `likely_flaky`
- `likely_regression`
- `insufficient_data`

Only continue remediation for `likely_flaky` tests.

### Step 4: Reproduce Flakiness Locally

1. Create a branch from `origin/master`:
```bash
git fetch origin
git checkout -b fix/flaky-<RUN_ID>-<SHORT_TEST_NAME> origin/master
```
2. Build once, then run the flaky test repeatedly in the same browser/protocol configuration as CI.
3. Confirm non-determinism before changing code.

Use project conventions (build first, then `--no-build`):
```bash
cd lib
BROWSER=<BROWSER> PROTOCOL=<PROTOCOL> dotnet build PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj
for i in {1..10}; do
  BROWSER=<BROWSER> PROTOCOL=<PROTOCOL> dotnet test PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj --filter "FullyQualifiedName~<TEST_NAME>" --no-build -- NUnit.TestOutputXml=TestResults || true
done
```

### Step 5: Implement a Real Flaky-Test Fix

Implement a root-cause fix in production/test code, focusing on race conditions and ordering issues. Do not silence failures by skipping tests or adding expectation suppressions unless explicitly requested.

### Step 6: Validate the Fix

1. Re-run the targeted flaky test repeatedly and confirm stability.
2. Run related test coverage.
3. Run broader validation for the affected test area.

### Step 7: Commit and Open PR

1. Commit with a message referencing the failed run and flaky test.
2. Create a PR summarizing:
- Failed run investigated
- Flaky test(s) identified
- Root cause
- Fix implemented
- Validation evidence (repeat runs)

Use:
```bash
git add -A
git commit -m "Fix flaky test: <TEST_NAME> (run <RUN_ID>)"
gh pr create --title "Fix flaky test: <TEST_NAME>" --body "<SUMMARY>" --base master
```

### Step 8: Final Report

Return:
```text
## Flaky Test Remediation Report

- Run: <RUN_URL>
- Failed jobs analyzed: <COUNT>
- Candidates:
  - <TEST_NAME> - <CLASSIFICATION>
- Fixed tests:
  - <TEST_NAME> - <ROOT CAUSE> - <STATUS>
- PR: <PR_URL>
```
