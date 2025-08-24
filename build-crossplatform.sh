#!/bin/bash

# Cross-platform build script for MusicTaggingLight
echo "Building MusicTaggingLight for multiple platforms..."

# Create publish directory
mkdir -p publish

# Build for Windows x64
echo "Building for Windows x64..."
dotnet publish MusicTaggingLight/MusicTaggingLight.csproj -c Release -r win-x64 --self-contained -o publish/win-x64

# Build for Linux x64  
echo "Building for Linux x64..."
dotnet publish MusicTaggingLight/MusicTaggingLight.csproj -c Release -r linux-x64 --self-contained -o publish/linux-x64

# Build for macOS x64
echo "Building for macOS x64..."
dotnet publish MusicTaggingLight/MusicTaggingLight.csproj -c Release -r osx-x64 --self-contained -o publish/osx-x64

echo "Cross-platform build completed!"
echo "Artifacts available in:"
echo "  - Windows: publish/win-x64/"
echo "  - Linux:   publish/linux-x64/" 
echo "  - macOS:   publish/osx-x64/"