#!/usr/bin/env python3
"""
SK Frequency Router Benchmark Visualization Tool
This script visualizes the benchmark results from the SK Frequency Router tests
"""

import json
import sys
import os
import numpy as np
import matplotlib.pyplot as plt
from collections import Counter, defaultdict
from datetime import datetime

def load_data(filename):
    """Load benchmark data from JSON file"""
    try:
        with open(filename, 'r') as f:
            data = json.load(f)
        return data
    except Exception as e:
        print(f"Error loading data: {e}")
        return None

def analyze_data(data):
    """Analyze the benchmark data"""
    results = {
        'sk_frequencies': [],
        'keyword_frequencies': [],
        'sk_economic_values': [],
        'keyword_economic_values': [],
        'sk_processing_times': [],
        'keyword_processing_times': [],
        'agreements': [],
        'disagreements': []
    }
    
    # Process data
    for item in data:
        # Only process SkFrequencyRouter results
        if 'RecommendedFrequency' not in item:
            continue
            
        frequency = item['RecommendedFrequency']
        goal = item['Goal']
        context = item.get('Context', '')
        economic_value = item.get('EconomicValue', 0)
        
        # Store frequencies and economic values
        results['sk_frequencies'].append(frequency)
        results['sk_economic_values'].append(economic_value)
        
        # If we have execution time, store it
        if 'ExecutionTime' in item:
            # Convert ticks to milliseconds
            ticks = int(item['ExecutionTime'].replace(' Ticks', ''))
            ms = ticks / 10000  # Convert ticks to milliseconds
            results['sk_processing_times'].append(ms)
    
    return results

def create_visualizations(results, output_dir='.'):
    """Create visualizations from the analyzed data"""
    # Create output directory if it doesn't exist
    os.makedirs(output_dir, exist_ok=True)
    
    # Set style
    plt.style.use('ggplot')
    
    # 1. Frequency Distribution
    create_frequency_distribution(results, output_dir)
    
    # 2. Economic Value Distribution
    create_economic_value_distribution(results, output_dir)
    
    # 3. Processing Time Comparison
    if results['sk_processing_times']:
        create_processing_time_visualization(results, output_dir)
    
    # Create summary visualization
    create_summary_visualization(results, output_dir)
    
    print(f"Visualizations saved to {output_dir}")

def create_frequency_distribution(results, output_dir):
    """Create frequency distribution visualization"""
    plt.figure(figsize=(10, 6))
    
    # Count frequencies
    sk_freq_count = Counter(results['sk_frequencies'])
    
    # Get ordered frequencies and their counts
    frequencies = ['immediate', 'continuous', 'analysis', 'optimization', 'evolution']
    sk_counts = [sk_freq_count.get(freq, 0) for freq in frequencies]
    
    # Create grouped bar chart
    x = np.arange(len(frequencies))
    width = 0.35
    
    fig, ax = plt.subplots(figsize=(12, 7))
    sk_bars = ax.bar(x, sk_counts, width, label='SK Router', color='#3498db')
    
    # Add labels and title
    ax.set_xlabel('Frequency')
    ax.set_ylabel('Count')
    ax.set_title('Frequency Distribution: SK Router')
    ax.set_xticks(x)
    ax.set_xticklabels(frequencies)
    ax.legend()
    
    # Add count labels on bars
    for bar in sk_bars:
        height = bar.get_height()
        ax.annotate(f'{height}',
                    xy=(bar.get_x() + bar.get_width() / 2, height),
                    xytext=(0, 3),  # 3 points vertical offset
                    textcoords="offset points",
                    ha='center', va='bottom')
    
    # Add percentage labels
    total_sk = sum(sk_counts)
    for i, bar in enumerate(sk_bars):
        height = bar.get_height()
        if height > 0:
            percentage = height / total_sk * 100
            ax.annotate(f'{percentage:.1f}%',
                        xy=(bar.get_x() + bar.get_width() / 2, height / 2),
                        ha='center', va='center',
                        color='white', fontweight='bold')
    
    plt.tight_layout()
    plt.savefig(os.path.join(output_dir, 'frequency_distribution.png'), dpi=300)
    plt.close()

