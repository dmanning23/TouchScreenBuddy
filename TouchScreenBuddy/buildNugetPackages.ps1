rm *.nupkg
nuget pack .\TouchScreenBuddy.nuspec -IncludeReferencedProjects -Prop Configuration=Release
nuget push *.nupkg