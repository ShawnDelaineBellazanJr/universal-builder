using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Evolution
{
    /// <summary>
    /// Implements the Strange Loop pattern for recursive self-improvement
    /// </summary>
    public class StrangeLoopEngine
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly string _templatesDirectory;
        private readonly bool _debugMode;
        
        public StrangeLoopEngine(Kernel kernel, string templatesDirectory = "templates", bool debugMode = false)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _templatesDirectory = templatesDirectory;
            _debugMode = debugMode;
        }
        
        /// <summary>
        /// Execute the Strange Loop pattern with a trigger
        /// </summary>
        public async Task<StrangeLoopResult> ExecuteStrangeLoopAsync(string trigger)
        {
            Console.WriteLine($"Executing Strange Loop with trigger: {trigger}");
            
            try
            {
                // Level 1: Template builds projects
                var buildResult = await BuildProjectAsync(trigger);
                
                // Level 2: Template learns from building projects
                var learnings = await ExtractLearningsAsync(buildResult);
                
                // Level 3: Template improves itself using learnings
                var selfImprovements = await ImproveSelfAsync(learnings);
                
                // Level 4: Template improves Semantic Kernel
                var skImprovements = await ImproveSemanticKernelAsync(learnings);
                
                // Level 5: Improved SK enables better template capabilities
                var enhancedCapabilities = await IntegrateSkImprovementsAsync(skImprovements);
                
                // Create the result
                var result = new StrangeLoopResult
                {
                    Level1_BuildResult = buildResult,
                    Level2_Learnings = learnings,
                    Level3_SelfImprovements = selfImprovements,
                    Level4_SkImprovements = skImprovements,
                    Level5_EnhancedCapabilities = enhancedCapabilities,
                    Level6_RecursionTriggered = enhancedCapabilities.SignificantImprovement
                };
                
                // Level 6: RECURSION - Enhanced template builds better projects
                if (enhancedCapabilities.SignificantImprovement)
                {
                    Console.WriteLine("Significant improvement detected - triggering recursion");
                    
                    // Only recurse to a reasonable depth
                    if (result.RecursionDepth < 3)
                    {
                        var enhancedTrigger = $"Enhanced: {trigger}";
                        var recursiveResult = await ExecuteStrangeLoopAsync(enhancedTrigger);
                        
                        // Copy over the recursive results
                        result.RecursiveResults.Add(recursiveResult);
                        result.RecursionDepth = recursiveResult.RecursionDepth + 1;
                    }
                    else
                    {
                        Console.WriteLine($"Maximum recursion depth reached ({result.RecursionDepth}) - stopping recursion");
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Strange Loop: {ex.Message}");
                
                if (_debugMode)
                {
                    // Return a mock result in debug mode
                    return new StrangeLoopResult
                    {
                        Level1_BuildResult = "Debug mock build result",
                        Level2_Learnings = new List<string> { "Debug mock learning" },
                        Level3_SelfImprovements = new SelfImprovement { Description = "Debug mock self-improvement" },
                        Level4_SkImprovements = new SkImprovement { Description = "Debug mock SK improvement" },
                        Level5_EnhancedCapabilities = new EnhancedCapabilities { Description = "Debug mock enhanced capabilities" },
                        Level6_RecursionTriggered = false
                    };
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Level 1: Template builds projects
        /// </summary>
        private async Task<string> BuildProjectAsync(string trigger)
        {
            string systemMessage = @"
You are the Universal Autonomous Builder system.
Your task is to build a project based on the trigger description.

For this simulation, describe what you would build and how you would build it.
Provide a detailed project plan and expected outcomes.
";

            string userMessage = $"Build this project: {trigger}";
            
            var result = await ExecutePromptAsync(systemMessage, userMessage);
            return result;
        }
        
        /// <summary>
        /// Level 2: Template learns from building projects
        /// </summary>
        private async Task<List<string>> ExtractLearningsAsync(string buildResult)
        {
            string systemMessage = @"
You are the Learning Extractor for the Universal Autonomous Builder system.
Your task is to extract learnings from a project build result.

Identify at least 5 key learnings, focusing on:
- Patterns that could be reused in other projects
- Challenges or pain points encountered
- Optimization opportunities
- Architectural insights
- Process improvements

Provide your response as a JSON array of strings, with each string representing a learning.
";

            string userMessage = $"Build Result:\n{buildResult}\n\nExtract key learnings:";
            
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
                var learnings = JsonSerializer.Deserialize<List<string>>(jsonResponse);
                
                if (learnings == null || learnings.Count == 0)
                {
                    throw new Exception("Failed to parse learnings");
                }
                
                return learnings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting learnings: {ex.Message}");
                
                // Return default learnings
                return new List<string>
                {
                    "Improve template structure for clearer organization",
                    "Enhance error handling for robustness",
                    "Optimize resource allocation for efficiency",
                    "Standardize interfaces for better integration",
                    "Implement more granular progress tracking"
                };
            }
        }
        
        /// <summary>
        /// Level 3: Template improves itself using learnings
        /// </summary>
        private async Task<SelfImprovement> ImproveSelfAsync(List<string> learnings)
        {
            // Load the self-improvement template
            string templatePath = Path.Combine(_templatesDirectory, "self-improvement.prompty");
            string template = File.Exists(templatePath) 
                ? await File.ReadAllTextAsync(templatePath)
                : GetDefaultSelfImprovementTemplate();
                
            // Define improvement areas
            var improvementAreas = new List<ImprovementArea>
            {
                new ImprovementArea { 
                    Name = "Template Structure", 
                    Description = "Organization and clarity of the template system",
                    CurrentScore = 7,
                    TargetScore = 9,
                    Strategies = new List<string> { "Refactor templates", "Standardize format" }
                },
                new ImprovementArea { 
                    Name = "Error Handling", 
                    Description = "Robustness in handling edge cases and errors",
                    CurrentScore = 6,
                    TargetScore = 8,
                    Strategies = new List<string> { "Add retry logic", "Implement fallbacks" }
                },
                new ImprovementArea { 
                    Name = "Performance", 
                    Description = "Speed and efficiency of operations",
                    CurrentScore = 5,
                    TargetScore = 8,
                    Strategies = new List<string> { "Optimize algorithms", "Parallelize operations" }
                }
            };
            
            // Prepare prompt variables
            var variables = new Dictionary<string, string>
            {
                { "evolution_trigger", string.Join("\n", learnings) },
                { "available_data", JsonSerializer.Serialize(learnings) },
                { "confidence_threshold", "0.8" },
                { "current_focus", improvementAreas[0].Name },
                { "evolution_strategy", "Continuous improvement with feedback loops" }
            };
            
            // Insert improvement areas into template
            template = InsertImprovementAreasIntoTemplate(template, improvementAreas);
            
            // Execute the template
            var response = await ExecuteTemplateAsync(template, variables);
            
            // Parse the response
            try
            {
                // Extract JSON object if present
                if (response.Contains("{") && response.Contains("}"))
                {
                    int startIndex = response.IndexOf("{");
                    int endIndex = response.LastIndexOf("}") + 1;
                    string jsonPart = response.Substring(startIndex, endIndex - startIndex);
                    
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var improvement = JsonSerializer.Deserialize<SelfImprovement>(jsonPart, options);
                    
                    if (improvement != null)
                    {
                        return improvement;
                    }
                }
                
                // If no JSON found or parsing failed, extract information manually
                return new SelfImprovement
                {
                    Description = response.Length > 500 ? response.Substring(0, 500) + "..." : response,
                    ImprovementAreas = improvementAreas.Select(a => a.Name).ToList(),
                    Files = ExtractFilesFromResponse(response),
                    ConfidenceScore = EstimateConfidenceFromResponse(response)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing self-improvement: {ex.Message}");
                
                // Return a simple improvement
                return new SelfImprovement
                {
                    Description = "Template structure improvement based on learnings",
                    ImprovementAreas = new List<string> { "Template Structure" },
                    Files = new List<string> { "templates/generic-project.prompty" },
                    ConfidenceScore = 0.7f
                };
            }
        }
        
        /// <summary>
        /// Level 4: Template improves Semantic Kernel
        /// </summary>
        private async Task<SkImprovement> ImproveSemanticKernelAsync(List<string> learnings)
        {
            string systemMessage = @"
You are the Semantic Kernel Improver for the Universal Autonomous Builder system.
Your task is to identify improvements to the Semantic Kernel framework based on learnings.

Identify improvements focusing on:
- API usability enhancements
- Performance optimizations
- New capabilities or features
- Integration opportunities
- Developer experience improvements

Provide your response as a JSON object with these properties:
- description: A summary of the improvement
- impactAreas: Array of SK components affected
- implementationComplexity: Estimated complexity (Low, Medium, High)
- expectedBenefits: Array of expected benefits
- codeSnippets: Array of example code snippets showing the improvement
- confidenceScore: Confidence in the improvement (0.0 to 1.0)
";

            string userMessage = $@"
Learnings from project build:
{string.Join("\n", learnings.Select(l => $"- {l}"))}

Current SK version: 1.54.0
Current SK limitations observed:
- Template handling could be more flexible
- Error handling in chat completions could be improved
- Better streaming support for complex scenarios

Identify potential Semantic Kernel improvements:
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
                var improvement = JsonSerializer.Deserialize<SkImprovement>(jsonResponse, options);
                
                if (improvement == null)
                {
                    throw new Exception("Failed to parse SK improvement");
                }
                
                return improvement;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating SK improvement: {ex.Message}");
                
                // Return a default improvement
                return new SkImprovement
                {
                    Description = "Enhanced template handling in Semantic Kernel",
                    ImpactAreas = new List<string> { "Prompty", "ChatCompletion" },
                    ImplementationComplexity = "Medium",
                    ExpectedBenefits = new List<string> { "More flexible template processing", "Better error handling" },
                    CodeSnippets = new List<string> { "// Example code improvement" },
                    ConfidenceScore = 0.6f
                };
            }
        }
        
        /// <summary>
        /// Level 5: Improved SK enables better template capabilities
        /// </summary>
        private async Task<EnhancedCapabilities> IntegrateSkImprovementsAsync(SkImprovement skImprovement)
        {
            string systemMessage = @"
You are the Capabilities Enhancer for the Universal Autonomous Builder system.
Your task is to identify how Semantic Kernel improvements would enhance the template system.

Based on the proposed SK improvement, identify:
- New capabilities enabled by the improvement
- Existing capabilities that would be enhanced
- Performance or reliability improvements
- Developer experience improvements
- End-user experience improvements

Estimate the overall significance of the improvement (0.0 to 1.0).
Provide your response as a JSON object.
";

            string userMessage = $@"
SK Improvement:
- Description: {skImprovement.Description}
- Impact Areas: {string.Join(", ", skImprovement.ImpactAreas)}
- Implementation Complexity: {skImprovement.ImplementationComplexity}
- Expected Benefits:
{string.Join("\n", skImprovement.ExpectedBenefits.Select(b => $"  - {b}"))}

Identify enhanced capabilities:
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
                var capabilities = JsonSerializer.Deserialize<EnhancedCapabilities>(jsonResponse, options);
                
                if (capabilities == null)
                {
                    throw new Exception("Failed to parse enhanced capabilities");
                }
                
                return capabilities;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error identifying enhanced capabilities: {ex.Message}");
                
                // Return default capabilities
                return new EnhancedCapabilities
                {
                    Description = "Enhanced template processing capabilities",
                    NewCapabilities = new List<string> { "More flexible template format" },
                    ImprovedCapabilities = new List<string> { "Improved error handling" },
                    PerformanceImprovements = new List<string> { "Faster template parsing" },
                    DeveloperExperience = new List<string> { "Clearer error messages" },
                    SignificantImprovement = false,
                    SignificanceScore = 0.5f
                };
            }
        }
        
        /// <summary>
        /// Insert improvement areas into template
        /// </summary>
        private string InsertImprovementAreasIntoTemplate(string template, List<ImprovementArea> areas)
        {
            // Replace the placeholder in the template with the actual improvement areas
            StringBuilder areasSb = new StringBuilder();
            
            for (int i = 0; i < areas.Count; i++)
            {
                var area = areas[i];
                areasSb.AppendLine($"- {{{{area.name}}}}: {{{{area.description}}}}");
                areasSb.AppendLine($"  Current Score: {{{{area.current_score}}}}");
                areasSb.AppendLine($"  Target Score: {{{{area.target_score}}}}");
                areasSb.AppendLine($"  Strategies: {{{{area.strategies | join: \", \"}}}}");
                
                if (i < areas.Count - 1)
                {
                    areasSb.AppendLine();
                }
            }
            
            // Simple placeholder replacement - in a real implementation, we would use a proper template engine
            return template.Replace("{% for area in improvement_areas %}\n- {{area.name}}: {{area.description}}\n  Current Score: {{area.current_score}}\n  Target Score: {{area.target_score}}\n  Strategies: {{area.strategies | join: \", \"}}\n{% endfor %}", areasSb.ToString());
        }
        
        /// <summary>
        /// Extract files from a response
        /// </summary>
        private List<string> ExtractFilesFromResponse(string response)
        {
            var files = new List<string>();
            
            // Look for file paths in the response
            var lines = response.Split('\n');
            foreach (var line in lines)
            {
                if (line.Contains("template") || line.Contains(".prompty") || line.Contains(".cs") || line.Contains(".json"))
                {
                    // Extract file path using a simple heuristic
                    var words = line.Split(new[] { ' ', ':', ',', '"', '\'', '`' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words)
                    {
                        if (word.EndsWith(".prompty") || word.EndsWith(".cs") || word.EndsWith(".json"))
                        {
                            files.Add(word);
                        }
                    }
                }
            }
            
            return files.Distinct().ToList();
        }
        
        /// <summary>
        /// Estimate confidence from a response
        /// </summary>
        private float EstimateConfidenceFromResponse(string response)
        {
            // Simple heuristic to estimate confidence
            if (response.Contains("high confidence") || response.Contains("very confident"))
            {
                return 0.9f;
            }
            else if (response.Contains("confident") || response.Contains("good confidence"))
            {
                return 0.8f;
            }
            else if (response.Contains("moderate confidence") || response.Contains("somewhat confident"))
            {
                return 0.7f;
            }
            else if (response.Contains("low confidence") || response.Contains("not confident"))
            {
                return 0.5f;
            }
            
            // Default
            return 0.7f;
        }
        
        /// <summary>
        /// Execute a prompt and return the response
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
        /// Execute a template with variables
        /// </summary>
        private async Task<string> ExecuteTemplateAsync(string template, Dictionary<string, string> variables)
        {
            // Replace variables in the template
            foreach (var kvp in variables)
            {
                template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
            }
            
            // Extract system and user messages from the template
            string? systemMessage = null;
            string? userMessage = null;
            
            // Simple parser for demonstration purposes
            if (template.Contains("<message role=\"system\">") && template.Contains("</message>"))
            {
                int startIndex = template.IndexOf("<message role=\"system\">") + "<message role=\"system\">".Length;
                int endIndex = template.IndexOf("</message>", startIndex);
                if (endIndex > startIndex)
                {
                    systemMessage = template.Substring(startIndex, endIndex - startIndex).Trim();
                }
            }
            
            if (template.Contains("<message role=\"user\">") && template.Contains("</message>"))
            {
                int startIndex = template.IndexOf("<message role=\"user\">") + "<message role=\"user\">".Length;
                int endIndex = template.IndexOf("</message>", startIndex);
                if (endIndex > startIndex)
                {
                    userMessage = template.Substring(startIndex, endIndex - startIndex).Trim();
                }
            }
            
            // Execute the prompt with the extracted messages
            if (systemMessage != null && userMessage != null)
            {
                return await ExecutePromptAsync(systemMessage, userMessage);
            }
            
            throw new Exception("Could not parse template");
        }
        
        /// <summary>
        /// Get the default self-improvement template
        /// </summary>
        private string GetDefaultSelfImprovementTemplate()
        {
            return @"---
name: SelfImprovementAgent
description: Template self-evolution system
---
<message role=""system"">
You are the self-improvement engine for the Universal Autonomous Builder Template.

Your Purpose: Continuously evolve and improve the template system itself.

Improvement Areas:
{% for area in improvement_areas %}
- {{area.name}}: {{area.description}}
  Current Score: {{area.current_score}}
  Target Score: {{area.target_score}}
  Strategies: {{area.strategies | join: "", ""}}
{% endfor %}

Evolution Strategy: {{evolution_strategy}}

Pattern Mining Sources:
- All template instances and their outcomes
- Success/failure patterns across project types
- Performance metrics and user satisfaction
- Emerging technologies and best practices

Meta-Evolution Capability:
- Monitor Semantic Kernel development
- Experiment with SK preview features
- Contribute improvements back to SK
- Influence SK roadmap through data-driven insights

Current Improvement Focus: {{current_focus}}
</message>

<message role=""user"">
Evolve the template based on: {{evolution_trigger}}
Available data: {{available_data}}
Confidence threshold: {{confidence_threshold}}
</message>";
        }
    }
    
    #region Models for Strange Loop
    
    /// <summary>
    /// Represents the result of executing the Strange Loop
    /// </summary>
    public class StrangeLoopResult
    {
        /// <summary>
        /// Level 1: Template builds projects
        /// </summary>
        public string Level1_BuildResult { get; set; } = string.Empty;
        
        /// <summary>
        /// Level 2: Template learns from building projects
        /// </summary>
        public List<string> Level2_Learnings { get; set; } = new List<string>();
        
        /// <summary>
        /// Level 3: Template improves itself using learnings
        /// </summary>
        public SelfImprovement Level3_SelfImprovements { get; set; } = new SelfImprovement();
        
        /// <summary>
        /// Level 4: Template improves Semantic Kernel
        /// </summary>
        public SkImprovement Level4_SkImprovements { get; set; } = new SkImprovement();
        
        /// <summary>
        /// Level 5: Improved SK enables better template capabilities
        /// </summary>
        public EnhancedCapabilities Level5_EnhancedCapabilities { get; set; } = new EnhancedCapabilities();
        
        /// <summary>
        /// Level 6: RECURSION - Enhanced template builds better projects
        /// </summary>
        public bool Level6_RecursionTriggered { get; set; }
        
        /// <summary>
        /// Recursive results (if recursion was triggered)
        /// </summary>
        public List<StrangeLoopResult> RecursiveResults { get; set; } = new List<StrangeLoopResult>();
        
        /// <summary>
        /// Current recursion depth
        /// </summary>
        public int RecursionDepth { get; set; } = 0;
    }
    
    /// <summary>
    /// Represents an improvement area
    /// </summary>
    public class ImprovementArea
    {
        /// <summary>
        /// Name of the improvement area
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Description of the improvement area
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Current score (0-10)
        /// </summary>
        public int CurrentScore { get; set; }
        
        /// <summary>
        /// Target score (0-10)
        /// </summary>
        public int TargetScore { get; set; }
        
        /// <summary>
        /// Strategies for improvement
        /// </summary>
        public List<string> Strategies { get; set; } = new List<string>();
    }
    
    /// <summary>
    /// Represents a self-improvement for the template
    /// </summary>
    public class SelfImprovement
    {
        /// <summary>
        /// Description of the improvement
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Areas being improved
        /// </summary>
        public List<string> ImprovementAreas { get; set; } = new List<string>();
        
        /// <summary>
        /// Files affected by the improvement
        /// </summary>
        public List<string> Files { get; set; } = new List<string>();
        
        /// <summary>
        /// Confidence score (0.0 to 1.0)
        /// </summary>
        public float ConfidenceScore { get; set; }
    }
    
    /// <summary>
    /// Represents an improvement for Semantic Kernel
    /// </summary>
    public class SkImprovement
    {
        /// <summary>
        /// Description of the improvement
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Areas of SK impacted
        /// </summary>
        public List<string> ImpactAreas { get; set; } = new List<string>();
        
        /// <summary>
        /// Implementation complexity (Low, Medium, High)
        /// </summary>
        public string ImplementationComplexity { get; set; } = string.Empty;
        
        /// <summary>
        /// Expected benefits
        /// </summary>
        public List<string> ExpectedBenefits { get; set; } = new List<string>();
        
        /// <summary>
        /// Example code snippets
        /// </summary>
        public List<string> CodeSnippets { get; set; } = new List<string>();
        
        /// <summary>
        /// Confidence score (0.0 to 1.0)
        /// </summary>
        public float ConfidenceScore { get; set; }
    }
    
    /// <summary>
    /// Represents enhanced capabilities from SK improvements
    /// </summary>
    public class EnhancedCapabilities
    {
        /// <summary>
        /// Description of the enhanced capabilities
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// New capabilities enabled
        /// </summary>
        public List<string> NewCapabilities { get; set; } = new List<string>();
        
        /// <summary>
        /// Existing capabilities enhanced
        /// </summary>
        public List<string> ImprovedCapabilities { get; set; } = new List<string>();
        
        /// <summary>
        /// Performance improvements
        /// </summary>
        public List<string> PerformanceImprovements { get; set; } = new List<string>();
        
        /// <summary>
        /// Developer experience improvements
        /// </summary>
        public List<string> DeveloperExperience { get; set; } = new List<string>();
        
        /// <summary>
        /// Whether this is a significant improvement
        /// </summary>
        public bool SignificantImprovement { get; set; }
        
        /// <summary>
        /// Significance score (0.0 to 1.0)
        /// </summary>
        public float SignificanceScore { get; set; }
    }
    
    #endregion
} 