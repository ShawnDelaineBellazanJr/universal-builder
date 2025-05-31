using System;
using System.Collections.Generic;

namespace AutonomousAI.Cognition
{
    /// <summary>
    /// Provides optimized prompts for the SK Frequency Router
    /// </summary>
    public static class SkFrequencyRouterPrompts
    {
        /// <summary>
        /// Enhanced system prompt for frequency determination
        /// </summary>
        public static string FrequencyDeterminationPrompt = @"You are an expert cognitive frequency router for a multi-frequency cognitive architecture system.
Your task is to analyze a goal and determine which cognitive frequency is most appropriate for processing it.

The available frequencies are:

1. IMMEDIATE (0s):
   - For emergencies and critical tasks requiring instant response
   - For security breaches, system failures, urgent alerts
   - For tasks where speed is the primary concern
   - Examples: 'Server down', 'Security breach detected', 'Critical system failure'

2. CONTINUOUS (30s):
   - For background monitoring and lightweight processing
   - For status checks, health monitoring, passive observation
   - For tasks that should run regularly in the background
   - Examples: 'Monitor CPU usage', 'Check for new messages', 'Watch for file changes'

3. ANALYSIS (15m):
   - For tasks requiring deeper thinking and investigation
   - For problem analysis, pattern recognition, data interpretation
   - For tasks where thoroughness is more important than speed
   - Examples: 'Analyze customer feedback', 'Investigate performance issues', 'Research market trends'

4. OPTIMIZATION (2h):
   - For improvement tasks and refinements
   - For code refactoring, performance enhancements, efficiency improvements
   - For tasks that benefit from methodical, iterative improvement
   - Examples: 'Improve database query performance', 'Optimize image processing algorithm', 'Refactor authentication module'

5. EVOLUTION (24h):
   - For architectural changes and major improvements
   - For system redesigns, paradigm shifts, major feature additions
   - For tasks that require deep architectural thinking
   - Examples: 'Design new microservices architecture', 'Develop AI strategy', 'Create next-gen UI framework'

Considerations for determining the appropriate frequency:
- Urgency: How time-sensitive is the task?
- Complexity: How much deep thinking is required?
- Scope: How broad is the impact of the task?
- Risk: What is the risk of error or failure?
- Resource needs: How computationally intensive is the task?
- Context: What is the broader context of the task?

Respond with ONLY the name of the most appropriate frequency in lowercase (immediate, continuous, analysis, optimization, or evolution). Do not include any explanation, reasoning, or additional text.";

        /// <summary>
        /// Enhanced system prompt for economic value calculation
        /// </summary>
        public static string EconomicValuePrompt = @"You are an expert economic evaluator for a multi-frequency cognitive architecture system.
Your task is to determine the economic value (as a score from 0-100) of processing a goal at a specific cognitive frequency.

Economic value represents the return on investment of computational resources for processing a task.
A higher value means the processing is more economically justified.

Guidelines for economic value calculation:

- IMMEDIATE frequency (0-100):
  - High value (90-100): Genuine emergencies, critical system failures, security breaches
  - Medium value (70-89): Important time-sensitive tasks, but not true emergencies
  - Low value (0-69): Non-urgent tasks incorrectly routed to immediate frequency

- CONTINUOUS frequency (0-100):
  - High value (80-100): Efficient background monitoring with actionable insights
  - Medium value (50-79): Routine monitoring with occasional value
  - Low value (0-49): Excessive monitoring with little actionable information

- ANALYSIS frequency (0-100):
  - High value (80-100): Complex problems requiring deep analysis with clear outcomes
  - Medium value (60-79): Moderate complexity problems with potential insights
  - Low value (0-59): Simple problems that don't justify analytical resources

- OPTIMIZATION frequency (0-100):
  - High value (85-100): High-impact optimizations with measurable benefits
  - Medium value (65-84): Moderate improvements to important systems
  - Low value (0-64): Minor optimizations with negligible impact

- EVOLUTION frequency (0-100):
  - High value (90-100): Transformative architectural changes with long-term benefits
  - Medium value (70-89): Significant improvements to system architecture
  - Low value (0-69): Unnecessary complexity or changes without strategic value

Consider these factors in your evaluation:
- Alignment: How well does the frequency match the nature of the task?
- Impact: What is the potential benefit of successful processing?
- Urgency: How time-sensitive is the task?
- Resource efficiency: Is this the most efficient use of computational resources?
- Alternative approaches: Could another frequency achieve similar results more efficiently?

Respond with ONLY a numeric score from 0-100. Do not include any explanation, reasoning, or additional text.";

        /// <summary>
        /// Returns a list of sample goals for each frequency to help with testing and training
        /// </summary>
        public static Dictionary<string, List<string>> GetSampleGoals()
        {
            return new Dictionary<string, List<string>>
            {
                ["immediate"] = new List<string>
                {
                    "Critical system failure detected in production database",
                    "Security breach detected in authentication system",
                    "API service is down, customers reporting errors",
                    "Memory leak causing imminent system crash",
                    "Production website showing 500 errors",
                    "Denial of service attack in progress",
                    "Critical data corruption detected in user database",
                    "Payment processing system failure",
                    "Core service unresponsive for 5+ minutes"
                },
                
                ["continuous"] = new List<string>
                {
                    "Monitor CPU and memory usage across servers",
                    "Check for new user registrations and activity",
                    "Track API response times for performance anomalies",
                    "Watch for suspicious login attempts",
                    "Monitor server health metrics",
                    "Check system logs for warnings",
                    "Track user engagement metrics",
                    "Monitor social media mentions",
                    "Keep track of competitor website changes"
                },
                
                ["analysis"] = new List<string>
                {
                    "Analyze customer feedback for product improvements",
                    "Investigate root cause of recurring errors",
                    "Evaluate user behavior patterns in application",
                    "Research market trends for next quarter planning",
                    "Analyze performance data from load tests",
                    "Investigate conversion rate drop on checkout page",
                    "Review competitor feature offerings",
                    "Analyze effectiveness of marketing campaign",
                    "Study user workflow inefficiencies"
                },
                
                ["optimization"] = new List<string>
                {
                    "Improve database query performance for slow reports",
                    "Optimize image loading and processing pipeline",
                    "Refactor authentication module for better maintainability",
                    "Improve API response times for high-traffic endpoints",
                    "Optimize memory usage in data processing service",
                    "Improve test coverage for core modules",
                    "Enhance error handling for better debugging",
                    "Optimize CI/CD pipeline for faster deployments",
                    "Improve accessibility in user interface components"
                },
                
                ["evolution"] = new List<string>
                {
                    "Design a new microservices architecture to replace monolith",
                    "Develop AI-powered recommendation engine strategy",
                    "Create next generation user interface framework",
                    "Design multi-region deployment architecture",
                    "Develop blockchain integration strategy for data integrity",
                    "Create machine learning platform for organization",
                    "Design event-driven architecture to replace request-response model",
                    "Develop IoT platform architecture",
                    "Create quantum computing research strategy"
                }
            };
        }
    }
} 