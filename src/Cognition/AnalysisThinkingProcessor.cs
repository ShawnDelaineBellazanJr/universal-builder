using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Cognition
{
    /// <summary>
    /// Handles analysis (15m) cognitive frequency processing
    /// </summary>
    public class AnalysisThinkingProcessor
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly bool _debugMode;
        
        public AnalysisThinkingProcessor(Kernel kernel, bool debugMode = false)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _debugMode = debugMode;
        }
        
        /// <summary>
        /// Perform deep analysis on a goal
        /// </summary>
        public async Task<CognitiveResponse> DeepAnalyzeGoalAsync(string goal, int economicValue)
        {
            Console.WriteLine($"Performing deep analysis on goal: {goal}");
            
            try
            {
                // Check economic justification
                if (economicValue < EconomicConsciousness.AnalysisThreshold)
                {
                    return new CognitiveResponse
                    {
                        Success = false,
                        Message = $"Goal economic value ({economicValue}) below analysis threshold ({EconomicConsciousness.AnalysisThreshold})",
                        Frequency = CognitiveFrequency.Analysis,
                        EconomicValue = economicValue
                    };
                }
                
                // Step 1: Chain-of-thought reasoning
                var reasoning = await ExecuteChainOfThoughtAsync(goal);
                
                // Step 2: Mine success patterns
                var patterns = await MineSuccessPatternsAsync(goal);
                
                // Step 3: Determine project type
                var projectType = await DetermineProjectTypeAsync(goal, patterns);
                
                // Step 4: Generate comprehensive build plan
                var buildPlan = await GenerateBuildPlanAsync(goal, projectType, patterns, reasoning);
                
                // Step 5: Analyze opportunity for optimization frequency
                var (shouldOptimize, optimizationReason) = await AnalyzeOptimizationOpportunityAsync(buildPlan);
                
                // Collect actions performed
                var actions = new List<string>
                {
                    "Executed chain-of-thought reasoning",
                    "Mined success patterns",
                    $"Determined project type: {projectType}",
                    "Generated comprehensive build plan",
                    "Analyzed optimization opportunity"
                };
                
                // Collect artifacts
                var artifacts = new Dictionary<string, string>
                {
                    { "chain_of_thought", reasoning },
                    { "success_patterns", patterns },
                    { "project_type", projectType },
                    { "build_plan", buildPlan }
                };
                
                // Determine if we should escalate to optimization frequency
                CognitiveFrequency? nextFrequency = null;
                if (shouldOptimize)
                {
                    nextFrequency = CognitiveFrequency.Optimization;
                    actions.Add($"Escalated to optimization frequency: {optimizationReason}");
                    artifacts["optimization_reason"] = optimizationReason;
                }
                
                return new CognitiveResponse
                {
                    Success = true,
                    Message = "Deep analysis completed successfully",
                    Frequency = CognitiveFrequency.Analysis,
                    EconomicValue = economicValue,
                    Actions = actions,
                    Artifacts = artifacts,
                    NextFrequency = nextFrequency
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in analysis processing: {ex.Message}");
                
                if (_debugMode)
                {
                    return new CognitiveResponse
                    {
                        Success = false,
                        Message = $"Debug mode error in analysis processing: {ex.Message}",
                        Frequency = CognitiveFrequency.Analysis,
                        EconomicValue = economicValue,
                        DebugInformation = ex.ToString()
                    };
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Execute chain-of-thought reasoning for a goal
        /// </summary>
        private async Task<string> ExecuteChainOfThoughtAsync(string goal)
        {
            string systemMessage = @"
You are the Analysis Thinking Processor for the Universal Autonomous Builder system.
Your task is to perform chain-of-thought reasoning about a goal.

Follow these steps:
1. Break down the goal into components
2. Analyze the context and requirements
3. Identify key challenges and considerations
4. Explore potential approaches
5. Recommend the best approach with justification

Provide detailed reasoning that shows your step-by-step thinking process.
";

            string userMessage = $"Goal: {goal}\n\nPerform chain-of-thought reasoning:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content?.Trim() ?? "Unable to perform chain-of-thought reasoning";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in chain-of-thought reasoning: {ex.Message}");
                return $"Error in chain-of-thought reasoning: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Mine success patterns for a goal
        /// </summary>
        private async Task<string> MineSuccessPatternsAsync(string goal)
        {
            // In a real implementation, this would use GitHub MCP to mine patterns
            // For now, we'll simulate the result
            
            string systemMessage = @"
You are the Pattern Mining Processor for the Universal Autonomous Builder system.
Your task is to identify success patterns relevant to the goal.

Imagine you have access to a database of successful projects. Identify:
1. Common architectural patterns relevant to this goal
2. Typical challenges and how they were overcome
3. Reusable components or approaches
4. Best practices for this type of project
5. Anti-patterns to avoid

Provide a concise summary of 5-10 success patterns.
";

            string userMessage = $"Goal: {goal}\n\nIdentify success patterns:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content?.Trim() ?? "Unable to mine success patterns";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error mining success patterns: {ex.Message}");
                return $"Error mining success patterns: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Determine the project type for a goal
        /// </summary>
        private async Task<string> DetermineProjectTypeAsync(string goal, string patterns)
        {
            string systemMessage = @"
You are the Project Type Classifier for the Universal Autonomous Builder system.
Your task is to determine the most appropriate project type for the goal.

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

Provide ONLY the project type as a single word or phrase from the list above.
";

            string userMessage = $"Goal: {goal}\nPatterns: {patterns}\n\nDetermine project type:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                var content = response.Content?.Trim() ?? "web-service";
                
                // Ensure we get a valid project type
                var validTypes = new[] 
                {
                    "ios-app", "android-app", "web-service", "web-app", "data-pipeline",
                    "ml-model", "cli-tool", "desktop-app", "game", "smart-contract",
                    "embedded-system", "autonomous-agent", "template-evolution", "sk-enhancement"
                };
                
                foreach (var type in validTypes)
                {
                    if (content.Contains(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return type;
                    }
                }
                
                return "web-service"; // Default
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error determining project type: {ex.Message}");
                return "web-service"; // Default
            }
        }
        
        /// <summary>
        /// Generate a comprehensive build plan
        /// </summary>
        private async Task<string> GenerateBuildPlanAsync(string goal, string projectType, string patterns, string reasoning)
        {
            string systemMessage = @"
You are the Build Plan Generator for the Universal Autonomous Builder system.
Your task is to create a comprehensive build plan for the goal.

Include the following sections:
1. Executive Summary
2. Architecture Overview
3. Component Breakdown
4. Implementation Steps
5. Testing Strategy
6. Deployment Plan
7. Maintenance Considerations

Keep the plan well-structured and actionable, focusing on specific steps to achieve the goal.
";

            string userMessage = $"Goal: {goal}\nProject Type: {projectType}\nPatterns: {patterns}\nReasoning: {reasoning}\n\nGenerate build plan:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content?.Trim() ?? "Unable to generate build plan";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating build plan: {ex.Message}");
                return $"Error generating build plan: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Analyze if this goal presents an opportunity for optimization
        /// </summary>
        private async Task<(bool ShouldOptimize, string Reason)> AnalyzeOptimizationOpportunityAsync(string buildPlan)
        {
            string systemMessage = @"
You are the Optimization Opportunity Analyzer for the Universal Autonomous Builder system.
Your task is to determine if this build plan presents an opportunity for optimization frequency processing.

Optimization frequency should be considered when:
1. The plan involves significant performance improvements
2. The plan addresses architectural improvements
3. The plan includes template enhancements
4. The plan involves system-wide changes
5. The economic value of optimization would be high

Respond with:
- Either 'OPTIMIZE: <reason>' if optimization is recommended
- Or 'NO_OPTIMIZATION' if not recommended
";

            string userMessage = $"Build Plan: {buildPlan}\n\nAnalyze optimization opportunity:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                var content = response.Content?.Trim() ?? "NO_OPTIMIZATION";
                
                if (content.StartsWith("OPTIMIZE:", StringComparison.OrdinalIgnoreCase))
                {
                    string reason = content.Substring("OPTIMIZE:".Length).Trim();
                    return (true, reason);
                }
                
                return (false, "No optimization opportunity identified");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing optimization opportunity: {ex.Message}");
                return (false, $"Error analyzing optimization opportunity: {ex.Message}");
            }
        }
    }
} 