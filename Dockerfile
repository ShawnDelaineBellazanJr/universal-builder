FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /app

# Install required tools
RUN apt-get update && apt-get install -y \
    git \
    inotify-tools \
    curl \
    jq \
    && rm -rf /var/lib/apt/lists/*

# Install GitHub CLI
RUN curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg \
    && chmod go+r /usr/share/keyrings/githubcli-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
    && apt-get update \
    && apt-get install -y gh \
    && rm -rf /var/lib/apt/lists/*

# Copy project files
COPY . .

# Restore dependencies
RUN dotnet restore

# Build the project
RUN dotnet build --configuration Release

# Set entry point to auto-run script
ENTRYPOINT ["/bin/bash", "auto-run.sh"]

# Default command is dev mode
CMD ["dev"] 