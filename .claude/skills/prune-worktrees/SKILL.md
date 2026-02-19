# Task

Prune git worktrees whose associated pull requests have been closed or merged.

## Steps

1. List all worktrees using `git worktree list`
2. Skip the main working tree (the first entry listed)
3. For each remaining worktree, extract the branch name
4. Use `gh pr list --head <branch> --state all --json number,state,title` to check if there is a PR for that branch
5. If the PR state is "CLOSED" or "MERGED", remove the worktree with `git worktree remove <path>`
6. If there is no PR for the branch, skip it and report it as "no PR found"
7. After processing all worktrees, run `git worktree prune` to clean up stale references
8. Report a summary: which worktrees were removed (and why), which were kept, and which had no PR
