#!/bin/bash

# Set colors for better readability
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== Multi-Frequency Cognitive Architecture Monitor ===${NC}"
echo

# Function to check workflow status
check_workflow_status() {
    echo -e "${YELLOW}Checking Multi-Frequency Cognitive Architecture workflow status...${NC}"
    gh run list --workflow=multi-frequency-cognition.yml --limit 5
    
    echo
    echo -e "${GREEN}Latest workflow details:${NC}"
    LATEST_RUN_ID=$(gh run list --workflow=multi-frequency-cognition.yml --limit 1 --json databaseId --jq '.[0].databaseId')
    
    if [ -n "$LATEST_RUN_ID" ]; then
        gh run view $LATEST_RUN_ID
    else
        echo -e "${RED}No workflow runs found.${NC}"
    fi
}

# Function to trigger a cognitive frequency
trigger_cognitive_frequency() {
    local frequency=$1
    local goal=$2
    
    echo -e "${YELLOW}Triggering $frequency frequency with goal: $goal${NC}"
    gh workflow run multi-frequency-cognition.yml --field goal="$goal" --field force_frequency=$frequency
    
    echo -e "${GREEN}Workflow triggered successfully!${NC}"
}

# Main menu
while true; do
    echo
    echo -e "${BLUE}Options:${NC}"
    echo "1. Check workflow status"
    echo "2. Trigger Immediate frequency"
    echo "3. Trigger Continuous frequency"
    echo "4. Trigger Analysis frequency"
    echo "5. Trigger Optimization frequency"
    echo "6. Trigger Evolution frequency"
    echo "0. Exit"
    
    read -p "Enter choice (0-6): " choice
    
    case $choice in
        1)
            check_workflow_status
            ;;
        2)
            read -p "Enter goal for Immediate frequency: " goal
            trigger_cognitive_frequency "immediate" "$goal"
            ;;
        3)
            read -p "Enter goal for Continuous frequency: " goal
            trigger_cognitive_frequency "continuous" "$goal"
            ;;
        4)
            read -p "Enter goal for Analysis frequency: " goal
            trigger_cognitive_frequency "analysis" "$goal"
            ;;
        5)
            read -p "Enter goal for Optimization frequency: " goal
            trigger_cognitive_frequency "optimization" "$goal"
            ;;
        6)
            read -p "Enter goal for Evolution frequency: " goal
            trigger_cognitive_frequency "evolution" "$goal"
            ;;
        0)
            echo -e "${GREEN}Exiting monitor. Goodbye!${NC}"
            exit 0
            ;;
        *)
            echo -e "${RED}Invalid choice. Please try again.${NC}"
            ;;
    esac
done 