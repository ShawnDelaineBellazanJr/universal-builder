# Universal Autonomous Builder

A self-evolving AI system running on GitHub that can autonomously build any project from a simple description, while continuously improving itself and its foundation (Semantic Kernel).

## Overview

Universal Autonomous Builder implements a sophisticated Strange Loop pattern with multiple components:

1. **GoalProcessor**: Analyzes and structures user intents
2. **StrategicPlanner**: Creates detailed build plans
3. **ChainOfThoughtReasoning**: Performs step-by-step reasoning
4. **StrangeLoopEngine**: Enables recursive self-improvement
5. **SK Evolution Engine**: Autonomously evolves system using Semantic Kernel

The system follows a PMCRO pattern (Planner, Maker, Checker, Reflector, Orchestrator) and implements a Strange Loop, where:

1. **Template builds projects** (Level 1)
2. **Template learns from building projects** (Level 2) 
3. **Template improves itself using learnings** (Level 3)
4. **Template improves Semantic Kernel** (Level 4)
5. **Improved SK enables better template capabilities** (Level 5)
6. **RECURSION - Enhanced template builds better projects** (Level 6)

## Key Features

- **Economic Consciousness**: Evaluates task value against thresholds
- **Template-Driven Development**: Uses advanced Prompty templates
- **GitHub Integration**: Uses GitHub as its database and deployment platform
- **Chain-of-Thought Reasoning**: Employs structured reasoning for complex problems
- **Self-Evolution**: Creates PRs to improve itself (Strange Loop pattern)
- **Meta-Evolution**: Contributes improvements to Semantic Kernel
- **Project Type Templates**: Specialized for various project types (iOS, web, etc.)
- **Multi-Frequency Cognition**: Processes at five different time scales (0s, 30s, 15m, 2h, 24h)
- **SK Evolution Engine**: Autonomous system for evolving codebase using Semantic Kernel
- **SK Frequency Router**: Intelligently routes tasks to appropriate cognitive frequencies using Semantic Kernel

## Architecture

The system consists of:

- **Goal Processing Layer**: Analyzes and structures user goals
- **Reasoning Layer**: Performs chain-of-thought reasoning about goals
- **Planning Layer**: Creates detailed project plans
- **Evolution Layer**: Enables self-improvement and meta-evolution
- **GitHub Integration Layer**: Interfaces with GitHub APIs
- **Cognitive Layer**: Implements multi-frequency cognitive processes
- **SK Evolution Layer**: Analyzes and evolves codebase using SK
- **Frequency Routing Layer**: Routes tasks to appropriate cognitive frequencies

## Getting Started

### Prerequisites

- GitHub repository
- GitHub Personal Access Token with repo, workflow, and gist permissions
- OpenAI API key
- .NET 9.0

### Setup

1. Clone this repository:
   ```bash
   git clone https://github.com/ShawnDelaineBellazanJr/AutonomousAI.git
   cd AutonomousAI
   ```

