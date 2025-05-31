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

## Utility Tools

Tools for project maintenance and management:

- **integration-test.sh**: Tests the integrity of the project structure.
- **update-paths.sh**: Helps update references to old file paths after restructuring.

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

### Testing Project Structure Integrity

```bash
./tools/integration-test.sh
```

### Updating Path References

```bash
./tools/update-paths.sh
```

This will:
1. Search for references to old file paths
2. Offer to update these references
3. Offer to create symbolic links for backwards compatibility 