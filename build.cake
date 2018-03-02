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
var buildInternalTestDriver = Directory("./DeepTest/InternalTestDriver/bin") + Directory(configuration);
var buildRemoteAssertionMessages = Directory("./DeepTest/RemoteAssertionMessages/bin") + Directory(configuration);
var buildRemoteTestDriverHandler = Directory("./DeepTest/RemoteTestDriverHandler/bin") + Directory(configuration);
var buildFramework = Directory("./DeepTest/Framework/bin") + Directory(configuration);
var buildRemoteTestingWrapper = Directory("./DeepTest/RemoteTestingWrapper/bin") + Directory(configuration);

// NFBench Mirror --- To be Obsolete
var nfbImportReferenceBuildDir = Directory("./NFBenchImport.Benchmark.Reference/bin") + Directory(configuration);
var nfbImportPerformanceBuildDir = Directory("./NFBenchImport.Benchmark.Performance/bin") + Directory(configuration);
var nfbClientAppBuildDir = Directory("./NFBenchImport.Services.ClientApplication/bin") + Directory(configuration);

// Test Directories --- To be obsolete
var nfbImportTestsBuildDir = Directory("./Test.NFBenchImport/bin") + Directory(configuration);

// Demo & Staging
var stagingDir = Directory("./staging");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildInternalTestDriver);
    CleanDirectory(buildRemoteAssertionMessages);
    CleanDirectory(buildRemoteTestDriverHandler);
    CleanDirectory(buildFramework);
    CleanDirectory(buildRemoteTestingWrapper);

    // To be removed from framework
    CleanDirectory(nfbImportReferenceBuildDir);
    CleanDirectory(nfbImportPerformanceBuildDir);
    CleanDirectory(nfbClientAppBuildDir);
    CleanDirectory(nfbImportTestsBuildDir);
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

    CopyFiles("./DeepTest/InternalTestDriver/bin/Debug/*.dll", stagingDir);
    CopyFiles("./DeepTest/InternalTestDriver/bin/Debug/*.exe", stagingDir);

    // To be obsoleted later
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
