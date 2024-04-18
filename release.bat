@echo off
dotnet build src/Limbo.Umbraco.Emply --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget