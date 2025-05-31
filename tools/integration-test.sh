#!/bin/bash

# Colors for better output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=====================================${NC}"
echo -e "${BLUE}Integration Test for Project Structure${NC}"
echo -e "${BLUE}=====================================${NC}"

# Check for required files in the new structure
check_file() {
    local file_path=$1
    local description=$2
    
    if [ -f "$file_path" ]; then
        echo -e "${GREEN}✓${NC} Found $description at $file_path"
        return 0
    else
        echo -e "${RED}✗${NC} Missing $description at $file_path"
        return 1
    fi
}

check_directory() {
    local dir_path=$1
    local description=$2
    
    if [ -d "$dir_path" ]; then
        echo -e "${GREEN}✓${NC} Found $description directory at $dir_path"
        return 0
    else
        echo -e "${RED}✗${NC} Missing $description directory at $dir_path"
        return 1
    fi
}

# Check main directories
echo -e "\n${BLUE}Checking main directories...${NC}"
main_dirs_ok=true
for dir in "src" "docs" "tools" "tests"; do
    if ! check_directory "$dir" "$dir"; then
        main_dirs_ok=false
    fi
done

# Check SK Frequency Router files
echo -e "\n${BLUE}Checking SK Frequency Router files...${NC}"
sk_router_ok=true
if ! check_file "src/Cognition/SkFrequencyRouter.cs" "SK Frequency Router"; then
    sk_router_ok=false
fi
if ! check_file "src/Cognition/SkFrequencyRouterPrompts.cs" "SK Frequency Router Prompts"; then
    sk_router_ok=false
fi
if ! check_file "src/Cognition/SkFrequencyRouterMonitor.cs" "SK Frequency Router Monitor"; then
    sk_router_ok=false
fi
if ! check_file "docs/components/sk-frequency-router.md" "SK Frequency Router documentation"; then
    sk_router_ok=false
fi

# Check tool scripts
echo -e "\n${BLUE}Checking tool scripts...${NC}"
tools_ok=true
if ! check_file "tools/benchmarks/benchmark-frequency-router.sh" "benchmark script"; then
    tools_ok=false
fi
if ! check_file "tools/benchmarks/run-benchmark-and-visualize.sh" "benchmark and visualization script"; then
    tools_ok=false
fi
if ! check_file "tools/visualization/generate-dashboard.py" "dashboard generator"; then
    tools_ok=false
fi
if ! check_file "tools/visualization/visualize-benchmark-results.py" "visualization script"; then
    tools_ok=false
fi

# Check test scripts
echo -e "\n${BLUE}Checking test scripts...${NC}"
tests_ok=true
if ! check_file "tests/sk-frequency-router/run-sk-frequency-router-test.sh" "SK Frequency Router test"; then
    tests_ok=false
fi

# Test running the benchmark script (without actually running it)
echo -e "\n${BLUE}Testing if benchmark script can be executed...${NC}"
chmod +x tools/benchmarks/benchmark-frequency-router.sh
if [ -x "tools/benchmarks/benchmark-frequency-router.sh" ]; then
    echo -e "${GREEN}✓${NC} Benchmark script is executable"
else
    echo -e "${RED}✗${NC} Benchmark script is not executable"
    tools_ok=false
fi

# Test running the test script (without actually running it)
echo -e "\n${BLUE}Testing if test script can be executed...${NC}"
chmod +x tests/sk-frequency-router/run-sk-frequency-router-test.sh
if [ -x "tests/sk-frequency-router/run-sk-frequency-router-test.sh" ]; then
    echo -e "${GREEN}✓${NC} Test script is executable"
else
    echo -e "${RED}✗${NC} Test script is not executable"
    tests_ok=false
fi

# Summary
echo -e "\n${BLUE}Integration Test Summary${NC}"
all_ok=true

if $main_dirs_ok; then
    echo -e "${GREEN}✓${NC} Main directories check passed"
else
    echo -e "${RED}✗${NC} Main directories check failed"
    all_ok=false
fi

if $sk_router_ok; then
    echo -e "${GREEN}✓${NC} SK Frequency Router files check passed"
else
    echo -e "${RED}✗${NC} SK Frequency Router files check failed"
    all_ok=false
fi

if $tools_ok; then
    echo -e "${GREEN}✓${NC} Tools check passed"
else
    echo -e "${RED}✗${NC} Tools check failed"
    all_ok=false
fi

if $tests_ok; then
    echo -e "${GREEN}✓${NC} Tests check passed"
else
    echo -e "${RED}✗${NC} Tests check failed"
    all_ok=false
fi

# Final result
echo -e "\n${BLUE}Final Result${NC}"
if $all_ok; then
    echo -e "${GREEN}✓ Integration test passed. The new project structure is valid.${NC}"
    exit 0
else
    echo -e "${RED}✗ Integration test failed. Please fix the issues above.${NC}"
    exit 1
fi 