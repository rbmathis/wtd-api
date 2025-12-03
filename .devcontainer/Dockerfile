FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0

# Install PostgreSQL client tools
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends postgresql-client \
    && apt-get clean -y && rm -rf /var/lib/apt/lists/*

# Install dotnet-ef tool globally
RUN dotnet tool install --global dotnet-ef

# Add dotnet tools to PATH
ENV PATH="${PATH}:/root/.dotnet/tools:/home/vscode/.dotnet/tools"

# Generate self-signed certificate for HTTPS development
RUN mkdir -p /https && \
    dotnet dev-certs https -ep /https/aspnetapp.pfx -p devpassword --trust

# Set the default shell to bash
SHELL ["/bin/bash", "-c"]
