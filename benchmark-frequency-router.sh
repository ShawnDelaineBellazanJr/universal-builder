#!/bin/bash

# Colors for better output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=====================================${NC}"
echo -e "${BLUE}Frequency Router Benchmark Tool${NC}"
echo -e "${BLUE}=====================================${NC}"

# Check for OPENAI_API_KEY
if [ -z "$OPENAI_API_KEY" ]; then
    echo -e "${YELLOW}Warning: OPENAI_API_KEY environment variable is not set${NC}"
    echo -e "Please set it before running the benchmark:"
    echo -e "export OPENAI_API_KEY=your_api_key_here"
    exit 1
fi

# Create benchmark project if it doesn't exist
if [ ! -d "FrequencyRouterBenchmark" ]; then
    echo -e "${BLUE}Creating benchmark project...${NC}"
    mkdir -p FrequencyRouterBenchmark
    cd FrequencyRouterBenchmark
    
    # Initialize .NET project
    dotnet new console
    
    # Add Semantic Kernel package
    dotnet add package Microsoft.SemanticKernel
    
    # Copy the required files
    cp ../src/Cognition/SkFrequencyRouter.cs .
    cp ../src/Cognition/SkFrequencyRouterPrompts.cs .
    cp ../src/Cognition/SkFrequencyRouterMonitor.cs .
    
    # Create Program.cs for the benchmark
    cat > Program.cs << EOT
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using AutonomousAI.Cognition;
using UniversalAutonomousBuilder.Cognition;

