#!/bin/bash

# Set debug mode
export DEBUG_MODE=true

# Test the Continuous Awareness Monitor
echo "Testing Continuous Awareness Monitor..."

# Set up the environment for local testing
export GITHUB_OWNER="ShawnDelaineBellazanJr"
export GITHUB_REPO="universal-builder"
export GITHUB_TOKEN="github_pat_debug_token"  # Using debug token
export OPENAI_API_KEY="sk-debug-mode-key"

# Get the full path of the project
PROJECT_PATH=$(pwd)/AutonomousAI.csproj

echo "Using project path: $PROJECT_PATH"

# Execute the monitoring process
echo "Running continuous monitoring..."
dotnet run --project "$PROJECT_PATH" -- cognitive-process --frequency continuous --goal "Check repository health" --economic-value 75

# Test the Analysis Thinking Processor
echo "Testing Analysis Thinking Processor..."
dotnet run --project "$PROJECT_PATH" -- cognitive-process --frequency analysis --goal "Analyze build patterns and optimize performance" --economic-value 85

echo "Testing complete!" 