#!/bin/bash

# Universal Builder Setup Script
# This script bootstraps the Universal Builder system from an empty directory

echo "Setting up Universal Builder..."

# Create directory structure
mkdir -p .github/workflows

# Initialize .NET project
echo "Initializing .NET project..."
dotnet new console -n UniversalBuilder -f net8.0
cd UniversalBuilder

# Add required packages
echo "Adding required packages..."
dotnet add package Microsoft.SemanticKernel --version 1.54.0 --prerelease
dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI --version 1.54.0 --prerelease
dotnet add package Microsoft.SemanticKernel.PromptTemplates.Handlebars --version 1.54.0 --prerelease
dotnet add package Octokit --version 7.1.0

# Copy generated files (this assumes files are in the parent directory)
echo "Copying project files..."
cp ../UniversalBuilder.cs .
cp ../GitHubMemory.cs .
cp ../UniversalBuilder.csproj .
mkdir -p .github/workflows
cp ../.github/workflows/universal-builder.yml .github/workflows/

# Build the project
echo "Building the project..."
dotnet build

echo "Universal Builder setup complete!"
echo "Next steps:"
echo "1. Push the code to a GitHub repository"
echo "2. Set up GitHub Secrets (OPENAI_API_KEY, DEFAULT_VALUE_THRESHOLD)"
echo "3. Create an issue or manually dispatch the workflow to run the Universal Builder" 