2. Create a `.env.local` file with your configuration:
   ```
   GITHUB_TOKEN=your_github_token
   OPENAI_API_KEY=your_openai_key
   DEBUG_MODE=true  # For local development
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the system:
   ```bash
   # Basic usage
   dotnet run universal-build --goal "Build a calculator app"
   
   # Specify project type
   dotnet run universal-build --goal "Build an iOS app for tracking workouts" --project-type ios-app
   
   # Disable recursion
   dotnet run universal-build --goal "Build a web service" --no-recursion
   ```

### Debug Mode

For local development and testing without GitHub API calls:

1. Set `DEBUG_MODE=true` in your `.env` or `.env.local` file
2. Create a `templates` directory with project templates
3. Run with debug mode enabled

## Available Commands

- **universal-build**: Builds a project based on a goal
  ```bash
  dotnet run universal-build --goal "Your goal here" --project-type [optional] --recursion/--no-recursion
  ```

- **check-evolution**: Checks for evolution opportunities
  ```bash
  dotnet run check-evolution --threshold 0.8 --auto-pr true
  ```

- **meta-evolution**: Runs meta-evolution to improve Semantic Kernel
  ```bash
  dotnet run meta-evolution --target semantic-kernel --confidence-threshold 0.9
  ```

- **generate-report**: Generates a report
  ```bash
  dotnet run generate-report --include-metrics --include-learnings --include-evolution
  ```

- **sk-evolution**: Runs the SK Evolution Engine
  ```bash
  cd sk-evolution
  ./run-sk-evolution.sh
  
  # Or run specific commands
  ./run-sk-evolution.sh analyze-dotnet
  ./run-sk-evolution.sh analyze-sk
  ./run-sk-evolution.sh propose-improvements
  ./run-sk-evolution.sh implement-improvements
  ```

- **test-frequency-router**: Tests the SK Frequency Router
  ```bash
  ./run-sk-frequency-router-test.sh
  ```

## Multi-Frequency Cognitive Architecture

The system implements a multi-frequency cognitive architecture with five frequencies:

- **Immediate (0s)**: Fast, reactive processing for urgent tasks
- **Continuous (30s)**: Regular background processing for continuous tasks
- **Analysis (15m)**: Deeper analytical processing for complex problems
- **Optimization (2h)**: Optimization processes for improving efficiency
- **Evolution (24h)**: Long-term evolution processes for system improvement

Each frequency has its own economic threshold (95, 50, 70, 85, 90) to determine when processing at that level is justified.

### SK Frequency Router

The SK Frequency Router is a Semantic Kernel-based component that intelligently routes tasks to the appropriate cognitive frequency based on:

- Task urgency and complexity
- Economic value assessment
- Available resources and constraints
- Task context and goals

It replaces keyword-based routing with AI-powered semantic analysis, ensuring tasks are processed at the most efficient frequency. The router:

1. Analyzes the goal and context using Semantic Kernel
2. Determines the most appropriate cognitive frequency
3. Calculates the economic value of processing at that frequency
4. Compares the economic value against frequency thresholds
5. Routes the task to the appropriate processor

## SK Evolution Engine

The SK Evolution Engine is an autonomous system that:

1. Analyzes the .NET SDK and Semantic Kernel repositories
2. Learns best practices and patterns from these codebases
3. Proposes improvements to our multi-frequency cognitive architecture
4. Implements these improvements and creates pull requests
5. Continuously evolves the codebase

It runs on a daily schedule and can be triggered manually through GitHub Actions. See the [SK Evolution Engine README](sk-evolution/README.md) for more details.

## Project Templates

The system supports various project types with specialized templates:

- **ios-app**: iOS mobile applications with SwiftUI
- **web-service**: Web services, APIs, and backends
- **web-app**: Web applications with frontend components
- **data-pipeline**: Data processing or ETL pipelines
- **ml-model**: Machine learning models or AI systems
- **autonomous-agent**: AI agents or autonomous systems
- **template-evolution**: Improvements to the builder template itself
- **sk-enhancement**: Improvements to the Semantic Kernel framework

## The Strange Loop Pattern

The system implements a Strange Loop pattern, where:

1. The system builds a project
2. It learns from this experience
3. It improves itself using these learnings
4. It improves Semantic Kernel
5. The improved SK enables better capabilities
6. This triggers recursion, repeating the cycle

This creates a system that can continuously improve itself and its foundation.

## Chain-of-Thought Reasoning

The system employs structured chain-of-thought reasoning:

1. **Goal Decomposition**: Breaks down complex goals
2. **Context Analysis**: Analyzes project context
3. **Strategy Selection**: Generates and selects strategies
4. **Execution Planning**: Creates detailed step-by-step plans
5. **Risk Assessment**: Identifies and mitigates risks
6. **Recursion Opportunity Identification**: Finds improvement opportunities

## License

MIT License

## Acknowledgments

Built using:
- Microsoft Semantic Kernel 1.54.0
- Microsoft.SemanticKernel.Prompty
- Octokit 