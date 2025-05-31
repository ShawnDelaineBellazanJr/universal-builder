using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UniversalAutonomousBuilder.Models;

namespace UniversalAutonomousBuilder.Cognition
{
    /// <summary>
    /// Manages the timing of different cognitive frequencies and adapts based on outcomes
    /// </summary>
    public class AdaptiveScheduler
    {
        // Default intervals for each frequency
        private static readonly Dictionary<CognitiveFrequency, TimeSpan> _defaultIntervals = new Dictionary<CognitiveFrequency, TimeSpan>
        {
            { CognitiveFrequency.Immediate, TimeSpan.Zero }, // Immediate (0s)
            { CognitiveFrequency.Continuous, TimeSpan.FromSeconds(30) }, // Continuous (30s)
            { CognitiveFrequency.Analysis, TimeSpan.FromMinutes(15) }, // Analysis (15m)
            { CognitiveFrequency.Optimization, TimeSpan.FromHours(2) }, // Optimization (2h)
            { CognitiveFrequency.Evolution, TimeSpan.FromHours(24) }, // Evolution (24h)
        };
        
        // Current intervals (can be modified through learning)
        private readonly Dictionary<CognitiveFrequency, TimeSpan> _currentIntervals;
        
        // Track execution history
        private readonly Dictionary<CognitiveFrequency, List<ExecutionRecord>> _executionHistory;
        
        // Track success rates
        private readonly Dictionary<CognitiveFrequency, SuccessMetrics> _successMetrics;
        
        // Track adaptive learning outcomes
        private readonly List<FrequencyAdaptation> _adaptations;
        
        /// <summary>
        /// Creates a new instance of the AdaptiveScheduler
        /// </summary>
        public AdaptiveScheduler()
        {
            _currentIntervals = new Dictionary<CognitiveFrequency, TimeSpan>(_defaultIntervals);
            _executionHistory = new Dictionary<CognitiveFrequency, List<ExecutionRecord>>();
            _successMetrics = new Dictionary<CognitiveFrequency, SuccessMetrics>();
            _adaptations = new List<FrequencyAdaptation>();
            
            // Initialize history and metrics for each frequency
            foreach (CognitiveFrequency frequency in Enum.GetValues(typeof(CognitiveFrequency)))
            {
                _executionHistory[frequency] = new List<ExecutionRecord>();
                _successMetrics[frequency] = new SuccessMetrics();
            }
        }
        
        /// <summary>
        /// Gets the interval for a specific cognitive frequency
        /// </summary>
        public TimeSpan GetInterval(CognitiveFrequency frequency)
        {
            return _currentIntervals[frequency];
        }
        
        /// <summary>
        /// Sets a custom interval for a specific cognitive frequency
        /// </summary>
        public void SetInterval(CognitiveFrequency frequency, TimeSpan interval)
        {
            if (interval < TimeSpan.Zero)
                throw new ArgumentException("Interval cannot be negative", nameof(interval));
                
            _currentIntervals[frequency] = interval;
            
            // Record this adaptation
            _adaptations.Add(new FrequencyAdaptation
            {
                Frequency = frequency,
                PreviousInterval = _currentIntervals[frequency],
                NewInterval = interval,
                Timestamp = DateTime.UtcNow,
                Reason = "Manual adjustment"
            });
        }
        
        /// <summary>
        /// Records the execution of a frequency
        /// </summary>
        public void RecordExecution(CognitiveFrequency frequency, bool success, TimeSpan duration, string goal, int economicValue)
        {
            var record = new ExecutionRecord
            {
                Frequency = frequency,
                Success = success,
                Duration = duration,
                Timestamp = DateTime.UtcNow,
                Goal = goal,
                EconomicValue = economicValue
            };
            
            _executionHistory[frequency].Add(record);
            
            // Update success metrics
            _successMetrics[frequency].TotalExecutions++;
            if (success)
            {
                _successMetrics[frequency].SuccessfulExecutions++;
            }
            
            // Trim history if it gets too large
            if (_executionHistory[frequency].Count > 100)
            {
                _executionHistory[frequency].RemoveAt(0);
            }
        }
        
