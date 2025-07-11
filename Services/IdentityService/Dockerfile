FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

RUN dotnet tool install --global dotnet-ef
COPY . ./
RUN dotnet build

ENV PATH="${PATH}:/root/.dotnet/tools"