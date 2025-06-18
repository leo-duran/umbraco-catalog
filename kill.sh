#!/bin/bash

# Kill any existing instances of the app
echo "Checking for existing instances of the app..."
EXISTING_PID=$(ps aux | grep "dotnet run --project src/Catalog" | grep -v grep | awk '{print $2}')
if [ ! -z "$EXISTING_PID" ]; then
  echo "Killing existing process with PID: $EXISTING_PID"
  kill $EXISTING_PID
  # Give it a moment to shut down
  sleep 2
fi

rm -rf src/Catalog/umbraco