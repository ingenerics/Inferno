using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.PackWpf);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetResultsDirectory(RootDirectory / ".tmp" / "testResults"));
        });

    Target PackCore => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(SourceDirectory / "Inferno.Core" / "Inferno.Core.csproj")
                .SetConfiguration(Configuration)
                .EnableNoDependencies()
                .EnableIncludeSymbols()
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(OutputDirectory));
        });

    Target PackReactive => _ => _
        .DependsOn(PackCore)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(SourceDirectory / "Inferno.Reactive" / "Inferno.Reactive.csproj")
                .SetConfiguration(Configuration)
                .EnableNoDependencies()
                .EnableIncludeSymbols()
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(OutputDirectory));
        });

    Target PackLifeCycle => _ => _
        .DependsOn(PackReactive)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(SourceDirectory / "Inferno.LifeCycle" / "Inferno.LifeCycle.csproj")
                .SetConfiguration(Configuration)
                .EnableNoDependencies()
                .EnableIncludeSymbols()
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(OutputDirectory));
        });

    Target PackWpfShared => _ => _
        .DependsOn(PackLifeCycle)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(SourceDirectory / "Inferno.Wpf.Shared" / "Inferno.Wpf.Shared.csproj")
                .SetConfiguration(Configuration)
                .EnableNoDependencies()
                .EnableIncludeSymbols()
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(OutputDirectory));
        });

    Target PackWpf => _ => _
        .DependsOn(PackWpfShared)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(SourceDirectory / "Inferno.Wpf" / "Inferno.Wpf.csproj")
                .SetConfiguration(Configuration)
                .EnableNoDependencies()
                .EnableIncludeSymbols()
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(OutputDirectory));
        });

    Target PackWpfMetro => _ => _
        .DependsOn(PackWpfShared)
        .TriggeredBy(PackWpf)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(SourceDirectory / "Inferno.Wpf.Metro" / "Inferno.Wpf.Metro.csproj")
                .SetConfiguration(Configuration)
                .EnableNoDependencies()
                .EnableIncludeSymbols()
                .EnableNoRestore()
                .EnableNoBuild()
                .SetOutputDirectory(OutputDirectory));
        });
}
