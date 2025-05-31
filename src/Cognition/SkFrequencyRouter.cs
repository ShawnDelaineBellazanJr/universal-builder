using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AutonomousAI.Cognition
{
    /// <summary>
    /// Semantic Kernel based Frequency Router 
    /// Uses SK to determine the appropriate cognitive frequency for a given task
    /// </summary>
    public class SkFrequencyRouter
    {
        private readonly Kernel _kernel;
        private readonly Dictionary<string, FrequencyConfig> _frequencies;
        private readonly IChatCompletionService _chatService;

        public SkFrequencyRouter(Kernel kernel, Dictionary<string, FrequencyConfig> frequencies)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _frequencies = frequencies ?? throw new ArgumentNullException(nameof(frequencies));
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
        }

        /// <summary>
        /// Determines the most appropriate cognitive frequency for a given goal
        /// </summary>
        /// <param name="goal">The goal to analyze</param>
        /// <param name="context">Additional context about the goal</param>
        /// <returns>The recommended frequency name and its configuration</returns>
        public async Task<(string Name, FrequencyConfig Config)> DetermineFrequencyAsync(string goal, string context = "")
        {
            // Use SK to analyze the goal and recommend a frequency
            var chatHistory = new ChatHistory();
            
            // Use the enhanced prompt for better accuracy
            chatHistory.AddSystemMessage(SkFrequencyRouterPrompts.FrequencyDeterminationPrompt);
            
            chatHistory.AddUserMessage($"Goal: {goal}\nContext: {context}");
            
            var response = await _chatService.GetChatMessageContentAsync(chatHistory);
            string recommendedFrequency = response.Content.Trim().ToLower();
            
            // Validate the response against available frequencies
            if (_frequencies.TryGetValue(recommendedFrequency, out var frequencyConfig))
            {
                return (recommendedFrequency, frequencyConfig);
            }
            
            // Default to analysis frequency if the response doesn't match
            return ("analysis", _frequencies["analysis"]);
        }

        /// <summary>
        /// Calculates the economic value of processing a goal at a specific frequency
        /// </summary>
        /// <param name="goal">The goal to evaluate</param>
        /// <param name="frequency">The frequency to evaluate</param>
        /// <returns>An economic value score (0-100)</returns>
        public async Task<int> CalculateEconomicValueAsync(string goal, string frequency)
        {
            var chatHistory = new ChatHistory();
            
            // Use the enhanced prompt for better economic evaluation
            chatHistory.AddSystemMessage(SkFrequencyRouterPrompts.EconomicValuePrompt);
            
            chatHistory.AddUserMessage($"Goal: {goal}\nFrequency: {frequency}");
            
            var response = await _chatService.GetChatMessageContentAsync(chatHistory);
            
            if (int.TryParse(response.Content.Trim(), out int economicValue))
            {
                return Math.Clamp(economicValue, 0, 100);
            }
            
            // Default value if parsing fails
            return 50;
        }
        
        /// <summary>
        /// Gets a sample goal for the specified frequency
        /// Useful for testing and demonstrations
        /// </summary>
        public string GetSampleGoal(string frequency)
        {
            var sampleGoals = SkFrequencyRouterPrompts.GetSampleGoals();
            if (sampleGoals.TryGetValue(frequency.ToLower(), out var goals) && goals.Count > 0)
            {
                var random = new Random();
                return goals[random.Next(goals.Count)];
            }
            
            return "Analyze system performance";  // Default sample goal
        }
    }

    public class FrequencyConfig
    {
        public string Interval { get; set; }
        public int Threshold { get; set; }
    }
} 