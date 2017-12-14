#tool nuget:?package=NUnit.ConsoleRunner&version=3.7.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var deepTestBuildDir = Directory("./DeepTest/bin") + Directory(configuration);
var deepTestWrapperBuildDir = Directory("./DeepTestWrapper/bin") + Directory(configuration);

var echoServerExampleTestSuiteBuildDir = Directory("./Test/Example/Test.Example.EchoChatDTSuite/bin") + Directory(configuration);
var echoServerExampleBuildDir = Directory("./Test/Example/Test.Example.EchoChatServer/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(deepTestBuildDir);
    CleanDirectory(deepTestWrapperBuildDir);
    CleanDirectory(echoServerExampleTestSuiteBuildDir);
    CleanDirectory(echoServerExampleBuildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("DeepTest.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild("DeepTest.sln", settings =>
        settings.SetVerbosity(Verbosity.Minimal));
});

Task("Run-Example-Deep-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    EnsureDirectoryExists("./staging");
    CleanDirectory("./staging");
        
    var files = GetFiles("./Test/Example/Test.Example.EchoChatDTSuite/bin/Debug/*");
    CopyFiles(files, "./staging/");
        
    NUnit3("./staging/Test.*.dll", new NUnit3Settings {
        NoResults = true
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Example-Deep-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
