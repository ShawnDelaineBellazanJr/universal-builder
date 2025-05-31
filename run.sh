#!/bin/bash

# Universal Autonomous Builder run script
# Usage: ./run.sh [command] [options]

# Set debug mode by default for easy testing
export DEBUG_MODE=true

# Default command is universal-build
COMMAND=${1:-universal-build}

# Default goal if not provided
DEFAULT_GOAL="Self-evolve and improve Universal Builder capabilities"

# Parse arguments
ARGS=""
if [ $# -gt 1 ]; then
    # Add remaining arguments
    shift
    ARGS="$@"
fi

# If no arguments and command is universal-build, add default goal
if [ "$COMMAND" == "universal-build" ] && [ -z "$ARGS" ]; then
    ARGS="--goal \"$DEFAULT_GOAL\""
fi

# Run the builder
echo "Running: dotnet run $COMMAND $ARGS"
eval "dotnet run $COMMAND $ARGS" 