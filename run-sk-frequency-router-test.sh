#!/bin/bash

# Colors for better output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=====================================${NC}"
echo -e "${BLUE}SK Frequency Router Test Runner${NC}"
echo -e "${BLUE}=====================================${NC}"

# Check for OPENAI_API_KEY
if [ -z "$OPENAI_API_KEY" ]; then
    echo -e "${YELLOW}Warning: OPENAI_API_KEY environment variable is not set${NC}"
    echo -e "Please set it before running the test:"
    echo -e "export OPENAI_API_KEY=your_api_key_here"
    exit 1
fi

# Create test project if it doesn't exist
if [ ! -d "SkFrequencyRouterTest" ]; then
    echo -e "${BLUE}Creating test project...${NC}"
    mkdir -p SkFrequencyRouterTest
    cd SkFrequencyRouterTest
    
    # Initialize .NET project
    dotnet new console
    
    # Add Semantic Kernel package
    dotnet add package Microsoft.SemanticKernel
    
    # Copy the test files
    cp ../src/Cognition/SkFrequencyRouter.cs .
    cp ../src/Cognition/SkFrequencyRouterTest.cs .
    
    # Create Program.cs
    cat > Program.cs << EOT
using System;
using System.Threading.Tasks;
using AutonomousAI.Cognition;

namespace SkFrequencyRouterTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Running SK Frequency Router Test...");
            await SkFrequencyRouterTest.RunTest();
        }
    }
}
EOT
    
    cd ..
else
    echo -e "${BLUE}Test project already exists${NC}"
fi

# Run the test
echo -e "${BLUE}Running SK Frequency Router Test...${NC}"
cd SkFrequencyRouterTest
dotnet run

echo -e "${GREEN}Test complete!${NC}" 