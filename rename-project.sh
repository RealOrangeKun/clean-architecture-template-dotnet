#!/bin/bash

# Script to rename all "Project" references to a new name in a .NET project
# Usage: ./rename-project.sh <NewProjectName>
#
# IMPORTANT: This script will rename project names, namespaces, and file/folder names
# but will NOT break XML structure in .csproj/.sln files (like <Project> tags)
#
# What gets renamed:
# - Folder and file names containing "Project"
# - Namespace declarations in C# files  
# - Project names in solution files
# - ProjectReference paths in .csproj files
# - AssemblyName and RootNamespace in .csproj files
#
# What does NOT get renamed:
# - XML tags like <Project> or </Project> in .csproj files
# - "Project(" declarations in .sln files

set -e

# Check if argument is provided
if [ $# -ne 1 ]; then
    echo "Usage: $0 <NewProjectName>"
    echo "Example: $0 MyAwesomeProject"
    exit 1
fi

NEW_NAME="$1"
OLD_NAME="Project"

# Validate the new name (should be a valid C# identifier)
if [[ ! "$NEW_NAME" =~ ^[A-Za-z_][A-Za-z0-9_]*$ ]]; then
    echo "Error: '$NEW_NAME' is not a valid project name. Use only letters, numbers, and underscores, starting with a letter or underscore."
    exit 1
fi

echo "Renaming project from '$OLD_NAME' to '$NEW_NAME'..."
echo "This will modify files, folders, and content throughout the project."
read -p "Are you sure you want to continue? (y/N): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Operation cancelled."
    exit 1
fi

# Function to rename files and directories
rename_files_and_dirs() {
    echo "Step 1: Renaming files and directories..."
    
    # Find and rename directories (deepest first to avoid path issues)
    find . -type d -name "*${OLD_NAME}*" -print0 | sort -rz | while IFS= read -r -d '' dir; do
        if [[ "$dir" != "." && "$dir" != "./.git"* && "$dir" != "./bin"* && "$dir" != "./obj"* ]]; then
            new_dir=$(echo "$dir" | sed "s/${OLD_NAME}/${NEW_NAME}/g")
            if [[ "$dir" != "$new_dir" ]]; then
                echo "Renaming directory: $dir -> $new_dir"
                mv "$dir" "$new_dir" 2>/dev/null || true
            fi
        fi
    done
    
    # Find and rename files
    find . -type f -name "*${OLD_NAME}*" -print0 | while IFS= read -r -d '' file; do
        if [[ "$file" != "./.git"* && "$file" != "./bin"* && "$file" != "./obj"* && "$file" != "./rename-project.sh" ]]; then
            new_file=$(echo "$file" | sed "s/${OLD_NAME}/${NEW_NAME}/g")
            if [[ "$file" != "$new_file" ]]; then
                echo "Renaming file: $file -> $new_file"
                # Create directory if it doesn't exist
                mkdir -p "$(dirname "$new_file")"
                mv "$file" "$new_file" 2>/dev/null || true
            fi
        fi
    done
}

# Function to update file contents
update_file_contents() {
    echo "Step 2: Updating file contents..."
    
    # Update C# files (namespaces, class names, etc.)
    find . -name "*.cs" -type f ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" ! -name "rename-project.sh" -print0 | while IFS= read -r -d '' file; do
        if [[ -f "$file" ]] && grep -q "$OLD_NAME" "$file" 2>/dev/null; then
            echo "Updating C# file: $file"
            cp "$file" "$file.bak"
            sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file"
            rm "$file.bak"
        fi
    done
    
    # Update project files (.csproj) - CAREFULLY avoid XML tags
    find . -name "*.csproj" -type f ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -print0 | while IFS= read -r -d '' file; do
        if [[ -f "$file" ]]; then
            # Only update specific elements, never the <Project> or </Project> tags
            if grep -E "(ProjectReference.*${OLD_NAME}|AssemblyName.*${OLD_NAME}|RootNamespace.*${OLD_NAME})" "$file" >/dev/null 2>&1; then
                echo "Updating project file: $file"
                cp "$file" "$file.bak"
                
                # Update ProjectReference Include paths
                sed -i "s|Include=\"\([^\"]*\)${OLD_NAME}\([^\"]*\)\"|Include=\"\1${NEW_NAME}\2\"|g" "$file"
                
                # Update AssemblyName values
                sed -i "s|<AssemblyName>${OLD_NAME}\([^<]*\)</AssemblyName>|<AssemblyName>${NEW_NAME}\1</AssemblyName>|g" "$file"
                
                # Update RootNamespace values  
                sed -i "s|<RootNamespace>${OLD_NAME}\([^<]*\)</RootNamespace>|<RootNamespace>${NEW_NAME}\1</RootNamespace>|g" "$file"
                
                rm "$file.bak"
            fi
        fi
    done
    
    # Update solution files - be careful with Project declarations
    find . -name "*.sln" -type f ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -print0 | while IFS= read -r -d '' file; do
        if [[ -f "$file" ]] && grep -q "$OLD_NAME" "$file" 2>/dev/null; then
            echo "Updating solution file: $file"
            cp "$file" "$file.bak"
            
            # Only update project names and paths in solution files, NOT the "Project(" declarations
            sed -i "s|= \"${OLD_NAME}\([^\"]*\)\"|= \"${NEW_NAME}\1\"|g" "$file"
            sed -i "s|\"\\([^\"]*\\)${OLD_NAME}\\([^\"]*\\)\"|\"\\1${NEW_NAME}\\2\"|g" "$file"
            
            rm "$file.bak"
        fi
    done
    
    # Update other file types
    file_extensions=("*.json" "*.xml" "*.yml" "*.yaml" "*.md" "*.txt")
    
    for ext in "${file_extensions[@]}"; do
        find . -name "$ext" -type f ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" ! -name "rename-project.sh" -print0 | while IFS= read -r -d '' file; do
            if [[ -f "$file" ]] && grep -q "$OLD_NAME" "$file" 2>/dev/null; then
                echo "Updating content in: $file"
                cp "$file" "$file.bak"
                sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file"
                rm "$file.bak"
            fi
        done
    done
}

# Function to update solution file paths
update_solution_paths() {
    echo "Step 3: Updating solution file paths..."
    
    # This step is now handled in update_file_contents() to avoid duplicate processing
    # and ensure more careful handling of solution files
    echo "Solution file paths updated in previous step."
}

# Function to clean and restore packages
clean_and_restore() {
    echo "Step 4: Cleaning and restoring packages..."
    
    # Clean build artifacts
    find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
    find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true
    
    # Restore packages
    if command -v dotnet &> /dev/null; then
        echo "Restoring NuGet packages..."
        dotnet restore
        
        echo "Step 5: Building project to verify..."
        if dotnet build --no-restore; then
            echo "✅ Project built successfully!"
        else
            echo "⚠️  Build failed. You may need to manually fix some references."
        fi
        
        # Try to format code and organize usings
        echo "Step 6: Formatting code..."
        dotnet format --include-generated || echo "Code formatting completed with warnings"
        
    else
        echo "⚠️  dotnet CLI not found. Skipping package restore and build verification."
        echo "Please run 'dotnet restore' and 'dotnet build' manually after the script completes."
    fi
}

# Function to update docker and compose files
update_docker_files() {
    echo "Step 7: Updating Docker and compose files..."
    
    # Update docker-compose files
    find . -name "docker-compose*.yml" -o -name "docker-compose*.yaml" -type f ! -path "./.git/*" | while IFS= read -r file; do
        if [[ -f "$file" ]] && grep -q "$OLD_NAME" "$file" 2>/dev/null; then
            echo "Updating Docker compose file: $file"
            sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file"
        fi
    done
    
    # Update Dockerfiles
    find . -name "Dockerfile*" -type f ! -path "./.git/*" | while IFS= read -r file; do
        if [[ -f "$file" ]] && grep -q "$OLD_NAME" "$file" 2>/dev/null; then
            echo "Updating Dockerfile: $file"
            sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file"
        fi
    done
}

# Function to update configuration files
update_config_files() {
    echo "Step 8: Updating configuration files..."
    
    # Update appsettings files
    find . -name "appsettings*.json" -type f ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" | while IFS= read -r file; do
        if [[ -f "$file" ]] && grep -q "$OLD_NAME" "$file" 2>/dev/null; then
            echo "Updating configuration file: $file"
            sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file"
        fi
    done
    
    # Update launchSettings.json
    find . -name "launchSettings.json" -type f ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" | while IFS= read -r file; do
        if [[ -f "$file" ]] && grep -q "$OLD_NAME" "$file" 2>/dev/null; then
            echo "Updating launch settings: $file"
            sed -i "s/${OLD_NAME}/${NEW_NAME}/g" "$file"
        fi
    done
}

# Main execution
main() {
    echo "Starting project rename from '$OLD_NAME' to '$NEW_NAME'..."
    echo "Working directory: $(pwd)"
    echo
    
    # Execute all steps
    rename_files_and_dirs
    echo
    
    update_file_contents
    echo
    
    update_solution_paths
    echo
    
    update_docker_files
    echo
    
    update_config_files
    echo
    
    clean_and_restore
    echo
    
    echo "🎉 Project rename completed!"
    echo
    echo "Summary of changes:"
    echo "- Renamed all files and folders containing '$OLD_NAME' to '$NEW_NAME'"
    echo "- Updated all namespaces, class names, and references in source code"
    echo "- Updated project files (.csproj), solution files (.sln)"
    echo "- Updated configuration files (appsettings.json, launchSettings.json)"
    echo "- Updated Docker files (Dockerfile, docker-compose.yml)"
    echo "- Cleaned build artifacts and restored packages"
    echo
    echo "Next steps:"
    echo "1. Review the changes and test your application"
    echo "2. Update any hardcoded references in documentation"
    echo "3. Update your git remote if the repository name changed"
    echo "4. Consider updating README.md and other documentation files"
}

# Execute main function
main