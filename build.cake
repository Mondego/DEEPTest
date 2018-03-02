#tool nuget:?package=NUnit.ConsoleRunner&version=3.7.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// DIRECTORIES
//////////////////////////////////////////////////////////////////////

// DeepTest
var deepTestBuildDir = Directory("./DeepTest/bin") + Directory(configuration);
var deepTestPluginBuildDir = Directory("./DeepTestPlugin/bin") + Directory(configuration);

// NFBench Mirror
var nfbImportReferenceBuildDir = Directory("./NFBenchImport.Benchmark.Reference/bin") + Directory(configuration);
var nfbImportPerformanceBuildDir = Directory("./NFBenchImport.Benchmark.Performance/bin") + Directory(configuration);
var nfbClientAppBuildDir = Directory("./NFBenchImport.Services.ClientApplication/bin") + Directory(configuration);

// Test Directories
var nfbImportTestsBuildDir = Directory("./Test.NFBenchImport/bin") + Directory(configuration);
var echoServerExampleTestSuiteBuildDir = Directory("./Test/Example/Test.Example.EchoChatDTSuite/bin") + Directory(configuration);
var stagingDir = Directory("./staging");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(deepTestBuildDir);
    CleanDirectory(deepTestPluginBuildDir);
    
    CleanDirectory(nfbImportReferenceBuildDir);
    CleanDirectory(nfbImportPerformanceBuildDir);
    CleanDirectory(nfbClientAppBuildDir);
    
    CleanDirectory(nfbImportTestsBuildDir);
    CleanDirectory(echoServerExampleTestSuiteBuildDir);
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

Task("Run-Benchmark-Deep-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    EnsureDirectoryExists(stagingDir);
    CleanDirectory(stagingDir);

    CopyFiles("./DeepTest/bin/Debug/*.dll", stagingDir);
    CopyFiles("./NFBenchImport.Benchmark.Reference/bin/Debug/*.exe", stagingDir);
    CopyFiles("./NFBenchImport.Benchmark.Performance/bin/Debug/*.exe", stagingDir);
    CopyFiles("./NFBenchImport.Services.ClientApplication/bin/Debug/*.exe", stagingDir);   
 
    NUnit3("./Test.NFBenchImport/bin/Debug/Test*.dll", new NUnit3Settings {
        NoResults = true
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Benchmark-Deep-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
