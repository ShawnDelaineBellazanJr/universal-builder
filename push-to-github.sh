#!/bin/bash

# Push to GitHub script for AutonomousAI
# This script helps push all changes to GitHub

set -e  # Exit on any error

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}AutonomousAI GitHub Push Tool${NC}"
echo "=============================="

# Check if git is installed
if ! command -v git &> /dev/null; then
  echo -e "${RED}❌ Git not found. Please install Git.${NC}"
  exit 1
fi

# Check if we're in a git repository
if ! git rev-parse --is-inside-work-tree &> /dev/null; then
  echo -e "${RED}❌ Not in a Git repository.${NC}"
  exit 1
fi

# Check if there are uncommitted changes
if ! git diff-index --quiet HEAD --; then
  echo -e "${YELLOW}⚠️ You have uncommitted changes.${NC}"
  git status
  
  read -p "Would you like to commit these changes? (y/n) " -n 1 -r
  echo
  if [[ $REPLY =~ ^[Yy]$ ]]; then
    read -p "Enter commit message: " commit_message
    git add .
    git commit -m "$commit_message"
    echo -e "${GREEN}✅ Changes committed.${NC}"
  else
    echo -e "${YELLOW}⚠️ Continuing without committing changes.${NC}"
  fi
fi

# Check remote repository
if ! git remote get-url origin &> /dev/null; then
  echo -e "${YELLOW}⚠️ No remote repository set.${NC}"
  read -p "Enter GitHub repository URL: " repo_url
  git remote add origin $repo_url
  echo -e "${GREEN}✅ Remote repository set.${NC}"
fi

# Check current branch
current_branch=$(git symbolic-ref --short HEAD)
echo -e "${BLUE}Current branch: $current_branch${NC}"

# Push to GitHub
echo -e "${BLUE}Pushing to GitHub...${NC}"
if git push -u origin $current_branch; then
  echo -e "${GREEN}✅ Successfully pushed to GitHub!${NC}"
else
  echo -e "${RED}❌ Failed to push to GitHub.${NC}"
  echo "Try running: git push -u origin $current_branch --force"
  exit 1
fi

# Show repository URL
repo_url=$(git remote get-url origin)
echo -e "${GREEN}✅ Code is now available at: ${BLUE}$repo_url${NC}"

# Print next steps
echo ""
echo -e "${BLUE}Next steps:${NC}"
echo "1. Set up GitHub Actions secrets for workflow"
echo "   - OPENAI_API_KEY"
echo "   - GITHUB_TOKEN (with repo, workflow, and gist permissions)"
echo "   - DEFAULT_VALUE_THRESHOLD (optional)"
echo "2. Trigger the workflow manually or create an issue"
echo "3. Check the Actions tab for workflow runs"
echo ""
echo -e "${YELLOW}For more information, see README.md and QUICKSTART.md${NC}" 