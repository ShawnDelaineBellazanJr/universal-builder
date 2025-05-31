using System;
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
            
            // Create and run the Universal Builder
            var repoOwner = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY_OWNER") ?? "ShawnDelaineBellazanJr";
            var repoName = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY")?.Split('/').Last() ?? "universal-builder";
            var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? "";
            var openAIKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? "";
            var intent = args.Length > 0 ? args[0] : "Self-evolve and improve Universal Builder capabilities";
            var valueThreshold = args.Length > 1 ? args[1] : "50";
            
            var builder = new UniversalBuilder(
                intent,
                valueThreshold,
                githubToken,
                openAIKey,
                repoOwner,
                repoName);
                
            await builder.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Program: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }
    }
} 