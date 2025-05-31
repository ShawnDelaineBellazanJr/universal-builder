.PHONY: build run watch workflow test docker-dev docker-watch docker-workflow setup clean help

# Default target
all: build

# Help message
help:
	@echo "AutonomousAI Makefile"
	@echo ""
	@echo "Usage:"
	@echo "  make build           - Build the project"
	@echo "  make run [ARGS]      - Run with optional intent and threshold"
	@echo "  make watch           - Watch for file changes and rebuild"
	@echo "  make workflow [ARGS] - Simulate GitHub Actions workflow"
	@echo "  make test            - Run tests"
	@echo "  make docker-dev      - Run in Docker dev mode"
	@echo "  make docker-watch    - Run Docker watcher service"
	@echo "  make docker-workflow - Run Docker workflow simulation"
	@echo "  make setup           - Setup development environment"
	@echo "  make clean           - Clean build artifacts"
	@echo ""
	@echo "Example: make run ARGS=\"Build a calculator app 50\""

# Build the project
build:
	dotnet build --configuration Release

# Run with optional arguments
run:
	./auto-run.sh dev $(ARGS)

# Watch for file changes
watch:
	./auto-run.sh watch

# Simulate GitHub Actions workflow
workflow:
	./auto-run.sh workflow $(ARGS)

# Run tests
test:
	./auto-run.sh test

# Docker development environment
docker-dev:
	docker-compose up autonomousai

# Docker watcher service
docker-watch:
	docker-compose up watcher

# Docker workflow simulation
docker-workflow:
	docker-compose up workflow

# Setup development environment
setup:
	@echo "Setting up development environment..."
	@if [ ! -f ".env" ]; then \
		echo "Creating .env file..."; \
		echo "OPENAI_API_KEY=" > .env; \
		echo "AZURE_OPENAI_API_KEY=" >> .env; \
		echo "AZURE_OPENAI_ENDPOINT=" >> .env; \
		echo "GITHUB_TOKEN=" >> .env; \
		echo "DEFAULT_VALUE_THRESHOLD=50" >> .env; \
		echo "Please edit .env with your API keys"; \
	fi
	@if ! command -v dotnet &> /dev/null; then \
		echo "Please install .NET SDK 9.0 or later"; \
	fi
	@if ! command -v docker &> /dev/null; then \
		echo "Docker is not installed. Consider installing Docker for containerized development."; \
	fi
	chmod +x auto-run.sh
	dotnet restore

# Clean build artifacts
clean:
	dotnet clean
	rm -rf bin/ obj/ 