# Tools

This directory contains various tools used in the Autonomous AI system, organized by purpose.

## Benchmarks

Tools for benchmarking and comparing components:

- **benchmark-frequency-router.sh**: Compares the SK Frequency Router with the keyword-based router.
- **run-benchmark-and-visualize.sh**: Runs the benchmark and generates visualizations.

## Visualization

Tools for visualizing and analyzing data:

- **visualize-benchmark-results.py**: Generates charts and graphs from benchmark data.
- **generate-dashboard.py**: Creates an interactive HTML dashboard for comprehensive analysis.

## Usage

### Benchmarking the SK Frequency Router

```bash
cd tools/benchmarks
./run-benchmark-and-visualize.sh [number_of_test_cases]
```

This will:
1. Run the benchmark with the specified number of test cases (default: 20)
2. Generate visualizations from the benchmark results
3. Create an interactive HTML dashboard

### Visualizing Existing Benchmark Results

```bash
cd tools/visualization
python3 visualize-benchmark-results.py [benchmark_results.json]
```

### Generating an HTML Dashboard

```bash
cd tools/visualization
python3 generate-dashboard.py [benchmark_results.json] [output_file.html]
``` 