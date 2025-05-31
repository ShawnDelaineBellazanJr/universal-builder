#!/bin/bash

# Run SK Evolution Engine script
# This script sets up and runs the SK Evolution Engine

# Colors for better output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check for environment file
if [ ! -f "SkEvolutionEngine/.env" ]; then
    echo -e "${YELLOW}Warning: .env file not found. Creating a basic one...${NC}"
    
    # Create a basic .env file
    cat > SkEvolutionEngine/.env << EOT
# OpenAI Configuration
OPENAI_API_KEY=
OPENAI_ORG_ID=

# GitHub Configuration
GITHUB_TOKEN=

# Logging
LOG_LEVEL=Information
EOT
    echo -e "${GREEN}Created basic .env file. Please edit it with your API keys.${NC}"
    echo -e "${YELLOW}Please edit SkEvolutionEngine/.env to add your API keys before running again.${NC}"
    exit 1
fi

# Check for command line arguments
if [ $# -eq 0 ]; then
    echo -e "${BLUE}Running full evolution cycle...${NC}"
    cd SkEvolutionEngine && dotnet run
else
    echo -e "${BLUE}Running with command: $1${NC}"
    cd SkEvolutionEngine && dotnet run -- "$@"
fi
