using System.Text.RegularExpressions;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Tools.DotNet.Foundations;

namespace TehWardy.AI.Tools.DotNet.Processings;

internal sealed class DotNetProcessingService(IDotNetService dotNetService, DotNetConfiguration dotNetConfig)
    : IDotNetProcessingService
{
    public IAsyncEnumerable<ProcessToken> DotNetTestAsync(string solutionOrProjectPath, string workingDirectory, string configuration, bool noBuild, string filter)
    {
        ValidatePathWithinRoot(workingDirectory);
        ValidatePathWithinRoot(solutionOrProjectPath);

        return dotNetService.DotNetTestAsync(solutionOrProjectPath, workingDirectory, configuration, noBuild, filter);
    }

    public IAsyncEnumerable<ProcessToken> DotNetBuildAsync(string solutionOrProjectPath, string workingDirectory, string configuration)
    {
        ValidatePathWithinRoot(workingDirectory);
        ValidatePathWithinRoot(solutionOrProjectPath);

        return dotNetService.DotNetBuildAsync(solutionOrProjectPath, workingDirectory, configuration);
    }

    public IAsyncEnumerable<ProcessToken> DotNetFormatAsync(string solutionOrProjectPath, string workingDirectory, bool verifyNoChanges)
    {
        ValidatePathWithinRoot(workingDirectory);
        ValidatePathWithinRoot(solutionOrProjectPath);

        return dotNetService.DotNetFormatAsync(solutionOrProjectPath, workingDirectory, verifyNoChanges);
    }

    public IAsyncEnumerable<ProcessToken> DotnetNewAsync(string template, string outputDirectory, string name, IDictionary<string, string> additionalArgs)
    {
        ValidateTemplate(template);
        ValidatePathWithinRoot(outputDirectory);
        ValidateAdditionalArgs(additionalArgs);

        return dotNetService.DotnetNewAsync(template, outputDirectory, name, additionalArgs);
    }

    public IAsyncEnumerable<ProcessToken> DotNetAddReference(string projectPath, string workingDirectory, string referencePath)
    {
        ValidatePathWithinRoot(workingDirectory);
        ValidatePathWithinRoot(projectPath);
        ValidatePathWithinRoot(referencePath);

        return dotNetService.DotNetAddReference(projectPath, workingDirectory, referencePath);
    }

    void ValidatePathWithinRoot(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("path is required.", nameof(path));

        string pathFull = Path.GetFullPath(path);

        if (!pathFull.StartsWith(dotNetConfig.RootWorkingDirectory, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Path '{pathFull}' is outside root '{dotNetConfig.RootWorkingDirectory}'.");
    }

    static void ValidateTemplate(string template)
    {
        if (string.IsNullOrWhiteSpace(template))
            throw new ArgumentException("template is required.", nameof(template));

        if (!Regex.IsMatch(template, "^[a-zA-Z0-9_.-]+$"))
            throw new ArgumentException("template contains invalid characters.", nameof(template));
    }


    static void ValidateAdditionalArgs(IDictionary<string, string> args)
    {
        if (args is null) return;

        foreach (var key in args.Keys)
        {
            if (!key.StartsWith("--", StringComparison.Ordinal))
                throw new ArgumentException($"Invalid dotnet arg key: '{key}'. Must start with '--'.");

            if (!Regex.IsMatch(key, "^--[a-zA-Z0-9-]+$"))
                throw new ArgumentException($"Invalid dotnet arg key: '{key}'.");
        }
    }
}