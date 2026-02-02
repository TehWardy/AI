using System.Text;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Tools.DotNet.Brokers;

namespace TehWardy.AI.Tools.DotNet.Foundations;

internal class DotNetService(IDotNetProcessBroker dotNetProcessBroker)
    : IDotNetService
{
    public IAsyncEnumerable<ProcessToken> DotNetTestAsync(string solutionOrProjectPath, string workingDirectory, string configuration, bool noBuild, string filter)
    {
        EnsureExists(workingDirectory);

        var args = new StringBuilder();
        args.Append($"test {Escape(solutionOrProjectPath)}");

        if (!string.IsNullOrWhiteSpace(configuration))
            args.Append($" -c {Escape(configuration)}");

        if (noBuild)
            args.Append(" --no-build");

        if (!string.IsNullOrWhiteSpace(filter))
            args.Append($" --filter {Escape(filter)}");

        return dotNetProcessBroker.ExecuteDotNetAsync(args.ToString(), workingDirectory);
    }

    public IAsyncEnumerable<ProcessToken> DotNetBuildAsync(string solutionOrProjectPath, string workingDirectory, string configuration)
    {
        EnsureExists(workingDirectory);

        var args = new StringBuilder();
        args.Append($"build {Escape(solutionOrProjectPath)}");

        if (!string.IsNullOrWhiteSpace(configuration))
            args.Append($" -c {Escape(configuration)}");

        return dotNetProcessBroker.ExecuteDotNetAsync(args.ToString(), workingDirectory);
    }

    public IAsyncEnumerable<ProcessToken> DotNetFormatAsync(string solutionOrProjectPath, string workingDirectory, bool verifyNoChanges)
    {
        EnsureExists(workingDirectory);

        var args = new StringBuilder();
        args.Append($"format {Escape(solutionOrProjectPath)}");

        if (verifyNoChanges)
            args.Append(" --verify-no-changes");

        return dotNetProcessBroker.ExecuteDotNetAsync(args.ToString(), workingDirectory);
    }

    public IAsyncEnumerable<ProcessToken> DotnetNewAsync(string template, string outputDirectory, string name, IDictionary<string, string> additionalArgs)
    {
        EnsureExists(outputDirectory);

        var args = new StringBuilder();
        args.Append($"new {Escape(template)}");
        args.Append($" -o {Escape(outputDirectory)}");
        args.Append($" -n {Escape(name)}");

        if (additionalArgs is not null)
        {
            foreach (var arg in additionalArgs)
                args.Append(" ").Append(arg.Key).Append(" ").Append(Escape(arg.Value));
        }

        return dotNetProcessBroker.ExecuteDotNetAsync(args.ToString(), outputDirectory);
    }

    public IAsyncEnumerable<ProcessToken> DotNetAddReference(string projectPath, string workingDirectory, string referencePath)
    {
        EnsureExists(workingDirectory);

        string args = $"add {Escape(projectPath)} reference {Escape(referencePath)}";
        return dotNetProcessBroker.ExecuteDotNetAsync(args, workingDirectory);
    }

    static string Escape(string value) =>
        "\"" + (value ?? "").Replace("\"", "\\\"") + "\"";

    void EnsureExists(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("path is required.", nameof(path));

        if (!dotNetProcessBroker.PathExists(path))
            dotNetProcessBroker.CreateDirectory(path);
    }
}