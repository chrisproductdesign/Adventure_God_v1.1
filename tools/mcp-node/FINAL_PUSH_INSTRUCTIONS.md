# Final Push Instructions for MCP Unity Setup

## Issue
The Unity Library files are still in the git history from the main branch, causing push failures due to large files (>100MB).

## Solution: Create a Fresh Repository

### Option 1: Create New Repository (Recommended)
1. Create a new repository on GitHub
2. Clone it locally:
   ```bash
   git clone <new-repo-url> adventure-god-mcp
   cd adventure-god-mcp
   ```

3. Copy only the essential files:
   ```bash
   # Copy MCP files
   cp -r ../Adventure_God_v1.1/tools/mcp-node/ .
   cp -r ../Adventure_God_v1.1/unity/Assets/Editor/McpUnityListener/ unity/Assets/Editor/
   
   # Copy Unity project files (excluding Library)
   cp -r ../Adventure_God_v1.1/unity/Assets/ unity/
   cp -r ../Adventure_God_v1.1/unity/ProjectSettings/ unity/
   cp -r ../Adventure_God_v1.1/unity/Packages/ unity/
   
   # Copy documentation
   cp ../Adventure_God_v1.1/README.md .
   cp ../Adventure_God_v1.1/.gitignore .
   ```

4. Add and commit:
   ```bash
   git add .
   git commit -m "feat(mcp): add node MCP shim and Unity editor listener"
   git push origin main
   ```

### Option 2: Use Git LFS (Alternative)
1. Install Git LFS:
   ```bash
   brew install git-lfs
   git lfs install
   ```

2. Track large Unity files:
   ```bash
   git lfs track "unity/Library/**/*.dylib"
   git lfs track "unity/Library/**/*.dll"
   git lfs track "unity/Library/**/*.so"
   git add .gitattributes
   git commit -m "chore: add Git LFS tracking for Unity Library files"
   ```

3. Push:
   ```bash
   git push origin mcp/unity-setup-clean
   ```

## Current Status
✅ MCP Node shim created and tested
✅ Unity Editor listener implemented
✅ Smoke tests passing
✅ Documentation complete
❌ Push blocked by Unity Library files in history

## Files Created
- `tools/mcp-node/package.json` - Node dependencies
- `tools/mcp-node/server.js` - MCP shim server
- `tools/mcp-node/mcp-tools.json` - MCP tools manifest
- `tools/mcp-node/smokeTest.js` - Smoke test script
- `tools/mcp-node/README.md` - Documentation
- `unity/Assets/Editor/McpUnityListener/UnityMcpListener.cs` - Unity listener
- `.gitignore` - Updated with Unity exclusions

## Testing Results
- ✅ Server starts on localhost:3000
- ✅ Health endpoint responds
- ✅ Unity listener starts on localhost:8081
- ✅ GameObject creation works
- ✅ All MCP tools functional

The MCP Unity integration is complete and working. Only the git push needs to be resolved using one of the above methods.
