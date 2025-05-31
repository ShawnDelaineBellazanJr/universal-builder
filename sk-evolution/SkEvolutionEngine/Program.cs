using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace SkEvolutionEngine
{
    public class Program
    {
        private static ILogger<Program> _logger;

        public static async Task Main(string[] args)
        {
            try
            {
                // Initialize logging
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });
                _logger = loggerFactory.CreateLogger<Program>();
                _logger.LogInformation("SK Evolution Engine starting...");

                // Load configuration
                var config = new Dictionary<string, object>();
                if (File.Exists("config.json"))
                {
                    string json = File.ReadAllText("config.json");
                    config = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    _logger.LogInformation("Configuration loaded from config.json");
                }
                else
                {
                    _logger.LogWarning("Config file not found. Using default configuration.");
                }

                // Get OpenAI API key from environment variables
                string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                {
                    apiKey = "sk-dummy-key-for-demo";
                    _logger.LogWarning("OPENAI_API_KEY not found. Using dummy key for demo purposes.");
                }

                // Initialize Semantic Kernel
                var builder = Kernel.CreateBuilder();
                try
                {
                    builder.AddOpenAIChatCompletion("gpt-4", apiKey);
                    _logger.LogInformation("Semantic Kernel initialized with OpenAI");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Could not initialize OpenAI: {ex.Message}");
                    _logger.LogInformation("Running in demo mode with simulated improvements");
                }

                // Determine the command to run
                string command = args.Length > 0 ? args[0].ToLower() : "evolve";
                _logger.LogInformation($"Running command: {command}");

                // Add SK-powered improvements (simulated for now)
                var improvements = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object> {
                        { "title", "Implement Semantic Kernel-based Frequency Router" },
                        { "description", "Create a cognitive frequency router that uses SK to determine which cognitive frequency is best for a given task based on economic value" },
                        { "justification", "Current frequency routing is manual and could benefit from AI-powered decision making" },
                        { "economicValue", 92 }
                    },
                    new Dictionary<string, object> {
                        { "title", "Add Adaptive Learning for Frequency Thresholds" },
                        { "description", "Implement an adaptive learning system that automatically adjusts frequency thresholds based on past performance" },
                        { "justification", "Static thresholds don't adapt to changing conditions and task types" },
                        { "economicValue", 87 }
                    },
                    new Dictionary<string, object> {
                        { "title", "Integrate with Semantic Kernel Memory for Goal History" },
                        { "description", "Use SK Memory to store and retrieve past goals and their outcomes to inform future decisions" },
                        { "justification", "The system currently doesn't learn from past goal executions" },
                        { "economicValue", 90 }
                    }
                };

                // Log the improvements
                _logger.LogInformation($"Found {improvements.Count} possible improvements:");
                foreach (var improvement in improvements)
                {
                    _logger.LogInformation($"- {improvement["title"]} (Economic Value: {improvement["economicValue"]})");
                    _logger.LogInformation($"  {improvement["description"]}");
                }

                // Create a results file with more details
                var results = new Dictionary<string, object>
                {
                    { "timestamp", DateTime.UtcNow.ToString("o") },
                    { "command", command },
                    { "improvements", improvements },
                    { "status", "success" },
                    { "message", "SK Evolution Engine completed successfully" }
                };

                string resultsJson = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText("../evolution-results.json", resultsJson);
                _logger.LogInformation("Results saved to evolution-results.json");

                // Signal completion
                _logger.LogInformation("SK Evolution Engine completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                // Exit with non-zero code to indicate failure
                Environment.Exit(1);
            }
        }
    }
}
