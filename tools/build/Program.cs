﻿using System.Runtime.CompilerServices;
using static Bullseye.Targets;
using static SimpleExec.Command;

var commandLineOptions = CommandLineOptions.Parse(args);

Directory.SetCurrentDirectory(GetSolutionDirectory());

string artifactsDir = Path.GetFullPath("artifacts");
string logsDir = Path.Combine(artifactsDir, "logs");
string buildLogFile = Path.Combine(logsDir, "build.binlog");
string packagesDir = Path.Combine(artifactsDir, "packages");

string solutionFile = "NHotkey.sln";

Target(
    "artifactDirectories",
    () =>
    {
        Directory.CreateDirectory(artifactsDir);
        Directory.CreateDirectory(logsDir);
        Directory.CreateDirectory(packagesDir);
    });

Target(
    "build",
    DependsOn("artifactDirectories"),
    () => Run(
        "dotnet",
        $"build -c \"{commandLineOptions.Configuration}\" /bl:\"{buildLogFile}\" \"{solutionFile}\""));

Target(
    "pack",
    DependsOn("artifactDirectories", "build"),
    () => Run(
        "dotnet",
        $"pack -c \"{commandLineOptions.Configuration}\" --no-build -o \"{packagesDir}\""));

Target("default", DependsOn("pack"));

if (commandLineOptions.ShowHelp)
{
    await CommandLineOptions.PrintUsageAsync();
    return;
}

await RunTargetsAndExitAsync(commandLineOptions.BullseyeArgs);

static string GetSolutionDirectory() =>
    Path.GetFullPath(Path.Combine(GetScriptDirectory(), @"..\.."));

static string GetScriptDirectory([CallerFilePath] string filename = null) => Path.GetDirectoryName(filename);