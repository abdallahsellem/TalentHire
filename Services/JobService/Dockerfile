# Use the .NET 9.0 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory
WORKDIR /app

# Copy only the .csproj files and restore dependencies
COPY *.csproj ./

# Install dotnet-ef globally
RUN dotnet tool install --global dotnet-ef


RUN dotnet restore

# Copy the rest of the source code
COPY . ./

# Build the application
RUN dotnet build



# Add global tools path to environment
ENV PATH="${PATH}:/root/.dotnet/tools"
