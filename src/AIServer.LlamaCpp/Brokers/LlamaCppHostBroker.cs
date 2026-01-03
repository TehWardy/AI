using System.Diagnostics;
using AIServer.LlamaCpp.Configurations;

namespace AIServer.LlamaCpp.Brokers;

internal sealed class LlamaCppHostBroker : ILlamaCppHostBroker
{
    private Process llamaCppProcess = null;
    private readonly LlamaCppConfiguration config;

    static readonly string llamaServerExePath = Path
        .Combine(AppContext.BaseDirectory, "LlamaCpp\\llama-server.exe");

    public LlamaCppHostBroker(LlamaCppConfiguration config) =>
        this.config = config;

    public ValueTask<Process> StartAsync(string modelName)
    {
        var modelPath = Path.Combine(config.ModelsPath, $"{modelName}.gguf");

        if (!File.Exists(modelPath))
            throw new FileNotFoundException($"Model not found at: {modelPath}");

        var startInfo = new ProcessStartInfo
        {
            FileName = llamaServerExePath,
            WorkingDirectory = Path.GetDirectoryName(llamaServerExePath),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("-m");
        startInfo.ArgumentList.Add(modelPath);
        startInfo.ArgumentList.Add("--port");
        startInfo.ArgumentList.Add(config.ServerPort.ToString());
        startInfo.ArgumentList.Add("-c");
        startInfo.ArgumentList.Add(config.ContextSize.ToString());
        startInfo.ArgumentList.Add("-ngl");
        startInfo.ArgumentList.Add(config.GpuLayerCount.ToString());

        // Start server
        llamaCppProcess = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        llamaCppProcess.Start();
        return ValueTask.FromResult(llamaCppProcess);
    }

    public ValueTask<bool> IsHostProcessRunningAsync() =>
        ValueTask.FromResult(llamaCppProcess is not null);

    public void Dispose()
    {
        try
        {
            if (!llamaCppProcess.HasExited)
                llamaCppProcess.Kill(entireProcessTree: true);
        }
        catch { /* ignore */ }
        finally
        {
            llamaCppProcess?.Dispose();
        }
    }

    
}