using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using UniversalAutonomousBuilder.Agents;
using UniversalAutonomousBuilder.Models;
using UniversalAutonomousBuilder.Reasoning;
using UniversalAutonomousBuilder.Evolution;

namespace UniversalAutonomousBuilder.Process
{
    /// <summary>
    /// The main orchestration process for the Universal Autonomous Builder
    /// </summary>
    public class UniversalBuilderProcess : IProcess
    {
        private readonly Kernel _kernel;
        private readonly string _templatesDirectory;
        private readonly bool _debugMode;
        
        private readonly GoalProcessor _goalProcessor;
        private readonly StrategicPlanner _strategicPlanner;
        private readonly ChainOfThoughtReasoning _chainOfThought;
        private readonly StrangeLoopEngine _strangeLoop;
        
        public UniversalBuilderProcess(
            Kernel kernel,
            string templatesDirectory = "templates",
            bool debugMode = false)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _templatesDirectory = templatesDirectory;
            _debugMode = debugMode;
            
            // Initialize agents
            _goalProcessor = new GoalProcessor(kernel, templatesDirectory, debugMode: debugMode);
            _strategicPlanner = new StrategicPlanner(kernel, templatesDirectory, debugMode: debugMode);
            _chainOfThought = new ChainOfThoughtReasoning(kernel, debugMode);
            _strangeLoop = new StrangeLoopEngine(kernel, templatesDirectory, debugMode);
            
            // Create templates directory if it doesn't exist
            if (!Directory.Exists(templatesDirectory))
            {
                Directory.CreateDirectory(templatesDirectory);
            }
        }
        
        /// <summary>
        /// Execute the Universal Autonomous Builder process
        /// </summary>
        /// <param name="goal">The user's goal or intent</param>
        /// <param name="projectType">Optional project type, will be auto-detected if not provided</param>
        /// <param name="recursionEnabled">Whether to enable recursive improvement</param>
        /// <returns>The process result</returns>
        public async Task<ProcessResult> ExecuteAsync(
            string goal,
            string? projectType = null,
            bool recursionEnabled = true)
        {
            Console.WriteLine($"Starting Universal Builder Process with goal: {goal}");
            
            try
            {
                // STEP 1: Goal Processing
                Console.WriteLine("Step 1: Processing goal...");
                var processedGoal = await _goalProcessor.ProcessGoalAsync(goal);
                
                // Override project type if specified
                if (!string.IsNullOrWhiteSpace(projectType))
                {
                    processedGoal.ProjectType = projectType;
                }
                
                // Check economic value
                if (processedGoal.EconomicValue < 50)
                {
                    Console.WriteLine($"Goal rejected: Economic value ({processedGoal.EconomicValue}) is below threshold (50)");
                    return new ProcessResult
                    {
                        Success = false,
                        ErrorMessage = $"Goal rejected: Economic value ({processedGoal.EconomicValue}) is below threshold (50)",
                        ProcessedGoal = processedGoal
                    };
                }
                
                // STEP 2: Chain of Thought Reasoning
                Console.WriteLine("Step 2: Performing chain of thought reasoning...");
                var reasoningResult = await _chainOfThought.ReasonAboutGoalAsync(processedGoal.Intent);
                
                // STEP 3: Strategic Planning
                Console.WriteLine("Step 3: Creating build plan...");
                var buildPlan = await _strategicPlanner.CreateBuildPlanAsync(processedGoal);
                
                // STEP 4: Apply Strange Loop pattern if recursion is enabled
                StrangeLoopResult? strangeLoopResult = null;
                if (recursionEnabled)
                {
                    Console.WriteLine("Step 4: Executing Strange Loop pattern...");
                    strangeLoopResult = await _strangeLoop.ExecuteStrangeLoopAsync(processedGoal.Intent);
                }
                
                // STEP 5: Create comprehensive process result
                var result = new ProcessResult
                {
                    Success = true,
                    ProcessedGoal = processedGoal,
                    ReasoningResult = reasoningResult,
                    BuildPlan = buildPlan,
                    StrangeLoopResult = strangeLoopResult,
                    CompletedAt = DateTime.UtcNow
                };
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Universal Builder Process: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                
                return new ProcessResult
                {
                    Success = false,
                    ErrorMessage = $"Error: {ex.Message}",
                    CompletedAt = DateTime.UtcNow
                };
            }
        }
    }
    
    /// <summary>
    /// Interface for process execution
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Execute the process
        /// </summary>
        Task<ProcessResult> ExecuteAsync(string goal, string? projectType = null, bool recursionEnabled = true);
    }
    
    /// <summary>
    /// Represents the result of the process execution
    /// </summary>
    public class ProcessResult
    {
        /// <summary>
        /// Whether the process completed successfully
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Error message if the process failed
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// Processed goal
        /// </summary>
        public ProcessedGoal? ProcessedGoal { get; set; }
        
        /// <summary>
        /// Chain of thought reasoning result
        /// </summary>
        public ReasoningResult? ReasoningResult { get; set; }
        
        /// <summary>
        /// Build plan
        /// </summary>
        public BuildPlan? BuildPlan { get; set; }
        
        /// <summary>
        /// Strange Loop result
        /// </summary>
        public StrangeLoopResult? StrangeLoopResult { get; set; }
        
        /// <summary>
        /// When the process completed
        /// </summary>
        public DateTime CompletedAt { get; set; }
    }
} 