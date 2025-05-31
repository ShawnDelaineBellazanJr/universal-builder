using System;
using System.Collections.Generic;

namespace UniversalAutonomousBuilder.Models
{
    /// <summary>
    /// Represents a processed goal after analysis by the GoalProcessor
    /// </summary>
    public class ProcessedGoal
    {
        /// <summary>
        /// The primary intent of the goal
        /// </summary>
        public string Intent { get; set; } = string.Empty;
        
        /// <summary>
        /// The type of project to build
        /// </summary>
        public string ProjectType { get; set; } = string.Empty;
        
        /// <summary>
        /// The complexity level (1-10)
        /// </summary>
        public int Complexity { get; set; }
        
        /// <summary>
        /// Detailed requirements extracted from the goal
        /// </summary>
        public List<string> Requirements { get; set; } = new List<string>();
        
        /// <summary>
        /// Constraints or limitations identified
        /// </summary>
        public List<string> Constraints { get; set; } = new List<string>();
        
        /// <summary>
        /// Keywords or tags related to the goal
        /// </summary>
        public List<string> Keywords { get; set; } = new List<string>();
        
        /// <summary>
        /// Related project templates that might be useful
        /// </summary>
        public List<string> RelatedTemplates { get; set; } = new List<string>();
        
        /// <summary>
        /// Timestamp when the goal was processed
        /// </summary>
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Economic value assessment (0-100)
        /// </summary>
        public int EconomicValue { get; set; }
        
        /// <summary>
        /// Whether the goal has sufficient information to proceed
        /// </summary>
        public bool HasSufficientInformation { get; set; }
        
        /// <summary>
        /// Reason why information might be insufficient
        /// </summary>
        public string InsufficientInformationReason { get; set; } = string.Empty;
    }
} 