        /// <summary>
        /// Gets the next scheduled time for a specific frequency
        /// </summary>
        public DateTime GetNextScheduledTime(CognitiveFrequency frequency)
        {
            // Get the last execution time
            DateTime lastExecution = DateTime.MinValue;
            
            var history = _executionHistory[frequency];
            if (history.Count > 0)
            {
                lastExecution = history.Max(r => r.Timestamp);
            }
            
            // If never executed, schedule immediately
            if (lastExecution == DateTime.MinValue)
            {
                return DateTime.UtcNow;
            }
            
            // Calculate next scheduled time
            return lastExecution.Add(_currentIntervals[frequency]);
        }
        
        /// <summary>
        /// Determines if it's time to execute a specific frequency
        /// </summary>
        public bool ShouldExecute(CognitiveFrequency frequency)
        {
            return DateTime.UtcNow >= GetNextScheduledTime(frequency);
        }
        
        /// <summary>
        /// Gets the success rate for a specific frequency
        /// </summary>
        public double GetSuccessRate(CognitiveFrequency frequency)
        {
            var metrics = _successMetrics[frequency];
            
            if (metrics.TotalExecutions == 0)
                return 1.0; // No executions yet, assume 100% success
                
            return (double)metrics.SuccessfulExecutions / metrics.TotalExecutions;
        }
        
        /// <summary>
        /// Get the current interval settings for all frequencies
        /// </summary>
        public Dictionary<CognitiveFrequency, TimeSpan> GetCurrentFrequencySettings()
        {
            return new Dictionary<CognitiveFrequency, TimeSpan>(_currentIntervals);
        }
        
        /// <summary>
        /// Get the metrics for all frequencies
        /// </summary>
        public Dictionary<CognitiveFrequency, FrequencyMetricData> GetFrequencyMetrics()
        {
            var metrics = new Dictionary<CognitiveFrequency, FrequencyMetricData>();
            
            foreach (var frequency in _successMetrics.Keys)
            {
                var data = new FrequencyMetricData
                {
                    ExecutionCount = _successMetrics[frequency].TotalExecutions,
                    SuccessCount = _successMetrics[frequency].SuccessfulExecutions,
                    WastedExecutionCount = 0 // Calculate based on your criteria
                };
                
                // Calculate total execution time
                if (_executionHistory.ContainsKey(frequency) && _executionHistory[frequency].Count > 0)
                {
                    data.TotalExecutionTime = TimeSpan.FromTicks(
                        _executionHistory[frequency].Sum(r => r.Duration.Ticks)
                    );
                    
                    // Calculate wasted executions (example: executions that took less than 10% of interval)
                    var interval = _currentIntervals[frequency];
                    if (interval > TimeSpan.Zero)
                    {
                        data.WastedExecutionCount = _executionHistory[frequency].Count(
                            r => !r.Success || r.Duration.TotalMilliseconds < interval.TotalMilliseconds * 0.1
                        );
                    }
                }
                
                metrics[frequency] = data;
            }
            
            return metrics;
        }
        
