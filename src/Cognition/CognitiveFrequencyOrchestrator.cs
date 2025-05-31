using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Cognition
{
    /// <summary>
    /// Orchestrates the multi-frequency cognitive architecture
    /// </summary>
    public class CognitiveFrequencyOrchestrator
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatService;
        private readonly EconomicConsciousness _economics;
        private readonly AdaptiveScheduler _scheduler;
        private readonly bool _debugMode;
        private readonly bool _useSKRouter;
        
        public CognitiveFrequencyOrchestrator(
            Kernel kernel, 
            EconomicConsciousness economics,
            AdaptiveScheduler scheduler,
            bool debugMode = false,
            bool useSKRouter = true) // Default to using SK-based router
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _economics = economics ?? throw new ArgumentNullException(nameof(economics));
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _debugMode = debugMode;
            _useSKRouter = useSKRouter;
        }
        
        /// <summary>
        /// Process a goal through the multi-frequency cognitive architecture
        /// </summary>
        public async Task<CognitiveResponse> ProcessGoalAsync(string goal)
        {
            Console.WriteLine($"Processing goal through multi-frequency cognition: {goal}");
            
            try
            {
                // Step 1: Determine the appropriate cognitive frequency
                CognitiveFrequency frequency;
                if (_useSKRouter)
                {
                    frequency = await DetermineFrequencyWithSKAsync(goal);
                    Console.WriteLine($"SK-based frequency router selected: {frequency}");
                }
                else
                {
                    frequency = await DetermineCognitiveFrequencyAsync(goal);
                    Console.WriteLine($"Keyword-based frequency router selected: {frequency}");
                }
                
                // Step 2: Evaluate the economic value of the goal
                int economicValue = await _economics.EvaluateGoalValueAsync(goal);
                
                // Step 3: Check if processing is economically justified
                var justification = await _economics.JustifyResourceUsageAsync(frequency, economicValue);
                
                if (!justification.Justified)
                {
                    Console.WriteLine($"Goal processing not justified: {justification.Reason}");
                    
                    // If an alternative frequency is suggested, use that instead
                    if (justification.AlternativeFrequency.HasValue)
                    {
                        frequency = justification.AlternativeFrequency.Value;
                        Console.WriteLine($"Using alternative frequency: {frequency}");
                        
                        // Re-evaluate justification with new frequency
                        justification = await _economics.JustifyResourceUsageAsync(frequency, economicValue);
                        
                        if (!justification.Justified)
                        {
                            return new CognitiveResponse
                            {
                                Success = false,
                                Message = $"Goal processing not justified at any frequency. {justification.Reason}",
                                Frequency = frequency,
                                EconomicValue = economicValue
                            };
                        }
                    }
                    else
                    {
                        return new CognitiveResponse
                        {
                            Success = false,
                            Message = justification.Reason,
                            Frequency = frequency,
                            EconomicValue = economicValue
                        };
                    }
                }
                
                // Step 4: Route to the appropriate frequency processor
                var result = await RouteToFrequencyProcessorAsync(goal, frequency, economicValue);
                
                // Step 5: Update adaptive learning based on outcome
                await _scheduler.LearnFromOutcomeAsync(frequency, result.Success, result.ExecutionTime);
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in cognitive frequency orchestration: {ex.Message}");
                
                if (_debugMode)
                {
                    return new CognitiveResponse
                    {
                        Success = false,
                        Message = $"Debug mode error: {ex.Message}",
                        Frequency = CognitiveFrequency.Analysis,
                        EconomicValue = 50,
                        DebugInformation = ex.ToString()
                    };
                }
                
                throw;
            }
        }
        
        /// <summary>
        /// Determine the appropriate cognitive frequency using Semantic Kernel
        /// </summary>
        private async Task<CognitiveFrequency> DetermineFrequencyWithSKAsync(string goal)
        {
            try
            {
                // Create a prompt for the AI to determine the best frequency
                string systemMessage = @"
You are a cognitive frequency router for the multi-frequency cognitive architecture.
Your task is to determine the most appropriate cognitive frequency for a given goal.

The available frequencies are:
1. IMMEDIATE (0s): For emergencies, critical issues, security breaches - highest priority tasks requiring instant response
2. CONTINUOUS (30s): For monitoring, health checks, status tracking - background awareness tasks
3. ANALYSIS (15m): For problem-solving, planning, designing - tasks requiring deep thought
4. OPTIMIZATION (2h): For improvements, enhancements, refinements - tasks to make things better
5. EVOLUTION (24h): For architectural changes, transformations, paradigm shifts - fundamental changes

Consider the nature of the goal, its urgency, complexity, and scope.
Respond with a single word: IMMEDIATE, CONTINUOUS, ANALYSIS, OPTIMIZATION, or EVOLUTION.
";
                string userMessage = $"Goal: {goal}\n\nDetermine the most appropriate cognitive frequency:";
                
                var chatHistory = new ChatHistory();
                chatHistory.AddSystemMessage(systemMessage);
                chatHistory.AddUserMessage(userMessage);
                
                var response = await _chatService.GetChatMessageContentAsync(chatHistory);
                var content = response.Content?.Trim().ToUpperInvariant() ?? "ANALYSIS";
                
                // Parse the response to get the frequency
                if (content.Contains("IMMEDIATE"))
                    return CognitiveFrequency.Immediate;
                else if (content.Contains("CONTINUOUS"))
                    return CognitiveFrequency.Continuous;
                else if (content.Contains("ANALYSIS"))
                    return CognitiveFrequency.Analysis;
                else if (content.Contains("OPTIMIZATION"))
                    return CognitiveFrequency.Optimization;
                else if (content.Contains("EVOLUTION"))
                    return CognitiveFrequency.Evolution;
                else
                    return CognitiveFrequency.Analysis; // Default to Analysis if unable to determine
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SK-based frequency determination: {ex.Message}");
                // Fall back to keyword-based determination
                return await DetermineCognitiveFrequencyAsync(goal);
            }
        }
        
        /// <summary>
        /// Determine the appropriate cognitive frequency for a goal using keywords
        /// </summary>
        private async Task<CognitiveFrequency> DetermineCognitiveFrequencyAsync(string goal)
        {
            // Check for immediate frequency keywords (emergencies, critical issues)
            if (ContainsImmediateKeywords(goal))
            {
                return CognitiveFrequency.Immediate;
            }
            
            // Check for evolution frequency keywords (architectural changes)
            if (ContainsEvolutionKeywords(goal))
            {
                return CognitiveFrequency.Evolution;
            }
            
            // Check for optimization frequency keywords (improvements)
            if (ContainsOptimizationKeywords(goal))
            {
                return CognitiveFrequency.Optimization;
            }
            
            // Check for continuous frequency keywords (monitoring)
            if (ContainsContinuousKeywords(goal))
            {
                return CognitiveFrequency.Continuous;
            }
            
            // Default to analysis frequency for most goals
            return CognitiveFrequency.Analysis;
        }
        
        /// <summary>
        /// Route a goal to the appropriate frequency processor
        /// </summary>
        private async Task<CognitiveResponse> RouteToFrequencyProcessorAsync(
            string goal, 
            CognitiveFrequency frequency,
            int economicValue)
        {
            Console.WriteLine($"Routing goal to {frequency} frequency processor");
            
            // Start timing
            var startTime = DateTime.UtcNow;
            
            // Route to the appropriate processor
            CognitiveResponse response;
            
            switch (frequency)
            {
                case CognitiveFrequency.Immediate:
                    var immediateProcessor = new ImmediateReflexProcessor(_kernel, _debugMode);
                    response = await immediateProcessor.ProcessCriticalGoalAsync(goal, economicValue);
                    break;
                    
                case CognitiveFrequency.Continuous:
                    var continuousProcessor = new ContinuousAwarenessMonitor(_kernel, _debugMode);
                    response = await continuousProcessor.MonitorEcosystemAsync(goal);
                    break;
                    
                case CognitiveFrequency.Analysis:
                    var analysisProcessor = new AnalysisThinkingProcessor(_kernel, _debugMode);
                    response = await analysisProcessor.DeepAnalyzeGoalAsync(goal, economicValue);
                    break;
                    
                case CognitiveFrequency.Optimization:
                    var optimizationProcessor = new OptimizationReflectionProcessor(_kernel, _debugMode);
                    response = await optimizationProcessor.ReflectAndOptimizeAsync(goal, economicValue);
                    break;
                    
                case CognitiveFrequency.Evolution:
                    var evolutionProcessor = new EvolutionArchitectureProcessor(_kernel, _debugMode);
                    response = await evolutionProcessor.ExecuteArchitecturalEvolutionAsync(goal, economicValue);
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(frequency), $"Unsupported frequency: {frequency}");
            }
            
            // Calculate execution time
            var executionTime = DateTime.UtcNow - startTime;
            response.ExecutionTime = executionTime;
            response.Frequency = frequency;
            response.EconomicValue = economicValue;
            
            return response;
        }
        
        /// <summary>
        /// Check if the goal contains immediate frequency keywords
        /// </summary>
        private bool ContainsImmediateKeywords(string goal)
        {
            var keywords = new[] 
            { 
                "emergency", "critical", "urgent", "immediate", "security", "vulnerability", 
                "breach", "fix", "broken", "down", "crash", "failure", "outage" 
            };
            
            return ContainsAnyKeyword(goal, keywords);
        }
        
        /// <summary>
        /// Check if the goal contains continuous frequency keywords
        /// </summary>
        private bool ContainsContinuousKeywords(string goal)
        {
            var keywords = new[] 
            { 
                "monitor", "health", "status", "check", "watch", "observe", "track", 
                "scan", "survey", "inspect", "diagnose" 
            };
            
            return ContainsAnyKeyword(goal, keywords);
        }
        
        /// <summary>
        /// Check if the goal contains optimization frequency keywords
        /// </summary>
        private bool ContainsOptimizationKeywords(string goal)
        {
            var keywords = new[] 
            { 
                "optimize", "improve", "enhance", "refine", "upgrade", "boost", "streamline", 
                "efficiency", "performance", "speed", "quality", "reliability" 
            };
            
            return ContainsAnyKeyword(goal, keywords);
        }
        
        /// <summary>
        /// Check if the goal contains evolution frequency keywords
        /// </summary>
        private bool ContainsEvolutionKeywords(string goal)
        {
            var keywords = new[] 
            { 
                "evolve", "architecture", "transform", "redesign", "restructure", "reinvent", 
                "revolution", "paradigm", "foundation", "fundamental", "meta-evolution" 
            };
            
            return ContainsAnyKeyword(goal, keywords);
        }
        
        /// <summary>
        /// Check if the goal contains any of the specified keywords
        /// </summary>
        private bool ContainsAnyKeyword(string goal, string[] keywords)
        {
            if (string.IsNullOrWhiteSpace(goal))
                return false;
                
            goal = goal.ToLower();
            
            foreach (var keyword in keywords)
            {
                if (goal.Contains(keyword.ToLower()))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Adapt cognitive frequencies based on learning
        /// </summary>
        public async Task<FrequencyAdaptation> AdaptCognitiveFrequenciesAsync()
        {
            Console.WriteLine("Adapting cognitive frequencies based on learning");
            
            // Get current frequency settings
            var currentSettings = _scheduler.GetCurrentFrequencySettings();
            
            // Analyze effectiveness
            var adaptations = await AnalyzeFrequencyEffectivenessAsync(currentSettings);
            
            // Apply adaptations
            if (adaptations.RecommendedChanges.Count > 0)
            {
                await _scheduler.UpdateFrequencySettingsAsync(adaptations.RecommendedChanges);
            }
            
            return adaptations;
        }
        
        /// <summary>
        /// Analyze frequency effectiveness
        /// </summary>
        private async Task<FrequencyAdaptation> AnalyzeFrequencyEffectivenessAsync(Dictionary<CognitiveFrequency, TimeSpan> currentSettings)
        {
            // Get metrics from scheduler
            var metrics = _scheduler.GetFrequencyMetrics();
            
            // Analyze metrics and recommend changes
            var adaptation = new FrequencyAdaptation();
            
            foreach (var metric in metrics)
            {
                var frequency = metric.Key;
                var data = metric.Value;
                
                // Skip frequencies with too few executions
                if (data.ExecutionCount < 5)
                    continue;
                    
                // Calculate success rate
                float successRate = (float)data.SuccessCount / data.ExecutionCount;
                
                // Calculate average execution time
                TimeSpan averageExecutionTime = data.TotalExecutionTime / data.ExecutionCount;
                
                // Generate questions based on metrics
                if (successRate < 0.6f)
                {
                    adaptation.Questions.Add($"Why is the success rate for {frequency} frequency so low ({successRate:P0})?");
                }
                
                if (data.WastedExecutionCount > data.ExecutionCount * 0.3)
                {
                    adaptation.Questions.Add($"Are we executing {frequency} frequency too often? {data.WastedExecutionCount} wasted executions out of {data.ExecutionCount}.");
                }
                
                // Generate recommended changes
                TimeSpan newInterval = currentSettings[frequency];
                bool shouldChange = false;
                string justification = string.Empty;
                
                if (successRate < 0.5f && data.ExecutionCount > 10)
                {
                    // If success rate is very low, increase interval (less frequent)
                    newInterval = TimeSpan.FromMilliseconds(newInterval.TotalMilliseconds * 1.5);
                    shouldChange = true;
                    justification = $"Low success rate ({successRate:P0}) suggests frequency is too high";
                }
                else if (data.WastedExecutionCount > data.SuccessCount && data.ExecutionCount > 10)
                {
                    // If more wasted than successful executions, increase interval
                    newInterval = TimeSpan.FromMilliseconds(newInterval.TotalMilliseconds * 1.3);
                    shouldChange = true;
                    justification = $"More wasted ({data.WastedExecutionCount}) than successful executions ({data.SuccessCount})";
                }
                else if (successRate > 0.9f && averageExecutionTime.TotalMilliseconds < newInterval.TotalMilliseconds * 0.5)
                {
                    // If very successful and fast, decrease interval (more frequent)
                    newInterval = TimeSpan.FromMilliseconds(newInterval.TotalMilliseconds * 0.8);
                    shouldChange = true;
                    justification = $"High success rate ({successRate:P0}) and fast execution suggests frequency could be higher";
                }
                
                if (shouldChange)
                {
                    adaptation.RecommendedChanges.Add(new FrequencyChange
                    {
                        Frequency = frequency,
                        CurrentInterval = currentSettings[frequency],
                        RecommendedInterval = newInterval,
                        Confidence = CalculateConfidence(data.ExecutionCount, successRate),
                        Justification = justification
                    });
                }
            }
            
            return adaptation;
        }
        
        /// <summary>
        /// Calculate confidence in a recommendation based on data
        /// </summary>
        private float CalculateConfidence(int executionCount, float successRate)
        {
            // More executions = higher confidence
            float executionConfidence = Math.Min(0.5f, executionCount / 100f);
            
            // Success rate closer to 0 or 1 = higher confidence
            float successConfidence = Math.Max(Math.Abs(successRate - 0.5f) * 2, 0.2f);
            
            return executionConfidence + successConfidence;
        }
    }
} 