#!/bin/bash

# Colors for better output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=====================================${NC}"
echo -e "${BLUE}Path Update Helper${NC}"
echo -e "${BLUE}=====================================${NC}"
echo -e "This script helps update references to old file paths after the project restructuring."

# Define the path mappings (old path → new path)
declare -A path_mappings=(
    ["run-sk-frequency-router-test.sh"]="tests/sk-frequency-router/run-sk-frequency-router-test.sh"
    ["benchmark-frequency-router.sh"]="tools/benchmarks/benchmark-frequency-router.sh"
    ["run-benchmark-and-visualize.sh"]="tools/benchmarks/run-benchmark-and-visualize.sh"
    ["visualize-benchmark-results.py"]="tools/visualization/visualize-benchmark-results.py"
    ["generate-dashboard.py"]="tools/visualization/generate-dashboard.py"
    ["SK-FREQUENCY-ROUTER-SUMMARY.md"]="docs/components/SK-FREQUENCY-ROUTER-SUMMARY.md"
    ["docs/sk-frequency-router.md"]="docs/components/sk-frequency-router.md"
)

# Function to search for old path references
search_references() {
    local old_path=$1
    local file_types=("*.sh" "*.cs" "*.md" "*.py" "*.yml" "*.json")
    
    echo -e "\n${BLUE}Searching for references to '${old_path}'...${NC}"
    
    for type in "${file_types[@]}"; do
        # Exclude directories that are likely to have binary files or third-party code
        results=$(grep -l --include="${type}" -r "${old_path}" --exclude-dir=".git" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null)
        
        if [ -n "$results" ]; then
            echo -e "${YELLOW}Found references in the following files:${NC}"
            echo "$results" | while read -r file; do
                echo "  - $file"
            done
        fi
    done
}

# Function to update references in a file
update_references() {
    local file=$1
    local old_path=$2
    local new_path=$3
    
    if grep -q "${old_path}" "${file}"; then
        echo -e "  Updating references in ${file}..."
        sed -i "s|${old_path}|${new_path}|g" "${file}"
        echo -e "  ${GREEN}✓${NC} Updated"
    fi
}

# Main functionality
echo -e "\n${BLUE}Step 1: Searching for references to old paths${NC}"
for old_path in "${!path_mappings[@]}"; do
    search_references "${old_path}"
done

echo -e "\n${BLUE}Step 2: Would you like to update these references? (y/n)${NC}"
read -r update_choice

if [[ "$update_choice" =~ ^[Yy]$ ]]; then
    echo -e "\n${BLUE}Updating references...${NC}"
    
    for old_path in "${!path_mappings[@]}"; do
        new_path="${path_mappings[$old_path]}"
        
        echo -e "\n${YELLOW}Updating references to '${old_path}' → '${new_path}'${NC}"
        
        file_types=("*.sh" "*.cs" "*.md" "*.py" "*.yml" "*.json")
        for type in "${file_types[@]}"; do
            files=$(grep -l --include="${type}" -r "${old_path}" --exclude-dir=".git" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null)
            
            if [ -n "$files" ]; then
                echo "$files" | while read -r file; do
                    update_references "${file}" "${old_path}" "${new_path}"
                done
            fi
        done
    done
    
    echo -e "\n${GREEN}References updated successfully!${NC}"
else
    echo -e "\n${YELLOW}No changes made.${NC}"
fi

echo -e "\n${BLUE}Step 3: Creating symbolic links for backwards compatibility? (y/n)${NC}"
read -r symlink_choice

if [[ "$symlink_choice" =~ ^[Yy]$ ]]; then
    echo -e "\n${BLUE}Creating symbolic links...${NC}"
    
    for old_path in "${!path_mappings[@]}"; do
        new_path="${path_mappings[$old_path]}"
        
        # Skip if the old path already exists
        if [ -e "${old_path}" ]; then
            echo -e "  ${YELLOW}Skipping ${old_path} - file already exists${NC}"
            continue
        fi
        
        # Create the directory structure if needed
        old_dir=$(dirname "${old_path}")
        if [ "$old_dir" != "." ] && [ ! -d "$old_dir" ]; then
            mkdir -p "$old_dir"
        fi
        
        # Create the symbolic link
        ln -s "${new_path}" "${old_path}" 2>/dev/null
        
        if [ $? -eq 0 ]; then
            echo -e "  ${GREEN}✓${NC} Created symbolic link: ${old_path} → ${new_path}"
        else
            echo -e "  ${RED}✗${NC} Failed to create symbolic link: ${old_path} → ${new_path}"
        fi
    done
    
    echo -e "\n${GREEN}Symbolic links created successfully!${NC}"
else
    echo -e "\n${YELLOW}No symbolic links created.${NC}"
fi

echo -e "\n${BLUE}Path update process completed.${NC}"
echo -e "For more information about the new project structure, see PROJECT_STRUCTURE.md" 