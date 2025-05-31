using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutonomousAI;

// Entry point for the application
class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Universal Builder starting...");
            
            // Load environment variables from .env.local if exists
            LoadEnvironmentVariables();
            
            // Set debug mode based on environment variable
            bool debugMode = GetEnvironmentVariableBool("DEBUG_MODE", false);
            
            // Create and run the Universal Builder
            var repoOwner = GetEnvironmentVariable("GITHUB_OWNER") ?? 
                           GetEnvironmentVariable("GITHUB_REPOSITORY_OWNER") ?? 
                           "ShawnDelaineBellazanJr";
                           
            var repoName = GetEnvironmentVariable("GITHUB_REPO") ?? 
                          GetEnvironmentVariable("GITHUB_REPOSITORY")?.Split('/').Last() ?? 
                          "AutonomousAI";
                          
            var githubToken = GetEnvironmentVariable("GITHUB_TOKEN") ?? "";
            
            if (string.IsNullOrEmpty(githubToken) && !debugMode)
            {
                throw new ArgumentException("GitHub token is required. Set GITHUB_TOKEN environment variable or enable DEBUG_MODE.");
            }
            
            // For debugging mode, use a placeholder token
            if (debugMode && string.IsNullOrEmpty(githubToken))
            {
                githubToken = "github_pat_debug_mode_token";
                Console.WriteLine("WARNING: Running in DEBUG mode with fake GitHub token. Limited functionality available.");
            }
            
            var openAIKey = GetEnvironmentVariable("OPENAI_API_KEY") ?? 
                           GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? 
                           "";
                           
            if (string.IsNullOrEmpty(openAIKey) && !debugMode)
            {
                throw new ArgumentException("OpenAI API key is required. Set OPENAI_API_KEY or AZURE_OPENAI_API_KEY environment variable or enable DEBUG_MODE.");
            }
            
            // For debugging mode, use a placeholder API key
            if (debugMode && string.IsNullOrEmpty(openAIKey))
            {
                openAIKey = "sk-debug-mode-key";
                Console.WriteLine("WARNING: Running in DEBUG mode with fake OpenAI API key. Limited functionality available.");
            }
            
            var intent = args.Length > 0 ? args[0] : "Self-evolve and improve Universal Builder capabilities";
            var valueThreshold = args.Length > 1 ? args[1] : GetEnvironmentVariable("DEFAULT_VALUE_THRESHOLD") ?? "50";
            
            // Display configuration for debugging
            if (debugMode)
            {
                Console.WriteLine($"Debug Configuration:");
                Console.WriteLine($"- Repository Owner: {repoOwner}");
                Console.WriteLine($"- Repository Name: {repoName}");
                Console.WriteLine($"- GitHub Token: {(string.IsNullOrEmpty(githubToken) ? "Not Set" : "Set")}");
                Console.WriteLine($"- OpenAI Key: {(string.IsNullOrEmpty(openAIKey) ? "Not Set" : "Set")}");
                Console.WriteLine($"- Intent: {intent}");
                Console.WriteLine($"- Value Threshold: {valueThreshold}");
                Console.WriteLine($"- MCP Enabled: {GetEnvironmentVariableBool("MCP_ENABLED", true)}");
            }
            
            var builder = new UniversalBuilder(
                intent,
                valueThreshold,
                githubToken,
                openAIKey,
                repoOwner,
                repoName,
                debugMode);
                
            await builder.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Program: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }
    }
    
    // Helper method to load environment variables from .env.local file
    private static void LoadEnvironmentVariables()
    {
        string[] envFiles = { ".env.local", ".env" };
        
        foreach (string envFile in envFiles)
        {
            if (File.Exists(envFile))
            {
                Console.WriteLine($"Loading environment variables from {envFile}");
                
                foreach (var line in File.ReadAllLines(envFile))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;
                        
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        
                        // Remove quotes if present
                        if (value.StartsWith("\"") && value.EndsWith("\""))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }
                        
                        Environment.SetEnvironmentVariable(key, value);
                    }
                }
                
                break; // Only load from the first file found
            }
        }
    }
    
    // Helper method to get environment variables with fallback
    private static string GetEnvironmentVariable(string name, string fallback = null)
    {
        return Environment.GetEnvironmentVariable(name) ?? fallback;
    }
    
    // Helper method to get boolean environment variables
    private static bool GetEnvironmentVariableBool(string name, bool fallback)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
            return fallback;
            
        return value.ToLower() == "true" || value == "1" || value.ToLower() == "yes";
    }
} 