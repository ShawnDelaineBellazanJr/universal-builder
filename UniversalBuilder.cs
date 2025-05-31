using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Octokit;

namespace AutonomousAI
{
    /// <summary>
    /// Universal Builder: A self-evolving system that runs on GitHub Actions,
    /// uses GitHub as its database, and implements the PMCRO pattern
    /// (Planner, Maker, Checker, Reflector, Orchestrator).
    /// </summary>
    public class UniversalBuilder
    {
        private readonly Kernel _kernel;
        private readonly GitHubMemory _githubMemory;
        private readonly string _intent;
        private readonly int _valueThreshold;
        private readonly string _repoOwner;
        private readonly string _repoName;
        private readonly string _githubToken;
        private readonly string _openAIKey;

        private const string DEFAULT_VALUE_THRESHOLD = "50";
        private const string MODEL_ID = "gpt-4"; // Can be configured based on requirements
        private const string UNIVERSAL_TEMPLATE_GIST_ID = "defaultTemplates"; // Set your default template Gist ID here

        /// <summary>
        /// Constructor for the Universal Builder
        /// </summary>
        /// <param name="intent">The build intent</param>
        /// <param name="valueThreshold">Value threshold for economic decisions</param>
        /// <param name="githubToken">GitHub token for API access</param>
        /// <param name="openAIKey">OpenAI API key for LLM access</param>
        /// <param name="repoOwner">Repository owner</param>
        /// <param name="repoName">Repository name</param>
        public UniversalBuilder(
            string intent,
            string valueThreshold,
            string githubToken,
            string openAIKey,
            string repoOwner,
            string repoName)
        {
            _intent = intent;
            _valueThreshold = int.TryParse(valueThreshold, out int threshold) ? threshold : int.Parse(DEFAULT_VALUE_THRESHOLD);
            _githubToken = githubToken;
            _openAIKey = openAIKey;
            _repoOwner = repoOwner;
            _repoName = repoName;

            // Initialize GitHub memory
            _githubMemory = new GitHubMemory(_githubToken, _repoOwner, _repoName);

            // Initialize Semantic Kernel with proper API for 1.54.0
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(MODEL_ID, _openAIKey);
            _kernel = builder.Build();
        }

