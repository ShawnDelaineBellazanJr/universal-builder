#!/usr/bin/env python3
"""
SK Frequency Router Dashboard Generator
This script creates an HTML dashboard from benchmark results
"""

import json
import sys
import os
import numpy as np
import matplotlib.pyplot as plt
import matplotlib
matplotlib.use('Agg')  # Use non-interactive backend
from collections import Counter, defaultdict
from datetime import datetime
import base64
from io import BytesIO

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
        'goals': [],
        'contexts': [],
        'timestamps': [],
        'successful': [],
        'disagreements': []
    }
    
    # Process data
    for item in data:
        # Skip items without frequency info
        if 'RecommendedFrequency' not in item:
            continue
            
        frequency = item['RecommendedFrequency']
        goal = item.get('Goal', '')
        context = item.get('Context', '')
        economic_value = item.get('EconomicValue', 0)
        timestamp = item.get('Timestamp', '')
        was_successful = item.get('WasSuccessful', True)
        
        # Store data
        results['sk_frequencies'].append(frequency)
        results['sk_economic_values'].append(economic_value)
        results['goals'].append(goal)
        results['contexts'].append(context)
        results['timestamps'].append(timestamp)
        results['successful'].append(was_successful)
        
        # If we have execution time, store it
        if 'ExecutionTime' in item:
            try:
                # Try to parse ticks directly
                ticks = int(item['ExecutionTime'].replace(' Ticks', ''))
                ms = ticks / 10000  # Convert ticks to milliseconds
            except:
                # If that fails, try to parse the TimeSpan format
                time_parts = item['ExecutionTime'].split(':')
                if len(time_parts) == 3:
                    hours, minutes, seconds = time_parts
                    seconds = float(seconds)
                    ms = (int(hours) * 3600 + int(minutes) * 60 + seconds) * 1000
                else:
                    ms = 0
            
            results['sk_processing_times'].append(ms)
    
    return results

def fig_to_base64(fig):
    """Convert matplotlib figure to base64 string for HTML embedding"""
    buf = BytesIO()
    fig.savefig(buf, format='png', dpi=300, bbox_inches='tight')
    buf.seek(0)
    img_str = base64.b64encode(buf.read()).decode('utf-8')
    return img_str

def create_frequency_chart(results):
    """Create frequency distribution chart"""
    # Count frequencies
    sk_freq_count = Counter(results['sk_frequencies'])
    
    # Get ordered frequencies and their counts
    frequencies = ['immediate', 'continuous', 'analysis', 'optimization', 'evolution']
    sk_counts = [sk_freq_count.get(freq, 0) for freq in frequencies]
    
    # Create chart
    fig, ax = plt.subplots(figsize=(10, 6))
    bars = ax.bar(frequencies, sk_counts, color='#3498db')
    
    # Add count labels on bars
    for bar in bars:
        height = bar.get_height()
        if height > 0:
            ax.annotate(f'{height}',
                        xy=(bar.get_x() + bar.get_width() / 2, height),
                        xytext=(0, 3),
                        textcoords="offset points",
                        ha='center', va='bottom')
    
    # Add percentage labels
    total = sum(sk_counts)
    for i, bar in enumerate(bars):
        height = bar.get_height()
        if height > 0:
            percentage = height / total * 100
            ax.annotate(f'{percentage:.1f}%',
                        xy=(bar.get_x() + bar.get_width() / 2, height / 2),
                        ha='center', va='center',
                        color='white', fontweight='bold')
    
    ax.set_xlabel('Frequency')
    ax.set_ylabel('Count')
    ax.set_title('Frequency Distribution')
    plt.tight_layout()
    
    return fig_to_base64(fig)

def create_economic_chart(results):
    """Create economic value chart"""
    # Group economic values by frequency
    freq_values = defaultdict(list)
    for freq, value in zip(results['sk_frequencies'], results['sk_economic_values']):
        freq_values[freq].append(value)
    
    # Calculate average values for each frequency
    frequencies = ['immediate', 'continuous', 'analysis', 'optimization', 'evolution']
    avg_values = []
    
    for freq in frequencies:
        values = freq_values.get(freq, [])
        if values:
            avg_values.append(np.mean(values))
        else:
            avg_values.append(0)
    
    # Create chart
    fig, ax = plt.subplots(figsize=(10, 6))
    bars = ax.bar(frequencies, avg_values, color='#2ecc71')
    
    # Add value labels
    for i, v in enumerate(avg_values):
        if v > 0:
            ax.annotate(f'{v:.1f}',
                        xy=(i, v),
                        xytext=(0, 3),
                        textcoords="offset points",
                        ha='center', va='bottom')
    
    ax.set_xlabel('Frequency')
    ax.set_ylabel('Average Economic Value')
    ax.set_title('Average Economic Value by Frequency')
    ax.set_ylim(0, 100)
    plt.tight_layout()
    
    return fig_to_base64(fig)

