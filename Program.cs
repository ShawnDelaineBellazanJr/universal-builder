using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Octokit;

namespace SkEvolutionEngine
{
    /// <summary>
    /// SK Evolution Engine - An autonomous system for evolving Semantic Kernel implementations
    /// </summary>
    public class Program
    {
        private static ILogger<Program> _logger;
        private static Kernel _kernel;
        private static GitHubClient _gitHubClient;
        private static Config _config;
        private static HttpClient _httpClient = new HttpClient();

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
                LoadConfiguration();
                _logger.LogInformation("Configuration loaded");

                // Initialize Semantic Kernel
                await InitializeSemanticKernel();
                _logger.LogInformation("Semantic Kernel initialized");

                // Initialize GitHub client
                InitializeGitHubClient();
                _logger.LogInformation("GitHub client initialized");

                // Parse command line arguments
                if (args.Length > 0)
                {
                    await ExecuteCommand(args);
                }
                else
                {
                    // Run the full evolution cycle if no arguments provided
                    await RunEvolutionCycle();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static void LoadConfiguration()
        {
            if (File.Exists("config.json"))
            {
                string json = File.ReadAllText("config.json");
                _config = JsonSerializer.Deserialize<Config>(json);
            }
            else
            {
                _config = new Config();
                _logger.LogWarning("Config file not found. Using default configuration.");
            }

            // Load environment variables from .env file if it exists
            if (File.Exists(".env"))
            {
                foreach (var line in File.ReadAllLines(".env"))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        Environment.SetEnvironmentVariable(key, value);
                    }
                }
            }
        }

