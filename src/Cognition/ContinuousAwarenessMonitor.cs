using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Cognition
{
    /// <summary>
    /// Handles continuous (30s) cognitive frequency monitoring
    /// </summary>
    public class ContinuousAwarenessMonitor
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly bool _debugMode;
        
        private Dictionary<string, object> _awarenessState = new Dictionary<string, object>();
        private DateTime _lastSignificantEventTime = DateTime.MinValue;
        
        public ContinuousAwarenessMonitor(Kernel kernel, bool debugMode = false)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _debugMode = debugMode;
        }
        
        /// <summary>
        /// Monitor the ecosystem continuously
        /// </summary>
        public async Task<CognitiveResponse> MonitorEcosystemAsync(string goal = "continuous monitoring")
        {
            Console.WriteLine("Performing continuous ecosystem monitoring");
            
            try
            {
                // Get repository health status
                var repositoryHealth = await CheckRepositoryHealthAsync();
                
                // Monitor template instances
                var instanceStates = await MonitorTemplateInstancesAsync();
                
                // Check for significant events
                var significantEvents = await CheckForSignificantEventsAsync();
                
                // Update awareness state
                _awarenessState["repository_health"] = repositoryHealth;
                _awarenessState["instance_states"] = instanceStates;
                _awarenessState["significant_events"] = significantEvents;
                _awarenessState["last_check_time"] = DateTime.UtcNow;
                
                // Determine if we need to escalate to analysis frequency
                bool shouldEscalate = significantEvents.Count > 0 || 
                                      repositoryHealth.Contains("attention needed") ||
                                      instanceStates.Contains("issue detected");
                
                // Collect actions performed
                var actions = new List<string>
                {
                    "Checked repository health",
                    "Monitored template instances",
                    "Checked for significant events"
                };
                
                // Collect artifacts
                var artifacts = new Dictionary<string, string>
                {
                    { "repository_health", repositoryHealth },
                    { "instance_states", instanceStates },
                    { "significant_events", string.Join(", ", significantEvents) }
                };
                
                // Determine next frequency based on findings
                CognitiveFrequency? nextFrequency = null;
                if (shouldEscalate)
                {
                    nextFrequency = CognitiveFrequency.Analysis;
                    actions.Add("Escalated to analysis frequency due to significant findings");
                }
                
                return new CognitiveResponse
                {
                    Success = true,
                    Message = shouldEscalate ? "Monitoring detected issues requiring analysis" : "Continuous monitoring completed successfully",
                    Frequency = CognitiveFrequency.Continuous,
                    Actions = actions,
                    Artifacts = artifacts,
                    NextFrequency = nextFrequency
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in continuous monitoring: {ex.Message}");
                
                if (_debugMode)
                {
                    return new CognitiveResponse
                    {
                        Success = false,
                        Message = $"Debug mode error in continuous monitoring: {ex.Message}",
                        Frequency = CognitiveFrequency.Continuous,
                        DebugInformation = ex.ToString()
                    };
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Check the health of the repository
        /// </summary>
        private async Task<string> CheckRepositoryHealthAsync()
        {
            // In a real implementation, this would use GitHub MCP to check repository health
            // For now, we'll simulate the result
            
            if (_debugMode)
            {
                return "Debug mode: Repository health normal";
            }
            
            try
            {
                string systemMessage = @"
You are the repository health monitor for the Universal Autonomous Builder system.
Based on the current time and awareness state, simulate a repository health check.

Possible health states:
- normal: Everything is functioning normally
- attention_needed: Some minor issues that should be addressed
- critical: Serious issues requiring immediate attention

Provide a brief health report (50-100 characters) with one of these states.
";

                string userMessage = $"Current time: {DateTime.UtcNow}\nLast significant event: {_lastSignificantEventTime}\n\nCheck repository health:";
                
                var chatHistory = new ChatHistory();
                chatHistory.AddSystemMessage(systemMessage);
                chatHistory.AddUserMessage(userMessage);
                
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content?.Trim() ?? "Unable to check repository health";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking repository health: {ex.Message}");
                return "Error checking repository health - assuming normal state";
            }
        }
        
        /// <summary>
        /// Monitor the status of template instances
        /// </summary>
        private async Task<string> MonitorTemplateInstancesAsync()
        {
            // In a real implementation, this would use GitHub MCP to monitor template instances
            // For now, we'll simulate the result
            
            if (_debugMode)
            {
                return "Debug mode: 3 instances running normally";
            }
            
            try
            {
                string systemMessage = @"
You are the template instance monitor for the Universal Autonomous Builder system.
Based on the current time and awareness state, simulate monitoring of template instances.

Provide a brief status report (50-100 characters) on template instances.
Include either 'all instances normal', 'issue detected', or 'critical issue detected'.
";

                string userMessage = $"Current time: {DateTime.UtcNow}\nLast check time: {(_awarenessState.ContainsKey("last_check_time") ? _awarenessState["last_check_time"].ToString() : "never")}\n\nMonitor template instances:";
                
                var chatHistory = new ChatHistory();
                chatHistory.AddSystemMessage(systemMessage);
                chatHistory.AddUserMessage(userMessage);
                
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content?.Trim() ?? "Unable to monitor template instances";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error monitoring template instances: {ex.Message}");
                return "Error monitoring template instances - assuming normal state";
            }
        }
        
        /// <summary>
        /// Check for significant events requiring attention
        /// </summary>
        private async Task<List<string>> CheckForSignificantEventsAsync()
        {
            // In a real implementation, this would use GitHub MCP to check for significant events
            // For now, we'll simulate the result
            
            if (_debugMode)
            {
                return new List<string>();
            }
            
            try
            {
                string systemMessage = @"
You are the significant event detector for the Universal Autonomous Builder system.
Based on the current time, simulate a check for significant events.

If you should simulate a significant event, provide 1-2 short event descriptions.
If no significant events, return an empty list.

Examples of significant events:
- New pull request requires review
- Template instance crashed in repository X
- New Semantic Kernel preview package detected
- Security vulnerability in dependency

Respond with either an empty JSON array [] if no events, or a JSON array of event strings.
";

                string userMessage = $"Current time: {DateTime.UtcNow}\nLast significant event: {_lastSignificantEventTime}\n\nCheck for significant events:";
                
                var chatHistory = new ChatHistory();
                chatHistory.AddSystemMessage(systemMessage);
                chatHistory.AddUserMessage(userMessage);
                
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                var content = response.Content?.Trim() ?? "[]";
                
                // Parse events from response
                var events = new List<string>();
                
                if (content.Contains("[") && content.Contains("]"))
                {
                    // Extract array content
                    int startIndex = content.IndexOf('[');
                    int endIndex = content.LastIndexOf(']');
                    string arrayContent = content.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
                    
                    // If array is not empty, parse events
                    if (!string.IsNullOrWhiteSpace(arrayContent) && arrayContent != "")
                    {
                        // Split by commas and clean up quotes
                        string[] eventStrings = arrayContent.Split(',');
                        foreach (string eventString in eventStrings)
                        {
                            string cleanEvent = eventString.Trim();
                            
                            // Remove quotes if present
                            if (cleanEvent.StartsWith("\"") && cleanEvent.EndsWith("\""))
                            {
                                cleanEvent = cleanEvent.Substring(1, cleanEvent.Length - 2);
                            }
                            
                            if (!string.IsNullOrWhiteSpace(cleanEvent))
                            {
                                events.Add(cleanEvent);
                                _lastSignificantEventTime = DateTime.UtcNow;
                            }
                        }
                    }
                }
                
                return events;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking for significant events: {ex.Message}");
                return new List<string>();
            }
        }
    }
} 