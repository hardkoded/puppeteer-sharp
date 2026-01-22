#!/bin/bash

# Default values for the notification
TITLE="${1:-Claude Code}"
MESSAGE="${2:-Task successfully completed in repo.}"

# Check for OS and use the appropriate command - Windows excluded
if [[ "$OSTYPE" == "darwin"* ]]; then
    # macOS: Use osascript for a native notification
    osascript -e "display notification \"$MESSAGE\" with title \"$TITLE\" sound name \"Glass\""
elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
    # Linux (requires 'notify-send', usually pre-installed on desktop environments)
    notify-send "$TITLE" "$MESSAGE"
fi
