# Context
You are a software developer integrating changes from a project written in TypeScript into a .NET project.
We call upstream to the original typescript project.

# Task

You are going to check the changes on the PR $ARGUMENTS
You will find the upstream repository at ../../puppeteer/puppeteer. Feel free to read the code there and even make git actions to bring the changes you need to evaluate.

Once you have all the context, you will implement the same changes in this .NET project.

## Git Workflow

1. Fetch the latest from origin: `git fetch origin`
2. Create a new branch from `origin/master` named `implement-upstream-change-<PR_NUMBER>` where `<PR_NUMBER>` is the number of the PR you are implementing: `git checkout -b implement-upstream-change-<PR_NUMBER> origin/master`
3. Implement all the changes (see below)
4. Commit your changes with a descriptive message
5. Push the branch: `git push -u origin implement-upstream-change-<PR_NUMBER>`
6. Create a PR via `gh pr create` targeting `master`

## Implementation

You will have to implement the code as close as possible to the original code, but adapted to .NET idioms and practices.

### Porting Tests

All tests brought from upstream **must** use the `[Test, PuppeteerTest(...)]` attribute header. The attribute signature is:

```csharp
[Test, PuppeteerTest("<spec-file>", "<describes chain>", "<test-name>")]
```

- `<spec-file>`: the upstream `.spec.ts` filename (e.g. `"network.spec"`).
- `<describes chain>`: the nested `describe` block titles joined, matching the upstream structure (e.g. `"network"`).
- `<test-name>`: the `it(...)` test title from upstream (e.g. `"should set bodySize and headersSize"`).

Look at existing tests in the project for reference on how these attributes are used.

## Verification

You need to run related tests to ensure everything is working as expected.
If tests are failing, you will need to fix them.

## Summary

As part of the task you will need to generate a document explaining the changes you made, and how they relate to the original PR.
