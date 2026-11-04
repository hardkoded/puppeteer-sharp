# Work on Open Issues

## Context
You are automating the implementation of multiple open GitHub issues using parallel sub-agents.

## Task

List open issues matching the filter `$ARGUMENTS` and implement each one by delegating to `/implement-upstream-change`.

## Workflow

### Step 1: List Matching Issues

Fetch open issues matching the filter:
```bash
gh issue list --state open --search "$ARGUMENTS" --json number,title,url --limit 100
```

Print the list of issues found so the user can see what will be processed.

### Step 2: Process Issues in Batches of 3

For each batch of up to 3 issues, launch `general-purpose` sub-agents **in parallel** (in a single message with multiple tool uses) using **isolated worktrees** (`isolation: "worktree"`). Wait for each batch to complete before starting the next.

Each sub-agent receives the following prompt (fill in `<ISSUE_NUMBER>`, `<ISSUE_URL>`, and `<ISSUE_TITLE>`):

~~~
You are working on GitHub issue #<ISSUE_NUMBER>: <ISSUE_TITLE>
Issue URL: <ISSUE_URL>

Run the /implement-upstream-change skill passing the issue URL as argument:

Use the Skill tool with skill: "implement-upstream-change" and args: "<ISSUE_URL>"

After the skill completes, if the issue's work is fully done (either a PR was created and merged, or a new PR was just created), close the issue with a comment:

```bash
gh issue close <ISSUE_NUMBER> --comment "Closed via implement-upstream-change. <brief description of what was done or which PR addressed it>"
```
~~~

### Step 3: Final Report

After all batches complete, print a summary:

```
## Issues Implementation Report

### Processed
- #XXXX - <title> - <status: success/failed> - <closed/open>
- ...

### Cleanup
Run `/remove-all-worktrees` to clean up worktree directories.
```
