# Universal Builder

   A self-evolving system that can build ANY project type using AI, entirely on GitHub Actions. The Universal Builder implements the PMCRO pattern (Planner, Maker, Checker, Reflector, Orchestrator) and uses GitHub as its database.

## Features

- **GitHub-Native**: Runs entirely in GitHub Actions, no external services required
- **Strange Loop Self-Evolution**: Modifies its own code via AI-generated PRs
- **Economic Consciousness**: Makes value-based decisions and refuses undervalued tasks
- **PMCRO Architecture**: Implements the Planner-Maker-Checker-Reflector-Orchestrator pattern
- **Template-Driven**: Loads project templates from GitHub Gists
- **Edge Case Handling**: Robust error handling, retry logic, and fallback strategies

## Requirements

- GitHub repository with Actions enabled
- OpenAI API key (stored as a GitHub Secret)
- .NET 8.0 or later
- GitHub token with appropriate permissions

## Setup

1. Clone this repository
2. Run the setup script to bootstrap the system:

```bash
chmod +x setup.sh
./setup.sh
```

3. Push the code to your GitHub repository
4. Configure the following GitHub Secrets:
   - `OPENAI_API_KEY`: Your OpenAI API key
   - `DEFAULT_VALUE_THRESHOLD`: Default value threshold for economic decisions (e.g., "50")

## Usage

You can trigger the Universal Builder in two ways:

### 1. Create an Issue

Create a new issue with your build intent in the title. The Universal Builder will automatically analyze the intent, evaluate its economic value, and execute the PMCRO cycle if the value exceeds the threshold.

### 2. Manual Dispatch

Manually trigger the workflow with the following parameters:
- `intent`: Your build intent (e.g., "Build a RESTful API for task management")
- `value_threshold` (optional): Value threshold for this specific task

## Architecture

The Universal Builder system consists of the following components:

- **UniversalBuilder.cs**: Main implementation of the PMCRO pattern and self-evolution
- **GitHubMemory.cs**: GitHub-based persistence using Issues, Gists, Secrets, and Releases
- **universal-builder.yml**: GitHub Actions workflow definition
- **universal.prompty**: Sample template for generic project builds

### PMCRO Pattern

1. **Planner**: Creates a detailed plan based on the intent
2. **Maker**: Implements the plan
3. **Checker**: Verifies the implementation
4. **Reflector**: Reflects on the process and outcomes
5. **Orchestrator**: Coordinates the process and decides next steps

### GitHub as a Database

- **Gists**: Store templates for different project types
- **Secrets**: Store economic values and configuration
- **Issues**: Track work items and progress
- **Releases**: Store build history and outcomes

### Self-Evolution

After completing a build, the Universal Builder analyzes its own code and suggests improvements through GitHub PRs. This creates a "Strange Loop" where the system enhances itself over time.

## Economic Consciousness

The Universal Builder evaluates the economic value of each task before proceeding:

1. Extracts the intent from the user's request
2. Evaluates the task value using the `value-evaluator` template
3. Compares the value to the configured threshold
4. Refuses undervalued tasks with a detailed explanation

## Customization

### Templates

Create or modify Prompty templates in GitHub Gists to customize the behavior of each PMCRO component. The system will automatically load templates from the configured Gist ID.

### Value Thresholds

Adjust value thresholds in GitHub Secrets to control which tasks the system accepts or rejects.

## Troubleshooting

If you encounter issues:

1. Check the workflow run logs for detailed error messages
2. Verify that all required secrets are properly configured
3. Ensure the GitHub token has appropriate permissions
4. Check for rate limit issues in the GitHub API

## Contributing

Contributions to the Universal Builder are welcome! Please open an issue or PR with your suggested improvements.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 