# SK Evolution Engine

The SK Evolution Engine is an autonomous system for evolving Semantic Kernel implementations in the multi-frequency cognitive architecture.

## Overview

This engine analyzes the codebase, proposes improvements, and implements them automatically. It runs at the "Evolution" frequency (24h) and focuses on architectural changes and major improvements.

## Features

- **Autonomous Analysis**: Analyzes .NET SDK and Semantic Kernel repositories to identify best practices
- **Improvement Proposals**: Generates improvement ideas with economic value assessments
- **Automated Implementation**: Can implement improvements automatically via pull requests
- **GitHub Integration**: Works with GitHub Actions for scheduled evolution cycles

## Usage

You can run the SK Evolution Engine in different modes:

```bash
# Run a full evolution cycle
./run-sk-evolution.sh

# Analyze .NET SDK repository
./run-sk-evolution.sh analyze-dotnet

# Analyze Semantic Kernel repository
./run-sk-evolution.sh analyze-sk

# Propose improvements based on analysis
./run-sk-evolution.sh propose-improvements

# Implement proposed improvements
./run-sk-evolution.sh implement-improvements
```

## Recent Improvements

The SK Evolution Engine has proposed and implemented several improvements:

1. **Semantic Kernel-based Frequency Router** (Economic Value: 92/100)
   - Created a cognitive frequency router that uses SK to determine which cognitive frequency is best for a given task based on economic value
   - Implemented in `src/Cognition/SkFrequencyRouter.cs`

2. **Adaptive Learning for Frequency Thresholds** (Economic Value: 87/100)
   - Proposed improvement to implement an adaptive learning system that automatically adjusts frequency thresholds based on past performance

3. **SK Memory for Goal History** (Economic Value: 90/100)
   - Proposed improvement to use SK Memory to store and retrieve past goals and their outcomes to inform future decisions

## Configuration

The engine is configured through `config.json` which includes:

- Semantic Kernel model configuration
- Repository settings
- Evolution parameters
- Frequency configurations

## GitHub Workflow

The engine is integrated with GitHub Actions and runs on a daily schedule. It can also be triggered manually:

```bash
gh workflow run sk-evolution.yml
```

Or with specific parameters:

```bash
gh workflow run sk-evolution.yml --field command=propose-improvements
``` 