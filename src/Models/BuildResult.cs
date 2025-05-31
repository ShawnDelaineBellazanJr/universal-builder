using System;
using System.Collections.Generic;

namespace UniversalAutonomousBuilder.Models
{
    /// <summary>
    /// Represents the result of a build step execution
    /// </summary>
    public class StepResult
    {
        /// <summary>
        /// Identifier of the step
        /// </summary>
        public string StepId { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether the step was successful
        /// </summary>
        public bool IsSuccessful { get; set; }
        
        /// <summary>
        /// Output produced by the step
        /// </summary>
        public string Output { get; set; } = string.Empty;
        
        /// <summary>
        /// Error message if the step failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
        
        /// <summary>
        /// Artifacts produced by this step
        /// </summary>
        public List<string> Artifacts { get; set; } = new List<string>();
        
        /// <summary>
        /// Start time of the step execution
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// End time of the step execution
        /// </summary>
        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// Duration of the step execution in seconds
        /// </summary>
        public double DurationSeconds => (EndTime - StartTime).TotalSeconds;
        
        /// <summary>
        /// Whether the step required self-healing
        /// </summary>
        public bool RequiredSelfHealing { get; set; }
        
        /// <summary>
        /// Number of self-healing attempts
        /// </summary>
        public int SelfHealingAttempts { get; set; }
        
        /// <summary>
        /// Issues encountered during step execution
        /// </summary>
        public List<string> Issues { get; set; } = new List<string>();
        
        /// <summary>
        /// Resolution for each issue
        /// </summary>
        public Dictionary<string, string> Resolutions { get; set; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// Represents the overall result of a build execution
    /// </summary>
    public class BuildResult
    {
        /// <summary>
        /// Unique identifier for the build result
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>
        /// Results for each step
        /// </summary>
        public List<StepResult> Steps { get; set; } = new List<StepResult>();
        
        /// <summary>
        /// Whether the overall build was successful
        /// </summary>
        public bool IsSuccessful => Steps.All(s => s.IsSuccessful);
        
        /// <summary>
        /// Start time of the build
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// End time of the build
        /// </summary>
        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// Duration of the build in seconds
        /// </summary>
        public double DurationSeconds => (EndTime - StartTime).TotalSeconds;
        
        /// <summary>
        /// Number of steps that required self-healing
        /// </summary>
        public int SelfHealingStepsCount => Steps.Count(s => s.RequiredSelfHealing);
        
        /// <summary>
        /// Overall success rate (percentage of successful steps)
        /// </summary>
        public double SuccessRate => Steps.Count > 0 ? (double)Steps.Count(s => s.IsSuccessful) / Steps.Count * 100 : 0;
        
        /// <summary>
        /// All artifacts produced by the build
        /// </summary>
        public List<string> AllArtifacts
        {
            get
            {
                var artifacts = new List<string>();
                foreach (var step in Steps)
                {
                    artifacts.AddRange(step.Artifacts);
                }
                return artifacts;
            }
        }
        
        /// <summary>
        /// All issues encountered during the build
        /// </summary>
        public List<string> AllIssues
        {
            get
            {
                var issues = new List<string>();
                foreach (var step in Steps)
                {
                    issues.AddRange(step.Issues);
                }
                return issues;
            }
        }
    }
} 