        private static async Task InitializeSemanticKernel()
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("OPENAI_API_KEY environment variable is not set");
            }

            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(_config.SemanticKernel.Model, apiKey);
            builder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Information));
            _kernel = builder.Build();
        }

        private static void InitializeGitHubClient()
        {
            string token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("GITHUB_TOKEN environment variable is not set. GitHub operations will be limited.");
                _gitHubClient = new GitHubClient(new ProductHeaderValue("SK-Evolution-Engine"));
            }
            else
            {
                _gitHubClient = new GitHubClient(new ProductHeaderValue("SK-Evolution-Engine"))
                {
                    Credentials = new Credentials(token)
                };
            }
        }

        private static async Task ExecuteCommand(string[] args)
        {
            string command = args[0].ToLower();

            switch (command)
            {
                case "analyze-dotnet":
                    await AnalyzeDotnetRepo();
                    break;

                case "analyze-sk":
                    await AnalyzeSemanticKernelRepo();
                    break;

                case "evolve":
                    await RunEvolutionCycle();
                    break;

                case "propose-improvements":
                    await ProposeImprovements();
                    break;

                case "implement-improvements":
                    await ImplementImprovements();
                    break;

                default:
                    _logger.LogError($"Unknown command: {command}");
                    _logger.LogInformation("Available commands: analyze-dotnet, analyze-sk, evolve, propose-improvements, implement-improvements");
                    break;
            }
        }

        private static async Task RunEvolutionCycle()
        {
            _logger.LogInformation("Starting evolution cycle...");

            try
            {
                // Step 1: Analyze .NET SDK repository
                _logger.LogInformation("Step 1: Analyzing .NET SDK repository...");
                var dotnetAnalysis = await AnalyzeDotnetRepo();

                // Step 2: Analyze Semantic Kernel repository
                _logger.LogInformation("Step 2: Analyzing Semantic Kernel repository...");
                var skAnalysis = await AnalyzeSemanticKernelRepo();

                // Step 3: Propose improvements based on analysis
                _logger.LogInformation("Step 3: Proposing improvements...");
                var improvements = await ProposeImprovements(dotnetAnalysis, skAnalysis);

                // Step 4: Implement improvements
                _logger.LogInformation("Step 4: Implementing improvements...");
                var implementationResults = await ImplementImprovements(improvements);

                // Step 5: Create pull request with improvements
                _logger.LogInformation("Step 5: Creating pull request...");
                await CreatePullRequest(implementationResults);

                _logger.LogInformation("Evolution cycle completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during evolution cycle: {ex.Message}");
                _logger.LogError(ex.StackTrace);
            }
        }

        private static async Task<string> AnalyzeDotnetRepo()
        {
            _logger.LogInformation("Analyzing .NET SDK repository...");

            try
            {
                // Use ChatCompletion for analysis
                var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

                // Fetch key files and patterns from .NET SDK
                var repo = _config.Repositories.DotnetSdk;
                var contents = await _gitHubClient.Repository.Content.GetAllContents(repo.Owner, repo.Name, "src");

                // Analyze project structure
                var structurePrompt = $"Analyze the .NET SDK repository structure based on these files: {string.Join(", ", contents.Take(20).Select(c => c.Path))}";
                
                var chatHistory = new ChatHistory();
                chatHistory.AddSystemMessage("You are an expert .NET and Semantic Kernel architect tasked with analyzing the .NET SDK repository structure and identifying best practices and patterns.");
                chatHistory.AddUserMessage(structurePrompt);
                
                var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
                string analysis = response.Content;

                _logger.LogInformation("Analysis of .NET SDK repository completed.");
                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error analyzing .NET SDK repository: {ex.Message}");
                throw;
            }
        }

        private static async Task<string> AnalyzeSemanticKernelRepo()
        {
            _logger.LogInformation("Analyzing Semantic Kernel repository...");

            try
            {
                // Use ChatCompletion for analysis
                var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

                // Fetch key files and patterns from Semantic Kernel
                var repo = _config.Repositories.SemanticKernel;
                var contents = await _gitHubClient.Repository.Content.GetAllContents(repo.Owner, repo.Name, "dotnet/src/SemanticKernel.Core");

                // Analyze project structure and patterns
                var structurePrompt = $"Analyze the Semantic Kernel repository structure based on these files: {string.Join(", ", contents.Take(20).Select(c => c.Path))}";
                
                var chatHistory = new ChatHistory();
                chatHistory.AddSystemMessage("You are an expert .NET and Semantic Kernel architect tasked with analyzing the Semantic Kernel repository structure and identifying areas for improvement.");
                chatHistory.AddUserMessage(structurePrompt);
                
                var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
                string analysis = response.Content;

                _logger.LogInformation("Analysis of Semantic Kernel repository completed.");
                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error analyzing Semantic Kernel repository: {ex.Message}");
                throw;
            }
        }

        private static async Task<List<Improvement>> ProposeImprovements(string dotnetAnalysis = null, string skAnalysis = null)
        {
            _logger.LogInformation("Proposing improvements based on analysis...");

            try
            {
                // If analyses were not provided, run them now
                if (string.IsNullOrEmpty(dotnetAnalysis))
                {
                    dotnetAnalysis = await AnalyzeDotnetRepo();
                }

                if (string.IsNullOrEmpty(skAnalysis))
                {
                    skAnalysis = await AnalyzeSemanticKernelRepo();
                }

                // Use ChatCompletion to propose improvements
                var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
                
                var chatHistory = new ChatHistory();
                chatHistory.AddSystemMessage(@"You are an expert .NET and Semantic Kernel architect. 
Based on analyses of the .NET SDK and Semantic Kernel repositories, propose specific improvements 
that could be made to our multi-frequency cognitive architecture implementation. 
For each improvement, provide a detailed description, justification, and implementation strategy.
Format your response as a structured JSON array of improvement objects.");
                
                chatHistory.AddUserMessage($"Based on the following analyses, propose improvements for our multi-frequency cognitive architecture implementation:\n\n.NET SDK Analysis:\n{dotnetAnalysis}\n\nSemantic Kernel Analysis:\n{skAnalysis}");
                
                var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
                
                // Try to parse improvements from the response
                string content = response.Content;
                List<Improvement> improvements = new List<Improvement>();
                
                // Find JSON array in the content
                int startIndex = content.IndexOf('[');
                int endIndex = content.LastIndexOf(']');
                
                if (startIndex >= 0 && endIndex > startIndex)
                {
                    string json = content.Substring(startIndex, endIndex - startIndex + 1);
                    try
                    {
                        improvements = JsonSerializer.Deserialize<List<Improvement>>(json);
                    }
                    catch (JsonException)
                    {
                        _logger.LogWarning("Failed to parse improvements JSON directly. Trying alternative approach.");
                        
                        // Try to extract individual improvements
                        var splitContent = content.Split(new[] { "Improvement " }, StringSplitOptions.RemoveEmptyEntries);
                        
                        foreach (var part in splitContent.Skip(1)) // Skip the first part (before "Improvement 1")
                        {
                            var improvement = new Improvement
                            {
                                Title = ExtractProperty(part, "Title:"),
                                Description = ExtractProperty(part, "Description:"),
                                Justification = ExtractProperty(part, "Justification:"),
                                ImplementationStrategy = ExtractProperty(part, "Implementation Strategy:"),
                                EconomicValue = 85 // Default value
                            };
                            
                            improvements.Add(improvement);
                        }
                    }
                }

                _logger.LogInformation($"Proposed {improvements.Count} improvements.");
                
                // Limit the number of improvements as specified in config
                return improvements.Take(_config.Evolution.MaxImprovementsPerCycle).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error proposing improvements: {ex.Message}");
                throw;
            }
        }

        private static string ExtractProperty(string text, string propertyName)
        {
            int startIndex = text.IndexOf(propertyName);
            if (startIndex < 0) return string.Empty;
            
            startIndex += propertyName.Length;
            int endIndex = text.IndexOf('\n', startIndex);
            if (endIndex < 0) endIndex = text.Length;
            
            return text.Substring(startIndex, endIndex - startIndex).Trim();
        }

        private static async Task<List<ImplementationResult>> ImplementImprovements(List<Improvement> improvements = null)
        {
            _logger.LogInformation("Implementing improvements...");

            try
            {
                // If improvements were not provided, propose them now
                if (improvements == null || !improvements.Any())
                {
                    improvements = await ProposeImprovements();
                }

                var results = new List<ImplementationResult>();
                var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

                // Clone the repository locally
                var repo = _config.Repositories.SelfRepo;
                string repoDir = Path.Combine(Path.GetTempPath(), "sk-evolution", Guid.NewGuid().ToString());
                Directory.CreateDirectory(repoDir);

                _logger.LogInformation($"Cloning repository to {repoDir}...");
                await CloneRepository(repo.Owner, repo.Name, repoDir);

                // Implement each improvement
                foreach (var improvement in improvements)
                {
                    _logger.LogInformation($"Implementing improvement: {improvement.Title}");
                    
                    // Create a new branch for the improvement
                    string branchName = $"sk-evolution/{Guid.NewGuid().ToString().Substring(0, 8)}";
                    await CreateBranch(repoDir, branchName);
                    
                    // Generate implementation code
                    var chatHistory = new ChatHistory();
                    chatHistory.AddSystemMessage(@"You are an expert .NET and Semantic Kernel developer. 
Implement the proposed improvement for our multi-frequency cognitive architecture. 
Provide the exact code changes needed, including file paths and line numbers where possible.
The implementation should be specific, practical, and ready to be applied to the repository.");
                    
                    chatHistory.AddUserMessage($"Implement the following improvement:\n\nTitle: {improvement.Title}\nDescription: {improvement.Description}\nJustification: {improvement.Justification}\nImplementation Strategy: {improvement.ImplementationStrategy}");
                    
                    var response = await chatCompletionService.GetChatMessageContentAsync(chatHistory);
                    
                    // Apply the implementation
                    var implementation = new ImplementationResult
                    {
                        Improvement = improvement,
                        Branch = branchName,
                        ImplementationDetails = response.Content,
                        FilesChanged = new List<string>()
                    };
                    
                    // Try to extract and apply specific file changes
                    var fileChanges = ExtractFileChanges(response.Content);
                    foreach (var change in fileChanges)
                    {
                        string filePath = Path.Combine(repoDir, change.FilePath);
                        string directory = Path.GetDirectoryName(filePath);
                        
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        
                        if (File.Exists(filePath))
                        {
                            // Update existing file
                            string content = File.ReadAllText(filePath);
                            if (change.LineNumber > 0 && change.LineCount > 0)
                            {
                                // Replace specific lines
                                var lines = content.Split('\n').ToList();
                                int startLine = Math.Min(change.LineNumber - 1, lines.Count);
                                int endLine = Math.Min(startLine + change.LineCount, lines.Count);
                                
                                lines.RemoveRange(startLine, endLine - startLine);
                                lines.InsertRange(startLine, change.Content.Split('\n'));
                                
                                content = string.Join('\n', lines);
                            }
                            else
                            {
                                // Replace entire file content
                                content = change.Content;
                            }
                            
                            File.WriteAllText(filePath, content);
                        }
                        else
                        {
                            // Create new file
                            File.WriteAllText(filePath, change.Content);
                        }
                        
                        implementation.FilesChanged.Add(change.FilePath);
                    }
                    
                    // Commit the changes
                    await CommitChanges(repoDir, $"SK-Evolution: Implement {improvement.Title}");
                    
                    // Push the branch
                    await PushBranch(repoDir, branchName);
                    
                    results.Add(implementation);
                }

                _logger.LogInformation($"Implemented {results.Count} improvements.");
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error implementing improvements: {ex.Message}");
                throw;
            }
        }

        private static List<FileChange> ExtractFileChanges(string content)
        {
            var changes = new List<FileChange>();
            
            // Look for file blocks in markdown format: ```path/to/file``` or ```csharp:path/to/file```
            var codeBlockRegex = new System.Text.RegularExpressions.Regex(@"```(?:csharp:)?([^`\r\n]+)?\n(.*?)```", System.Text.RegularExpressions.RegexOptions.Singleline);
            var matches = codeBlockRegex.Matches(content);
            
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                string filePath = match.Groups[1].Value.Trim();
                string code = match.Groups[2].Value;
                
                // Check if this is a valid file path
                if (string.IsNullOrEmpty(filePath) || filePath.Contains(' '))
                    continue;
                
                // Parse line numbers if specified (e.g., path/to/file:10-20)
                int lineNumber = 0;
                int lineCount = 0;
                
                if (filePath.Contains(':'))
                {
                    var parts = filePath.Split(':');
                    filePath = parts[0];
                    
                    if (parts.Length > 1 && parts[1].Contains('-'))
                    {
                        var lineRange = parts[1].Split('-');
                        if (lineRange.Length == 2)
                        {
                            int.TryParse(lineRange[0], out lineNumber);
                            int endLine;
                            if (int.TryParse(lineRange[1], out endLine))
                            {
                                lineCount = endLine - lineNumber + 1;
                            }
                        }
                    }
                }
                
                changes.Add(new FileChange
                {
                    FilePath = filePath,
                    Content = code,
                    LineNumber = lineNumber,
                    LineCount = lineCount
                });
            }
            
            return changes;
        }

        private static async Task CloneRepository(string owner, string name, string directory)
        {
            // Use the git command line tool to clone the repository
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"clone https://github.com/{owner}/{name}.git {directory}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            await process.WaitForExitAsync();
            
            if (process.ExitCode != 0)
            {
                throw new Exception($"Failed to clone repository: {process.StandardError.ReadToEnd()}");
            }
        }

        private static async Task CreateBranch(string repoDir, string branchName)
        {
            // Use the git command line tool to create a new branch
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"checkout -b {branchName}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = repoDir
                }
            };
            
            process.Start();
            await process.WaitForExitAsync();
            
            if (process.ExitCode != 0)
            {
                throw new Exception($"Failed to create branch: {process.StandardError.ReadToEnd()}");
            }
        }

        private static async Task CommitChanges(string repoDir, string message)
        {
            // Stage all changes
            var stageProcess = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = "add .",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = repoDir
                }
            };
            
            stageProcess.Start();
            await stageProcess.WaitForExitAsync();
            
            if (stageProcess.ExitCode != 0)
            {
                throw new Exception($"Failed to stage changes: {stageProcess.StandardError.ReadToEnd()}");
            }
            
            // Commit the changes
            var commitProcess = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"commit -m \"{message}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = repoDir
                }
            };
            
            commitProcess.Start();
            await commitProcess.WaitForExitAsync();
            
            if (commitProcess.ExitCode != 0)
            {
                throw new Exception($"Failed to commit changes: {commitProcess.StandardError.ReadToEnd()}");
            }
        }

        private static async Task PushBranch(string repoDir, string branchName)
        {
            // Push the branch to the remote repository
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = $"push -u origin {branchName}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = repoDir
                }
            };
            
            process.Start();
            await process.WaitForExitAsync();
            
            if (process.ExitCode != 0)
            {
                throw new Exception($"Failed to push branch: {process.StandardError.ReadToEnd()}");
            }
        }

        private static async Task CreatePullRequest(List<ImplementationResult> results)
        {
            _logger.LogInformation("Creating pull request with improvements...");

            try
            {
                // Group results by branch
                var groupedResults = results.GroupBy(r => r.Branch);
                
                foreach (var group in groupedResults)
                {
                    string branch = group.Key;
                    var branchResults = group.ToList();
                    
                    if (branchResults.Any())
                    {
                        // Create pull request
                        var repo = _config.Repositories.SelfRepo;
                        var title = $"SK-Evolution: {branchResults.Count} improvements to multi-frequency cognitive architecture";
                        
                        var body = "# SK Evolution: Autonomous Improvements\n\n";
                        body += "This pull request was automatically generated by the SK Evolution Engine.\n\n";
                        body += "## Improvements\n\n";
                        
                        foreach (var result in branchResults)
                        {
                            body += $"### {result.Improvement.Title}\n\n";
                            body += $"**Description**: {result.Improvement.Description}\n\n";
                            body += $"**Justification**: {result.Improvement.Justification}\n\n";
                            body += "**Files Changed**:\n";
                            
                            foreach (var file in result.FilesChanged)
                            {
                                body += $"- {file}\n";
                            }
                            
                            body += "\n";
                        }
                        
                        var newPr = new NewPullRequest(title, branch, "main")
                        {
                            Body = body
                        };
                        
                        var pr = await _gitHubClient.PullRequest.Create(repo.Owner, repo.Name, newPr);
                        
                        _logger.LogInformation($"Created pull request #{pr.Number}: {pr.HtmlUrl}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating pull request: {ex.Message}");
                throw;
            }
        }
    }

    // Configuration models
    public class Config
    {
        public SemanticKernelConfig SemanticKernel { get; set; } = new SemanticKernelConfig();
        public RepositoriesConfig Repositories { get; set; } = new RepositoriesConfig();
        public EvolutionConfig Evolution { get; set; } = new EvolutionConfig();
        public Dictionary<string, FrequencyConfig> Frequencies { get; set; } = new Dictionary<string, FrequencyConfig>();
    }

    public class SemanticKernelConfig
    {
        public string Model { get; set; } = "gpt-4";
        public double Temperature { get; set; } = 0.7;
        public int MaxTokens { get; set; } = 4000;
        public double TopP { get; set; } = 0.95;
    }

    public class RepositoriesConfig
    {
        public RepositoryConfig DotnetSdk { get; set; } = new RepositoryConfig { Owner = "dotnet", Name = "sdk", Branch = "main" };
        public RepositoryConfig SemanticKernel { get; set; } = new RepositoryConfig { Owner = "microsoft", Name = "semantic-kernel", Branch = "main" };
        public RepositoryConfig SelfRepo { get; set; } = new RepositoryConfig { Owner = "ShawnDelaineBellazanJr", Name = "universal-builder", Branch = "main" };
    }

    public class RepositoryConfig
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
    }

    public class EvolutionConfig
    {
        public string CycleInterval { get; set; } = "24h";
        public int EconomicThreshold { get; set; } = 90;
        public int MaxImprovementsPerCycle { get; set; } = 3;
        public List<string> Stages { get; set; } = new List<string> { "analyze", "learn", "propose", "implement", "validate", "integrate" };
    }

    public class FrequencyConfig
    {
        public string Interval { get; set; }
        public int Threshold { get; set; }
    }

    // Improvement models
    public class Improvement
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Justification { get; set; }
        public string ImplementationStrategy { get; set; }
        public int EconomicValue { get; set; }
    }

    public class ImplementationResult
    {
        public Improvement Improvement { get; set; }
        public string Branch { get; set; }
        public string ImplementationDetails { get; set; }
        public List<string> FilesChanged { get; set; }
    }

    public class FileChange
    {
        public string FilePath { get; set; }
        public string Content { get; set; }
        public int LineNumber { get; set; }
        public int LineCount { get; set; }
    }
} 