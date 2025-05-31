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
            
            chatHistory.AddSystemMessage(@"You are an AI cognitive frequency router. Your task is to analyze a goal and determine 
which cognitive frequency is most appropriate for processing it. The available frequencies are:

- Immediate (0s): For emergencies and critical tasks requiring instant response
- Continuous (30s): For background monitoring and lightweight processing
- Analysis (15m): For tasks requiring deeper thinking and analysis
- Optimization (2h): For improvement tasks and optimizations
- Evolution (24h): For architectural changes and major improvements

Consider factors like urgency, complexity, potential impact, and resource requirements when making your decision.
Respond with ONLY the name of the most appropriate frequency, nothing else.");
            
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
            
            chatHistory.AddSystemMessage(@"You are an AI economic evaluator for cognitive processing. Your task is to 
determine the economic value (as a score from 0-100) of processing a goal at a specific cognitive frequency.
Consider factors like:
- Urgency of the goal
- Complexity of the task
- Resource requirements
- Potential impact
- Cost/benefit ratio
Respond with ONLY a numeric score from 0-100, nothing else.");
            
            chatHistory.AddUserMessage($"Goal: {goal}\nFrequency: {frequency}");
            
            var response = await _chatService.GetChatMessageContentAsync(chatHistory);
            
            if (int.TryParse(response.Content.Trim(), out int economicValue))
            {
                return Math.Clamp(economicValue, 0, 100);
            }
            
            // Default value if parsing fails
            return 50;
        }
    }

    public class FrequencyConfig
    {
        public string Interval { get; set; }
        public int Threshold { get; set; }
    }
} 