using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Cognition
{
    /// <summary>
    /// Provides economic consciousness for the multi-frequency cognitive architecture
    /// </summary>
    public class EconomicConsciousness
    {
        // Value thresholds for different cognitive frequencies
        public const int ImmediateThreshold = 95;  // Only critical goals
        public const int ContinuousThreshold = 50; // Low-cost monitoring
        public const int AnalysisThreshold = 70;   // Thoughtful consideration
        public const int OptimizationThreshold = 85; // High-value improvements
        public const int EvolutionThreshold = 90;  // Transformational changes
        
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly bool _debugMode;
        
        public EconomicConsciousness(Kernel kernel, bool debugMode = false)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _debugMode = debugMode;
        }
        
        /// <summary>
        /// Evaluate the economic value of a goal (0-100)
        /// </summary>
        public async Task<int> EvaluateGoalValueAsync(string goal)
        {
            if (string.IsNullOrWhiteSpace(goal))
                return 0;
                
            Console.WriteLine($"Evaluating economic value of goal: {goal}");
            
            try
            {
                // Check for critical keywords first for fast evaluation
                var criticalKeywords = new[] { 
                    "emergency", "critical", "security", "vulnerability", "broken",
                    "down", "crash", "outage", "failure", "breach", "attack"
                };
                
                foreach (var keyword in criticalKeywords)
                {
                    if (goal.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        // Critical goals get highest priority
                        return 95;
                    }
                }
                
                // Check for important evolution/architecture keywords
                var evolutionKeywords = new[] {
                    "evolution", "architecture", "transform", "optimize", "improve",
                    "enhance", "foundation", "framework", "paradigm", "meta-evolution"
                };
                
                int baseValue = 50; // Default moderate value
                
                foreach (var keyword in evolutionKeywords)
                {
                    if (goal.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        baseValue += 10;
                    }
                }
                
                // Cap at 90 for keyword-based evaluation
                baseValue = Math.Min(baseValue, 90);
                
                // If value is still moderate, use AI evaluation
                if (baseValue < 80)
                {
                    var aiValue = await EvaluateValueWithAIAsync(goal);
                    
                    // Blend keyword-based and AI-based evaluation
                    baseValue = (baseValue + aiValue) / 2;
                }
                
                return Math.Min(baseValue, 100);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error evaluating economic value: {ex.Message}");
                
                if (_debugMode)
                {
                    // In debug mode, return a moderate value
                    return 75;
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Evaluate a goal's value using AI
        /// </summary>
        private async Task<int> EvaluateValueWithAIAsync(string goal)
        {
            string systemMessage = @"
You are an economic evaluator for the Universal Autonomous Builder system.
Your task is to assess the economic value of a goal on a scale from 0 to 100, where:
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

            string userMessage = $"Goal: {goal}\n\nAssess the economic value:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                var content = response.Content ?? "50";
                
                // Try to extract a number from the response
                string numberStr = "";
                foreach (var c in content)
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
                
                // Default to moderate value if parsing fails
                return 50;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AI value evaluation: {ex.Message}");
                
                if (_debugMode)
                {
                    return 60; // Slightly above default in debug mode
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Justify resource usage for a given frequency and goal value
        /// </summary>
        public async Task<ResourceJustification> JustifyResourceUsageAsync(
            CognitiveFrequency frequency, 
            int goalValue)
        {
            var threshold = GetThresholdForFrequency(frequency);
            bool justified = goalValue >= threshold;
            
            var justification = new ResourceJustification
            {
                Justified = justified,
                Reason = justified
                    ? $"Goal value {goalValue} exceeds {frequency} threshold {threshold}"
                    : $"Goal value {goalValue} below {frequency} threshold {threshold}"
            };
            
            // If not justified, suggest an alternative frequency
            if (!justified)
            {
                justification.AlternativeFrequency = SuggestAlternativeFrequency(goalValue);
            }
            
            return justification;
        }
        
        /// <summary>
        /// Get the value threshold for a frequency
        /// </summary>
        private int GetThresholdForFrequency(CognitiveFrequency frequency)
        {
            return frequency switch
            {
                CognitiveFrequency.Immediate => ImmediateThreshold,
                CognitiveFrequency.Continuous => ContinuousThreshold,
                CognitiveFrequency.Analysis => AnalysisThreshold,
                CognitiveFrequency.Optimization => OptimizationThreshold,
                CognitiveFrequency.Evolution => EvolutionThreshold,
                _ => 100 // Default to a high threshold
            };
        }
        
        /// <summary>
        /// Suggest an alternative frequency based on goal value
        /// </summary>
        private CognitiveFrequency? SuggestAlternativeFrequency(int goalValue)
        {
            // Try to find a frequency that matches the goal value
            if (goalValue >= ContinuousThreshold && goalValue < AnalysisThreshold)
                return CognitiveFrequency.Continuous;
                
            if (goalValue >= AnalysisThreshold && goalValue < OptimizationThreshold)
                return CognitiveFrequency.Analysis;
                
            if (goalValue >= OptimizationThreshold && goalValue < EvolutionThreshold)
                return CognitiveFrequency.Optimization;
                
            if (goalValue >= EvolutionThreshold && goalValue < ImmediateThreshold)
                return CognitiveFrequency.Evolution;
                
            // If value is below all thresholds, don't suggest an alternative
            if (goalValue < ContinuousThreshold)
                return null;
                
            return CognitiveFrequency.Analysis; // Default to analysis
        }
    }
} 