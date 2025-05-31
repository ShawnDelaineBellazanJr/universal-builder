# Project Structure Guide

This document provides a guide for navigating the Autonomous AI project structure.

## Directory Organization

The project is organized into the following key directories:

```
AutonomousAI/
├── src/                  # Source code
│   ├── Agents/           # Agent implementations
│   ├── Cognition/        # Cognitive architecture components
│   ├── Evolution/        # System self-improvement components
│   ├── Models/           # Data models and schemas
│   ├── Process/          # Process management components
│   └── Reasoning/        # Reasoning and decision-making components
│
├── docs/                 # Documentation
│   ├── architecture/     # System architecture documentation
│   └── components/       # Component-specific documentation
│
├── tools/                # Development and analysis tools
│   ├── benchmarks/       # Benchmarking tools
│   └── visualization/    # Data visualization tools
│
├── tests/                # Test suites
│   └── sk-frequency-router/ # Tests for SK Frequency Router
│
└── scripts               # Root-level scripts for running the system
```

## Key Files

### Source Code

- **src/Cognition/CognitiveFrequencyOrchestrator.cs**: Coordinates processing across all cognitive frequencies
- **src/Cognition/SkFrequencyRouter.cs**: Routes tasks to appropriate cognitive frequencies
- **src/Cognition/SkFrequencyRouterPrompts.cs**: Provides prompts for the router
- **src/Cognition/SkFrequencyRouterMonitor.cs**: Monitors router performance

### Documentation

- **docs/components/sk-frequency-router.md**: Detailed documentation for SK Frequency Router
- **docs/components/SK-FREQUENCY-ROUTER-SUMMARY.md**: Implementation summary

### Tools

- **tools/benchmarks/benchmark-frequency-router.sh**: Benchmarks the SK Frequency Router
- **tools/benchmarks/run-benchmark-and-visualize.sh**: Runs benchmark and generates visualizations
- **tools/visualization/visualize-benchmark-results.py**: Generates charts from benchmark data
- **tools/visualization/generate-dashboard.py**: Creates an interactive HTML dashboard
- **tools/integration-test.sh**: Tests the project structure integrity

### Tests

- **tests/sk-frequency-router/run-sk-frequency-router-test.sh**: Runs tests for the SK Frequency Router

## Navigation Tips

### Finding Implementation Files

Source code is organized by domain in the `src` directory. For example, all cognitive components are in `src/Cognition/`.

### Finding Documentation

Documentation is organized by type in the `docs` directory:
- Component-specific documentation is in `docs/components/`
- Architecture documentation is in `docs/architecture/`

### Running Tools

Tools are organized by purpose in the `tools` directory:
- Benchmarking tools are in `tools/benchmarks/`
- Visualization tools are in `tools/visualization/`

### Running Tests

Tests are organized by component in the `tests` directory.

## Common Tasks

### Running the SK Frequency Router Test

```bash
cd tests/sk-frequency-router
./run-sk-frequency-router-test.sh
```

### Benchmarking the SK Frequency Router

```bash
cd tools/benchmarks
./run-benchmark-and-visualize.sh [number_of_test_cases]
```

### Generating a Dashboard

```bash
cd tools/visualization
python3 generate-dashboard.py [benchmark_results.json] [output_file.html]
```

### Testing Project Structure Integrity

```bash
./tools/integration-test.sh
```

## Adding New Components

When adding new components to the project:

1. Add source code to the appropriate subdirectory of `src/`
2. Add documentation to `docs/components/`
3. Add tests to a new subdirectory of `tests/`
4. Add any tools to the appropriate subdirectory of `tools/`
5. Update READMEs to reference the new component 