#tool nuget:?package=NUnit.ConsoleRunner&version=3.7.0
#addin "Cake.Incubator"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////

var DeepTestSolutionPath = "DeepTest.sln";

var RootDir = Directory("./");
var DeepTestFrameworkDir = Directory("./DeepTest/");
var TestDir = Directory("./Test/"); 
var ExamplesDir = Directory("./Examples/");

// Demo & Staging
var stagingDir = Directory("./staging");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    foreach(var path in GetSubDirectories(DeepTestFrameworkDir))
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/bin/" + configuration);
        CleanDirectories(path + "/obj/" + configuration);
    }

    foreach(var path in GetSubDirectories(TestDir))
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/bin/" + configuration);
        CleanDirectories(path + "/obj/" + configuration);
    }

    foreach(var path in GetSubDirectories(ExamplesDir))
    {
        Information("Cleaning {0}", path);
        CleanDirectories(path + "/bin/" + configuration);
        CleanDirectories(path + "/obj/" + configuration);
    }
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(DeepTestSolutionPath);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild(DeepTestSolutionPath, settings =>
        settings.SetVerbosity(Verbosity.Minimal));
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    EnsureDirectoryExists(stagingDir);
    CleanDirectory(stagingDir);
    
    foreach(var path in GetSubDirectories(ExamplesDir))
    {
        Information("Copying Example Project {0}", path);
        CopyFiles(
            GetFiles(path + "/bin/" + configuration + "/*.exe"),
            stagingDir
        );
        CopyFiles(
            GetFiles(path + "/bin/" + configuration + "/*.dll"),
            stagingDir
        );
    }

    CopyFiles(
        GetFiles("./DeepTest/RemoteTestingWrapper/bin/Debug/*.dll"),
        stagingDir
    );

    NUnit3("./Test/**/bin/Debug/*.Tests.dll", new NUnit3Settings {
        NoResults = true
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);
