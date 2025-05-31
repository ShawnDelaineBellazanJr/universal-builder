using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Cognition
{
    /// <summary>
    /// Handles optimization (2h) cognitive frequency processing
    /// </summary>
    public class OptimizationReflectionProcessor
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly bool _debugMode;
        
        // Track template improvements across runs
        private static readonly List<TemplateImprovement> _templateImprovements = new List<TemplateImprovement>();
        
        public OptimizationReflectionProcessor(Kernel kernel, bool debugMode = false)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _debugMode = debugMode;
        }
        
        /// <summary>
        /// Reflect on system performance and optimize
        /// </summary>
        public async Task<CognitiveResponse> ReflectAndOptimizeAsync(string goal, int economicValue)
        {
            Console.WriteLine($"Performing optimization reflection: {goal}");
            
            try
            {
                // Check economic justification
                if (economicValue < EconomicConsciousness.OptimizationThreshold)
                {
                    return new CognitiveResponse
                    {
                        Success = false,
                        Message = $"Goal economic value ({economicValue}) below optimization threshold ({EconomicConsciousness.OptimizationThreshold})",
                        Frequency = CognitiveFrequency.Optimization,
                        EconomicValue = economicValue
                    };
                }
                
                // Step 1: Analyze performance metrics
                var performanceMetrics = await AnalyzePerformanceMetricsAsync();
                
                // Step 2: Identify optimization opportunities
                var optimizationOpportunities = await IdentifyOptimizationOpportunitiesAsync(performanceMetrics);
                
                // Step 3: Generate template improvements
                var templateImprovements = await GenerateTemplateImprovementsAsync(optimizationOpportunities);
                
                // Step 4: Check if architecture change is needed
                var (requiresArchitectureChange, architecturalChanges) = await CheckArchitecturalChangeAsync(optimizationOpportunities);
                
                // Step 5: Analyze evolution opportunity
                var (shouldEvolve, evolutionReason) = await AnalyzeEvolutionOpportunityAsync(templateImprovements, requiresArchitectureChange);
                
                // Record template improvements for future reference
                foreach (var improvement in templateImprovements)
                {
                    if (improvement.Confidence > 0.8)
                    {
                        _templateImprovements.Add(improvement);
                    }
                }
                
                // Collect actions performed
                var actions = new List<string>
                {
                    "Analyzed performance metrics",
                    "Identified optimization opportunities",
                    "Generated template improvements",
                    "Checked for architectural change requirements",
                    "Analyzed evolution opportunity"
                };
                
                // Collect artifacts
                var artifacts = new Dictionary<string, string>
                {
                    { "performance_metrics", performanceMetrics },
                    { "optimization_opportunities", optimizationOpportunities },
                    { "template_improvements", FormatTemplateImprovements(templateImprovements) },
                    { "architectural_changes", architecturalChanges }
                };
                
                // Determine if we should escalate to evolution frequency
                CognitiveFrequency? nextFrequency = null;
                if (shouldEvolve)
                {
                    nextFrequency = CognitiveFrequency.Evolution;
                    actions.Add($"Escalated to evolution frequency: {evolutionReason}");
                    artifacts["evolution_reason"] = evolutionReason;
                }
                
                return new CognitiveResponse
                {
                    Success = true,
                    Message = "Optimization reflection completed successfully",
                    Frequency = CognitiveFrequency.Optimization,
                    EconomicValue = economicValue,
                    Actions = actions,
                    Artifacts = artifacts,
                    NextFrequency = nextFrequency
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in optimization processing: {ex.Message}");
                
                if (_debugMode)
                {
                    return new CognitiveResponse
                    {
                        Success = false,
                        Message = $"Debug mode error in optimization processing: {ex.Message}",
                        Frequency = CognitiveFrequency.Optimization,
                        EconomicValue = economicValue,
                        DebugInformation = ex.ToString()
                    };
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Analyze system performance metrics
        /// </summary>
        private async Task<string> AnalyzePerformanceMetricsAsync()
        {
            // In a real implementation, this would use GitHub MCP to analyze actual metrics
            // For now, we'll simulate the result
            
            string systemMessage = @"
You are the Performance Metrics Analyzer for the Universal Autonomous Builder system.
Your task is to simulate analysis of system performance metrics.

Generate a realistic performance metrics analysis covering:
1. System processing times
2. Resource utilization
3. Success rates for different cognitive frequencies
4. Integration effectiveness
5. User satisfaction metrics

Provide a concise metrics analysis as if you had access to real system performance data.
";

            string userMessage = "Analyze performance metrics:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content?.Trim() ?? "Unable to analyze performance metrics";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing performance metrics: {ex.Message}");
                return "Error analyzing performance metrics - using default analysis";
            }
        }
        
        /// <summary>
        /// Identify optimization opportunities
        /// </summary>
        private async Task<string> IdentifyOptimizationOpportunitiesAsync(string performanceMetrics)
        {
            string systemMessage = @"
You are the Optimization Opportunity Identifier for the Universal Autonomous Builder system.
Your task is to identify optimization opportunities based on the performance metrics.

Identify 5-10 specific optimization opportunities covering:
1. Performance bottlenecks
2. Resource inefficiencies
3. Template improvement opportunities
4. Process enhancements
5. User experience improvements

For each opportunity, provide:
- Description of the opportunity
- Expected impact (High, Medium, Low)
- Implementation complexity (High, Medium, Low)
- Priority (1-5, where 1 is highest)
";

            string userMessage = $"Performance Metrics:\n{performanceMetrics}\n\nIdentify optimization opportunities:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                return response.Content?.Trim() ?? "Unable to identify optimization opportunities";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error identifying optimization opportunities: {ex.Message}");
                return "Error identifying optimization opportunities";
            }
        }
        
        /// <summary>
        /// Generate template improvements
        /// </summary>
        private async Task<List<TemplateImprovement>> GenerateTemplateImprovementsAsync(string optimizationOpportunities)
        {
            string systemMessage = @"
You are the Template Improvement Generator for the Universal Autonomous Builder system.
Your task is to generate template improvements based on optimization opportunities.

For each template improvement, provide:
- template_name: Name of the template to improve
- description: Clear description of the improvement
- changes: Specific changes to make
- expected_benefit: Expected benefit of the improvement
- confidence: Confidence score (0.0 to 1.0)

Generate 3-5 specific template improvements in JSON format as an array of objects.
";

            string userMessage = $"Optimization Opportunities:\n{optimizationOpportunities}\n\nGenerate template improvements:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                var content = response.Content?.Trim() ?? "[]";
                
                // Parse improvements from response
                var improvements = new List<TemplateImprovement>();
                
                if (content.Contains("[") && content.Contains("]"))
                {
                    // Extract JSON array
                    int startIndex = content.IndexOf('[');
                    int endIndex = content.LastIndexOf(']') + 1;
                    string jsonArray = content.Substring(startIndex, endIndex - startIndex);
                    
                    // Try to parse as individual JSON objects if full array parsing fails
                    try
                    {
                        // Simple parsing approach for demonstration
                        string[] objects = jsonArray.Trim('[', ']').Split(new[] { "},{" }, StringSplitOptions.None);
                        
                        foreach (var obj in objects)
                        {
                            var improvement = new TemplateImprovement();
                            
                            string cleanObj = obj;
                            if (obj.StartsWith("{"))
                                cleanObj = obj;
                            else if (obj.EndsWith("}"))
                                cleanObj = "{" + obj;
                            else
                                cleanObj = "{" + obj + "}";
                            
                            // Extract fields using simple string parsing
                            improvement.TemplateName = ExtractJsonField(cleanObj, "template_name");
                            improvement.Description = ExtractJsonField(cleanObj, "description");
                            improvement.Changes = ExtractJsonField(cleanObj, "changes");
                            improvement.ExpectedBenefit = ExtractJsonField(cleanObj, "expected_benefit");
                            
                            // Parse confidence
                            string confidenceStr = ExtractJsonField(cleanObj, "confidence");
                            if (float.TryParse(confidenceStr, out float confidence))
                            {
                                improvement.Confidence = confidence;
                            }
                            else
                            {
                                improvement.Confidence = 0.5f; // Default
                            }
                            
                            if (!string.IsNullOrWhiteSpace(improvement.TemplateName))
                            {
                                improvements.Add(improvement);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing template improvements: {ex.Message}");
                    }
                }
                
                // If no improvements were parsed, add a default one
                if (improvements.Count == 0)
                {
                    improvements.Add(new TemplateImprovement
                    {
                        TemplateName = "generic-project.prompty",
                        Description = "Default template improvement",
                        Changes = "Add improved error handling and performance optimizations",
                        ExpectedBenefit = "Better error handling and performance",
                        Confidence = 0.5f
                    });
                }
                
                return improvements;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating template improvements: {ex.Message}");
                
                // Return a default improvement
                return new List<TemplateImprovement>
                {
                    new TemplateImprovement
                    {
                        TemplateName = "generic-project.prompty",
                        Description = "Default template improvement",
                        Changes = "Add improved error handling and performance optimizations",
                        ExpectedBenefit = "Better error handling and performance",
                        Confidence = 0.5f
                    }
                };
            }
        }
        
        /// <summary>
        /// Extract a field from a JSON object string
        /// </summary>
        private string ExtractJsonField(string json, string fieldName)
        {
            string fieldPattern = $"\"{fieldName}\"\\s*:\\s*\"([^\"]*)\"|'{fieldName}'\\s*:\\s*'([^']*)'";
            
            // Simple regex-like parsing for demonstration
            int fieldIndex = json.IndexOf($"\"{fieldName}\":", StringComparison.OrdinalIgnoreCase);
            if (fieldIndex < 0)
                fieldIndex = json.IndexOf($"'{fieldName}':", StringComparison.OrdinalIgnoreCase);
                
            if (fieldIndex < 0)
                return string.Empty;
                
            int valueStartIndex = json.IndexOf(':', fieldIndex) + 1;
            while (valueStartIndex < json.Length && char.IsWhiteSpace(json[valueStartIndex]))
                valueStartIndex++;
                
            if (valueStartIndex >= json.Length)
                return string.Empty;
                
            char delimiter = json[valueStartIndex];
            if (delimiter == '"' || delimiter == '\'')
            {
                // String value
                int valueEndIndex = json.IndexOf(delimiter, valueStartIndex + 1);
                if (valueEndIndex < 0)
                    return string.Empty;
                    
                return json.Substring(valueStartIndex + 1, valueEndIndex - valueStartIndex - 1);
            }
            else
            {
                // Number or boolean value
                int valueEndIndex = json.IndexOfAny(new[] { ',', '}' }, valueStartIndex);
                if (valueEndIndex < 0)
                    valueEndIndex = json.Length;
                    
                return json.Substring(valueStartIndex, valueEndIndex - valueStartIndex).Trim();
            }
        }
        
        /// <summary>
        /// Format template improvements as a string
        /// </summary>
        private string FormatTemplateImprovements(List<TemplateImprovement> improvements)
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine("Template Improvements:");
            
            foreach (var improvement in improvements)
            {
                result.AppendLine($"- Template: {improvement.TemplateName}");
                result.AppendLine($"  Description: {improvement.Description}");
                result.AppendLine($"  Changes: {improvement.Changes}");
                result.AppendLine($"  Expected Benefit: {improvement.ExpectedBenefit}");
                result.AppendLine($"  Confidence: {improvement.Confidence:P0}");
                result.AppendLine();
            }
            
            return result.ToString();
        }
        
        /// <summary>
        /// Check if architectural changes are needed
        /// </summary>
        private async Task<(bool RequiresChange, string Changes)> CheckArchitecturalChangeAsync(string optimizationOpportunities)
        {
            string systemMessage = @"
You are the Architectural Change Analyzer for the Universal Autonomous Builder system.
Your task is to determine if architectural changes are needed based on optimization opportunities.

Architectural changes should be considered when:
1. Multiple components need significant restructuring
2. Core interfaces need to be redesigned
3. New major capabilities need to be added
4. Existing capabilities need to be fundamentally changed
5. Performance issues require architectural solutions

Respond with:
- Either 'ARCHITECTURE_CHANGE: <description>' if changes are needed
- Or 'NO_ARCHITECTURE_CHANGE' if not needed
";

            string userMessage = $"Optimization Opportunities:\n{optimizationOpportunities}\n\nAnalyze architectural change needs:";
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userMessage);
            
            try
            {
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                var content = response.Content?.Trim() ?? "NO_ARCHITECTURE_CHANGE";
                
                if (content.StartsWith("ARCHITECTURE_CHANGE:", StringComparison.OrdinalIgnoreCase))
                {
                    string description = content.Substring("ARCHITECTURE_CHANGE:".Length).Trim();
                    return (true, description);
                }
                
                return (false, "No architectural changes needed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking architectural change: {ex.Message}");
                return (false, "Error checking architectural change - assuming no changes needed");
            }
        }
        
        /// <summary>
        /// Analyze if evolution is needed
        /// </summary>
        private async Task<(bool ShouldEvolve, string Reason)> AnalyzeEvolutionOpportunityAsync(
            List<TemplateImprovement> templateImprovements, 
            bool requiresArchitecturalChange)
        {
            // If architectural change is required, we should evolve
            if (requiresArchitecturalChange)
            {
                return (true, "Architectural changes required");
            }
            
            // If there are high-confidence template improvements, we may need to evolve
            int highConfidenceCount = templateImprovements.Count(i => i.Confidence > 0.8);
            if (highConfidenceCount >= 3)
            {
                return (true, $"{highConfidenceCount} high-confidence template improvements identified");
            }
            
            // If accumulated improvements are significant, we should evolve
            if (_templateImprovements.Count >= 5)
            {
                return (true, $"{_templateImprovements.Count} accumulated template improvements");
            }
            
            // Otherwise, no evolution needed
            return (false, "Insufficient evolution triggers");
        }
    }
    
    /// <summary>
    /// Represents a template improvement
    /// </summary>
    public class TemplateImprovement
    {
        /// <summary>
        /// Name of the template to improve
        /// </summary>
        public string TemplateName { get; set; } = string.Empty;
        
        /// <summary>
        /// Description of the improvement
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Specific changes to make
        /// </summary>
        public string Changes { get; set; } = string.Empty;
        
        /// <summary>
        /// Expected benefit of the improvement
        /// </summary>
        public string ExpectedBenefit { get; set; } = string.Empty;
        
        /// <summary>
        /// Confidence score (0.0 to 1.0)
        /// </summary>
        public float Confidence { get; set; }
    }
} 