def create_economic_value_distribution(results, output_dir):
    """Create economic value distribution visualization"""
    plt.figure(figsize=(12, 7))
    
    # Group economic values by frequency
    freq_values = defaultdict(list)
    for freq, value in zip(results['sk_frequencies'], results['sk_economic_values']):
        freq_values[freq].append(value)
    
    # Create box plot for economic values by frequency
    frequencies = ['immediate', 'continuous', 'analysis', 'optimization', 'evolution']
    data = [freq_values.get(freq, []) for freq in frequencies]
    
    # Only include frequencies that have data
    valid_freqs = []
    valid_data = []
    for i, values in enumerate(data):
        if values:
            valid_freqs.append(frequencies[i])
            valid_data.append(values)
    
    if not valid_data:
        print("No economic value data to visualize")
        return
    
    plt.boxplot(valid_data, patch_artist=True, boxprops=dict(facecolor='#3498db'))
    plt.xlabel('Frequency')
    plt.ylabel('Economic Value')
    plt.title('Economic Value Distribution by Frequency')
    plt.xticks(range(1, len(valid_freqs) + 1), valid_freqs)
    
    # Add mean value labels
    for i, values in enumerate(valid_data):
        if values:
            mean_val = np.mean(values)
            plt.text(i + 1, np.min(values) - 5, f'Mean: {mean_val:.1f}', 
                     ha='center', va='top', fontweight='bold')
    
    plt.grid(True, linestyle='--', alpha=0.7)
    plt.tight_layout()
    plt.savefig(os.path.join(output_dir, 'economic_value_distribution.png'), dpi=300)
    plt.close()

def create_processing_time_visualization(results, output_dir):
    """Create processing time visualization"""
    plt.figure(figsize=(10, 6))
    
    # Group processing times by frequency
    freq_times = defaultdict(list)
    for freq, time in zip(results['sk_frequencies'], results['sk_processing_times']):
        freq_times[freq].append(time)
    
    # Calculate average processing time per frequency
    frequencies = ['immediate', 'continuous', 'analysis', 'optimization', 'evolution']
    avg_times = []
    for freq in frequencies:
        times = freq_times.get(freq, [])
        if times:
            avg_times.append(np.mean(times))
        else:
            avg_times.append(0)
    
    # Create bar chart
    plt.bar(frequencies, avg_times, color='#3498db')
    plt.xlabel('Frequency')
    plt.ylabel('Average Processing Time (ms)')
    plt.title('Average Processing Time by Frequency')
    
    # Add value labels
    for i, v in enumerate(avg_times):
        if v > 0:
            plt.text(i, v + 0.1, f'{v:.1f} ms', ha='center')
    
    plt.grid(True, linestyle='--', alpha=0.7, axis='y')
    plt.tight_layout()
    plt.savefig(os.path.join(output_dir, 'processing_time.png'), dpi=300)
    plt.close()

def create_summary_visualization(results, output_dir):
    """Create summary visualization with key metrics"""
    plt.figure(figsize=(12, 8))
    
    # Calculate key metrics
    total_decisions = len(results['sk_frequencies'])
    freq_distribution = Counter(results['sk_frequencies'])
    avg_economic_value = np.mean(results['sk_economic_values']) if results['sk_economic_values'] else 0
    avg_processing_time = np.mean(results['sk_processing_times']) if results['sk_processing_times'] else 0
    
    # Create text for summary
    summary_text = [
        f"Total Decisions: {total_decisions}",
        f"Average Economic Value: {avg_economic_value:.2f}/100",
        f"Average Processing Time: {avg_processing_time:.2f} ms",
        "\nFrequency Distribution:",
    ]
    
    frequencies = ['immediate', 'continuous', 'analysis', 'optimization', 'evolution']
    for freq in frequencies:
        count = freq_distribution.get(freq, 0)
        percentage = (count / total_decisions * 100) if total_decisions > 0 else 0
        summary_text.append(f"  {freq}: {count} ({percentage:.1f}%)")
    
    # Calculate economic value statistics by frequency
    summary_text.append("\nAverage Economic Value by Frequency:")
    
    freq_values = defaultdict(list)
    for freq, value in zip(results['sk_frequencies'], results['sk_economic_values']):
        freq_values[freq].append(value)
    
    for freq in frequencies:
        values = freq_values.get(freq, [])
        if values:
            avg = np.mean(values)
            summary_text.append(f"  {freq}: {avg:.2f}/100")
    
    # Plot the summary
    plt.text(0.5, 0.5, '\n'.join(summary_text), ha='center', va='center', fontsize=12,
             bbox=dict(boxstyle='round', facecolor='#f0f0f0', alpha=0.8))
    
    plt.axis('off')
    plt.title('SK Frequency Router Performance Summary', fontsize=16)
    plt.tight_layout()
    plt.savefig(os.path.join(output_dir, 'summary.png'), dpi=300)
    plt.close()

def main():
    # Check arguments
    if len(sys.argv) < 2:
        print("Usage: python visualize-benchmark-results.py <benchmark_results.json> [output_directory]")
        sys.exit(1)
    
    # Get arguments
    input_file = sys.argv[1]
    output_dir = sys.argv[2] if len(sys.argv) > 2 else 'benchmark_visualizations'
    
    # Create timestamp-based directory
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    output_dir = f"{output_dir}_{timestamp}"
    
    # Load data
    data = load_data(input_file)
    if not data:
        print(f"Could not load data from {input_file}")
        sys.exit(1)
    
    # Analyze data
    results = analyze_data(data)
    
    # Create visualizations
    create_visualizations(results, output_dir)

if __name__ == "__main__":
    main() 