def create_processing_time_chart(results):
    """Create processing time chart"""
    if not results['sk_processing_times']:
        return None
        
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
    
    # Create chart
    fig, ax = plt.subplots(figsize=(10, 6))
    bars = ax.bar(frequencies, avg_times, color='#9b59b6')
    
    # Add value labels
    for i, v in enumerate(avg_times):
        if v > 0:
            ax.annotate(f'{v:.1f} ms',
                        xy=(i, v),
                        xytext=(0, 3),
                        textcoords="offset points",
                        ha='center', va='bottom')
    
    ax.set_xlabel('Frequency')
    ax.set_ylabel('Average Processing Time (ms)')
    ax.set_title('Average Processing Time by Frequency')
    plt.tight_layout()
    
    return fig_to_base64(fig)

def create_success_rate_chart(results):
    """Create success rate chart"""
    # Group success by frequency
    freq_success = defaultdict(list)
    for freq, success in zip(results['sk_frequencies'], results['successful']):
        freq_success[freq].append(success)
    
    # Calculate success rate per frequency
    frequencies = ['immediate', 'continuous', 'analysis', 'optimization', 'evolution']
    success_rates = []
    
    for freq in frequencies:
        successes = freq_success.get(freq, [])
        if successes:
            success_rates.append(sum(successes) / len(successes) * 100)
        else:
            success_rates.append(0)
    
    # Create chart
    fig, ax = plt.subplots(figsize=(10, 6))
    bars = ax.bar(frequencies, success_rates, color='#e74c3c')
    
    # Add value labels
    for i, v in enumerate(success_rates):
        if v > 0:
            ax.annotate(f'{v:.1f}%',
                        xy=(i, v),
                        xytext=(0, 3),
                        textcoords="offset points",
                        ha='center', va='bottom')
    
    ax.set_xlabel('Frequency')
    ax.set_ylabel('Success Rate (%)')
    ax.set_title('Success Rate by Frequency')
    ax.set_ylim(0, 100)
    plt.tight_layout()
    
    return fig_to_base64(fig)

