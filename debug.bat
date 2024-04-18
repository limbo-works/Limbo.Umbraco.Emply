@echo off
dotnet build src/Limbo.Umbraco.Emply --configuration Debug /t:rebuild /t:pack -p:PackageOutputPath=c:\nuget\Umbraco10