        /// <summary>
        /// Main entry point for the Universal Builder
        /// </summary>
        /// <returns>Execution task</returns>
        public async Task RunAsync()
        {
            Console.WriteLine($"Starting Universal Builder with intent: {_intent}");
            Console.WriteLine($"Value threshold: {_valueThreshold}");

            try
            {
                // Validate intent
                if (string.IsNullOrWhiteSpace(_intent))
                {
                    throw new ArgumentException("Intent cannot be empty. Please provide a valid build intent.");
                }

                // Evaluate economic value of the task
                int taskValue = await EvaluateTaskValue(_intent);
                if (taskValue < _valueThreshold)
                {
                    string reason = $"Task value ({taskValue}) is below threshold ({_valueThreshold}). Refusing to proceed.";
                    Console.WriteLine(reason);
                    await LogValueRefusal(_intent, taskValue, _valueThreshold, reason);
                    return;
                }

                // Execute the PMCRO cycle
                await ExecutePMCROCycle();

                // Consider self-evolution after completing the task
                await EvolveItself();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Universal Builder: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        #region PMCRO Pattern Implementation

        /// <summary>
        /// Executes the full PMCRO cycle
        /// </summary>
        private async Task ExecutePMCROCycle()
        {
            Console.WriteLine("Executing PMCRO cycle...");

            // Create a tracking issue for this build
            int issueNumber = await _githubMemory.CreateIssueAsync(
                $"Universal Builder: {_intent}",
                $"Building with intent: {_intent}\nValue: {_valueThreshold}",
                new[] { "universal-builder", "in-progress" });

            try
            {
                // Step 1: Planner - Create a plan
                string plan = await Plan();
                await _githubMemory.AddIssueCommentAsync(issueNumber, $"## Plan\n\n{plan}");

                // Step 2: Maker - Implement the plan
                string implementation = await Make(plan);
                await _githubMemory.AddIssueCommentAsync(issueNumber, $"## Implementation\n\n{implementation}");

                // Step 3: Checker - Verify the implementation
                string checkResult = await Check(implementation);
                await _githubMemory.AddIssueCommentAsync(issueNumber, $"## Check\n\n{checkResult}");

                // Step 4: Reflector - Reflect on the process
                string reflection = await Reflect(plan, implementation, checkResult);
                await _githubMemory.AddIssueCommentAsync(issueNumber, $"## Reflection\n\n{reflection}");

                // Step 5: Orchestrator - Coordinate and decide next steps
                string nextSteps = await Orchestrate(plan, implementation, checkResult, reflection);
                await _githubMemory.AddIssueCommentAsync(issueNumber, $"## Next Steps\n\n{nextSteps}");

                // Update issue status
                await _githubMemory.UpdateIssueAsync(issueNumber, state: ItemState.Closed);

                // Create a release for history
                await _githubMemory.CreateReleaseAsync(
                    $"build-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    $"Build: {_intent}",
                    $"Plan: {plan}\nImplementation: {implementation}\nCheck: {checkResult}\nReflection: {reflection}\nNext Steps: {nextSteps}");
            }
            catch (Exception ex)
            {
                await _githubMemory.AddIssueCommentAsync(issueNumber, $"## Error\n\n```\n{ex.Message}\n{ex.StackTrace}\n```");
                await _githubMemory.UpdateIssueAsync(issueNumber, state: ItemState.Open);
                throw;
            }
        }

        /// <summary>
        /// Planner: Creates a plan based on the intent
        /// </summary>
        private async Task<string> Plan()
        {
            Console.WriteLine("Planning...");

            try
            {
                // Load the Planner template from GitHub Gist
                string template = await LoadPromptTemplate("planner");

                // Define input variables for the template
                var variables = new Dictionary<string, string>
                {
                    { "intent", _intent },
                    { "datetime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
                };

                // Execute the template with Semantic Kernel
                var result = await ExecutePromptTemplateAsync(template, variables);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Planning: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Maker: Implements the plan
        /// </summary>
        private async Task<string> Make(string plan)
        {
            Console.WriteLine("Making...");

            try
            {
                // Load the Maker template from GitHub Gist
                string template = await LoadPromptTemplate("maker");

                // Define input variables for the template
                var variables = new Dictionary<string, string>
                {
                    { "intent", _intent },
                    { "plan", plan },
                    { "datetime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
                };

                // Execute the template with Semantic Kernel
                var result = await ExecutePromptTemplateAsync(template, variables);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Making: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Checker: Verifies the implementation
        /// </summary>
        private async Task<string> Check(string implementation)
        {
            Console.WriteLine("Checking...");

            try
            {
                // Load the Checker template from GitHub Gist
                string template = await LoadPromptTemplate("checker");

                // Define input variables for the template
                var variables = new Dictionary<string, string>
                {
                    { "intent", _intent },
                    { "implementation", implementation },
                    { "datetime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
                };

                // Execute the template with Semantic Kernel
                var result = await ExecutePromptTemplateAsync(template, variables);

                // If checks fail, retry up to 3 times with adjustments
                int retryCount = 0;
                while (retryCount < 3 && result.Contains("FAIL:"))
                {
                    Console.WriteLine($"Check failed, retrying with adjustments ({retryCount + 1}/3)...");
                    
                    // Load the CheckerRetry template
                    string retryTemplate = await LoadPromptTemplate("checker-retry");
                    
                    var retryVariables = new Dictionary<string, string>
                    {
                        { "intent", _intent },
                        { "implementation", implementation },
                        { "check_result", result },
                        { "retry_count", (retryCount + 1).ToString() },
                        { "datetime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
                    };
                    
                    implementation = await ExecutePromptTemplateAsync(retryTemplate, retryVariables);
                    result = await Check(implementation);
                    retryCount++;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Checking: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Reflector: Reflects on the process and outcomes
        /// </summary>
        private async Task<string> Reflect(string plan, string implementation, string checkResult)
        {
            Console.WriteLine("Reflecting...");

            try
            {
                // Load the Reflector template from GitHub Gist
                string template = await LoadPromptTemplate("reflector");

                // Define input variables for the template
                var variables = new Dictionary<string, string>
                {
                    { "intent", _intent },
                    { "plan", plan },
                    { "implementation", implementation },
                    { "check_result", checkResult },
                    { "datetime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
                };

                // Get past releases to inform reflection
                var pastReleases = await _githubMemory.GetReleasesAsync(5);
                if (pastReleases.Any())
                {
                    variables.Add("past_releases", JsonSerializer.Serialize(pastReleases.Select(r => new { r.Name, r.Body })));
                }

                // Execute the template with Semantic Kernel
                var result = await ExecutePromptTemplateAsync(template, variables);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Reflecting: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Orchestrator: Coordinates the process and decides next steps
        /// </summary>
        private async Task<string> Orchestrate(string plan, string implementation, string checkResult, string reflection)
        {
            Console.WriteLine("Orchestrating...");

            try
            {
                // Load the Orchestrator template from GitHub Gist
                string template = await LoadPromptTemplate("orchestrator");

                // Define input variables for the template
                var variables = new Dictionary<string, string>
                {
                    { "intent", _intent },
                    { "plan", plan },
                    { "implementation", implementation },
                    { "check_result", checkResult },
                    { "reflection", reflection },
                    { "datetime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
                };

                // Execute the template with Semantic Kernel
                var result = await ExecutePromptTemplateAsync(template, variables);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Orchestrating: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Economic Consciousness

        /// <summary>
        /// Evaluates the economic value of a task based on the intent
        /// </summary>
        /// <param name="intent">The task intent</param>
        /// <returns>Numeric value of the task</returns>
        private async Task<int> EvaluateTaskValue(string intent)
        {
            Console.WriteLine("Evaluating task value...");

            try
            {
                // Load the value evaluator template from GitHub Gist
                string template = await LoadPromptTemplate("value-evaluator");

                // Define input variables for the template
                var variables = new Dictionary<string, string>
                {
                    { "intent", intent },
                    { "datetime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
                };

                // Execute the template with Semantic Kernel
                string result = await ExecutePromptTemplateAsync(template, variables);

                // Parse the value from the result
                if (int.TryParse(result.Trim(), out int value))
                {
                    Console.WriteLine($"Task value: {value}");
                    return value;
                }
                else
                {
                    Console.WriteLine($"Failed to parse task value from: {result}");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error evaluating task value: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Logs a value-based refusal to an issue
        /// </summary>
        private async Task LogValueRefusal(string intent, int taskValue, int threshold, string reason)
        {
            try
            {
                int issueNumber = await _githubMemory.CreateIssueAsync(
                    $"Value Refusal: {intent}",
                    $"Intent: {intent}\nValue: {taskValue}\nThreshold: {threshold}\nReason: {reason}",
                    new[] { "universal-builder", "value-refusal" });

                await _githubMemory.UpdateIssueAsync(issueNumber, state: ItemState.Closed);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging value refusal: {ex.Message}");
            }
        }

        #endregion

        #region Template Handling

        /// <summary>
        /// Loads a prompt template from GitHub Gist
        /// </summary>
        /// <param name="templateName">Name of the template to load</param>
        /// <returns>Template content</returns>
        private async Task<string> LoadPromptTemplate(string templateName)
        {
            try
            {
                // Try to load template from GitHub Gist
                string gistId = UNIVERSAL_TEMPLATE_GIST_ID;
                string templateContent = await _githubMemory.GetTemplateAsync(gistId, $"{templateName}.prompty");

                // Validate template
                if (_githubMemory.ValidateTemplate(templateContent))
                {
                    return templateContent;
                }
                else
                {
                    throw new InvalidOperationException($"Template {templateName} is not valid");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading template {templateName}: {ex.Message}");
                
                // Fallback to default templates
                return GetDefaultTemplate(templateName);
            }
        }

        /// <summary>
        /// Gets a default template if GitHub Gist template loading fails
        /// </summary>
        /// <param name="templateName">Name of the template</param>
        /// <returns>Default template content</returns>
        private string GetDefaultTemplate(string templateName)
        {
            // Default templates as fallbacks
            switch (templateName)
            {
                case "planner":
                    return @"system: You are a Planner in the Universal Builder system. Your job is to create a detailed plan based on the user's intent.
user: Create a detailed plan for: {{intent}}
Date: {{datetime}}
assistant: <answer>\n";

                case "maker":
                    return @"system: You are a Maker in the Universal Builder system. Your job is to implement the plan provided.
user: Implement the following plan for: {{intent}}
Plan: {{plan}}
Date: {{datetime}}
assistant: <answer>\n";

                case "checker":
                    return @"system: You are a Checker in the Universal Builder system. Your job is to verify the implementation and identify any issues.
user: Check the following implementation for: {{intent}}
Implementation: {{implementation}}
Date: {{datetime}}
assistant: <answer>\n";

                case "checker-retry":
                    return @"system: You are a Checker Retry in the Universal Builder system. Your job is to fix the implementation based on the check results.
user: Fix the following implementation for: {{intent}}
Implementation: {{implementation}}
Check Results: {{check_result}}
Retry: {{retry_count}}
Date: {{datetime}}
assistant: <answer>\n";

                case "reflector":
                    return @"system: You are a Reflector in the Universal Builder system. Your job is to reflect on the process and identify learnings.
user: Reflect on the following build process for: {{intent}}
Plan: {{plan}}
Implementation: {{implementation}}
Check Result: {{check_result}}
Date: {{datetime}}
assistant: <answer>\n";

                case "orchestrator":
                    return @"system: You are an Orchestrator in the Universal Builder system. Your job is to coordinate the process and decide next steps.
user: Orchestrate the following build for: {{intent}}
Plan: {{plan}}
Implementation: {{implementation}}
Check Result: {{check_result}}
Reflection: {{reflection}}
Date: {{datetime}}
assistant: <answer>\n";

                case "value-evaluator":
                    return @"system: You are an Economic Evaluator in the Universal Builder system. Your job is to assign a numeric value (0-100) to tasks based on their potential impact, complexity, and alignment with system goals.
user: Evaluate the economic value of the following task: {{intent}}
Date: {{datetime}}
Return only a numeric value between 0 and 100.
assistant: <answer>\n";

                case "self-evolution":
                    return @"system: You are a Self-Evolution agent in the Universal Builder system. Your job is to propose improvements to the Universal Builder itself.
user: Analyze the Universal Builder system and propose code improvements. You must provide complete files, not just code snippets or changes.

Use the following format for your response:
1. First, explain what issues you've identified and how you'll fix them
2. For each file you're modifying, include the full code with your changes using the format:
```csharp:FILENAME
// Full file content with your changes
```

Remember: 
- You must include complete files with all necessary code, not just the parts you're changing
- Each code block must start with ```csharp:FILENAME to clearly identify which file it's for
- Your proposed changes should not introduce new bugs or break compatibility
- Consider improving error handling, performance, security, and overall system robustness

Date: {{datetime}}
assistant: <answer>\n";

                default:
                    throw new ArgumentException($"No default template available for {templateName}");
            }
        }

        /// <summary>
        /// Executes a prompt template with the given variables using Semantic Kernel
        /// </summary>
        /// <param name="template">The template content</param>
        /// <param name="variables">Template variables</param>
        /// <returns>Execution result</returns>
        private async Task<string> ExecutePromptTemplateAsync(string template, Dictionary<string, string> variables)
        {
            try
            {
                // Create kernel arguments from variables
                var kernelArguments = new KernelArguments();
                foreach (var kvp in variables)
                {
                    kernelArguments[kvp.Key] = kvp.Value;
                }
                
                // Execute the prompt with the kernel - using direct prompt execution
                var result = await _kernel.InvokePromptAsync(template, kernelArguments);
                
                return result.GetValue<string>() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing template: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Self-Evolution

        /// <summary>
        /// Evolves the Universal Builder system by suggesting improvements to its own code
        /// </summary>
        private async Task EvolveItself()
        {
            Console.WriteLine("Considering self-evolution...");

            try
            {
                // Load the self-evolution template from GitHub Gist
                string template = await LoadPromptTemplate("self-evolution");

                // Define input variables for the template
                var variables = new Dictionary<string, string>
                {
                    { "datetime", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") }
                };

                // Get past releases to inform evolution
                var pastReleases = await _githubMemory.GetReleasesAsync(5);
                if (pastReleases.Any())
                {
                    variables.Add("past_releases", JsonSerializer.Serialize(pastReleases.Select(r => new { r.Name, r.Body })));
                }

                // Execute the template with Semantic Kernel
                string evolutionPlan = await ExecutePromptTemplateAsync(template, variables);

                // Parse the evolution plan and extract code changes
                if (string.IsNullOrWhiteSpace(evolutionPlan) || !evolutionPlan.Contains("```"))
                {
                    Console.WriteLine("No valid evolution plan generated");
                    return;
                }

                // Extract the code changes
                string[] codeBlocks = evolutionPlan.Split("```", StringSplitOptions.RemoveEmptyEntries);
                var codeChanges = new Dictionary<string, string>();

                foreach (var block in codeBlocks)
                {
                    if (block.StartsWith("csharp:") || block.StartsWith("cs:"))
                    {
                        // Extract file path and code
                        string[] lines = block.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                        if (lines.Length < 2)
                            continue;

                        string firstLine = lines[0].Trim();
                        string filePath = firstLine.Replace("csharp:", "").Replace("cs:", "").Trim();
                        
                        // Default to main file if path is empty
                        if (string.IsNullOrWhiteSpace(filePath))
                        {
                            filePath = "UniversalBuilder.cs";
                        }

                        string code = string.Join("\n", lines.Skip(1));
                        codeChanges[filePath] = code;
                        Console.WriteLine($"Found code block for file: {filePath} ({code.Length} characters)");
                    }
                }

                if (codeChanges.Count == 0)
                {
                    Console.WriteLine("No code changes extracted from evolution plan");
                    return;
                }

                // Create a PR with the changes
                string branchName = $"self-evolution-{DateTime.UtcNow:yyyyMMddHHmmss}";
                string prTitle = $"Self-Evolution: Improvements from {DateTime.UtcNow:yyyy-MM-dd}";
                string prDescription = $"# Self-Evolution\n\n{evolutionPlan}";

                // Log the information about what's being changed
                Console.WriteLine($"Self-evolution creating PR '{prTitle}' with changes to {codeChanges.Count} files");
                
                // Write changes to file system - we can't directly interact with git from here,
                // but we can write files that the GitHub Actions workflow can then commit
                foreach (var change in codeChanges)
                {
                    string filePath = change.Key;
                    string newContent = change.Value;
                    
                    try 
                    {
                        // Ensure directory exists
                        string? directory = Path.GetDirectoryName(filePath);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        
                        // Write the new content to the file
                        File.WriteAllText(filePath, newContent);
                        Console.WriteLine($"Updated file: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error writing file {filePath}: {ex.Message}");
                    }
                }
                
                // The GitHub Actions workflow will detect these changes, create a branch, and make a PR

                // Log the evolution plan to an issue for transparency
                await _githubMemory.CreateIssueAsync(
                    prTitle,
                    $"Evolution plan:\n\n{evolutionPlan}",
                    new[] { "universal-builder", "self-evolution" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in self-evolution: {ex.Message}");
            }
        }

        #endregion
    }
} 