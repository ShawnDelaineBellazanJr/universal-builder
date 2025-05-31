#!/bin/bash

# Test script for AutonomousAI development tools
# This script verifies that all the auto-run tools are working correctly

set -e  # Exit on any error

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}AutonomousAI Development Tools Test${NC}"
echo "======================================"

# Check if .env file exists, create a test one if not
if [ ! -f ".env" ]; then
  echo -e "${YELLOW}Creating test .env file...${NC}"
  echo "OPENAI_API_KEY=test_key" > .env
  echo "AZURE_OPENAI_API_KEY=test_key" >> .env
  echo "AZURE_OPENAI_ENDPOINT=https://test.openai.azure.com/" >> .env
  echo "GITHUB_TOKEN=test_token" >> .env
  echo "DEFAULT_VALUE_THRESHOLD=50" >> .env
fi

# Test if dotnet is installed
if ! command -v dotnet &> /dev/null; then
  echo -e "${RED}❌ .NET SDK not found. Please install .NET SDK 9.0 or later.${NC}"
  exit 1
else
  DOTNET_VERSION=$(dotnet --version)
  echo -e "${GREEN}✅ .NET SDK found: $DOTNET_VERSION${NC}"
fi

# Test if auto-run.sh is executable
if [ ! -x "auto-run.sh" ]; then
  echo -e "${RED}❌ auto-run.sh is not executable. Run 'chmod +x auto-run.sh'.${NC}"
  exit 1
else
  echo -e "${GREEN}✅ auto-run.sh is executable${NC}"
fi

# Test if ci-cd-run.sh is executable
if [ ! -x "ci-cd-run.sh" ]; then
  echo -e "${RED}❌ ci-cd-run.sh is not executable. Run 'chmod +x ci-cd-run.sh'.${NC}"
  exit 1
else
  echo -e "${GREEN}✅ ci-cd-run.sh is executable${NC}"
fi

# Test if Makefile exists
if [ ! -f "Makefile" ]; then
  echo -e "${RED}❌ Makefile not found.${NC}"
  exit 1
else
  echo -e "${GREEN}✅ Makefile exists${NC}"
fi

# Test if Docker is installed (optional)
if ! command -v docker &> /dev/null; then
  echo -e "${YELLOW}⚠️ Docker not found. Docker-based workflows will not be available.${NC}"
else
  DOCKER_VERSION=$(docker --version)
  echo -e "${GREEN}✅ Docker found: $DOCKER_VERSION${NC}"
  
  # Test if Docker Compose is installed
  if ! command -v docker-compose &> /dev/null; then
    echo -e "${YELLOW}⚠️ Docker Compose not found. Docker Compose workflows will not be available.${NC}"
  else
    COMPOSE_VERSION=$(docker-compose --version)
    echo -e "${GREEN}✅ Docker Compose found: $COMPOSE_VERSION${NC}"
  fi
fi

# Test build
echo -e "${BLUE}Testing build...${NC}"
if dotnet build --nologo > /dev/null; then
  echo -e "${GREEN}✅ Build successful${NC}"
else
  echo -e "${RED}❌ Build failed${NC}"
  exit 1
fi

echo ""
echo -e "${GREEN}✅ All development tools are correctly set up!${NC}"
echo ""
echo -e "${BLUE}Next steps:${NC}"
echo "1. Edit the .env file with your actual API keys"
echo "2. Run ./auto-run.sh dev \"Your intent\" 50"
echo "3. For Docker development: make docker-dev"
echo "4. For help with commands: make help"
echo ""
echo -e "${YELLOW}For full documentation, see QUICKSTART.md${NC}" 