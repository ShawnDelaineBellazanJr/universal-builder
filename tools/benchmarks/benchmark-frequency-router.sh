#!/bin/bash

# Colors for better output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=====================================${NC}"
echo -e "${BLUE}SK Frequency Router Benchmark${NC}"
echo -e "${BLUE}=====================================${NC}"

# Check for OPENAI_API_KEY
if [ -z "$OPENAI_API_KEY" ]; then
    echo -e "${YELLOW}Warning: OPENAI_API_KEY environment variable is not set${NC}"
    echo -e "Please set it before running the benchmark:"
    echo -e "export OPENAI_API_KEY=your_api_key_here"
    exit 1
fi

# Create benchmark directory if it doesn't exist
echo -e "${BLUE}Setting up benchmark environment...${NC}"
BENCHMARK_DIR="FrequencyRouterBenchmark"
if [ ! -d "$BENCHMARK_DIR" ]; then
    mkdir -p $BENCHMARK_DIR
fi

cd $BENCHMARK_DIR

# Clean existing files
rm -f *.cs *.csproj *.json Program.cs

# Copy source files
echo -e "${BLUE}Copying source files...${NC}"
cp ../../src/Cognition/SkFrequencyRouter.cs .
cp ../../src/Cognition/SkFrequencyRouterPrompts.cs .
cp ../../src/Cognition/SkFrequencyRouterMonitor.cs .

# Create benchmark project
echo -e "${BLUE}Creating benchmark project...${NC}"
dotnet new console

# Number of test cases to run
TEST_CASES=${1:-20}
echo -e "${BLUE}Will run ${TEST_CASES} test cases${NC}"

# Create benchmark program
echo -e "${BLUE}Creating benchmark program...${NC}"
cat > Program.cs << EOT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Linq;

