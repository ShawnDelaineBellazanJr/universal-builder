#!/bin/bash

# Colors for better output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=====================================${NC}"
echo -e "${BLUE}SK Frequency Router Test${NC}"
echo -e "${BLUE}=====================================${NC}"

# Check for OPENAI_API_KEY
if [ -z "$OPENAI_API_KEY" ]; then
    echo -e "${YELLOW}Warning: OPENAI_API_KEY environment variable is not set${NC}"
    echo -e "Please set it before running the test:"
    echo -e "export OPENAI_API_KEY=your_api_key_here"
    exit 1
fi

# Create test directory if it doesn't exist
TEST_DIR="SkFrequencyRouterTest"
if [ ! -d "$TEST_DIR" ]; then
    echo -e "${BLUE}Creating test directory...${NC}"
    mkdir -p $TEST_DIR
    cd $TEST_DIR
else
    # Clean up existing test directory
    echo -e "${BLUE}Cleaning up test directory...${NC}"
    cd $TEST_DIR
    rm -f *.cs *.csproj *.json Program.cs
fi

# Copy source files
echo -e "${BLUE}Copying source files...${NC}"
cp ../../src/Cognition/SkFrequencyRouter.cs .
cp ../../src/Cognition/SkFrequencyRouterTest.cs .
cp ../../src/Cognition/SkFrequencyRouterMonitor.cs .
cp ../../src/Cognition/SkFrequencyRouterPrompts.cs .

# Create test project
echo -e "${BLUE}Creating test project...${NC}"
dotnet new console

# Create test program
echo -e "${BLUE}Creating test program...${NC}"
cat > Program.cs << 'EOF'
using System;
using System.Threading.Tasks;

namespace SkFrequencyRouterTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Running SK Frequency Router Test...\n");
            
            await SkFrequencyRouterTest.RunTest();
            
            Console.WriteLine("\nTest completed.");
        }
    }
}
EOF

# Add package references
echo -e "${BLUE}Adding package references...${NC}"
dotnet add package Microsoft.SemanticKernel

# Build the test project
echo -e "${BLUE}Building test project...${NC}"
dotnet build

# Run the test
echo -e "${BLUE}Running basic test...${NC}"
dotnet run

