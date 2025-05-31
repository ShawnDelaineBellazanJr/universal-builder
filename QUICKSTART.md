# AutonomousAI Quick Start Guide

This guide will help you get AutonomousAI up and running quickly, with options for local development or Docker-based workflows.

## Prerequisites

- .NET SDK 9.0 or later
- Git
- Docker (optional, for containerized development)
- GitHub Personal Access Token (for GitHub API integration)
- OpenAI API Key or Azure OpenAI credentials

## Quick Start

### Option 1: Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/AutonomousAI.git
   cd AutonomousAI
   ```

2. **Setup environment**
   ```bash
   make setup
   ```
   This creates a `.env` file. Edit it to add your API keys.

3. **Build and run**
   ```bash
   make run ARGS="Build a calculator app 50"
   ```
   Where "Build a calculator app" is your intent and "50" is the value threshold.

### Option 2: Docker Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/AutonomousAI.git
   cd AutonomousAI
   ```

2. **Setup environment**
   ```bash
   cp .env-example .env
   ```
   Edit `.env` to add your API keys.

3. **Run with Docker**
   ```bash
   make docker-dev
   ```

## Development Modes

AutonomousAI supports multiple development modes:

- **Dev Mode**: Standard build and run
  ```bash
  make run
  ```

- **Watch Mode**: Auto-rebuilds when files change
  ```bash
  make watch
  ```

- **Workflow Mode**: Simulates GitHub Actions workflow
  ```bash
  make workflow
  ```

- **Test Mode**: Runs tests
  ```bash
  make test
  ```

## Common Tasks

### Running with a specific intent

```bash
make run ARGS="Build a to-do app with React 75"
```

### Starting the file watcher

```bash
make watch
```

### Simulating GitHub Actions workflow

```bash
make workflow ARGS="Self-evolve and improve capabilities 50"
```

### Running in Docker with file watching

```bash
make docker-watch
```

## Troubleshooting

1. **API Key Issues**
   - Ensure your API keys are correctly set in the `.env` file
   - For OpenAI, verify your account has available credits

2. **GitHub Authentication**
   - Ensure your GitHub token has the necessary permissions:
     - `repo` for repository access
     - `workflow` for workflow management
     - `gist` for Gist operations

3. **Docker Issues**
   - Ensure Docker and Docker Compose are installed
   - Try rebuilding the Docker image: `docker-compose build --no-cache`

## Getting Help

Run `make help` to see all available commands and options.

For more detailed documentation, see the README.md file. 