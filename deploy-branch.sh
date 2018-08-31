#!/bin/bash

set -e

if [ "${TRAVIS_PULL_REQUEST}" = "false" ] && [ "${TRAVIS_BRANCH}" = "develop" ]; then
  echo "Deploying CI..."
  
  export Version=$(cat version)-beta-$(date +%Y%m%d%H%M%S)
  dotnet pack -c Release --output $PWD/artifacts/ci

  dotnet nuget push artifacts/ci/*.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json
else
  echo "Skipping CI deploy"
fi