namespace FrequencyRouterBenchmark
{
    public class FrequencyConfig
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int DelaySeconds { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Running SK Frequency Router Benchmark...\n");
            
            // Parse command line argument for number of test cases
            int testCases = 20; // Default value
            if (args.Length > 0 && int.TryParse(args[0], out int parsedValue))
            {
                testCases = parsedValue;
            }
            
            // Initialize frequencies
            var frequencies = new Dictionary<string, FrequencyConfig> {
                { "immediate", new FrequencyConfig { Name = "immediate", Description = "Immediate frequency (0s)", DelaySeconds = 0 } },
                { "continuous", new FrequencyConfig { Name = "continuous", Description = "Continuous frequency (30s)", DelaySeconds = 30 } },
                { "analysis", new FrequencyConfig { Name = "analysis", Description = "Analysis frequency (15m)", DelaySeconds = 900 } },
                { "optimization", new FrequencyConfig { Name = "optimization", Description = "Optimization frequency (2h)", DelaySeconds = 7200 } },
                { "evolution", new FrequencyConfig { Name = "evolution", Description = "Evolution frequency (24h)", DelaySeconds = 86400 } }
            };
            
            try
            {
                // Initialize Semantic Kernel
                var kernel = Kernel.CreateBuilder()
                    .AddOpenAIChatCompletion(
                        modelId: "gpt-3.5-turbo",
                        apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"))
                    .Build();
                
                // Determine whether to skip SK router for debugging
                bool skipSkRouter = Environment.GetEnvironmentVariable("SKIP_SK_ROUTER") == "true";
                
                // Initialize routers and monitor
                var skRouter = !skipSkRouter ? new SkFrequencyRouter(kernel, frequencies) : null;
                
                // Create keyword-based router function
                var keywordRouter = CreateKeywordRouter(frequencies);
                
                var monitor = new SkFrequencyRouterMonitor("benchmark-results.json");
                
                // Run benchmark
                await RunBenchmark(testCases, skRouter, keywordRouter, monitor);
                
                Console.WriteLine("\nBenchmark completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        static async Task RunBenchmark(
            int testCases, 
            SkFrequencyRouter skRouter,
            Func<string, string, (string frequency, FrequencyConfig config)> keywordRouter,
            SkFrequencyRouterMonitor monitor)
        {
            // Get a mix of sample goals and randomly generated goals
            var sampleGoals = SkFrequencyRouterPrompts.GetSampleGoals();
            var allGoals = new List<(string goal, string context)>();
            
            // Add sample goals first
            foreach (var kvp in sampleGoals)
            {
                foreach (var goal in kvp.Value)
                {
                    allGoals.Add((goal, $"Sample {kvp.Key} goal"));
                }
            }
            
            // Run benchmark
            Console.WriteLine($"Running benchmark with {testCases} test cases...\n");
            
            int agreements = 0;
            int skImmediate = 0, skContinuous = 0, skAnalysis = 0, skOptimization = 0, skEvolution = 0;
            int kwImmediate = 0, kwContinuous = 0, kwAnalysis = 0, kwOptimization = 0, kwEvolution = 0;
            
            var random = new Random();
            var stopwatch = new Stopwatch();
            var disagreements = new List<object>();
            
            for (int i = 0; i < testCases; i++)
            {
                // Choose a goal (from samples or index if we need more)
                var (goal, context) = i < allGoals.Count 
                    ? allGoals[i] 
                    : (GetRandomGoal(), "Random generated goal");
                
                Console.WriteLine($"Test case {i+1}/{testCases}:");
                Console.WriteLine($"Goal: {goal}");
                Console.WriteLine($"Context: {context}");
                
                // Run SK Router
                string skFrequency = "analysis"; // Default
                double skTime = 0;
                int economicValue = 0;
                
                if (skRouter != null)
                {
                    stopwatch.Restart();
                    var result = await skRouter.DetermineFrequencyAsync(goal, context);
                    stopwatch.Stop();
                    skTime = stopwatch.ElapsedTicks;
                    
                    skFrequency = result.Name;
                    economicValue = await skRouter.CalculateEconomicValueAsync(skFrequency, goal, context);
                    
                    Console.WriteLine($"SK Router: {skFrequency} (took {skTime} Ticks)");
                    Console.WriteLine($"Economic Value: {economicValue}/100");
                    
                    // Update counts
                    switch (skFrequency)
                    {
                        case "immediate": skImmediate++; break;
                        case "continuous": skContinuous++; break;
                        case "analysis": skAnalysis++; break;
                        case "optimization": skOptimization++; break;
                        case "evolution": skEvolution++; break;
                    }
                }
                
                // Run Keyword Router
                stopwatch.Restart();
                var (kwFrequency, _) = keywordRouter(goal, context);
                stopwatch.Stop();
                var kwTime = stopwatch.ElapsedTicks;
                
                Console.WriteLine($"Keyword Router: {kwFrequency} (took {kwTime} Ticks)");
                
                // Update counts
                switch (kwFrequency)
                {
                    case "immediate": kwImmediate++; break;
                    case "continuous": kwContinuous++; break;
                    case "analysis": kwAnalysis++; break;
                    case "optimization": kwOptimization++; break;
                    case "evolution": kwEvolution++; break;
                }
                
                // Check agreement
                bool agree = skFrequency == kwFrequency;
                if (agree)
                {
                    agreements++;
                    Console.WriteLine($"Result: {GREEN}AGREE{NC}");
                }
                else
                {
                    Console.WriteLine($"Result: {RED}DISAGREE{NC}");
                    
                    // Save disagreement for analysis
                    disagreements.Add(new {
                        Goal = goal,
                        Context = context,
                        SkFrequency = skFrequency,
                        KwFrequency = kwFrequency,
                        EconomicValue = economicValue
                    });
                }
                
                // Record in monitor
                monitor.RecordDecision(goal, context, skFrequency, economicValue, true, $"{skTime} Ticks");
                
                Console.WriteLine();
            }
            
            // Print summary
            Console.WriteLine("Benchmark Summary:");
            Console.WriteLine($"Total test cases: {testCases}");
            Console.WriteLine($"Agreements: {agreements}/{testCases} ({(double)agreements/testCases*100:F2}%)");
            
            Console.WriteLine("\nSK Router Distribution:");
            Console.WriteLine($"Immediate: {skImmediate} ({(double)skImmediate/testCases*100:F2}%)");
            Console.WriteLine($"Continuous: {skContinuous} ({(double)skContinuous/testCases*100:F2}%)");
            Console.WriteLine($"Analysis: {skAnalysis} ({(double)skAnalysis/testCases*100:F2}%)");
            Console.WriteLine($"Optimization: {skOptimization} ({(double)skOptimization/testCases*100:F2}%)");
            Console.WriteLine($"Evolution: {skEvolution} ({(double)skEvolution/testCases*100:F2}%)");
            
            Console.WriteLine("\nKeyword Router Distribution:");
            Console.WriteLine($"Immediate: {kwImmediate} ({(double)kwImmediate/testCases*100:F2}%)");
            Console.WriteLine($"Continuous: {kwContinuous} ({(double)kwContinuous/testCases*100:F2}%)");
            Console.WriteLine($"Analysis: {kwAnalysis} ({(double)kwAnalysis/testCases*100:F2}%)");
            Console.WriteLine($"Optimization: {kwOptimization} ({(double)kwOptimization/testCases*100:F2}%)");
            Console.WriteLine($"Evolution: {kwEvolution} ({(double)kwEvolution/testCases*100:F2}%)");
            
            if (disagreements.Any())
            {
                Console.WriteLine("\nSample Disagreements:");
                foreach (var d in disagreements.Take(5))
                {
                    Console.WriteLine(JsonSerializer.Serialize(d, new JsonSerializerOptions { WriteIndented = true }));
                }
            }
            
            // Save results
            monitor.SaveLog();
            Console.WriteLine("\nResults saved to benchmark-results.json");
            
            // Save disagreements for analysis
            if (disagreements.Any())
            {
                File.WriteAllText("disagreements.json", 
                    JsonSerializer.Serialize(disagreements, new JsonSerializerOptions { WriteIndented = true }));
                Console.WriteLine("Disagreements saved to disagreements.json");
            }
        }
        
        static Func<string, string, (string frequency, FrequencyConfig config)> CreateKeywordRouter(
            Dictionary<string, FrequencyConfig> frequencies)
        {
            // Create dictionary of keywords for each frequency
            var immediateKeywords = new[] { "urgent", "critical", "emergency", "immediate", "now", "asap", "quickly", "respond", "fix", "broken" };
            var continuousKeywords = new[] { "monitor", "track", "watch", "observe", "maintain", "continuous", "ongoing", "regular", "keep", "update" };
            var analysisKeywords = new[] { "analyze", "research", "study", "investigate", "examine", "review", "assess", "evaluate", "understand", "learn" };
            var optimizationKeywords = new[] { "optimize", "improve", "enhance", "refactor", "efficiency", "performance", "streamline", "tune", "upgrade", "better" };
            var evolutionKeywords = new[] { "evolve", "transform", "innovate", "reimagine", "design", "architect", "future", "vision", "strategy", "long-term" };
            
            // Return router function
            return (goal, context) => {
                goal = goal.ToLower();
                context = context?.ToLower() ?? "";
                string combinedText = $"{goal} {context}";
                
                // Count keyword matches for each frequency
                int immediateCount = immediateKeywords.Count(k => combinedText.Contains(k));
                int continuousCount = continuousKeywords.Count(k => combinedText.Contains(k));
                int analysisCount = analysisKeywords.Count(k => combinedText.Contains(k));
                int optimizationCount = optimizationKeywords.Count(k => combinedText.Contains(k));
                int evolutionCount = evolutionKeywords.Count(k => combinedText.Contains(k));
                
                // Find the frequency with the most keyword matches
                var counts = new Dictionary<string, int> {
                    { "immediate", immediateCount },
                    { "continuous", continuousCount },
                    { "analysis", analysisCount },
                    { "optimization", optimizationCount },
                    { "evolution", evolutionCount }
                };
                
                string maxFrequency = counts.OrderByDescending(kvp => kvp.Value).First().Key;
                
                // If no keywords matched (all counts are 0), default to analysis
                if (counts[maxFrequency] == 0)
                {
                    maxFrequency = "analysis";
                }
                
                return (maxFrequency, frequencies[maxFrequency]);
            };
        }
        
        static string GetRandomGoal()
        {
            var random = new Random();
            var goals = new[]
            {
                "Fix the authentication system",
                "Monitor server performance",
                "Analyze user feedback from the survey",
                "Optimize database queries for the checkout process",
                "Design a new microservices architecture",
                "Respond to customer support tickets",
                "Track system errors in real-time",
                "Research new machine learning techniques",
                "Improve the performance of the search algorithm",
                "Create a vision for the next generation product",
                "Debug the payment processing module",
                "Watch for security vulnerabilities",
                "Study user behavior patterns",
                "Refactor the legacy code base",
                "Develop a roadmap for system evolution",
                "Address critical bugs in production",
                "Maintain system uptime",
                "Evaluate competitor products",
                "Enhance API response times",
                "Transform the user experience architecture"
            };
            
            return goals[random.Next(goals.Length)];
        }
    }
}
EOT

# Add package references
echo -e "${BLUE}Adding package references...${NC}"
dotnet add package Microsoft.SemanticKernel
dotnet add package System.Text.Json

# Build the benchmark project
echo -e "${BLUE}Building benchmark project...${NC}"
dotnet build

# Run the benchmark
echo -e "${BLUE}Running benchmark with ${TEST_CASES} test cases...${NC}"
dotnet run "$TEST_CASES"

# Go back to original directory
cd ..

echo -e "${GREEN}Benchmark completed!${NC}"
echo -e "Benchmark results are available in the ${BENCHMARK_DIR} directory." 