        /// <summary>
        /// Update frequency settings based on recommended changes
        /// </summary>
        public async Task UpdateFrequencySettingsAsync(List<FrequencyChange> changes)
        {
            foreach (var change in changes)
            {
                // Only apply changes with sufficient confidence
                if (change.Confidence >= 0.7f)
                {
                    _currentIntervals[change.Frequency] = change.RecommendedInterval;
                    Console.WriteLine($"Updated {change.Frequency} frequency interval: {change.CurrentInterval} -> {change.RecommendedInterval} ({change.Justification})");
                    
                    // Record this adaptation
                    _adaptations.Add(new FrequencyAdaptation
                    {
                        Frequency = change.Frequency,
                        PreviousInterval = change.CurrentInterval,
                        NewInterval = change.RecommendedInterval,
                        Timestamp = DateTime.UtcNow,
                        Reason = change.Justification
                    });
                }
            }
            
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// Record an execution for learning purposes
        /// </summary>
        public async Task LearnFromOutcomeAsync(CognitiveFrequency frequency, bool success, TimeSpan executionTime)
        {
            // Record the execution with placeholder values for goal and economic value
            RecordExecution(
                frequency, 
                success, 
                executionTime, 
                "Generated from LearnFromOutcomeAsync", 
                75 // Default economic value
            );
            
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// Analyzes execution history and recommends frequency interval changes
        /// </summary>
        public async Task<AdaptiveScheduleResult> RecommendFrequencyChangesAsync()
        {
            var result = new AdaptiveScheduleResult
            {
                Questions = new List<string>(),
                Adaptations = new List<string>(),
                RecommendedChanges = new List<FrequencyChange>()
            };
            
            // Analyze each frequency
            foreach (CognitiveFrequency frequency in Enum.GetValues(typeof(CognitiveFrequency)))
            {
                // Skip immediate frequency (it's always 0)
                if (frequency == CognitiveFrequency.Immediate)
                    continue;
                    
                var history = _executionHistory[frequency];
                
                // Need at least 5 executions to make recommendations
                if (history.Count < 5)
                {
                    result.Questions.Add($"Not enough execution history for {frequency} frequency (need at least 5, have {history.Count})");
                    continue;
                }
                
                // Get success rate
                double successRate = GetSuccessRate(frequency);
                
                // Get current interval
                TimeSpan currentInterval = _currentIntervals[frequency];
                
                // Calculate average economic value
                double avgEconomicValue = history.Average(r => r.EconomicValue);
                
                // Calculate value/time ratio (economic value per unit of time)
                double valueTimeRatio = avgEconomicValue / currentInterval.TotalSeconds;
                
                // Analyze and make recommendations
                TimeSpan recommendedInterval = currentInterval;
                string justification = "";
                
                if (successRate < 0.7) // Less than 70% success rate
                {
                    // If success rate is low, increase interval to allow more time
                    recommendedInterval = TimeSpan.FromTicks((long)(currentInterval.Ticks * 1.5)); // 50% increase
                    justification = $"Low success rate ({successRate:P0}), increasing interval to allow more processing time";
                    
                    result.Adaptations.Add($"Increasing {frequency} interval due to low success rate");
                }
                else if (successRate > 0.9 && valueTimeRatio > 0.5) // High success and good value/time ratio
                {
                    // If success rate is high and value/time ratio is good, decrease interval for more frequent runs
                    recommendedInterval = TimeSpan.FromTicks((long)(currentInterval.Ticks * 0.8)); // 20% decrease
                    justification = $"High success rate ({successRate:P0}) and good value/time ratio, decreasing interval for more frequent runs";
                    
                    result.Adaptations.Add($"Decreasing {frequency} interval due to high success rate and good value/time ratio");
                }
                else if (valueTimeRatio < 0.2) // Low value/time ratio
                {
                    // If value/time ratio is low, increase interval to conserve resources
                    recommendedInterval = TimeSpan.FromTicks((long)(currentInterval.Ticks * 1.2)); // 20% increase
                    justification = $"Low value/time ratio ({valueTimeRatio:F2}), increasing interval to conserve resources";
                    
                    result.Adaptations.Add($"Increasing {frequency} interval due to low value/time ratio");
                }
                else
                {
                    // No change needed
                    justification = "Current interval performing well";
                }
                
                // Ensure intervals have reasonable minimums
                TimeSpan minInterval = _defaultIntervals[frequency] / 2; // Half of default as minimum
                if (recommendedInterval < minInterval)
                {
                    recommendedInterval = minInterval;
                    justification = $"{justification} (limited to minimum threshold of {minInterval})";
                }
                
                // Ensure intervals have reasonable maximums
                TimeSpan maxInterval = _defaultIntervals[frequency] * 2; // Double of default as maximum
                if (recommendedInterval > maxInterval)
                {
                    recommendedInterval = maxInterval;
                    justification = $"{justification} (limited to maximum threshold of {maxInterval})";
                }
                
                // Only recommend changes if the difference is significant (>10%)
                double percentChange = Math.Abs((recommendedInterval.TotalSeconds - currentInterval.TotalSeconds) / currentInterval.TotalSeconds);
                if (percentChange > 0.1) // More than 10% change
                {
                    result.RecommendedChanges.Add(new FrequencyChange
                    {
                        Frequency = frequency,
                        CurrentInterval = currentInterval,
                        RecommendedInterval = recommendedInterval,
                        SuccessRate = successRate,
                        ValueTimeRatio = valueTimeRatio,
                        Justification = justification
                    });
                    
                    result.Questions.Add($"Should {frequency} frequency interval be changed from {FormatTimeSpan(currentInterval)} to {FormatTimeSpan(recommendedInterval)}?");
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Apply recommended frequency changes
        /// </summary>
        public void ApplyRecommendedChanges(List<FrequencyChange> changes)
        {
            foreach (var change in changes)
            {
                _currentIntervals[change.Frequency] = change.RecommendedInterval;
                
                // Record this adaptation
                _adaptations.Add(new FrequencyAdaptation
                {
                    Frequency = change.Frequency,
                    PreviousInterval = change.CurrentInterval,
                    NewInterval = change.RecommendedInterval,
                    Timestamp = DateTime.UtcNow,
                    Reason = change.Justification
                });
            }
        }
        
        /// <summary>
        /// Get all frequency adaptations
        /// </summary>
        public List<FrequencyAdaptation> GetAdaptations()
        {
            return _adaptations.ToList();
        }
        
        /// <summary>
        /// Format a TimeSpan as a human-readable string
        /// </summary>
        private string FormatTimeSpan(TimeSpan ts)
        {
            if (ts.TotalDays >= 1)
                return $"{ts.TotalDays:F1}d";
            else if (ts.TotalHours >= 1)
                return $"{ts.TotalHours:F1}h";
            else if (ts.TotalMinutes >= 1)
                return $"{ts.TotalMinutes:F1}m";
            else
                return $"{ts.TotalSeconds:F0}s";
        }
    }
    
    /// <summary>
    /// Represents a record of frequency execution
    /// </summary>
    public class ExecutionRecord
    {
        /// <summary>
        /// The cognitive frequency
        /// </summary>
        public CognitiveFrequency Frequency { get; set; }
        
        /// <summary>
        /// Whether the execution was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// The duration of the execution
        /// </summary>
        public TimeSpan Duration { get; set; }
        
        /// <summary>
        /// When the execution occurred
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// The goal being processed
        /// </summary>
        public string Goal { get; set; } = string.Empty;
        
        /// <summary>
        /// The economic value of the goal
        /// </summary>
        public int EconomicValue { get; set; }
    }
    
    /// <summary>
    /// Tracks success metrics for a frequency
    /// </summary>
    public class SuccessMetrics
    {
        /// <summary>
        /// Total number of executions
        /// </summary>
        public int TotalExecutions { get; set; }
        
        /// <summary>
        /// Number of successful executions
        /// </summary>
        public int SuccessfulExecutions { get; set; }
    }
    
    /// <summary>
    /// Represents an adaptation to a frequency interval
    /// </summary>
    public class FrequencyAdaptation
    {
        /// <summary>
        /// The cognitive frequency
        /// </summary>
        public CognitiveFrequency Frequency { get; set; }
        
        /// <summary>
        /// The previous interval
        /// </summary>
        public TimeSpan PreviousInterval { get; set; }
        
        /// <summary>
        /// The new interval
        /// </summary>
        public TimeSpan NewInterval { get; set; }
        
        /// <summary>
        /// When the adaptation occurred
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// The reason for the adaptation
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Represents a recommended change to a frequency interval
    /// </summary>
    public class FrequencyChange
    {
        /// <summary>
        /// The cognitive frequency
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
        /// The success rate of the frequency
        /// </summary>
        public double SuccessRate { get; set; }
        
        /// <summary>
        /// The value/time ratio of the frequency
        /// </summary>
        public double ValueTimeRatio { get; set; }
        
        /// <summary>
        /// The justification for the change
        /// </summary>
        public string Justification { get; set; } = string.Empty;
        
        /// <summary>
        /// Confidence in the recommendation (0.0 to 1.0)
        /// </summary>
        public float Confidence { get; set; }
    }
    
    /// <summary>
    /// Results from adaptive scheduling analysis
    /// </summary>
    public class AdaptiveScheduleResult
    {
        /// <summary>
        /// Questions raised during analysis
        /// </summary>
        public List<string> Questions { get; set; } = new List<string>();
        
        /// <summary>
        /// Adaptation recommendations
        /// </summary>
        public List<string> Adaptations { get; set; } = new List<string>();
        
        /// <summary>
        /// Recommended frequency changes
        /// </summary>
        public List<FrequencyChange> RecommendedChanges { get; set; } = new List<FrequencyChange>();
    }
    
    /// <summary>
    /// Frequency metrics data for reporting
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
} 