# Run comprehensive test
echo -e "${BLUE}Running comprehensive test...${NC}"
cat > ComprehensiveTest.cs << 'EOF'
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public class ComprehensiveTest
{
    public static async Task RunTest()
    {
        Console.WriteLine("Running comprehensive test...");
        
        // Initialize Semantic Kernel
        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(
                modelId: "gpt-3.5-turbo",
                apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
            .Build();
        
        // Initialize frequencies
        var frequencies = new Dictionary<string, FrequencyConfig> {
            { "immediate", new FrequencyConfig { Name = "immediate", Description = "Immediate frequency (0s)", DelaySeconds = 0 } },
            { "continuous", new FrequencyConfig { Name = "continuous", Description = "Continuous frequency (30s)", DelaySeconds = 30 } },
            { "analysis", new FrequencyConfig { Name = "analysis", Description = "Analysis frequency (15m)", DelaySeconds = 900 } },
            { "optimization", new FrequencyConfig { Name = "optimization", Description = "Optimization frequency (2h)", DelaySeconds = 7200 } },
            { "evolution", new FrequencyConfig { Name = "evolution", Description = "Evolution frequency (24h)", DelaySeconds = 86400 } }
        };
        
        // Initialize router and monitor
        var router = new SkFrequencyRouter(kernel, frequencies);
        var monitor = new SkFrequencyRouterMonitor("comprehensive-test-log.json");
        
        // Test cases for each frequency
        var testCases = new Dictionary<string, List<(string goal, string context)>> {
            { "immediate", new List<(string, string)> {
                ("Respond to user question about system status", "User is waiting for an answer"),
                ("Fix a critical bug in the production environment", "System is down due to this bug"),
                ("Process user input and provide a response", "User is interacting with the system now")
            }},
            { "continuous", new List<(string, string)> {
                ("Monitor system performance", "Background process"),
                ("Check for new messages", "Regular checking process"),
                ("Update user interface with new data", "Keeping UI fresh")
            }},
            { "analysis", new List<(string, string)> {
                ("Analyze user behavior patterns", "Understanding usage patterns"),
                ("Review code quality", "Improving code base"),
                ("Generate insights from system data", "Creating value from data")
            }},
            { "optimization", new List<(string, string)> {
                ("Optimize database queries", "Improving system performance"),
                ("Refactor code for better maintainability", "Technical debt reduction"),
                ("Tune algorithm parameters", "Performance enhancement")
            }},
            { "evolution", new List<(string, string)> {
                ("Design new system architecture", "Major system improvement"),
                ("Research and implement new AI capabilities", "Adding new features"),
                ("Evaluate and propose system-wide improvements", "Strategic enhancement")
            }}
        };
        
        var totalTests = 0;
        var correctTests = 0;
        
        foreach (var frequency in testCases.Keys)
        {
            Console.WriteLine($"\nTesting {frequency} frequency cases:");
            
            foreach (var (goal, context) in testCases[frequency])
            {
                totalTests++;
                
                var result = await router.DetermineFrequencyAsync(goal, context);
                var economicValue = await router.CalculateEconomicValueAsync(result.Name, goal, context);
                
                monitor.RecordDecision(goal, context, result.Name, economicValue, result.Name == frequency);
                
                Console.WriteLine($"  Goal: {goal}");
                Console.WriteLine($"  Context: {context}");
                Console.WriteLine($"  Expected: {frequency}");
                Console.WriteLine($"  Actual: {result.Name}");
                Console.WriteLine($"  Economic Value: {economicValue}");
                
                if (result.Name == frequency)
                {
                    correctTests++;
                    Console.WriteLine($"  Result: {GREEN}Correct{NC}");
                }
                else
                {
                    Console.WriteLine($"  Result: {RED}Incorrect{NC}");
                }
                
                Console.WriteLine();
            }
        }
        
        // Generate report
        Console.WriteLine("\nTest Summary:");
        Console.WriteLine($"Total test cases: {totalTests}");
        Console.WriteLine($"Correct classifications: {correctTests}");
        Console.WriteLine($"Accuracy: {(double)correctTests / totalTests * 100:F2}%");
        
        var report = monitor.GenerateReport();
        Console.WriteLine("\nFrequency Distribution:");
        foreach (var freq in report.FrequencyDistribution)
        {
            Console.WriteLine($"  {freq.Key}: {freq.Value} ({(double)freq.Value / totalTests * 100:F2}%)");
        }
        
        Console.WriteLine("\nSaving test log...");
        monitor.SaveLog();
        Console.WriteLine("Comprehensive test completed.");
    }
}
EOF

echo -e "${BLUE}Building comprehensive test...${NC}"
dotnet add package System.Text.Json
dotnet build

echo -e "${BLUE}Running comprehensive test...${NC}"
dotnet run --ComprehensiveTest

# Go back to original directory
cd ..

echo -e "${GREEN}SK Frequency Router tests completed!${NC}"
echo -e "Test results are available in the $TEST_DIR directory."

# Copy the monitor back to src if needed
if [ ! -f "../../src/Cognition/SkFrequencyRouterMonitor.cs" ]; then
    echo -e "${BLUE}Copying SkFrequencyRouterMonitor back to src...${NC}"
    cp $TEST_DIR/SkFrequencyRouterMonitor.cs ../../src/Cognition/
fi

if [ ! -f "../../src/Cognition/SkFrequencyRouterPrompts.cs" ]; then
    echo -e "${BLUE}Copying SkFrequencyRouterPrompts back to src...${NC}"
    cp $TEST_DIR/SkFrequencyRouterPrompts.cs ../../src/Cognition/
fi 