#!/bin/bash

# AutonomousAI Auto-Run Script
# This script provides rapid development cycles for the AutonomousAI system

set -e  # Exit on any error

MODE=${1:-"dev"}  # Default to dev mode
INTENT=${2:-"Self-evolve and improve AutonomousAI capabilities"}
VALUE_THRESHOLD=${3:-"50"}

echo "🚀 AutonomousAI Auto-Run"
echo "Mode: $MODE"
echo "Intent: $INTENT"
echo "Value Threshold: $VALUE_THRESHOLD"

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Environment setup
if [ ! -f ".env" ]; then
  echo -e "${YELLOW}No .env file found. Creating one...${NC}"
  echo "OPENAI_API_KEY=" > .env
  echo "AZURE_OPENAI_API_KEY=" >> .env
  echo "AZURE_OPENAI_ENDPOINT=" >> .env
  echo "DEFAULT_VALUE_THRESHOLD=50" >> .env
  echo -e "${YELLOW}Please edit .env file with your API keys${NC}"
  exit 1
else
  source .env
fi

# Function to build and run
build_and_run() {
  echo -e "${BLUE}Building AutonomousAI...${NC}"
  dotnet build --configuration Release
  
  if [ $? -eq 0 ]; then
    echo -e "${GREEN}Build successful!${NC}"
    echo -e "${BLUE}Running with intent: $INTENT${NC}"
    dotnet run --project . --configuration Release -- "$INTENT" "$VALUE_THRESHOLD" | tee last_run.log
  else
    echo -e "${RED}Build failed!${NC}"
    exit 1
  fi
}

# Function to run tests
run_tests() {
  echo -e "${BLUE}Running tests...${NC}"
  dotnet test
}

# Function to simulate GitHub Actions workflow locally
simulate_workflow() {
  echo -e "${BLUE}Simulating GitHub Actions workflow locally...${NC}"
  
  # Set up environment variables similar to GitHub Actions
  export GITHUB_TOKEN=$GITHUB_TOKEN
  export OPENAI_API_KEY=$OPENAI_API_KEY
  export AZURE_OPENAI_API_KEY=$AZURE_OPENAI_API_KEY
  export AZURE_OPENAI_ENDPOINT=$AZURE_OPENAI_ENDPOINT
  export VALUE_THRESHOLD=$VALUE_THRESHOLD
  
  # Run with the same command as in the workflow
  dotnet run --project . --configuration Release -- "$INTENT" "$VALUE_THRESHOLD" | tee workflow_simulation.log
  
  echo -e "${GREEN}Workflow simulation completed!${NC}"
  echo "Results saved to workflow_simulation.log"
}

# Function to set up file watchers for auto-rebuild
setup_watchers() {
  echo -e "${BLUE}Setting up file watchers for auto-rebuild...${NC}"
  echo -e "${YELLOW}Note: This requires 'inotifywait' from inotify-tools package${NC}"
  
  if ! command -v inotifywait &> /dev/null; then
    echo -e "${RED}inotifywait not found. Please install inotify-tools:${NC}"
    echo "sudo apt-get install inotify-tools"
    exit 1
  fi
  
  echo -e "${GREEN}Starting file watcher...${NC}"
  echo -e "${YELLOW}Press Ctrl+C to stop watching${NC}"
  
  while true; do
    inotifywait -e modify -e create -e delete -r --exclude '(bin|obj|\.git)' .
    echo -e "${BLUE}Changes detected! Rebuilding...${NC}"
    dotnet build --configuration Release
    
    if [ $? -eq 0 ]; then
      echo -e "${GREEN}Auto-rebuild successful!${NC}"
    else
      echo -e "${RED}Auto-rebuild failed!${NC}"
    fi
  done
}

# Main logic based on mode
case $MODE in
  "dev")
    build_and_run
    ;;
  "watch")
    setup_watchers
    ;;
  "test")
    run_tests
    ;;
  "workflow")
    simulate_workflow
    ;;
  "full")
    run_tests
    build_and_run
    ;;
  *)
    echo -e "${RED}Unknown mode: $MODE${NC}"
    echo "Usage: ./auto-run.sh [dev|watch|test|workflow|full] [intent] [value_threshold]"
    exit 1
    ;;
esac

echo -e "${GREEN}AutonomousAI Auto-Run completed!${NC}" 