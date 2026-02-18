# Implement Puppeteer Release

## Context
You are automating the implementation of all PRs from a Puppeteer release into PuppeteerSharp using parallel sub-agents and git worktrees.

## Task

Process the GitHub release at `$ARGUMENTS` and implement each non-documentation PR in parallel (max 3 concurrent).

## Workflow

### Step 1: Parse the Release

Extract the tag from the `$ARGUMENTS` URL (e.g. `https://github.com/nicknisi/puppeteer/releases/tag/puppeteer-v24.7.2` → `puppeteer-v24.7.2`).

Fetch the release body:
```bash
gh release view <tag> --repo nicknisi/puppeteer --json body -q .body
```

### Step 2: Extract and Filter PRs

Parse all PR references from the release notes. PRs appear as `[#NUMBER](URL)` links in the body.

**Skip documentation-only PRs**: Any PR listed under a `Documentation` section header should be skipped. Track these for the final report.

### Step 3: Fetch origin

Run `git fetch origin` once before processing any PRs.

### Step 4: Process PRs in Batches of 3

For each batch of up to 3 PRs, launch 3 `general-purpose` Task sub-agents **in parallel** (in a single message with multiple tool uses). Wait for the batch to complete before starting the next batch.

Each sub-agent receives the following self-contained prompt (fill in `<PR_NUMBER>` and `<PR_URL>`):

~~~
You are implementing upstream Puppeteer PR #<PR_NUMBER> (<PR_URL>) into the PuppeteerSharp .NET project.

## Setup

1. Create a git worktree for your work:
   ```bash
   git worktree add -b implement-upstream-change-<PR_NUMBER> ../puppeteer-sharp-pr-<PR_NUMBER> origin/master
   ```
2. Change your working directory to `../puppeteer-sharp-pr-<PR_NUMBER>` for ALL subsequent commands.

## Research

3. Read the upstream PR to understand the changes:
   ```bash
   gh pr view <PR_NUMBER> --repo nicknisi/puppeteer --json title,body,files
   gh pr diff <PR_NUMBER> --repo nicknisi/puppeteer
   ```
4. Read the relevant upstream source files in `../../puppeteer/puppeteer` to understand context.
5. Read the corresponding PuppeteerSharp files to understand the current .NET implementation.

## Implementation

6. Implement the changes following .NET idioms and PuppeteerSharp patterns:
   - Upstream `puppeteer-core/src/api/*` maps to abstract base classes
   - Upstream `puppeteer-core/src/bidi/*` maps to `Bidi/` classes
   - Upstream `puppeteer-core/src/cdp/*` maps to `Cdp/` classes
   - All new tests must use `[Test, PuppeteerTest("<spec-file>", "<describes chain>", "<test-name>")]`
   - Look at existing tests for reference on attribute usage

## Verification

7. Build and run related tests:
   ```bash
   cd ../puppeteer-sharp-pr-<PR_NUMBER>
   BROWSER=CHROME PROTOCOL=cdp dotnet build lib/PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj
   BROWSER=CHROME PROTOCOL=cdp dotnet test lib/PuppeteerSharp.Tests/PuppeteerSharp.Tests.csproj --filter "FullyQualifiedName~<RelevantTestClass>" --no-build -- NUnit.TestOutputXml=TestResults
   ```
   Fix any failing tests before proceeding.

## Commit, Push, and PR

8. Stage and commit your changes with a descriptive message referencing the upstream PR.
9. Push the branch:
   ```bash
   git push -u origin implement-upstream-change-<PR_NUMBER>
   ```
10. Create a PR:
    ```bash
    gh pr create --title "Implement upstream PR #<PR_NUMBER>" --body "Implements changes from <PR_URL>" --base master
    ```

## Important
- ALL file operations and commands must run inside the worktree at `../puppeteer-sharp-pr-<PR_NUMBER>`
- Do NOT modify the main working tree
- If the PR has no meaningful code changes for PuppeteerSharp (e.g. infra-only, upstream-tooling), commit a no-op with a note and still create the PR
~~~

### Step 5: Final Report

After all batches complete, print a summary:

```
## Release Implementation Report

### Implemented
- #XXXX - <title> - <status: success/failed>
- ...

### Skipped (Documentation)
- #XXXX - <title>
- ...

### Cleanup
Run `/remove-all-worktrees` to clean up worktree directories.
```
