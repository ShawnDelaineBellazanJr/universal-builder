# Universal Builder

A self-evolving AI system running entirely on GitHub Actions that can build anything from a simple description.

## Overview

Universal Builder implements the PMCRO pattern (Planner, Maker, Checker, Reflector, Orchestrator) to create a system that:

1. **Plans** a solution based on your intent
2. **Makes** the implementation
3. **Checks** for issues
4. **Reflects** on the process
5. **Orchestrates** everything and determines next steps

The system runs entirely on GitHub Actions and uses GitHub itself as its database - storing templates in Gists, work tracking in Issues, history in Releases, and evolving through Pull Requests.

## Key Features

- **Economic Consciousness**: Evaluates task value against thresholds
- **Template-Driven Development**: Loads templates from GitHub Gists
- **GitHub-Native Infrastructure**: Uses GitHub as its database
- **Self-Evolution**: Creates PRs to improve itself (Strange Loop pattern)
- **24/7 Autonomous Operation**: Runs on a scheduled basis
- **Auto-Run Tools**: Rapid development cycle tools for local and Docker environments

## Architecture

The system consists of:

- **UniversalBuilder.cs**: Main implementation with PMCRO pattern
- **GitHubMemory.cs**: GitHub-based persistence layer
- **GitHub Actions Workflow**: Triggers the system
- **Prompty Templates**: Drives the system's behavior
- **Auto-Run Scripts**: Accelerates development cycles

## Getting Started

### Prerequisites

- GitHub repository
- GitHub Personal Access Token with repo, workflow, and gist permissions
- OpenAI API key

### Debug Mode

For local development and testing without GitHub API calls:

1. Set `DEBUG_MODE=true` in your `.env` or `.env.local` file
2. Files are stored locally in:
   - `debug_issues/` - For issue tracking
   - `debug_prs/` - For pull requests
   - `debug_releases/` - For release history
3. Create templates in the `templates/` folder to override default templates

### Setup

1. Fork this repository
2. Add the following secrets to your repository:
   - `GITHUB_TOKEN`: Your GitHub Personal Access Token
   - `OPENAI_API_KEY`: Your OpenAI API key
   - `DEFAULT_VALUE_THRESHOLD`: Default value threshold (e.g., "50")
3. For local development:
   ```bash
   # Set up environment
   make setup
   
   # Run with your intent
   make run ARGS="Build a calculator app 50"
   ```

### Usage

You can trigger the Universal Builder in three ways:

1. **Manual Trigger**: Go to Actions tab → Universal Builder → Run workflow → Enter your intent
2. **Issue Creation**: Create a new issue with your intent as the title
3. **Scheduled Run**: The system runs automatically every 5 minutes to check for work or self-evolve
4. **Local Development**:
   ```bash
   ./auto-run.sh dev "Build a web app" 50
   ```
5. **Docker Development**:
   ```bash
   make docker-dev
   ```

## Development Tools

The project includes several tools to accelerate development:

- **auto-run.sh**: Script for rapid development cycles
  ```bash
  ./auto-run.sh [dev|watch|test|workflow] [intent] [value_threshold]
  ```
- **Docker environment**: Containerized development
  ```bash
  # Start development container
  make docker-dev
  
  # Watch for file changes
  make docker-watch
  
  # Simulate GitHub Actions workflow
  make docker-workflow
  ```
- **Makefile**: Common development commands
  ```bash
  # See all available commands
  make help
  ```
- **CI/CD script**: For continuous integration pipelines
  ```bash
  ./ci-cd-run.sh "Build intent" 50 github-actions
  ```

For more details, see [QUICKSTART.md](QUICKSTART.md)

## How It Works

### The PMCRO Cycle

For each build request, the system:

1. **Planner**: Creates a detailed plan based on your intent
2. **Maker**: Implements the plan with concrete steps and code
3. **Checker**: Verifies the implementation against your intent
4. **Reflector**: Analyzes what worked, what didn't, and why
5. **Orchestrator**: Decides on next steps and improvements

### Economic Consciousness

Before starting work, the system evaluates the economic value of your request against a threshold. This prevents wasting resources on low-value tasks.

### Self-Evolution

The system continuously improves itself by:

1. Analyzing its own code and performance
2. Generating improvements
3. Creating pull requests with these improvements
4. Learning from its history to make better decisions

## Template System

The system uses Prompty templates stored in GitHub Gists. You can customize these templates to change how the system:

- Plans solutions
- Implements code
- Checks for issues
- Reflects on its work
- Makes economic decisions

## Extending the System

You can extend the Universal Builder by:

1. Creating custom Prompty templates
2. Modifying the PMCRO cycle
3. Adding new persistence mechanisms
4. Integrating with other systems via GitHub webhooks

## License

MIT License

## Acknowledgments

Built using:
- Microsoft Semantic Kernel 1.54.0
- Octokit
- GitHub Actions 