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

dotnet build

dotnet run --project src/Catalog &

# Store the PID of the dotnet process
APP_PID=$!

# Function to check if site is responding
check_site() {
  # Try to connect to the site, ignoring SSL certificate errors
  curl -k -s -o /dev/null -w "%{http_code}" https://localhost:44389/ | grep -q "200\|302\|301"
  return $?
}

# Wait for the site to start up by checking if it's responding
echo "Waiting for the site to start up on https://localhost:44389..."
MAX_ATTEMPTS=20  # Maximum number of attempts (20 seconds)
ATTEMPTS=0

while ! check_site; do
  ATTEMPTS=$((ATTEMPTS+1))
  if [ $ATTEMPTS -ge $MAX_ATTEMPTS ]; then
    echo "Site did not start after $MAX_ATTEMPTS seconds. Please check for errors."
    # Kill the dotnet process if it's still running
    kill $APP_PID 2>/dev/null
    exit 1
  fi
  echo "Waiting for site to respond... ($ATTEMPTS/$MAX_ATTEMPTS)"
  sleep 1
done

echo "Site is now responding. Opening browser..."
open https://localhost:44389/umbraco