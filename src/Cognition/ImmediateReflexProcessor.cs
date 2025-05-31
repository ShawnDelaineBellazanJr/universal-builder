using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Cognition
{
    /// <summary>
    /// Handles immediate (0s) cognitive frequency processing
    /// </summary>
    public class ImmediateReflexProcessor
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly bool _debugMode;
        
        public ImmediateReflexProcessor(Kernel kernel, bool debugMode = false)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _debugMode = debugMode;
        }
        
        /// <summary>
        /// Process a critical goal that requires immediate response
        /// </summary>
        public async Task<CognitiveResponse> ProcessCriticalGoalAsync(string goal, int economicValue)
        {
            Console.WriteLine($"Processing critical goal with immediate cognitive frequency: {goal}");
            
            try
            {
                // If economic value isn't high enough for immediate processing, defer to analysis
                if (economicValue < EconomicConsciousness.ImmediateThreshold)
                {
                    return new CognitiveResponse
                    {
                        Success = true,
                        Message = "Goal deferred to analysis frequency due to insufficient economic value",
                        Frequency = CognitiveFrequency.Immediate,
                        EconomicValue = economicValue,
                        NextFrequency = CognitiveFrequency.Analysis
                    };
                }
                
                // Classify the emergency type
                var emergencyType = await ClassifyEmergencyAsync(goal);
                
                // Create a unique emergency branch name
                string emergencyBranch = $"emergency-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
                
                // Generate the appropriate immediate response
                var response = await GenerateImmediateResponseAsync(goal, emergencyType);
                
                // Collect actions performed
                var actions = new List<string>
                {
                    $"Created emergency branch: {emergencyBranch}",
                    $"Classified emergency as: {emergencyType}",
                    $"Generated immediate response plan"
                };
                
                // Generate next steps
                var nextSteps = await GenerateNextStepsAsync(goal, emergencyType);
                
                return new CognitiveResponse
                {
                    Success = true,
                    Message = "Immediate response generated",
                    Frequency = CognitiveFrequency.Immediate,
                    EconomicValue = economicValue,
                    Actions = actions,
                    Artifacts = new Dictionary<string, string>
                    {
                        { "emergency_branch", emergencyBranch },
                        { "emergency_type", emergencyType },
                        { "immediate_response", response },
                        { "next_steps", nextSteps }
                    },
                    NextFrequency = CognitiveFrequency.Analysis // Escalate to thinking
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in immediate processing: {ex.Message}");
                
                if (_debugMode)
                {
                    return new CognitiveResponse
                    {
                        Success = false,
                        Message = $"Debug mode error in immediate processing: {ex.Message}",
                        Frequency = CognitiveFrequency.Immediate,
                        EconomicValue = economicValue,
                        DebugInformation = ex.ToString()
                    };
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Classify the type of emergency
        /// </summary>
        private async Task<string> ClassifyEmergencyAsync(string goal)
        {
            string systemMessage = @"
You are an emergency classifier for the Universal Autonomous Builder system.
Your task is to classify the type of emergency based on the goal description.

Emergency types:
- security_breach: Security vulnerabilities, attacks, or breaches
- system_failure: System crashes, failures, or outages
- data_corruption: Data loss, corruption, or integrity issues
- performance_issue: Critical performance degradation
- compliance_violation: Legal or compliance issues
- other_critical: Other critical issues

Provide ONLY the emergency type as a single word or phrase from the list above.
";

            string userMessage = $"Classify this emergency: {goal}";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                var content = response.Content?.Trim() ?? "other_critical";
                
                // Ensure we get a valid emergency type
                var validTypes = new[] 
                {
                    "security_breach", "system_failure", "data_corruption", 
                    "performance_issue", "compliance_violation", "other_critical"
                };
                
                foreach (var type in validTypes)
                {
                    if (content.Contains(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return type;
                    }
                }
                
                return "other_critical"; // Default
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error classifying emergency: {ex.Message}");
                return "other_critical"; // Default on error
            }
        }
        
        /// <summary>
        /// Generate an immediate response for the emergency
        /// </summary>
        private async Task<string> GenerateImmediateResponseAsync(string goal, string emergencyType)
        {
            string systemMessage = @"
You are the immediate response generator for the Universal Autonomous Builder system.
Your task is to generate an immediate response plan for an emergency.

Provide a concise, actionable response plan with:
1. Immediate containment steps
2. Quick diagnostic actions
3. Key stakeholders to notify
4. Initial remediation steps
5. Data preservation instructions

Keep your response under 500 characters and focus on immediate actions only.
";

            string userMessage = $"Emergency Goal: {goal}\nEmergency Type: {emergencyType}\n\nGenerate immediate response:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content?.Trim() ?? "Unable to generate immediate response";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating immediate response: {ex.Message}");
                return "Error generating immediate response - fallback to standard emergency protocol";
            }
        }
        
        /// <summary>
        /// Generate next steps after immediate response
        /// </summary>
        private async Task<string> GenerateNextStepsAsync(string goal, string emergencyType)
        {
            string systemMessage = @"
You are the next steps generator for the Universal Autonomous Builder system.
After immediate emergency response, you need to plan follow-up actions.

Provide 3-5 next steps that should happen after immediate response, focusing on:
1. Comprehensive analysis
2. Root cause investigation 
3. Long-term fixes
4. Verification and testing
5. Prevention measures

Keep your response under 500 characters and focus on what should happen after immediate response.
";

            string userMessage = $"Emergency Goal: {goal}\nEmergency Type: {emergencyType}\n\nGenerate next steps:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content?.Trim() ?? "Unable to generate next steps";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating next steps: {ex.Message}");
                return "1. Analyze root cause\n2. Develop comprehensive fix\n3. Test and verify\n4. Document incident";
            }
        }
    }
} 