def generate_html_dashboard(results, output_file):
    """Generate HTML dashboard from results"""
    # Generate charts
    frequency_chart = create_frequency_chart(results)
    economic_chart = create_economic_chart(results)
    processing_time_chart = create_processing_time_chart(results)
    success_chart = create_success_rate_chart(results)
    
    # Calculate overall metrics
    total_decisions = len(results['sk_frequencies'])
    avg_economic_value = np.mean(results['sk_economic_values']) if results['sk_economic_values'] else 0
    avg_processing_time = np.mean(results['sk_processing_times']) if results['sk_processing_times'] else 0
    success_rate = sum(results['successful']) / len(results['successful']) * 100 if results['successful'] else 0
    
    # Get frequency distribution
    freq_distribution = Counter(results['sk_frequencies'])
    freq_order = ['immediate', 'continuous', 'analysis', 'optimization', 'evolution']
    
    # Create frequency metrics HTML
    freq_metrics_html = ""
    for freq in freq_order:
        count = freq_distribution.get(freq, 0)
        percentage = count / total_decisions * 100 if total_decisions > 0 else 0
        freq_metrics_html += f"""
        <div class="frequency-metric">
            <div class="freq-name">{freq}</div>
            <div class="freq-count">{count}</div>
            <div class="freq-percentage">{percentage:.1f}%</div>
            <div class="freq-bar" style="width: {percentage}%"></div>
        </div>
        """
    
    # Create sample decisions table
    sample_decisions_html = ""
    max_samples = min(10, len(results['goals']))
    for i in range(max_samples):
        goal = results['goals'][i]
        context = results['contexts'][i] if i < len(results['contexts']) else ""
        frequency = results['sk_frequencies'][i]
        economic_value = results['sk_economic_values'][i]
        success = "✅" if results['successful'][i] else "❌"
        
        sample_decisions_html += f"""
        <tr>
            <td>{goal}</td>
            <td>{context}</td>
            <td>{frequency}</td>
            <td>{economic_value}</td>
            <td>{success}</td>
        </tr>
        """
    
    # Generate HTML content
    html_content = f"""<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>SK Frequency Router Dashboard</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f5f7fa;
            color: #333;
        }}
        .container {{
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }}
        header {{
            background-color: #34495e;
            color: white;
            padding: 20px;
            text-align: center;
            margin-bottom: 20px;
        }}
        h1, h2, h3 {{
            margin: 0;
        }}
        .subtitle {{
            margin-top: 10px;
            font-weight: normal;
            opacity: 0.8;
        }}
        .metrics-container {{
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            margin-bottom: 30px;
        }}
        .metric-card {{
            background-color: white;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            padding: 20px;
            flex: 1;
            min-width: 200px;
        }}
        .metric-title {{
            font-size: 14px;
            color: #7f8c8d;
            margin-bottom: 10px;
        }}
        .metric-value {{
            font-size: 28px;
            font-weight: bold;
            color: #2c3e50;
        }}
        .chart-container {{
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            margin-bottom: 30px;
        }}
        .chart-card {{
            background-color: white;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            padding: 20px;
            flex: 1 1 calc(50% - 20px);
        }}
        .chart-title {{
            font-size: 18px;
            margin-bottom: 15px;
            color: #2c3e50;
        }}
        .chart {{
            width: 100%;
            height: auto;
        }}
        .frequency-section {{
            background-color: white;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            padding: 20px;
            margin-bottom: 30px;
        }}
        .frequency-metric {{
            margin-bottom: 10px;
            position: relative;
            padding-bottom: 15px;
        }}
        .freq-name {{
            display: inline-block;
            width: 120px;
            font-weight: bold;
        }}
        .freq-count {{
            display: inline-block;
            width: 50px;
            text-align: right;
        }}
        .freq-percentage {{
            display: inline-block;
            width: 60px;
            text-align: right;
        }}
        .freq-bar {{
            position: absolute;
            height: 5px;
            background-color: #3498db;
            bottom: 0;
            left: 0;
            border-radius: 2px;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        th, td {{
            padding: 12px 15px;
            text-align: left;
            border-bottom: 1px solid #e1e1e1;
        }}
        th {{
            background-color: #f8f9fa;
            font-weight: bold;
        }}
        tr:hover {{
            background-color: #f5f5f5;
        }}
        .decisions-table {{
            background-color: white;
            border-radius: 8px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            padding: 20px;
            margin-bottom: 30px;
            overflow-x: auto;
        }}
        footer {{
            text-align: center;
            margin-top: 30px;
            padding: 20px;
            color: #7f8c8d;
            font-size: 14px;
        }}
    </style>
</head>
<body>
    <header>
        <h1>SK Frequency Router Dashboard</h1>
        <p class="subtitle">Performance Analysis and Visualization</p>
    </header>
    
    <div class="container">
        <div class="metrics-container">
            <div class="metric-card">
                <div class="metric-title">Total Decisions</div>
                <div class="metric-value">{total_decisions}</div>
            </div>
            <div class="metric-card">
                <div class="metric-title">Average Economic Value</div>
                <div class="metric-value">{avg_economic_value:.1f}/100</div>
            </div>
            <div class="metric-card">
                <div class="metric-title">Average Processing Time</div>
                <div class="metric-value">{avg_processing_time:.1f} ms</div>
            </div>
            <div class="metric-card">
                <div class="metric-title">Success Rate</div>
                <div class="metric-value">{success_rate:.1f}%</div>
            </div>
        </div>
        
        <div class="chart-container">
            <div class="chart-card">
                <div class="chart-title">Frequency Distribution</div>
                <img class="chart" src="data:image/png;base64,{frequency_chart}" alt="Frequency Distribution">
            </div>
            <div class="chart-card">
                <div class="chart-title">Economic Value by Frequency</div>
                <img class="chart" src="data:image/png;base64,{economic_chart}" alt="Economic Value">
            </div>
        </div>
        
        <div class="chart-container">
            <div class="chart-card">
                <div class="chart-title">Processing Time by Frequency</div>
                <img class="chart" src="data:image/png;base64,{processing_time_chart if processing_time_chart else ''}" alt="Processing Time">
            </div>
            <div class="chart-card">
                <div class="chart-title">Success Rate by Frequency</div>
                <img class="chart" src="data:image/png;base64,{success_chart}" alt="Success Rate">
            </div>
        </div>
        
        <div class="frequency-section">
            <h2>Frequency Distribution</h2>
            {freq_metrics_html}
        </div>
        
        <div class="decisions-table">
            <h2>Sample Routing Decisions</h2>
            <table>
                <thead>
                    <tr>
                        <th>Goal</th>
                        <th>Context</th>
                        <th>Frequency</th>
                        <th>Economic Value</th>
                        <th>Success</th>
                    </tr>
                </thead>
                <tbody>
                    {sample_decisions_html}
                </tbody>
            </table>
        </div>
        
        <footer>
            <p>Generated on {datetime.now().strftime('%Y-%m-%d %H:%M:%S')} • SK Frequency Router Benchmark Dashboard</p>
        </footer>
    </div>
</body>
</html>
    """
    
    # Write HTML file
    with open(output_file, 'w') as f:
        f.write(html_content)
    
    print(f"Dashboard generated: {output_file}")

def main():
    # Check arguments
    if len(sys.argv) < 2:
        print("Usage: python generate-dashboard.py <benchmark_results.json> [output_file]")
        sys.exit(1)
    
    # Get arguments
    input_file = sys.argv[1]
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    output_file = sys.argv[2] if len(sys.argv) > 2 else f"sk_router_dashboard_{timestamp}.html"
    
    # Load data
    data = load_data(input_file)
    if not data:
        print(f"Could not load data from {input_file}")
        sys.exit(1)
    
    # Analyze data
    results = analyze_data(data)
    
    # Generate dashboard
    generate_html_dashboard(results, output_file)

if __name__ == "__main__":
    main() 