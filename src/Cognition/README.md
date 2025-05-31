# Cognition Components

This directory contains the cognitive components of the Autonomous AI system, including the multi-frequency cognitive architecture.

## Key Components

### Frequency Router

- **SkFrequencyRouter.cs**: Core router that determines the appropriate cognitive frequency for processing a goal based on semantic understanding.
- **SkFrequencyRouterPrompts.cs**: Provides optimized prompts for frequency determination and economic value calculation.
- **SkFrequencyRouterMonitor.cs**: Tracks router performance and decisions to enable continuous improvement.
- **SkFrequencyRouterTest.cs**: Test suite for the SK Frequency Router.

### Frequency Processors

- **ImmediateReflexProcessor.cs**: Handles immediate tasks requiring quick response (0-second frequency).
- **ContinuousAwarenessMonitor.cs**: Manages continuous tasks (30-second frequency).
- **AnalysisThinkingProcessor.cs**: Processes analysis tasks (15-minute frequency).
- **OptimizationReflectionProcessor.cs**: Handles optimization tasks (2-hour frequency).
- **EvolutionArchitectureProcessor.cs**: Manages evolution tasks (24-hour frequency).

### Orchestration

- **CognitiveFrequencyOrchestrator.cs**: Coordinates processing across all cognitive frequencies.
- **AdaptiveScheduler.cs**: Manages scheduling and timing of cognitive processes.

### Economic Consciousness

- **EconomicConsciousness.cs**: Evaluates the economic value of cognitive processes.

## Documentation

For detailed documentation on these components, see the [docs/components](../../docs/components) directory.

### SK Frequency Router

The SK Frequency Router is a Semantic Kernel-based component that intelligently routes tasks to the appropriate cognitive frequency based on semantic analysis.

- [SK Frequency Router Documentation](../../docs/components/sk-frequency-router.md): Detailed documentation
- [SK Frequency Router Summary](../../docs/components/SK-FREQUENCY-ROUTER-SUMMARY.md): Implementation summary 