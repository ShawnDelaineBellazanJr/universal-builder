using System;
using System.Collections.Generic;

namespace UniversalAutonomousBuilder.Models
{
    /// <summary>
    /// Represents the different cognitive frequencies in the architecture
    /// </summary>
    public enum CognitiveFrequency
    {
        /// <summary>
        /// Immediate responses (0s) - Reflexes for critical goals
        /// </summary>
        Immediate,
        
        /// <summary>
        /// Continuous monitoring (30s) - Background awareness
        /// </summary>
        Continuous,
        
        /// <summary>
        /// Analysis cycles (15m) - Deep thinking
        /// </summary>
        Analysis,
        
        /// <summary>
        /// Optimization cycles (2h) - Self-improvement
        /// </summary>
        Optimization,
        
        /// <summary>
        /// Evolution cycles (24h) - Architectural metamorphosis
        /// </summary>
        Evolution
    }
    
    /// <summary>
    /// Represents a response from cognitive processing
    /// </summary>
    public class CognitiveResponse
    {
        /// <summary>
        /// Whether the cognitive processing was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Message describing the outcome
        /// </summary>
        public string Message { get; set; } = string.Empty;
        
        /// <summary>
        /// The cognitive frequency used
        /// </summary>
        public CognitiveFrequency Frequency { get; set; }
        
        /// <summary>
        /// The economic value of the goal
        /// </summary>
        public int EconomicValue { get; set; }
        
        /// <summary>
        /// The time taken to execute the cognitive processing
        /// </summary>
        public TimeSpan ExecutionTime { get; set; }
        
        /// <summary>
        /// Actions performed during processing
        /// </summary>
        public List<string> Actions { get; set; } = new List<string>();
        
        /// <summary>
        /// Output artifacts produced
        /// </summary>
        public Dictionary<string, string> Artifacts { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Debug information (only populated in debug mode)
        /// </summary>
        public string DebugInformation { get; set; } = string.Empty;
        
        /// <summary>
        /// Next frequency to use (if any)
        /// </summary>
        public CognitiveFrequency? NextFrequency { get; set; }
    }
    
    /// <summary>
    /// Represents the result of resource usage justification
    /// </summary>
    public class ResourceJustification
    {
        /// <summary>
        /// Whether the resource usage is justified
        /// </summary>
        public bool Justified { get; set; }
        
        /// <summary>
        /// Reason for the justification decision
        /// </summary>
        public string Reason { get; set; } = string.Empty;
        
        /// <summary>
        /// Alternative frequency to use (if any)
        /// </summary>
        public CognitiveFrequency? AlternativeFrequency { get; set; }
    }
    
    /// <summary>
    /// Represents a change to a cognitive frequency setting
    /// </summary>
    public class FrequencyChange
    {
        /// <summary>
        /// The cognitive frequency being changed
        /// </summary>
        public CognitiveFrequency Frequency { get; set; }
        
        /// <summary>
        /// The current interval
        /// </summary>
        public TimeSpan CurrentInterval { get; set; }
        
        /// <summary>
        /// The recommended interval
        /// </summary>
        public TimeSpan RecommendedInterval { get; set; }
        
        /// <summary>
        /// Confidence in the recommendation (0.0 to 1.0)
        /// </summary>
        public float Confidence { get; set; }
        
        /// <summary>
        /// Justification for the change
        /// </summary>
        public string Justification { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Represents frequency metrics data
    /// </summary>
    public class FrequencyMetricData
    {
        /// <summary>
        /// Total number of executions
        /// </summary>
        public int ExecutionCount { get; set; }
        
        /// <summary>
        /// Number of successful executions
        /// </summary>
        public int SuccessCount { get; set; }
        
        /// <summary>
        /// Number of wasted executions (no action taken)
        /// </summary>
        public int WastedExecutionCount { get; set; }
        
        /// <summary>
        /// Total execution time across all runs
        /// </summary>
        public TimeSpan TotalExecutionTime { get; set; }
    }
    
    /// <summary>
    /// Represents the result of frequency adaptation
    /// </summary>
    public class FrequencyAdaptation
    {
        /// <summary>
        /// Questions raised during adaptation analysis
        /// </summary>
        public List<string> Questions { get; set; } = new List<string>();
        
        /// <summary>
        /// Recommended changes to frequency settings
        /// </summary>
        public List<FrequencyChange> RecommendedChanges { get; set; } = new List<FrequencyChange>();
        
        /// <summary>
        /// Adaptations to be applied
        /// </summary>
        public List<string> Adaptations { get; set; } = new List<string>();
    }
    
    /// <summary>
    /// Represents learning insights from the adaptive learning framework
    /// </summary>
    public class LearningInsights
    {
        /// <summary>
        /// Questions raised by the system
        /// </summary>
        public List<string> Questions { get; set; } = new List<string>();
        
        /// <summary>
        /// Adaptations to be made
        /// </summary>
        public List<string> Adaptations { get; set; } = new List<string>();
        
        /// <summary>
        /// Patterns identified
        /// </summary>
        public List<string> Patterns { get; set; } = new List<string>();
    }
} 