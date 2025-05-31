using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Agents;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Agents
{
    /// <summary>
    /// Goal Processor Agent that analyzes and processes raw build goals
    /// </summary>
    public class GoalProcessor : BaseAgent
    {
        private readonly string _templatesDirectory;
        
        public GoalProcessor(Kernel kernel, string templatesDirectory = "templates", string model = "gpt-4", bool debugMode = false)
            : base(kernel, model, debugMode)
        {
            _templatesDirectory = templatesDirectory;
        }
        
        /// <summary>
        /// Process a raw goal into a structured ProcessedGoal
        /// </summary>
        /// <param name="rawGoal">The raw goal input from the user</param>
        /// <returns>A structured processed goal</returns>
        public async Task<ProcessedGoal> ProcessGoalAsync(string rawGoal)
        {
            Console.WriteLine($"Processing goal: {rawGoal}");
            
            try
            {
                // Step 1: Chain of thought analysis
                var analysis = await AnalyzeGoalAsync(rawGoal);
                
                // Step 2: Determine project type
                var projectType = await DetermineProjectTypeAsync(analysis, rawGoal);
                
                // Step 3: Assess complexity
                var complexity = await AssessComplexityAsync(analysis, rawGoal);
                
                // Step 4: Extract requirements
                var requirements = await ExtractRequirementsAsync(analysis, rawGoal);
                
                // Step 5: Assess economic value
                var economicValue = await AssessEconomicValueAsync(rawGoal, projectType);
                
                // Create the processed goal
                var processedGoal = new ProcessedGoal
                {
                    Intent = analysis.Intent,
                    ProjectType = projectType,
                    Complexity = complexity,
                    Requirements = requirements,
                    EconomicValue = economicValue,
                    HasSufficientInformation = true // Assume true by default
                };
                
                // Check if we have sufficient information
                if (requirements.Count < 2 || string.IsNullOrWhiteSpace(projectType))
                {
                    processedGoal.HasSufficientInformation = false;
                    processedGoal.InsufficientInformationReason = "Insufficient requirements or unclear project type.";
                }
                
                return processedGoal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing goal: {ex.Message}");
                
                if (_debugMode)
                {
                    // Return a mock response in debug mode
                    return new ProcessedGoal
                    {
                        Intent = rawGoal,
                        ProjectType = "mock-project",
                        Complexity = 5,
                        Requirements = new List<string> { "Mock requirement 1", "Mock requirement 2" },
                        EconomicValue = 75,
                        HasSufficientInformation = true
                    };
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Analyze the goal using chain of thought reasoning
        /// </summary>
        private async Task<GoalAnalysis> AnalyzeGoalAsync(string rawGoal)
        {
            string systemMessage = @"
You are an expert system analyst working on the Universal Autonomous Builder system.
Your task is to analyze a goal and extract the following information:
1. The primary intent or objective
2. The domain or context of the project
3. The stakeholders or users
4. The key features or requirements
5. The constraints or limitations
6. The expected outcomes or success criteria

Provide your analysis in JSON format with the following keys:
- intent: The primary intent or objective
- domain: The domain or context of the project
- stakeholders: Array of stakeholders or users
- features: Array of key features or requirements
- constraints: Array of constraints or limitations
- outcomes: Array of expected outcomes or success criteria
";

            string userMessage = $"Analyze the following goal using chain of thought reasoning:\n\n{rawGoal}";
            
            var response = await ExecutePromptAsync(systemMessage, userMessage);
            
            try
            {
                // Extract JSON from response (handle case where AI might add commentary)
                string jsonResponse = response;
                if (response.Contains("{") && response.Contains("}"))
                {
                    int startIndex = response.IndexOf("{");
                    int endIndex = response.LastIndexOf("}") + 1;
                    jsonResponse = response.Substring(startIndex, endIndex - startIndex);
                }
                
                // Parse the JSON response
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var analysis = JsonSerializer.Deserialize<GoalAnalysis>(jsonResponse, options);
                
                if (analysis == null)
                {
                    throw new Exception("Failed to parse goal analysis");
                }
                
                return analysis;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing goal analysis: {ex.Message}");
                
                // Fallback to simpler parsing approach
                return new GoalAnalysis
                {
                    Intent = rawGoal,
                    Domain = "unknown",
                    Stakeholders = new List<string> { "user" },
                    Features = new List<string> { "feature" },
                    Constraints = new List<string>(),
                    Outcomes = new List<string> { "functioning system" }
                };
            }
        }
        
        /// <summary>
        /// Determine the project type based on the analysis
        /// </summary>
        private async Task<string> DetermineProjectTypeAsync(GoalAnalysis analysis, string rawGoal)
        {
            string systemMessage = @"
You are an expert system architect working on the Universal Autonomous Builder system.
Your task is to determine the project type based on the goal and analysis.

Available project types:
- ios-app: iOS mobile applications
- android-app: Android mobile applications
- web-service: Web services, APIs, or backends
- web-app: Web applications with frontend components
- data-pipeline: Data processing or ETL pipelines
- ml-model: Machine learning models or AI systems
- cli-tool: Command-line interface tools or utilities
- desktop-app: Desktop applications
- game: Games for any platform
- smart-contract: Blockchain applications or smart contracts
- embedded-system: Embedded systems or IoT applications
- autonomous-agent: AI agents or autonomous systems
- template-evolution: Improvements to the builder template itself
- sk-enhancement: Improvements to the Semantic Kernel framework

Select exactly one project type from the list above.
";

            string userMessage = $"Goal: {rawGoal}\n\nAnalysis: {JsonSerializer.Serialize(analysis)}\n\nDetermine the project type:";
            
            var response = await ExecutePromptAsync(systemMessage, userMessage);
            
            // Extract the project type from the response
            foreach (string projectType in new[] { 
                "ios-app", "android-app", "web-service", "web-app", "data-pipeline", 
                "ml-model", "cli-tool", "desktop-app", "game", "smart-contract",
                "embedded-system", "autonomous-agent", "template-evolution", "sk-enhancement" 
            })
            {
                if (response.Contains(projectType, StringComparison.OrdinalIgnoreCase))
                {
                    return projectType;
                }
            }
            
            // Default to web-service if no match found
            return "web-service";
        }
        
        /// <summary>
        /// Assess the complexity of the project
        /// </summary>
        private async Task<int> AssessComplexityAsync(GoalAnalysis analysis, string rawGoal)
        {
            string systemMessage = @"
You are an expert project manager working on the Universal Autonomous Builder system.
Your task is to assess the complexity of a project on a scale from 1 to 10, where:
1 = Extremely simple, can be completed in hours
5 = Moderate complexity, requires days of work
10 = Extremely complex, requires weeks or months of work

Consider factors such as:
- Technical complexity
- Number of components or features
- Integration requirements
- Data handling requirements
- Security considerations
- Scalability needs
- Testing requirements

Provide a single integer from 1 to 10.
";

            string userMessage = $"Goal: {rawGoal}\n\nAnalysis: {JsonSerializer.Serialize(analysis)}\n\nAssess the complexity:";
            
            var response = await ExecutePromptAsync(systemMessage, userMessage);
            
            // Try to extract a number from the response
            foreach (char c in response)
            {
                if (char.IsDigit(c) && int.TryParse(c.ToString(), out int complexity) && complexity >= 1 && complexity <= 10)
                {
                    return complexity;
                }
            }
            
            // Default to complexity 5 if no valid number found
            return 5;
        }
        
        /// <summary>
        /// Extract requirements from the analysis
        /// </summary>
        private async Task<List<string>> ExtractRequirementsAsync(GoalAnalysis analysis, string rawGoal)
        {
            string systemMessage = @"
You are an expert requirements engineer working on the Universal Autonomous Builder system.
Your task is to extract a list of clear, actionable requirements from the goal and analysis.

Good requirements are:
- Clear and unambiguous
- Testable
- Feasible
- Necessary
- Prioritized

Extract 5-10 key requirements and format them as a JSON array of strings.
";

            string userMessage = $"Goal: {rawGoal}\n\nAnalysis: {JsonSerializer.Serialize(analysis)}\n\nExtract requirements:";
            
            var response = await ExecutePromptAsync(systemMessage, userMessage);
            
            try
            {
                // Extract JSON array from response
                string jsonResponse = response;
                if (response.Contains("[") && response.Contains("]"))
                {
                    int startIndex = response.IndexOf("[");
                    int endIndex = response.LastIndexOf("]") + 1;
                    jsonResponse = response.Substring(startIndex, endIndex - startIndex);
                }
                
                // Parse the JSON response
                var requirements = JsonSerializer.Deserialize<List<string>>(jsonResponse);
                
                if (requirements == null || requirements.Count == 0)
                {
                    throw new Exception("Failed to parse requirements");
                }
                
                return requirements;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing requirements: {ex.Message}");
                
                // Fallback to using features from the analysis
                if (analysis.Features.Count > 0)
                {
                    return analysis.Features;
                }
                
                // Last resort fallback
                return new List<string> { "Implement core functionality", "Ensure system reliability" };
            }
        }
        
        /// <summary>
        /// Assess the economic value of the project
        /// </summary>
        private async Task<int> AssessEconomicValueAsync(string rawGoal, string projectType)
        {
            string systemMessage = @"
You are an economic evaluator for the Universal Autonomous Builder system.
Your task is to assess the economic value of a project on a scale from 0 to 100, where:
0 = No economic value whatsoever
50 = Moderate economic value, worth building
100 = Extremely high economic value, transformative potential

Consider factors such as:
- Market potential
- Innovation level
- Alignment with system capabilities
- Potential for reuse
- Learning potential
- Resource efficiency

Provide a single integer from 0 to 100.
";

            string userMessage = $"Goal: {rawGoal}\n\nProject Type: {projectType}\n\nAssess the economic value:";
            
            var response = await ExecutePromptAsync(systemMessage, userMessage);
            
            // Try to extract a number from the response
            string numberStr = "";
            foreach (char c in response)
            {
                if (char.IsDigit(c))
                {
                    numberStr += c;
                }
                else if (numberStr.Length > 0 && !char.IsDigit(c))
                {
                    break;
                }
            }
            
            if (int.TryParse(numberStr, out int value) && value >= 0 && value <= 100)
            {
                return value;
            }
            
            // Default to value 50 if no valid number found
            return 50;
        }
    }
    
    /// <summary>
    /// Helper class to store goal analysis results
    /// </summary>
    public class GoalAnalysis
    {
        public string Intent { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public List<string> Stakeholders { get; set; } = new List<string>();
        public List<string> Features { get; set; } = new List<string>();
        public List<string> Constraints { get; set; } = new List<string>();
        public List<string> Outcomes { get; set; } = new List<string>();
    }
} 