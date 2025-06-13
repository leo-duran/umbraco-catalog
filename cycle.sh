#!/bin/bash

rm -rf src/Catalog/umbraco

dotnet build

dotnet run --project src/Catalog