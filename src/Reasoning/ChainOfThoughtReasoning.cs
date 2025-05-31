using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Reasoning
{
    /// <summary>
    /// Provides chain-of-thought reasoning capabilities
    /// </summary>
    public class ChainOfThoughtReasoning
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly bool _debugMode;
        
        public ChainOfThoughtReasoning(Kernel kernel, bool debugMode = false)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _debugMode = debugMode;
        }
        
        /// <summary>
        /// Performs chain-of-thought reasoning about a goal
        /// </summary>
        public async Task<ReasoningResult> ReasonAboutGoalAsync(string goal)
        {
            Console.WriteLine($"Performing chain-of-thought reasoning for goal: {goal}");
            
            var reasoning = new StringBuilder();
            
            try
            {
                // Step 1: Goal Decomposition
                reasoning.AppendLine("## Goal Analysis");
                reasoning.AppendLine($"Input Goal: {goal}");
                
                var decomposition = await DecomposeGoalAsync(goal);
                reasoning.AppendLine($"Decomposed Components: {string.Join(", ", decomposition)}");
                
                // Step 2: Context Analysis
                reasoning.AppendLine("\n## Context Analysis");
                var context = await AnalyzeContextAsync(goal);
                reasoning.AppendLine($"Project Type: {context.ProjectType}");
                reasoning.AppendLine($"Complexity: {context.Complexity}");
                reasoning.AppendLine($"Dependencies: {string.Join(", ", context.Dependencies)}");
                
                // Step 3: Strategy Selection
                reasoning.AppendLine("\n## Strategy Selection");
                var strategies = await GenerateStrategiesAsync(decomposition, context);
                foreach (var strategy in strategies)
                {
                    reasoning.AppendLine($"Strategy: {strategy.Name} (Confidence: {strategy.Confidence})");
                    reasoning.AppendLine($"  Approach: {strategy.Approach}");
                    reasoning.AppendLine($"  Expected Outcome: {strategy.ExpectedOutcome}");
                }
                
                // Step 4: Execution Planning
                reasoning.AppendLine("\n## Execution Planning");
                var selectedStrategy = strategies.OrderByDescending(s => s.Confidence).First();
                var executionPlan = await CreateExecutionPlanAsync(selectedStrategy);
                
                foreach (var step in executionPlan.Steps)
                {
                    reasoning.AppendLine($"Step {step.Order}: {step.Name}");
                    reasoning.AppendLine($"  Purpose: {step.Purpose}");
                    reasoning.AppendLine($"  Tools: {string.Join(", ", step.Tools)}");
                    reasoning.AppendLine($"  Success Criteria: {step.SuccessCriteria}");
                }
                
                // Step 5: Risk Assessment
                reasoning.AppendLine("\n## Risk Assessment");
                var risks = await AssessRisksAsync(executionPlan);
                foreach (var risk in risks)
                {
                    reasoning.AppendLine($"Risk: {risk.Description} (Probability: {risk.Probability}, Impact: {risk.Impact})");
                    reasoning.AppendLine($"  Mitigation: {risk.Mitigation}");
                }
                
                // Step 6: Recursive Improvement Opportunity
                reasoning.AppendLine("\n## Recursive Improvement Opportunities");
                var recursionOpportunities = await IdentifyRecursionOpportunitiesAsync(executionPlan);
                foreach (var opportunity in recursionOpportunities)
                {
                    reasoning.AppendLine($"Improvement: {opportunity.Description}");
                    reasoning.AppendLine($"  Trigger: {opportunity.Trigger}");
                    reasoning.AppendLine($"  Expected Benefit: {opportunity.ExpectedBenefit}");
                }
                
                return new ReasoningResult
                {
                    ChainOfThought = reasoning.ToString(),
                    SelectedStrategy = selectedStrategy,
                    ExecutionPlan = executionPlan,
                    RecursionOpportunities = recursionOpportunities
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in chain-of-thought reasoning: {ex.Message}");
                
                if (_debugMode)
                {
                    // Return a mock result in debug mode
                    return new ReasoningResult
                    {
                        ChainOfThought = $"Debug mock chain-of-thought reasoning for: {goal}",
                        SelectedStrategy = new Strategy { Name = "Mock Strategy", Confidence = 0.8f },
                        ExecutionPlan = new ExecutionPlan { Steps = new List<ExecutionStep>() },
                        RecursionOpportunities = new List<RecursionOpportunity>()
                    };
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Decomposes a goal into components
        /// </summary>
        private async Task<List<string>> DecomposeGoalAsync(string goal)
        {
            string systemMessage = @"
You are an expert goal analyst working on the Universal Autonomous Builder system.
Your task is to decompose a complex goal into simpler components.

Good goal decomposition:
- Breaks down the goal into manageable parts
- Identifies independent components
- Follows a logical sequence
- Maintains the original intent
- Makes implicit requirements explicit

Provide your response as a JSON array of strings, with each string representing a decomposed component.
";

            string userMessage = $"Goal: {goal}\n\nDecompose this goal into components:";
            
            return await ExecuteAndParseJsonArrayAsync<string>(systemMessage, userMessage) ?? 
                new List<string> { goal }; // Fallback if parsing fails
        }
        
        /// <summary>
        /// Analyzes the context of a goal
        /// </summary>
        private async Task<Context> AnalyzeContextAsync(string goal)
        {
            string systemMessage = @"
You are an expert context analyst working on the Universal Autonomous Builder system.
Your task is to analyze the context of a goal.

Provide the following information:
- projectType: The type of project (web-service, ios-app, etc.)
- complexity: A numeric assessment of complexity (1-10)
- dependencies: An array of external dependencies or systems
- constraints: An array of constraints or limitations
- assumptions: An array of assumptions being made

Provide your response as a JSON object.
";

            string userMessage = $"Goal: {goal}\n\nAnalyze the context of this goal:";
            
            try
            {
                var jsonResponse = await ExecutePromptAsync(systemMessage, userMessage);
                
                // Extract JSON object
                if (jsonResponse.Contains("{") && jsonResponse.Contains("}"))
                {
                    int startIndex = jsonResponse.IndexOf("{");
                    int endIndex = jsonResponse.LastIndexOf("}") + 1;
                    jsonResponse = jsonResponse.Substring(startIndex, endIndex - startIndex);
                }
                
                // Parse JSON
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var context = JsonSerializer.Deserialize<Context>(jsonResponse, options);
                
                if (context == null)
                {
                    throw new Exception("Failed to parse context");
                }
                
                return context;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing context: {ex.Message}");
                
                // Return default context
                return new Context
                {
                    ProjectType = "web-service",
                    Complexity = 5,
                    Dependencies = new List<string>(),
                    Constraints = new List<string>(),
                    Assumptions = new List<string>()
                };
            }
        }
        
        /// <summary>
        /// Generates strategies based on goal components and context
        /// </summary>
        private async Task<List<Strategy>> GenerateStrategiesAsync(List<string> components, Context context)
        {
            string systemMessage = @"
You are an expert strategist working on the Universal Autonomous Builder system.
Your task is to generate multiple strategies for achieving a goal.

For each strategy, provide:
- name: A descriptive name
- approach: The overall approach or methodology
- confidence: A confidence score (0.0 to 1.0)
- expectedOutcome: The expected outcome if this strategy succeeds
- risks: An array of potential risks
- timeEstimate: Estimated time to implement (in hours)

Generate at least 3 different strategies.
Provide your response as a JSON array of strategy objects.
";

            string userMessage = $@"
Goal Components:
{string.Join("\n", components.Select(c => $"- {c}"))}

Context:
- Project Type: {context.ProjectType}
- Complexity: {context.Complexity}
- Dependencies: {string.Join(", ", context.Dependencies)}
- Constraints: {string.Join(", ", context.Constraints)}
- Assumptions: {string.Join(", ", context.Assumptions)}

Generate strategies:
";
            
            return await ExecuteAndParseJsonArrayAsync<Strategy>(systemMessage, userMessage) ?? 
                new List<Strategy> { new Strategy { Name = "Default Strategy", Confidence = 0.5f } }; // Fallback
        }
        
        /// <summary>
        /// Creates an execution plan for a strategy
        /// </summary>
        private async Task<ExecutionPlan> CreateExecutionPlanAsync(Strategy strategy)
        {
            string systemMessage = @"
You are an expert planner working on the Universal Autonomous Builder system.
Your task is to create a detailed execution plan for a strategy.

For each step in the plan, provide:
- order: The step number (1-based)
- name: A descriptive name
- purpose: The purpose of this step
- tools: An array of tools needed
- successCriteria: How to determine if the step succeeded
- estimatedTime: Estimated time to complete (in minutes)

Provide your response as a JSON object with a 'steps' property containing an array of step objects.
";

            string userMessage = $@"
Strategy:
- Name: {strategy.Name}
- Approach: {strategy.Approach}
- Expected Outcome: {strategy.ExpectedOutcome}

Create an execution plan:
";
            
            try
            {
                var jsonResponse = await ExecutePromptAsync(systemMessage, userMessage);
                
                // Extract JSON object
                if (jsonResponse.Contains("{") && jsonResponse.Contains("}"))
                {
                    int startIndex = jsonResponse.IndexOf("{");
                    int endIndex = jsonResponse.LastIndexOf("}") + 1;
                    jsonResponse = jsonResponse.Substring(startIndex, endIndex - startIndex);
                }
                
                // Parse JSON
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var plan = JsonSerializer.Deserialize<ExecutionPlan>(jsonResponse, options);
                
                if (plan == null || plan.Steps == null || plan.Steps.Count == 0)
                {
                    throw new Exception("Failed to parse execution plan");
                }
                
                return plan;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating execution plan: {ex.Message}");
                
                // Return default plan
                return new ExecutionPlan
                {
                    Steps = new List<ExecutionStep>
                    {
                        new ExecutionStep { Order = 1, Name = "Plan", Purpose = "Create a detailed plan" },
                        new ExecutionStep { Order = 2, Name = "Implement", Purpose = "Implement the solution" },
                        new ExecutionStep { Order = 3, Name = "Test", Purpose = "Test the implementation" },
                        new ExecutionStep { Order = 4, Name = "Deploy", Purpose = "Deploy the solution" }
                    }
                };
            }
        }
        
        /// <summary>
        /// Assesses risks for an execution plan
        /// </summary>
        private async Task<List<Risk>> AssessRisksAsync(ExecutionPlan plan)
        {
            string systemMessage = @"
You are an expert risk analyst working on the Universal Autonomous Builder system.
Your task is to assess risks for an execution plan.

For each risk, provide:
- description: A clear description of the risk
- probability: The probability of occurrence (Low, Medium, High)
- impact: The potential impact (Low, Medium, High)
- mitigation: A strategy to mitigate the risk

Identify at least 3 potential risks.
Provide your response as a JSON array of risk objects.
";

            string userMessage = $@"
Execution Plan:
{string.Join("\n", plan.Steps.Select(s => $"- Step {s.Order}: {s.Name} - {s.Purpose}"))}

Assess risks:
";
            
            return await ExecuteAndParseJsonArrayAsync<Risk>(systemMessage, userMessage) ?? 
                new List<Risk> { new Risk { Description = "Default risk", Probability = "Medium", Impact = "Medium" } }; // Fallback
        }
        
        /// <summary>
        /// Identifies opportunities for recursive improvement
        /// </summary>
        private async Task<List<RecursionOpportunity>> IdentifyRecursionOpportunitiesAsync(ExecutionPlan plan)
        {
            string systemMessage = @"
You are an expert on recursive self-improvement working on the Universal Autonomous Builder system.
Your task is to identify opportunities for recursive improvement in an execution plan.

For each opportunity, provide:
- description: A clear description of the improvement opportunity
- trigger: What would trigger this improvement
- expectedBenefit: The expected benefit from this improvement
- confidence: Confidence in the benefit (0.0 to 1.0)

Identify at least 2 opportunities for recursive improvement.
Provide your response as a JSON array of opportunity objects.
";

            string userMessage = $@"
Execution Plan:
{string.Join("\n", plan.Steps.Select(s => $"- Step {s.Order}: {s.Name} - {s.Purpose}"))}

Identify recursive improvement opportunities:
";
            
            return await ExecuteAndParseJsonArrayAsync<RecursionOpportunity>(systemMessage, userMessage) ?? 
                new List<RecursionOpportunity> { new RecursionOpportunity { Description = "Default opportunity" } }; // Fallback
        }
        
        /// <summary>
        /// Executes a prompt and returns the raw response
        /// </summary>
        private async Task<string> ExecutePromptAsync(string systemMessage, string userMessage)
        {
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing prompt: {ex.Message}");
                
                if (_debugMode)
                {
                    // Return a mock response in debug mode
                    return $"DEBUG MODE: Response to prompt: {userMessage.Substring(0, Math.Min(50, userMessage.Length))}...";
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Executes a prompt and parses the response as a JSON array
        /// </summary>
        private async Task<List<T>?> ExecuteAndParseJsonArrayAsync<T>(string systemMessage, string userMessage)
        {
            try
            {
                var jsonResponse = await ExecutePromptAsync(systemMessage, userMessage);
                
                // Extract JSON array
                if (jsonResponse.Contains("[") && jsonResponse.Contains("]"))
                {
                    int startIndex = jsonResponse.IndexOf("[");
                    int endIndex = jsonResponse.LastIndexOf("]") + 1;
                    jsonResponse = jsonResponse.Substring(startIndex, endIndex - startIndex);
                }
                
                // Parse JSON
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<T>>(jsonResponse, options);
                
                if (result == null || result.Count == 0)
                {
                    throw new Exception($"Failed to parse JSON array of {typeof(T).Name}");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON array: {ex.Message}");
                return null;
            }
        }
    }
    
    #region Models for Chain of Thought Reasoning
    
    /// <summary>
    /// Represents the result of chain-of-thought reasoning
    /// </summary>
    public class ReasoningResult
    {
        /// <summary>
        /// The full chain-of-thought reasoning process
        /// </summary>
        public string ChainOfThought { get; set; } = string.Empty;
        
        /// <summary>
        /// The selected strategy
        /// </summary>
        public Strategy? SelectedStrategy { get; set; }
        
        /// <summary>
        /// The execution plan
        /// </summary>
        public ExecutionPlan? ExecutionPlan { get; set; }
        
        /// <summary>
        /// Opportunities for recursive improvement
        /// </summary>
        public List<RecursionOpportunity> RecursionOpportunities { get; set; } = new List<RecursionOpportunity>();
    }
    
    /// <summary>
    /// Represents the context of a goal
    /// </summary>
    public class Context
    {
        /// <summary>
        /// The type of project
        /// </summary>
        public string ProjectType { get; set; } = string.Empty;
        
        /// <summary>
        /// The complexity of the project (1-10)
        /// </summary>
        public int Complexity { get; set; }
        
        /// <summary>
        /// External dependencies or systems
        /// </summary>
        public List<string> Dependencies { get; set; } = new List<string>();
        
        /// <summary>
        /// Constraints or limitations
        /// </summary>
        public List<string> Constraints { get; set; } = new List<string>();
        
        /// <summary>
        /// Assumptions being made
        /// </summary>
        public List<string> Assumptions { get; set; } = new List<string>();
    }
    
    /// <summary>
    /// Represents a strategy for achieving a goal
    /// </summary>
    public class Strategy
    {
        /// <summary>
        /// Descriptive name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Overall approach or methodology
        /// </summary>
        public string Approach { get; set; } = string.Empty;
        
        /// <summary>
        /// Confidence score (0.0 to 1.0)
        /// </summary>
        public float Confidence { get; set; }
        
        /// <summary>
        /// Expected outcome if successful
        /// </summary>
        public string ExpectedOutcome { get; set; } = string.Empty;
        
        /// <summary>
        /// Potential risks
        /// </summary>
        public List<string> Risks { get; set; } = new List<string>();
        
        /// <summary>
        /// Estimated time to implement (in hours)
        /// </summary>
        public float TimeEstimate { get; set; }
    }
    
    /// <summary>
    /// Represents an execution plan
    /// </summary>
    public class ExecutionPlan
    {
        /// <summary>
        /// Steps in the execution plan
        /// </summary>
        public List<ExecutionStep> Steps { get; set; } = new List<ExecutionStep>();
    }
    
    /// <summary>
    /// Represents a step in an execution plan
    /// </summary>
    public class ExecutionStep
    {
        /// <summary>
        /// Step number (1-based)
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// Descriptive name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Purpose of the step
        /// </summary>
        public string Purpose { get; set; } = string.Empty;
        
        /// <summary>
        /// Tools needed
        /// </summary>
        public List<string> Tools { get; set; } = new List<string>();
        
        /// <summary>
        /// Success criteria
        /// </summary>
        public string SuccessCriteria { get; set; } = string.Empty;
        
        /// <summary>
        /// Estimated time to complete (in minutes)
        /// </summary>
        public int EstimatedTime { get; set; }
    }
    
    /// <summary>
    /// Represents a risk
    /// </summary>
    public class Risk
    {
        /// <summary>
        /// Description of the risk
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Probability of occurrence (Low, Medium, High)
        /// </summary>
        public string Probability { get; set; } = string.Empty;
        
        /// <summary>
        /// Potential impact (Low, Medium, High)
        /// </summary>
        public string Impact { get; set; } = string.Empty;
        
        /// <summary>
        /// Mitigation strategy
        /// </summary>
        public string Mitigation { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Represents an opportunity for recursive improvement
    /// </summary>
    public class RecursionOpportunity
    {
        /// <summary>
        /// Description of the opportunity
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// What would trigger this improvement
        /// </summary>
        public string Trigger { get; set; } = string.Empty;
        
        /// <summary>
        /// Expected benefit
        /// </summary>
        public string ExpectedBenefit { get; set; } = string.Empty;
        
        /// <summary>
        /// Confidence in the benefit (0.0 to 1.0)
        /// </summary>
        public float Confidence { get; set; }
    }
    
    #endregion
} 