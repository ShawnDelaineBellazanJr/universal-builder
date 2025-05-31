using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Agents
{
    /// <summary>
    /// Strategic Planner Agent that creates a detailed build plan
    /// </summary>
    public class StrategicPlanner : BaseAgent
    {
        private readonly string _templatesDirectory;
        
        public StrategicPlanner(Kernel kernel, string templatesDirectory = "templates", string model = "gpt-4", bool debugMode = false)
            : base(kernel, model, debugMode)
        {
            _templatesDirectory = templatesDirectory;
        }
        
        /// <summary>
        /// Create a build plan based on the processed goal
        /// </summary>
        /// <param name="goal">The processed goal</param>
        /// <returns>A detailed build plan</returns>
        public async Task<BuildPlan> CreateBuildPlanAsync(ProcessedGoal goal)
        {
            Console.WriteLine($"Creating build plan for: {goal.Intent}");
            
            try
            {
                // Step 1: Load the project type template
                var template = await LoadTemplateAsync(goal.ProjectType);
                
                // Step 2: Generate build steps
                var steps = await GenerateBuildStepsAsync(goal, template);
                
                // Step 3: Plan resource allocation
                var resources = await PlanResourcesAsync(goal, steps);
                
                // Step 4: Analyze dependencies
                var dependencies = await AnalyzeDependenciesAsync(steps);
                
                // Step 5: Define success criteria
                var successCriteria = await DefineSuccessCriteriaAsync(goal);
                
                // Step 6: Generate architecture overview
                var architecture = await GenerateArchitectureAsync(goal, steps);
                
                // Create the build plan
                var buildPlan = new BuildPlan
                {
                    Name = $"Plan for {goal.Intent}",
                    Steps = steps,
                    Resources = resources,
                    Dependencies = dependencies,
                    SuccessCriteria = successCriteria,
                    Architecture = architecture,
                    EstimatedTotalTimeMinutes = steps.Sum(s => s.EstimatedTimeMinutes)
                };
                
                return buildPlan;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating build plan: {ex.Message}");
                
                if (_debugMode)
                {
                    // Return a mock build plan in debug mode
                    return CreateMockBuildPlan(goal);
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Load a project type template
        /// </summary>
        private async Task<string> LoadTemplateAsync(string projectType)
        {
            string templatePath = Path.Combine(_templatesDirectory, $"{projectType}.prompty");
            
            if (File.Exists(templatePath))
            {
                return await File.ReadAllTextAsync(templatePath);
            }
            
            // Fall back to generic template if specific one doesn't exist
            string genericPath = Path.Combine(_templatesDirectory, "generic-project.prompty");
            if (File.Exists(genericPath))
            {
                return await File.ReadAllTextAsync(genericPath);
            }
            
            // Last resort, return a simple template
            return @"---
name: GenericProjectBuilder
description: Generic project builder template
---
<message role=""system"">
You are an autonomous project builder specialized in creating software projects.

Project Configuration:
- Type: {{project.type}}
- Complexity: {{project.complexity}}

Build Steps:
1. Requirements analysis
2. System design
3. Implementation
4. Testing
5. Deployment

Current Goal: {{goal}}
</message>";
        }
        
        /// <summary>
        /// Generate build steps based on the goal and template
        /// </summary>
        private async Task<List<BuildStep>> GenerateBuildStepsAsync(ProcessedGoal goal, string template)
        {
            string systemMessage = @"
You are an expert project planner working on the Universal Autonomous Builder system.
Your task is to generate a detailed list of build steps for a project based on the goal and requirements.

Each step should include:
- A clear name
- A detailed description
- Tools required to execute the step
- Success criteria
- Dependencies on other steps
- Estimated time to complete (in minutes)
- Risk level (1-5)
- Potential risks
- Risk mitigation strategies

Generate at least 5 and at most 15 steps, ensuring they cover the entire build process from planning to deployment.
Provide your response as a JSON array of step objects.
";

            string userMessage = $@"
Goal: {goal.Intent}
Project Type: {goal.ProjectType}
Complexity: {goal.Complexity}
Requirements:
{string.Join("\n", goal.Requirements.Select(r => $"- {r}"))}

Template Info:
{template.Substring(0, Math.Min(500, template.Length))}

Generate build steps:
";
            
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
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var stepDtos = JsonSerializer.Deserialize<List<BuildStepDto>>(jsonResponse, options);
                
                if (stepDtos == null || stepDtos.Count == 0)
                {
                    throw new Exception("Failed to parse build steps");
                }
                
                // Convert DTOs to BuildStep objects
                var steps = new List<BuildStep>();
                for (int i = 0; i < stepDtos.Count; i++)
                {
                    var dto = stepDtos[i];
                    steps.Add(new BuildStep
                    {
                        Id = Guid.NewGuid().ToString(),
                        Order = i + 1,
                        Name = dto.Name,
                        Description = dto.Description,
                        Tools = dto.Tools ?? new List<string>(),
                        SuccessCriteria = dto.SuccessCriteria,
                        Dependencies = dto.Dependencies ?? new List<string>(),
                        EstimatedTimeMinutes = dto.EstimatedTimeMinutes,
                        RiskLevel = dto.RiskLevel,
                        Risks = dto.Risks ?? new List<string>(),
                        RiskMitigation = dto.RiskMitigation ?? new List<string>()
                    });
                }
                
                return steps;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing build steps: {ex.Message}");
                
                // Return a set of default steps
                return CreateDefaultBuildSteps(goal);
            }
        }
        
        /// <summary>
        /// Plan resources required for the build
        /// </summary>
        private async Task<BuildResources> PlanResourcesAsync(ProcessedGoal goal, List<BuildStep> steps)
        {
            string systemMessage = @"
You are an expert resource planner working on the Universal Autonomous Builder system.
Your task is to identify the resources needed for a project based on the goal, requirements, and build steps.

Identify resources in these categories:
- External services: Third-party services or APIs needed
- Development tools: IDEs, frameworks, SDK, etc.
- Libraries: Software libraries or packages to use
- APIs: Specific APIs to integrate with
- Infrastructure: Deployment infrastructure, hosting, etc.

Provide your response as a JSON object.
";

            string userMessage = $@"
Goal: {goal.Intent}
Project Type: {goal.ProjectType}
Complexity: {goal.Complexity}
Requirements:
{string.Join("\n", goal.Requirements.Select(r => $"- {r}"))}

Build Steps:
{string.Join("\n", steps.Select(s => $"- {s.Name}: {s.Description}"))}

Identify required resources:
";
            
            var response = await ExecutePromptAsync(systemMessage, userMessage);
            
            try
            {
                // Extract JSON object from response
                string jsonResponse = response;
                if (response.Contains("{") && response.Contains("}"))
                {
                    int startIndex = response.IndexOf("{");
                    int endIndex = response.LastIndexOf("}") + 1;
                    jsonResponse = response.Substring(startIndex, endIndex - startIndex);
                }
                
                // Parse the JSON response
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resourcesDto = JsonSerializer.Deserialize<ResourcesDto>(jsonResponse, options);
                
                if (resourcesDto == null)
                {
                    throw new Exception("Failed to parse resources");
                }
                
                return new BuildResources
                {
                    ExternalServices = resourcesDto.ExternalServices ?? new List<string>(),
                    DevelopmentTools = resourcesDto.DevelopmentTools ?? new List<string>(),
                    Libraries = resourcesDto.Libraries ?? new List<string>(),
                    APIs = resourcesDto.APIs ?? new List<string>(),
                    Infrastructure = resourcesDto.Infrastructure ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing resources: {ex.Message}");
                
                // Return default resources based on project type
                return CreateDefaultResources(goal);
            }
        }
        
        /// <summary>
        /// Analyze dependencies between steps
        /// </summary>
        private async Task<Dictionary<string, List<string>>> AnalyzeDependenciesAsync(List<BuildStep> steps)
        {
            var dependencies = new Dictionary<string, List<string>>();
            
            // Create a simple dependency graph based on step order
            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                var dependsOn = new List<string>();
                
                // Look at explicit dependencies
                if (step.Dependencies.Count > 0)
                {
                    // Try to match dependency names with step names
                    foreach (var dependency in step.Dependencies)
                    {
                        var matchingStep = steps.FirstOrDefault(s => 
                            s.Name.Equals(dependency, StringComparison.OrdinalIgnoreCase) ||
                            s.Name.Contains(dependency, StringComparison.OrdinalIgnoreCase) ||
                            dependency.Contains(s.Name, StringComparison.OrdinalIgnoreCase));
                        
                        if (matchingStep != null)
                        {
                            dependsOn.Add(matchingStep.Id);
                        }
                    }
                }
                
                // If no explicit dependencies, assume it depends on the previous step
                if (dependsOn.Count == 0 && i > 0)
                {
                    dependsOn.Add(steps[i - 1].Id);
                }
                
                dependencies[step.Id] = dependsOn;
            }
            
            return dependencies;
        }
        
        /// <summary>
        /// Define success criteria for the build
        /// </summary>
        private async Task<List<string>> DefineSuccessCriteriaAsync(ProcessedGoal goal)
        {
            string systemMessage = @"
You are an expert quality assurance engineer working on the Universal Autonomous Builder system.
Your task is to define clear, measurable success criteria for a project based on the goal and requirements.

Good success criteria are:
- Specific and measurable
- Aligned with the goal and requirements
- Realistic and achievable
- Time-bound when appropriate
- Comprehensive in coverage

Generate 5-8 success criteria for the project.
Provide your response as a JSON array of strings.
";

            string userMessage = $@"
Goal: {goal.Intent}
Project Type: {goal.ProjectType}
Complexity: {goal.Complexity}
Requirements:
{string.Join("\n", goal.Requirements.Select(r => $"- {r}"))}

Define success criteria:
";
            
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
                var criteria = JsonSerializer.Deserialize<List<string>>(jsonResponse);
                
                if (criteria == null || criteria.Count == 0)
                {
                    throw new Exception("Failed to parse success criteria");
                }
                
                return criteria;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing success criteria: {ex.Message}");
                
                // Generate default success criteria based on goal
                return new List<string>
                {
                    $"The {goal.ProjectType} successfully implements all required functionality",
                    "All implemented features work as expected",
                    "The system performs efficiently under expected load",
                    "The system is secure and protects user data",
                    "The code is well-structured and maintainable"
                };
            }
        }
        
        /// <summary>
        /// Generate architecture overview
        /// </summary>
        private async Task<string> GenerateArchitectureAsync(ProcessedGoal goal, List<BuildStep> steps)
        {
            string systemMessage = @"
You are an expert system architect working on the Universal Autonomous Builder system.
Your task is to generate a high-level architecture overview for a project based on the goal, requirements, and build steps.

Include information about:
- System components and their interactions
- Data flow between components
- Key design patterns used
- Technology stack
- Deployment architecture

Provide a concise but comprehensive architecture overview as a text description.
";

            string userMessage = $@"
Goal: {goal.Intent}
Project Type: {goal.ProjectType}
Complexity: {goal.Complexity}
Requirements:
{string.Join("\n", goal.Requirements.Select(r => $"- {r}"))}

Build Steps:
{string.Join("\n", steps.Select(s => $"- {s.Name}: {s.Description}"))}

Generate architecture overview:
";
            
            var response = await ExecutePromptAsync(systemMessage, userMessage);
            return response;
        }
        
        /// <summary>
        /// Create a mock build plan for debug mode
        /// </summary>
        private BuildPlan CreateMockBuildPlan(ProcessedGoal goal)
        {
            var steps = CreateDefaultBuildSteps(goal);
            var resources = CreateDefaultResources(goal);
            
            return new BuildPlan
            {
                Name = $"Mock Plan for {goal.Intent}",
                Steps = steps,
                Resources = resources,
                Dependencies = steps.ToDictionary(s => s.Id, s => new List<string>()),
                SuccessCriteria = new List<string> { "Mock success criterion 1", "Mock success criterion 2" },
                Architecture = "Mock architecture overview",
                EstimatedTotalTimeMinutes = steps.Sum(s => s.EstimatedTimeMinutes)
            };
        }
        
        /// <summary>
        /// Create default build steps
        /// </summary>
        private List<BuildStep> CreateDefaultBuildSteps(ProcessedGoal goal)
        {
            var steps = new List<BuildStep>();
            
            // Step 1: Requirements Analysis
            steps.Add(new BuildStep
            {
                Id = Guid.NewGuid().ToString(),
                Order = 1,
                Name = "Requirements Analysis",
                Description = "Analyze and refine the requirements for the project",
                Tools = new List<string> { "documentation" },
                SuccessCriteria = "Clear and comprehensive requirements document",
                Dependencies = new List<string>(),
                EstimatedTimeMinutes = 60,
                RiskLevel = 1
            });
            
            // Step 2: Architecture Design
            steps.Add(new BuildStep
            {
                Id = Guid.NewGuid().ToString(),
                Order = 2,
                Name = "Architecture Design",
                Description = "Design the system architecture",
                Tools = new List<string> { "documentation", "diagramming" },
                SuccessCriteria = "Architecture document with component diagram",
                Dependencies = new List<string> { "Requirements Analysis" },
                EstimatedTimeMinutes = 90,
                RiskLevel = 2
            });
            
            // Step 3: Environment Setup
            steps.Add(new BuildStep
            {
                Id = Guid.NewGuid().ToString(),
                Order = 3,
                Name = "Environment Setup",
                Description = "Set up the development environment",
                Tools = new List<string> { "development-tools" },
                SuccessCriteria = "Working development environment with all required tools",
                Dependencies = new List<string> { "Architecture Design" },
                EstimatedTimeMinutes = 45,
                RiskLevel = 1
            });
            
            // Step 4: Core Implementation
            steps.Add(new BuildStep
            {
                Id = Guid.NewGuid().ToString(),
                Order = 4,
                Name = "Core Implementation",
                Description = "Implement the core functionality",
                Tools = new List<string> { "development-tools", "code-editor" },
                SuccessCriteria = "Core functionality working as expected",
                Dependencies = new List<string> { "Environment Setup" },
                EstimatedTimeMinutes = 180,
                RiskLevel = 3
            });
            
            // Step 5: Testing
            steps.Add(new BuildStep
            {
                Id = Guid.NewGuid().ToString(),
                Order = 5,
                Name = "Testing",
                Description = "Test the implementation",
                Tools = new List<string> { "testing-framework" },
                SuccessCriteria = "All tests pass with good coverage",
                Dependencies = new List<string> { "Core Implementation" },
                EstimatedTimeMinutes = 90,
                RiskLevel = 2
            });
            
            // Step 6: Deployment
            steps.Add(new BuildStep
            {
                Id = Guid.NewGuid().ToString(),
                Order = 6,
                Name = "Deployment",
                Description = "Deploy the application",
                Tools = new List<string> { "deployment-tools" },
                SuccessCriteria = "Application successfully deployed and accessible",
                Dependencies = new List<string> { "Testing" },
                EstimatedTimeMinutes = 45,
                RiskLevel = 2
            });
            
            return steps;
        }
        
        /// <summary>
        /// Create default resources based on project type
        /// </summary>
        private BuildResources CreateDefaultResources(ProcessedGoal goal)
        {
            var resources = new BuildResources();
            
            switch (goal.ProjectType)
            {
                case "web-service":
                case "web-app":
                    resources.DevelopmentTools = new List<string> { "Visual Studio Code", "Node.js", "npm" };
                    resources.Libraries = new List<string> { "Express.js", "React" };
                    resources.Infrastructure = "Docker container on cloud hosting";
                    break;
                    
                case "ios-app":
                    resources.DevelopmentTools = new List<string> { "Xcode", "Swift" };
                    resources.Libraries = new List<string> { "SwiftUI", "Core Data" };
                    resources.Infrastructure = "App Store deployment";
                    break;
                    
                case "android-app":
                    resources.DevelopmentTools = new List<string> { "Android Studio", "Kotlin" };
                    resources.Libraries = new List<string> { "Jetpack Compose", "Room" };
                    resources.Infrastructure = "Google Play Store deployment";
                    break;
                    
                case "data-pipeline":
                    resources.DevelopmentTools = new List<string> { "Python", "Jupyter Notebook" };
                    resources.Libraries = new List<string> { "pandas", "scikit-learn", "Apache Spark" };
                    resources.Infrastructure = "Cloud data processing";
                    break;
                    
                case "autonomous-agent":
                    resources.DevelopmentTools = new List<string> { "Visual Studio", ".NET" };
                    resources.Libraries = new List<string> { "Semantic Kernel", "Octokit" };
                    resources.Infrastructure = "GitHub Actions workflow";
                    break;
                    
                default:
                    resources.DevelopmentTools = new List<string> { "Visual Studio Code", ".NET" };
                    resources.Libraries = new List<string> { "Standard libraries" };
                    resources.Infrastructure = "Cloud hosting";
                    break;
            }
            
            return resources;
        }
    }
    
    /// <summary>
    /// DTO for build step parsing
    /// </summary>
    internal class BuildStepDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string>? Tools { get; set; }
        public string SuccessCriteria { get; set; } = string.Empty;
        public List<string>? Dependencies { get; set; }
        public int EstimatedTimeMinutes { get; set; } = 60;
        public int RiskLevel { get; set; } = 1;
        public List<string>? Risks { get; set; }
        public List<string>? RiskMitigation { get; set; }
    }
    
    /// <summary>
    /// DTO for resources parsing
    /// </summary>
    internal class ResourcesDto
    {
        public List<string>? ExternalServices { get; set; }
        public List<string>? DevelopmentTools { get; set; }
        public List<string>? Libraries { get; set; }
        public List<string>? APIs { get; set; }
        public string? Infrastructure { get; set; }
    }
} 