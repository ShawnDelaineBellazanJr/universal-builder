# SK Evolution Engine

An autonomous system for evolving the multi-frequency cognitive architecture using Semantic Kernel.

## Overview

The SK Evolution Engine is designed to analyze the .NET SDK and Semantic Kernel repositories, learn from their patterns and best practices, and autonomously propose and implement improvements to our multi-frequency cognitive architecture.

This system leverages the power of large language models through Microsoft's Semantic Kernel to continuously evolve our codebase with minimal human intervention.

## Features

- **Repository Analysis**: Analyzes the .NET SDK and Semantic Kernel repositories to learn best practices
- **Improvement Proposals**: Autonomously proposes improvements to the codebase
- **Implementation**: Implements the proposed improvements and creates pull requests
- **Economic Consciousness**: Evaluates the value of improvements before implementing them
- **Multiple Frequencies**: Supports the multi-frequency cognitive architecture (Immediate, Continuous, Analysis, Optimization, Evolution)

## Usage

### Prerequisites

- .NET 7.0 or later
- OpenAI API key (for GPT-4)
- GitHub token (for repository access)

### Setup

1. Create a `.env` file in the `SkEvolutionEngine` directory with your API keys:

```
# OpenAI Configuration
OPENAI_API_KEY=your_openai_api_key_here
OPENAI_ORG_ID=your_org_id_here (optional)

# GitHub Configuration
GITHUB_TOKEN=your_github_token_here

# Logging
LOG_LEVEL=Information
```

### Running the Engine

Use the `run-sk-evolution.sh` script to run the SK Evolution Engine:

```bash
# Run the full evolution cycle
./run-sk-evolution.sh

# Run a specific command
./run-sk-evolution.sh analyze-dotnet
./run-sk-evolution.sh analyze-sk
./run-sk-evolution.sh propose-improvements
./run-sk-evolution.sh implement-improvements
```

## Architecture

The SK Evolution Engine follows these steps in its evolution cycle:

1. **Analyze**: Study the .NET SDK and Semantic Kernel repositories to understand best practices
2. **Learn**: Extract patterns and principles from the analysis
3. **Propose**: Generate specific improvement proposals with justifications
4. **Implement**: Create code changes for the approved proposals
5. **Validate**: Ensure the changes work as expected and add value
6. **Integrate**: Create pull requests to integrate the changes

## Configuration

The system behavior can be customized through the `config.json` file:

- **semanticKernel**: Model settings (temperature, max tokens, etc.)
- **repositories**: GitHub repositories to analyze and target
- **evolution**: Settings for the evolution cycle (interval, threshold, etc.)
- **frequencies**: Configuration for each cognitive frequency

## GitHub Workflow

The SK Evolution Engine can be run automatically through GitHub Actions using the workflow in `.github/workflows/sk-evolution.yml`. This workflow runs daily and can also be triggered manually.

## Integration with Multi-Frequency Cognitive Architecture

The SK Evolution Engine is designed to work with our multi-frequency cognitive architecture:

- **Immediate (0s)**: Quick responses to urgent inputs
- **Continuous (30s)**: Ongoing processing of information
- **Analysis (15m)**: Deeper analysis of complex problems
- **Optimization (2h)**: Performance and design improvements
- **Evolution (24h)**: Long-term architectural evolution 