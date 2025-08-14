#!/usr/bin/env bash
set -euo pipefail

UNITY_PATH=${UNITY_PATH:-"/Applications/Unity/Hub/Editor/2022.3.62f1/Unity.app/Contents/MacOS/Unity"}
PROJECT_PATH=$(cd "$(dirname "$0")" && pwd)

"$UNITY_PATH" -batchmode -quit -nographics -projectPath "$PROJECT_PATH" -executeMethod HeadlessHarness.Run -logFile "$PROJECT_PATH/Headless.log"

echo "Headless run complete. See Headless.log for details."


