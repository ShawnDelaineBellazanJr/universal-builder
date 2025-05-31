flowchart TD
    subgraph "Multi-Frequency Cognitive Architecture"
        G[Goals] --> O[CognitiveFrequencyOrchestrator]
        E[EconomicConsciousness] --> O
        
        O --> I[ImmediateReflexProcessor<br/>0s]
        O --> C[ContinuousAwarenessMonitor<br/>30s]
        O --> A[AnalysisThinkingProcessor<br/>15m]
        O --> P[OptimizationReflectionProcessor<br/>2h]
        O --> V[EvolutionArchitectureProcessor<br/>24h]
        
        I --> R[CognitiveResponse]
        C --> R
        A --> R
        P --> R
        V --> R
        
        S[AdaptiveScheduler] <--> O
        S <--> L[Learning Feedback Loop]
        R --> L
    end
    
    subgraph "Strange Loop"
        SL1[Level 1: Template builds projects] --> SL2[Level 2: Template learns from building]
        SL2 --> SL3[Level 3: Template improves itself]
        SL3 --> SL4[Level 4: Template improves SK]
        SL4 --> SL5[Level 5: Improved SK enables better templates]
        SL5 --> SL6[Level 6: RECURSION]
        SL6 -.-> SL1
    end
    
    V --> SL1