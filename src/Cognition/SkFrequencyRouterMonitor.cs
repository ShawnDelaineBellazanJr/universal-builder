using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutonomousAI.Cognition;

namespace UniversalAutonomousBuilder.Cognition
{
    /// <summary>
    /// Monitors and tracks the performance of the SK Frequency Router
    /// </summary>
    public class SkFrequencyRouterMonitor
    {
        private readonly string _logFilePath;
        private readonly List<RoutingDecision> _decisions = new List<RoutingDecision>();
        private bool _isDirty = false;
        
        public SkFrequencyRouterMonitor(string logFilePath = "sk-frequency-router-log.json")
        {
            _logFilePath = logFilePath;
            LoadExistingLog();
        }
        
        /// <summary>
        /// Records a routing decision for monitoring and analysis
        /// </summary>
        public void RecordDecision(string goal, string context, string recommendedFrequency, int economicValue, bool wasSuccessful, TimeSpan executionTime)
        {
            var decision = new RoutingDecision
            {
                Timestamp = DateTime.UtcNow,
                Goal = goal,
                Context = context,
                RecommendedFrequency = recommendedFrequency,
                EconomicValue = economicValue,
                WasSuccessful = wasSuccessful,
                ExecutionTime = executionTime
            };
            
            _decisions.Add(decision);
            _isDirty = true;
            
            // Save logs periodically (every 10 decisions)
            if (_decisions.Count % 10 == 0)
            {
                SaveLog();
            }
        }
        
        /// <summary>
        /// Loads existing log data if available
        /// </summary>
        private void LoadExistingLog()
        {
            if (File.Exists(_logFilePath))
            {
                try
                {
                    string json = File.ReadAllText(_logFilePath);
                    var loadedDecisions = JsonSerializer.Deserialize<List<RoutingDecision>>(json);
                    if (loadedDecisions != null)
                    {
                        _decisions.AddRange(loadedDecisions);
                        Console.WriteLine($"Loaded {loadedDecisions.Count} existing routing decisions from log.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading routing decisions log: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Saves the current log data to file
        /// </summary>
        public void SaveLog()
        {
            if (!_isDirty) return;
            
            try
            {
                string json = JsonSerializer.Serialize(_decisions, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_logFilePath, json);
                _isDirty = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving routing decisions log: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Generates a report of router performance
        /// </summary>
        public RoutingPerformanceReport GenerateReport()
        {
            var report = new RoutingPerformanceReport
            {
                TotalDecisions = _decisions.Count,
                SuccessfulDecisions = _decisions.Count(d => d.WasSuccessful),
                AverageEconomicValue = _decisions.Any() ? _decisions.Average(d => d.EconomicValue) : 0,
                FrequencyDistribution = new Dictionary<string, int>(),
                AverageExecutionTimes = new Dictionary<string, TimeSpan>()
            };
            
            // Calculate frequency distribution
            var frequencyGroups = _decisions.GroupBy(d => d.RecommendedFrequency);
            foreach (var group in frequencyGroups)
            {
                string frequency = group.Key;
                int count = group.Count();
                report.FrequencyDistribution[frequency] = count;
                
                // Calculate average execution time
                var successfulDecisions = group.Where(d => d.WasSuccessful);
                if (successfulDecisions.Any())
                {
                    report.AverageExecutionTimes[frequency] = TimeSpan.FromTicks(
                        (long)successfulDecisions.Average(d => d.ExecutionTime.Ticks)
                    );
                }
            }
            
            // Calculate success rate by frequency
            report.SuccessRateByFrequency = new Dictionary<string, double>();
            foreach (var frequency in report.FrequencyDistribution.Keys)
            {
                int total = _decisions.Count(d => d.RecommendedFrequency == frequency);
                int successful = _decisions.Count(d => d.RecommendedFrequency == frequency && d.WasSuccessful);
                report.SuccessRateByFrequency[frequency] = total > 0 ? (double)successful / total : 0;
            }
            
            return report;
        }
    }
    
    /// <summary>
    /// Represents a single routing decision made by the SK Frequency Router
    /// </summary>
    public class RoutingDecision
    {
        public DateTime Timestamp { get; set; }
        public string Goal { get; set; }
        public string Context { get; set; }
        public string RecommendedFrequency { get; set; }
        public int EconomicValue { get; set; }
        public bool WasSuccessful { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }
    
    /// <summary>
    /// Performance report for the SK Frequency Router
    /// </summary>
    public class RoutingPerformanceReport
    {
        public int TotalDecisions { get; set; }
        public int SuccessfulDecisions { get; set; }
        public double AverageEconomicValue { get; set; }
        public Dictionary<string, int> FrequencyDistribution { get; set; }
        public Dictionary<string, double> SuccessRateByFrequency { get; set; }
        public Dictionary<string, TimeSpan> AverageExecutionTimes { get; set; }
        
        public double OverallSuccessRate => TotalDecisions > 0 ? (double)SuccessfulDecisions / TotalDecisions : 0;
        
        public override string ToString()
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("=== SK Frequency Router Performance Report ===");
            report.AppendLine($"Total Decisions: {TotalDecisions}");
            report.AppendLine($"Successful Decisions: {SuccessfulDecisions} ({OverallSuccessRate:P2})");
            report.AppendLine($"Average Economic Value: {AverageEconomicValue:F2}");
            
            report.AppendLine("\nFrequency Distribution:");
            foreach (var kvp in FrequencyDistribution.OrderByDescending(k => k.Value))
            {
                report.AppendLine($"  {kvp.Key}: {kvp.Value} ({(double)kvp.Value / TotalDecisions:P2})");
            }
            
            report.AppendLine("\nSuccess Rate by Frequency:");
            foreach (var kvp in SuccessRateByFrequency.OrderByDescending(k => k.Value))
            {
                report.AppendLine($"  {kvp.Key}: {kvp.Value:P2}");
            }
            
            report.AppendLine("\nAverage Execution Times by Frequency:");
            foreach (var kvp in AverageExecutionTimes)
            {
                report.AppendLine($"  {kvp.Key}: {kvp.Value.TotalSeconds:F2} seconds");
            }
            
            return report.ToString();
        }
    }
} 