#!/bin/bash

# AutonomousAI CI/CD Runner
# This script is designed to be run in CI/CD environments like GitHub Actions

set -e  # Exit on any error

INTENT=${1:-"Self-evolve and improve capabilities"}
VALUE_THRESHOLD=${2:-"50"}
CI_ENVIRONMENT=${3:-"github-actions"}

echo "🚀 AutonomousAI CI/CD Runner"
echo "Intent: $INTENT"
echo "Value Threshold: $VALUE_THRESHOLD"
echo "CI Environment: $CI_ENVIRONMENT"

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to run with full logging
run_with_logging() {
    echo -e "${BLUE}Building AutonomousAI...${NC}"
    dotnet build --configuration Release
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}Build successful!${NC}"
        echo -e "${BLUE}Running with intent: $INTENT${NC}"
        
        # Run with full logging
        dotnet run --project . --configuration Release -- "$INTENT" "$VALUE_THRESHOLD" | tee ci_run.log
        
        # Create artifacts directory if it doesn't exist
        mkdir -p artifacts
        
        # Copy logs to artifacts
        cp ci_run.log artifacts/
        
        # Return success only if the run was successful
        if [ ${PIPESTATUS[0]} -eq 0 ]; then
            echo -e "${GREEN}Run completed successfully!${NC}"
            return 0
        else
            echo -e "${RED}Run failed!${NC}"
            return 1
        fi
    else
        echo -e "${RED}Build failed!${NC}"
        return 1
    fi
}

# Function to prepare GitHub-specific environment
setup_github_environment() {
    # Configure Git for GitHub Actions
    git config --global user.name "AutonomousAI"
    git config --global user.email "autonomousai@noreply.github.com"
    
    # Set up GitHub CLI if token is available
    if [ ! -z "$GITHUB_TOKEN" ]; then
        echo -e "${BLUE}Setting up GitHub CLI...${NC}"
        echo "$GITHUB_TOKEN" | gh auth login --with-token
    else
        echo -e "${YELLOW}GITHUB_TOKEN not set. GitHub CLI operations will be limited.${NC}"
    fi
}

# Function to collect diagnostic information
collect_diagnostics() {
    echo -e "${BLUE}Collecting diagnostic information...${NC}"
    
    # Create diagnostics directory
    mkdir -p artifacts/diagnostics
    
    # Collect system information
    echo "System Information" > artifacts/diagnostics/system_info.txt
    uname -a >> artifacts/diagnostics/system_info.txt
    
    # Collect .NET information
    echo "Installed .NET SDKs:" > artifacts/diagnostics/dotnet_info.txt
    dotnet --list-sdks >> artifacts/diagnostics/dotnet_info.txt
    echo "Installed .NET runtimes:" >> artifacts/diagnostics/dotnet_info.txt
    dotnet --list-runtimes >> artifacts/diagnostics/dotnet_info.txt
    
    # Collect Git information
    echo "Git Configuration:" > artifacts/diagnostics/git_info.txt
    git config --list >> artifacts/diagnostics/git_info.txt
    
    # Collect environment variables (redacted)
    echo "Environment Variables (redacted):" > artifacts/diagnostics/env_info.txt
    env | grep -v "_KEY\|TOKEN\|SECRET" | sort >> artifacts/diagnostics/env_info.txt
    
    echo -e "${GREEN}Diagnostic information collected!${NC}"
}

# Main execution
case $CI_ENVIRONMENT in
    "github-actions")
        echo -e "${BLUE}Setting up for GitHub Actions...${NC}"
        setup_github_environment
        ;;
    "azure-devops")
        echo -e "${BLUE}Setting up for Azure DevOps...${NC}"
        # Add Azure DevOps specific setup here
        ;;
    "jenkins")
        echo -e "${BLUE}Setting up for Jenkins...${NC}"
        # Add Jenkins specific setup here
        ;;
    *)
        echo -e "${YELLOW}No specific setup for CI environment: $CI_ENVIRONMENT${NC}"
        ;;
esac

# Run the main process
run_with_logging

# Collect diagnostics regardless of success/failure
collect_diagnostics

# Final status message
if [ $? -eq 0 ]; then
    echo -e "${GREEN}CI/CD run completed successfully!${NC}"
    exit 0
else
    echo -e "${RED}CI/CD run failed! Check logs for details.${NC}"
    exit 1
fi 