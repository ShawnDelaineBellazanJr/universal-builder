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
    cp ../src/Cognition/SkFrequencyRouterMonitor.cs .
    
    # Create Program.cs
    cat > Program.cs << EOT
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutonomousAI.Cognition;
using UniversalAutonomousBuilder.Cognition;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;

namespace SkFrequencyRouterTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Running SK Frequency Router Test...");
            
            // Run the basic test
            await SkFrequencyRouterTest.RunTest();
            
            // Run the comprehensive test with monitoring
            await RunComprehensiveTest();
        }
        
        static async Task RunComprehensiveTest()
        {
            Console.WriteLine("\n\n=== Comprehensive SK Frequency Router Test with Monitoring ===");
            
            try
            {
                // Initialize Semantic Kernel
                string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                {
                    Console.WriteLine("OPENAI_API_KEY environment variable is not set");
                    return;
                }
                
                var builder = Kernel.CreateBuilder();
                builder.AddOpenAIChatCompletion("gpt-4", apiKey);
                builder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Information));
                var kernel = builder.Build();
                
                // Create frequency configurations
                var frequencies = new Dictionary<string, FrequencyConfig>
                {
                    ["immediate"] = new FrequencyConfig { Interval = "0s", Threshold = 95 },
                    ["continuous"] = new FrequencyConfig { Interval = "30s", Threshold = 50 },
                    ["analysis"] = new FrequencyConfig { Interval = "15m", Threshold = 70 },
                    ["optimization"] = new FrequencyConfig { Interval = "2h", Threshold = 85 },
                    ["evolution"] = new FrequencyConfig { Interval = "24h", Threshold = 90 }
                };
                
                // Create the router
                var router = new SkFrequencyRouter(kernel, frequencies);
                
                // Create the monitor
                var monitor = new SkFrequencyRouterMonitor("comprehensive-test-log.json");
                
                // Define test scenarios
                var testScenarios = new[]
                {
                    // Immediate scenarios
                    ("Critical system failure detected in production", "Emergency"),
                    ("Security breach detected in authentication system", "Urgent"),
                    ("Database server is down", "Critical"),
                    
                    // Continuous scenarios
                    ("Monitor CPU usage across servers", "Ongoing"),
                    ("Check for new user registrations", "Regular"),
                    ("Watch for API response time changes", "Background"),
                    
                    // Analysis scenarios
                    ("Evaluate customer feedback for product improvements", "Research"),
                    ("Analyze performance data from last quarter", "Data analysis"),
                    ("Review competitor product features", "Strategy"),
                    
                    // Optimization scenarios
                    ("Improve database query performance", "Technical debt"),
                    ("Refactor authentication module", "Code quality"),
                    ("Optimize image processing pipeline", "Performance"),
                    
                    // Evolution scenarios
                    ("Design a new microservices architecture", "Long-term"),
                    ("Develop AI-powered recommendation engine", "Innovation"),
                    ("Create next generation user interface", "Vision")
                };
                
                // Run tests and record results
                foreach (var (goal, context) in testScenarios)
                {
                    Console.WriteLine($"\nTesting scenario: \"{goal}\" (Context: {context})");
                    
                    // Determine frequency
                    var (frequencyName, config) = await router.DetermineFrequencyAsync(goal, context);
                    Console.WriteLine($"Recommended frequency: {frequencyName}");
                    
                    // Calculate economic value
                    int economicValue = await router.CalculateEconomicValueAsync(goal, frequencyName);
                    Console.WriteLine($"Economic value: {economicValue}/100");
                    
                    // Check if meets threshold
                    bool meetsThreshold = economicValue >= config.Threshold;
                    Console.WriteLine($"Meets threshold: {meetsThreshold}");
                    
                    // Simulate execution time (for testing purposes)
                    var executionTime = SimulateExecutionTime(frequencyName);
                    
                    // Record in monitor (simulate success as true for 80% of cases)
                    bool success = new Random().NextDouble() < 0.8;
                    monitor.RecordDecision(goal, context, frequencyName, economicValue, success, executionTime);
                }
                
                // Generate and display report
                var report = monitor.GenerateReport();
                Console.WriteLine("\n" + report.ToString());
                
                Console.WriteLine("\nComprehensive test completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in comprehensive test: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        static TimeSpan SimulateExecutionTime(string frequency)
        {
            var random = new Random();
            switch (frequency.ToLower())
            {
                case "immediate": return TimeSpan.FromMilliseconds(random.Next(50, 500));
                case "continuous": return TimeSpan.FromSeconds(random.Next(1, 30));
                case "analysis": return TimeSpan.FromMinutes(random.Next(1, 15));
                case "optimization": return TimeSpan.FromMinutes(random.Next(15, 120));
                case "evolution": return TimeSpan.FromHours(random.Next(1, 24));
                default: return TimeSpan.FromMinutes(random.Next(1, 60));
            }
        }
    }
}
EOT
    
    cd ..
else
    echo -e "${BLUE}Test project already exists${NC}"
    # Make sure we have the latest monitor file
    cp src/Cognition/SkFrequencyRouterMonitor.cs SkFrequencyRouterTest/
fi

# Run the test
echo -e "${BLUE}Running SK Frequency Router Test...${NC}"
cd SkFrequencyRouterTest
dotnet run

echo -e "${GREEN}Test complete!${NC}"

# Display results
if [ -f "comprehensive-test-log.json" ]; then
    echo -e "${BLUE}Test results saved to comprehensive-test-log.json${NC}"
fi 