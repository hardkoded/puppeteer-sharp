# Task

Remove all git worktrees from this repository.

## Steps

1. List all worktrees using `git worktree list`
2. Remove every worktree **except** the main working tree (the first entry listed)
3. After removing all worktrees, run `git worktree prune` to clean up stale references
4. Report what was removed
