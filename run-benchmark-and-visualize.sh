#!/bin/bash

# Colors for better output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=====================================${NC}"
echo -e "${BLUE}SK Frequency Router Benchmark & Visualization${NC}"
echo -e "${BLUE}=====================================${NC}"

# Check for OPENAI_API_KEY
if [ -z "$OPENAI_API_KEY" ]; then
    echo -e "${YELLOW}Warning: OPENAI_API_KEY environment variable is not set${NC}"
    echo -e "Please set it before running the benchmark:"
    echo -e "export OPENAI_API_KEY=your_api_key_here"
    exit 1
fi

# Check for python and required packages
if ! command -v python3 &> /dev/null; then
    echo -e "${RED}Error: python3 is required but not installed.${NC}"
    exit 1
fi

# Check for matplotlib and numpy
REQUIRED_PACKAGES=("matplotlib" "numpy")
MISSING_PACKAGES=()

for package in "${REQUIRED_PACKAGES[@]}"; do
    if ! python3 -c "import $package" &> /dev/null; then
        MISSING_PACKAGES+=("$package")
    fi
done

if [ ${#MISSING_PACKAGES[@]} -ne 0 ]; then
    echo -e "${YELLOW}Warning: The following Python packages are required but not installed:${NC}"
    for package in "${MISSING_PACKAGES[@]}"; do
        echo "  - $package"
    done
    echo -e "Would you like to install them now? (y/n)"
    read -r install_packages
    
    if [[ "$install_packages" =~ ^[Yy]$ ]]; then
        echo -e "${BLUE}Installing required packages...${NC}"
        python3 -m pip install "${MISSING_PACKAGES[@]}"
    else
        echo -e "${RED}Cannot continue without required packages.${NC}"
        exit 1
    fi
fi

# Make scripts executable
chmod +x benchmark-frequency-router.sh
chmod +x visualize-benchmark-results.py

# Number of test cases to run
TEST_CASES=${1:-20}
echo -e "${BLUE}Running benchmark with ${TEST_CASES} test cases...${NC}"

# Run the benchmark
./benchmark-frequency-router.sh "$TEST_CASES"

# Check if benchmark results file exists
RESULTS_FILE="FrequencyRouterBenchmark/benchmark-results.json"
if [ ! -f "$RESULTS_FILE" ]; then
    echo -e "${RED}Error: Benchmark results file not found at ${RESULTS_FILE}${NC}"
    exit 1
fi

# Run the visualization script
echo -e "${BLUE}Generating visualizations from benchmark results...${NC}"
python3 visualize-benchmark-results.py "$RESULTS_FILE"

echo -e "${GREEN}Benchmark and visualization complete!${NC}"
echo -e "Review the results in the benchmark_visualizations_* directory." 