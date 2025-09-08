# Use the .NET 9.0 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory
WORKDIR /app

# Copy shared library source first
COPY ["Shared/Kafka/", "Shared/Kafka/"]
RUN dotnet restore "Shared/Kafka/Kafka.csproj" --disable-parallel false

# Build shared library
RUN dotnet build "Shared/Kafka/Kafka.csproj" -c Release


# Copy the JobService.csproj to root /app directory
COPY ["Services/JobService/JobService.csproj", "./"]
RUN dotnet restore "JobService.csproj" --disable-parallel false

# Copy the rest of the JobService source code to root /app directory
COPY ["Services/JobService/", "./"]

# Install dotnet-ef globally
RUN dotnet tool install --global dotnet-ef

# Build the application (we're already in /app, so no need to change directory)
RUN dotnet build 

# Add global tools path to environment
ENV PATH="${PATH}:/root/.dotnet/tools"
