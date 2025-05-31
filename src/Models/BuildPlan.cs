using System;
using System.Collections.Generic;

namespace UniversalAutonomousBuilder.Models
{
    /// <summary>
    /// Represents a step in the build plan
    /// </summary>
    public class BuildStep
    {
        /// <summary>
        /// Unique identifier for the step
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// Name of the step
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Detailed description of what this step does
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Order of execution (1-based)
        /// </summary>
        public int Order { get; set; }
        
        /// <summary>
        /// Tools required to execute this step
        /// </summary>
        public List<string> Tools { get; set; } = new List<string>();
        
        /// <summary>
        /// Success criteria for this step
        /// </summary>
        public string SuccessCriteria { get; set; } = string.Empty;
        
        /// <summary>
        /// Dependencies on other steps
        /// </summary>
        public List<string> Dependencies { get; set; } = new List<string>();
        
        /// <summary>
        /// Estimated time to complete (in minutes)
        /// </summary>
        public int EstimatedTimeMinutes { get; set; }
        
        /// <summary>
        /// Risk level for this step (1-5)
        /// </summary>
        public int RiskLevel { get; set; } = 1;
        
        /// <summary>
        /// Potential risks associated with this step
        /// </summary>
        public List<string> Risks { get; set; } = new List<string>();
        
        /// <summary>
        /// Risk mitigation strategies
        /// </summary>
        public List<string> RiskMitigation { get; set; } = new List<string>();
        
        /// <summary>
        /// Whether this step is complete
        /// </summary>
        public bool IsComplete { get; set; }
        
        /// <summary>
        /// Result of executing this step
        /// </summary>
        public string Result { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the resources needed for the build
    /// </summary>
    public class BuildResources
    {
        /// <summary>
        /// External services required
        /// </summary>
        public List<string> ExternalServices { get; set; } = new List<string>();
        
        /// <summary>
        /// Development tools needed
        /// </summary>
        public List<string> DevelopmentTools { get; set; } = new List<string>();
        
        /// <summary>
        /// Libraries or frameworks to use
        /// </summary>
        public List<string> Libraries { get; set; } = new List<string>();
        
        /// <summary>
        /// APIs to integrate with
        /// </summary>
        public List<string> APIs { get; set; } = new List<string>();
        
        /// <summary>
        /// Infrastructure requirements
        /// </summary>
        public string Infrastructure { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a comprehensive build plan created by the StrategicPlanner
    /// </summary>
    public class BuildPlan
    {
        /// <summary>
        /// Unique identifier for the plan
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// Name of the plan
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Build steps in order of execution
        /// </summary>
        public List<BuildStep> Steps { get; set; } = new List<BuildStep>();
        
        /// <summary>
        /// Resources required for this build
        /// </summary>
        public BuildResources Resources { get; set; } = new BuildResources();
        
        /// <summary>
        /// Dependencies between components
        /// </summary>
        public Dictionary<string, List<string>> Dependencies { get; set; } = new Dictionary<string, List<string>>();
        
        /// <summary>
        /// Success criteria for the overall build
        /// </summary>
        public List<string> SuccessCriteria { get; set; } = new List<string>();
        
        /// <summary>
        /// Overall architecture description
        /// </summary>
        public string Architecture { get; set; } = string.Empty;
        
        /// <summary>
        /// Created timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Estimated total time to complete (in minutes)
        /// </summary>
        public int EstimatedTotalTimeMinutes { get; set; }
        
        /// <summary>
        /// Whether the plan is complete
        /// </summary>
        public bool IsComplete { get; set; }
    }
} 