using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;

namespace AutonomousAI.Cognition
{
    /// <summary>
    /// Test class for the SK Frequency Router
    /// </summary>
    public class SkFrequencyRouterTest
    {
        public static async Task RunTest()
        {
            Console.WriteLine("=== SK Frequency Router Test ===");
            
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
                
                // Test with different goals
                await TestGoal(router, "Server is down, need immediate response", "Critical production issue");
                await TestGoal(router, "Monitor system performance", "Background task");
                await TestGoal(router, "Analyze user feedback for product improvements", "Monthly review");
                await TestGoal(router, "Refactor authentication module", "Technical debt");
                await TestGoal(router, "Design a new architecture for scaling", "Long-term planning");
                
                Console.WriteLine("SK Frequency Router Test completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        private static async Task TestGoal(SkFrequencyRouter router, string goal, string context)
        {
            Console.WriteLine($"\nTesting goal: \"{goal}\" (Context: {context})");
            
            // Determine the appropriate frequency
            var (frequencyName, config) = await router.DetermineFrequencyAsync(goal, context);
            Console.WriteLine($"Recommended frequency: {frequencyName} (Interval: {config.Interval}, Threshold: {config.Threshold})");
            
            // Calculate economic value
            int economicValue = await router.CalculateEconomicValueAsync(goal, frequencyName);
            Console.WriteLine($"Economic value: {economicValue}/100");
            
            // Check if the economic value meets the threshold
            bool meetsThreshold = economicValue >= config.Threshold;
            Console.WriteLine($"Meets economic threshold: {meetsThreshold}");
        }
    }
} 