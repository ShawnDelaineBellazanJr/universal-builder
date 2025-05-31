# Autonomous AI

An autonomous AI system with multi-frequency cognitive architecture.

## Overview

This system implements a cognitive architecture with multiple processing frequencies, allowing different types of cognitive processes to run on different timescales:

- **Immediate** (0 seconds): Instant responses to critical events
- **Continuous** (30 seconds): Ongoing monitoring and awareness
- **Analysis** (15 minutes): Deeper thinking and problem solving
- **Optimization** (2 hours): System improvement and refinement
- **Evolution** (24 hours): Architectural changes and major improvements

## Project Structure

The project is organized into the following key directories:

- **src/**: Source code organized by domain
  - **Cognition/**: Multi-frequency cognitive architecture components
  - **Evolution/**: System self-improvement and evolution components
  - **Models/**: Data models and schemas
  - **Process/**: Process management components
  - **Reasoning/**: Reasoning and decision-making components
  - **Agents/**: Agent implementations

- **docs/**: Documentation
  - **components/**: Documentation for individual components
  - **architecture/**: System architecture documentation

- **tools/**: Tools for development, testing, and analysis
  - **benchmarks/**: Benchmarking tools
  - **visualization/**: Data visualization tools

- **tests/**: Test suites for various components

## Key Components

### SK Frequency Router

The SK Frequency Router is a Semantic Kernel-based component that intelligently routes tasks to the appropriate cognitive frequency based on semantic understanding rather than keyword matching.

- [Documentation](docs/components/sk-frequency-router.md)
- [Implementation Summary](docs/components/SK-FREQUENCY-ROUTER-SUMMARY.md)
- [Source Code](src/Cognition/SkFrequencyRouter.cs)
- [Tests](tests/sk-frequency-router/run-sk-frequency-router-test.sh)
- [Benchmarking](tools/benchmarks/benchmark-frequency-router.sh)
- [Visualization](tools/visualization/generate-dashboard.py)

### SK Evolution Engine

The SK Evolution Engine operates at the evolution frequency (24 hours) to analyze the system and propose architectural improvements.

Key capabilities:
- Analyzes the existing codebase
- Identifies potential improvements
- Designs implementation strategies
- Generates code changes
- Creates GitHub PRs for proposed enhancements

## Cognitive Frequency Architecture

The system processes information at different time scales, with each frequency handling different types of cognitive processes:

1. **Immediate (0s)**: Fast, reflexive responses to urgent inputs
2. **Continuous (30s)**: Ongoing monitoring and awareness
3. **Analysis (15m)**: Deeper thinking and problem-solving
4. **Optimization (2h)**: System improvement and refinement
5. **Evolution (24h)**: Architectural changes and major improvements

## Getting Started

### Prerequisites

- .NET 7.0 or higher
- OpenAI API key
- GitHub token (for Evolution Engine)

### Installation

1. Clone the repository
2. Set environment variables:
   ```
   export OPENAI_API_KEY=your_openai_key
   export GITHUB_TOKEN=your_github_token
   ```
3. Build the project:
   ```
   dotnet build
   ```

### Running the System

To run the system with all cognitive frequencies:

```
./run.sh
```

To run the SK Evolution Engine only:

```
./run-sk-evolution.sh
```

To run the SK Frequency Router tests:

```
./tests/sk-frequency-router/run-sk-frequency-router-test.sh
```

## Dependencies

- Microsoft.SemanticKernel
- Microsoft.SemanticKernel.Connectors.OpenAI
- Microsoft.SemanticKernel.Prompty
- Octokit 