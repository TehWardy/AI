using System.Diagnostics;
using AIServer.LlamaCpp.Configurations;

namespace AIServer.LlamaCpp.Brokers;

internal sealed partial class LlamaCppHostBroker : ILlamaCppHostBroker
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

        var args = new[]
        {
            "-m", $"\"{modelPath}\"",
            "--port", config.ServerPort.ToString(),
            "-c", config.ContextSize.ToString(),
            "-ngl", config.GpuLayerCount.ToString()
        };

        // Start server
        llamaCppProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = llamaServerExePath,
                Arguments = string.Join(" ", args),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            },
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