namespace FrequencyRouterBenchmark
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Frequency Router Benchmark ===");
            
            // Parse command line arguments
            bool skipKeywordRouter = args.Length > 0 && args[0] == "--sk-only";
            bool skipSkRouter = args.Length > 0 && args[0] == "--keyword-only";
            int testCases = args.Length > 1 ? int.Parse(args[1]) : 50;
            
            Console.WriteLine($"Running benchmark with {testCases} test cases");
            Console.WriteLine($"SK Router: {!skipSkRouter}");
            Console.WriteLine($"Keyword Router: {!skipKeywordRouter}");
            
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
                
                // Create routers
                var skRouter = !skipSkRouter ? new SkFrequencyRouter(kernel, frequencies) : null;
                var keywordRouter = !skipKeywordRouter ? new KeywordBasedRouter() : null;
                
                // Create a monitor to record results
                var monitor = new SkFrequencyRouterMonitor("benchmark-results.json");
                
                // Generate test cases from sample goals
                var testGoals = GenerateTestCases(testCases);
                
                // Run benchmark
                var results = await RunBenchmark(testGoals, skRouter, keywordRouter, monitor);
                
                // Print results
                PrintResults(results);
                
                // Save results
                monitor.SaveLog();
                
                Console.WriteLine("\nBenchmark completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in benchmark: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        static List<(string Goal, string Context)> GenerateTestCases(int count)
        {
            var sampleGoals = SkFrequencyRouterPrompts.GetSampleGoals();
            var random = new Random();
            var allGoals = new List<string>();
            var allContexts = new List<string>
            {
                "Production environment", "Development", "Testing", "High-priority", 
                "Low-priority", "User-facing", "Internal system", "Background task",
                "High-traffic service", "Legacy system", "New feature", "Bug fix",
                "Performance issue", "Security concern", "Data integrity", "UI/UX",
                "Backend service", "API endpoint", "Database", "Authentication",
                "Authorization", "Networking", "Caching", "Logging", "Monitoring"
            };
            
            // Collect all sample goals
            foreach (var kvp in sampleGoals)
            {
                allGoals.AddRange(kvp.Value);
            }
            
            // Generate test cases
            var testCases = new List<(string, string)>();
            for (int i = 0; i < count; i++)
            {
                string goal = allGoals[random.Next(allGoals.Count)];
                string context = allContexts[random.Next(allContexts.Count)];
                testCases.Add((goal, context));
            }
            
            return testCases;
        }
        
        static async Task<BenchmarkResults> RunBenchmark(
            List<(string Goal, string Context)> testCases,
            SkFrequencyRouter skRouter,
            KeywordBasedRouter keywordRouter,
            SkFrequencyRouterMonitor monitor)
        {
            var results = new BenchmarkResults();
            var stopwatch = new Stopwatch();
            
            Console.WriteLine("\nRunning benchmark...");
            
            foreach (var (goal, context) in testCases)
            {
                Console.Write(".");
                
                // Test SK router
                if (skRouter != null)
                {
                    stopwatch.Restart();
                    var (skFrequency, _) = await skRouter.DetermineFrequencyAsync(goal, context);
                    int skEconomicValue = await skRouter.CalculateEconomicValueAsync(goal, skFrequency);
                    stopwatch.Stop();
                    
                    results.SkRouterResults.Add(new RoutingResult
                    {
                        Goal = goal,
                        Context = context,
                        Frequency = skFrequency,
                        EconomicValue = skEconomicValue,
                        ProcessingTime = stopwatch.Elapsed
                    });
                    
                    // Record in monitor
                    monitor.RecordDecision(goal, context, skFrequency, skEconomicValue, true, stopwatch.Elapsed);
                }
                
                // Test keyword router
                if (keywordRouter != null)
                {
                    stopwatch.Restart();
                    string keywordFrequency = keywordRouter.DetermineFrequency(goal);
                    int keywordEconomicValue = keywordRouter.CalculateEconomicValue(goal, keywordFrequency);
                    stopwatch.Stop();
                    
                    results.KeywordRouterResults.Add(new RoutingResult
                    {
                        Goal = goal,
                        Context = context,
                        Frequency = keywordFrequency,
                        EconomicValue = keywordEconomicValue,
                        ProcessingTime = stopwatch.Elapsed
                    });
                }
            }
            
            Console.WriteLine("\nCompleted benchmark runs");
            
            // Calculate agreement rate
            if (skRouter != null && keywordRouter != null)
            {
                int agreements = 0;
                for (int i = 0; i < testCases.Count; i++)
                {
                    string skFrequency = results.SkRouterResults[i].Frequency;
                    string keywordFrequency = results.KeywordRouterResults[i].Frequency;
                    
                    if (skFrequency == keywordFrequency)
                    {
                        agreements++;
                    }
                }
                
                results.AgreementRate = (double)agreements / testCases.Count;
            }
            
            return results;
        }
        
        static void PrintResults(BenchmarkResults results)
        {
            Console.WriteLine("\n=== Benchmark Results ===");
            
            // Print SK router results if available
            if (results.SkRouterResults.Any())
            {
                var skResults = results.SkRouterResults;
                double avgTime = skResults.Average(r => r.ProcessingTime.TotalMilliseconds);
                var freqDist = skResults.GroupBy(r => r.Frequency)
                    .Select(g => new { Frequency = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count);
                
                Console.WriteLine("\nSK Frequency Router Results:");
                Console.WriteLine($"  Average processing time: {avgTime:F2} ms");
                Console.WriteLine("  Frequency distribution:");
                foreach (var freq in freqDist)
                {
                    Console.WriteLine($"    {freq.Frequency}: {freq.Count} ({(double)freq.Count / skResults.Count:P0})");
                }
            }
            
            // Print keyword router results if available
            if (results.KeywordRouterResults.Any())
            {
                var kwResults = results.KeywordRouterResults;
                double avgTime = kwResults.Average(r => r.ProcessingTime.TotalMilliseconds);
                var freqDist = kwResults.GroupBy(r => r.Frequency)
                    .Select(g => new { Frequency = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count);
                
                Console.WriteLine("\nKeyword-based Router Results:");
                Console.WriteLine($"  Average processing time: {avgTime:F2} ms");
                Console.WriteLine("  Frequency distribution:");
                foreach (var freq in freqDist)
                {
                    Console.WriteLine($"    {freq.Frequency}: {freq.Count} ({(double)freq.Count / kwResults.Count:P0})");
                }
            }
            
            // Print comparison if both routers were tested
            if (results.SkRouterResults.Any() && results.KeywordRouterResults.Any())
            {
                Console.WriteLine($"\nAgreement rate: {results.AgreementRate:P2}");
                
                // Find disagreements
                Console.WriteLine("\nSample disagreements:");
                int sampleCount = 0;
                for (int i = 0; i < Math.Min(results.SkRouterResults.Count, results.KeywordRouterResults.Count); i++)
                {
                    var skResult = results.SkRouterResults[i];
                    var kwResult = results.KeywordRouterResults[i];
                    
                    if (skResult.Frequency != kwResult.Frequency && sampleCount < 5)
                    {
                        Console.WriteLine($"  Goal: \"{skResult.Goal}\"");
                        Console.WriteLine($"    SK Router: {skResult.Frequency} (Economic value: {skResult.EconomicValue})");
                        Console.WriteLine($"    Keyword Router: {kwResult.Frequency} (Economic value: {kwResult.EconomicValue})");
                        Console.WriteLine();
                        sampleCount++;
                    }
                }
            }
        }
    }
    
    class KeywordBasedRouter
    {
        public string DetermineFrequency(string goal)
        {
            goal = goal.ToLower();
            
            // Check for immediate frequency keywords
            string[] immediateKeywords = {
                "emergency", "critical", "urgent", "immediate", "security", "vulnerability",
                "breach", "fix", "broken", "down", "crash", "failure", "outage"
            };
            
            // Check for continuous frequency keywords
            string[] continuousKeywords = {
                "monitor", "health", "status", "check", "watch", "observe", "track",
                "scan", "survey", "inspect", "diagnose"
            };
            
            // Check for optimization frequency keywords
            string[] optimizationKeywords = {
                "optimize", "improve", "enhance", "refine", "upgrade", "boost", "streamline",
                "efficiency", "performance", "speed", "quality", "reliability"
            };
            
            // Check for evolution frequency keywords
            string[] evolutionKeywords = {
                "evolve", "architecture", "transform", "redesign", "restructure", "reinvent",
                "revolution", "paradigm", "foundation", "fundamental", "meta-evolution"
            };
            
            // Determine frequency based on keywords
            if (ContainsAnyKeyword(goal, immediateKeywords))
                return "immediate";
            
            if (ContainsAnyKeyword(goal, evolutionKeywords))
                return "evolution";
            
            if (ContainsAnyKeyword(goal, optimizationKeywords))
                return "optimization";
            
            if (ContainsAnyKeyword(goal, continuousKeywords))
                return "continuous";
            
            // Default to analysis
            return "analysis";
        }
        
        public int CalculateEconomicValue(string goal, string frequency)
        {
            goal = goal.ToLower();
            
            // Simple economic value calculation based on keyword matching
            int baseValue = 70; // Default base value
            
            // Adjust based on goal length (proxy for complexity)
            int lengthAdjustment = Math.Min(goal.Length / 5, 10);
            
            // Adjust based on keywords
            int keywordAdjustment = 0;
            
            // High-value keywords
            string[] highValueKeywords = { "critical", "urgent", "security", "failure", "outage" };
            
            // Medium-value keywords
            string[] mediumValueKeywords = { "improve", "optimize", "performance", "efficiency" };
            
            // Adjust based on keyword presence
            foreach (var keyword in highValueKeywords)
            {
                if (goal.Contains(keyword))
                    keywordAdjustment += 5;
            }
            
            foreach (var keyword in mediumValueKeywords)
            {
                if (goal.Contains(keyword))
                    keywordAdjustment += 3;
            }
            
            // Adjust based on frequency alignment
            int frequencyAdjustment = 0;
            switch (frequency)
            {
                case "immediate":
                    if (ContainsAnyKeyword(goal, new[] { "critical", "urgent", "emergency" }))
                        frequencyAdjustment = 15;
                    break;
                case "continuous":
                    if (ContainsAnyKeyword(goal, new[] { "monitor", "track", "observe" }))
                        frequencyAdjustment = 10;
                    break;
                case "analysis":
                    if (ContainsAnyKeyword(goal, new[] { "analyze", "research", "investigate" }))
                        frequencyAdjustment = 10;
                    break;
                case "optimization":
                    if (ContainsAnyKeyword(goal, new[] { "improve", "optimize", "enhance" }))
                        frequencyAdjustment = 10;
                    break;
                case "evolution":
                    if (ContainsAnyKeyword(goal, new[] { "design", "architecture", "develop" }))
                        frequencyAdjustment = 15;
                    break;
            }
            
            // Calculate final value
            int economicValue = baseValue + lengthAdjustment + keywordAdjustment + frequencyAdjustment;
            
            // Ensure value is between 0 and 100
            return Math.Clamp(economicValue, 0, 100);
        }
        
        private bool ContainsAnyKeyword(string text, string[] keywords)
        {
            foreach (var keyword in keywords)
            {
                if (text.Contains(keyword))
                    return true;
            }
            
            return false;
        }
    }
    
    class RoutingResult
    {
        public string Goal { get; set; }
        public string Context { get; set; }
        public string Frequency { get; set; }
        public int EconomicValue { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }
    
    class BenchmarkResults
    {
        public List<RoutingResult> SkRouterResults { get; } = new List<RoutingResult>();
        public List<RoutingResult> KeywordRouterResults { get; } = new List<RoutingResult>();
        public double AgreementRate { get; set; }
    }
}
EOT
    
    cd ..
else
    echo -e "${BLUE}Benchmark project already exists${NC}"
    # Make sure we have the latest files
    cp src/Cognition/SkFrequencyRouter.cs FrequencyRouterBenchmark/
    cp src/Cognition/SkFrequencyRouterPrompts.cs FrequencyRouterBenchmark/
    cp src/Cognition/SkFrequencyRouterMonitor.cs FrequencyRouterBenchmark/
fi

# Run the benchmark
echo -e "${BLUE}Running benchmark...${NC}"
cd FrequencyRouterBenchmark

# Default to 20 test cases unless specified
TEST_CASES=${1:-20}

dotnet run -- $TEST_CASES

echo -e "${GREEN}Benchmark complete!${NC}"

# Display results
if [ -f "benchmark-results.json" ]; then
    echo -e "${BLUE}Benchmark results saved to benchmark-results.json${NC}"
fi 