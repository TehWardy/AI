using System.Diagnostics;
using AIServer.LlamaCpp.Configurations;

namespace AIServer.LlamaCpp.Brokers;

internal sealed partial class LlamaCppHostBroker : ILlamaCppHostBroker
{
    private readonly LlamaCppConfiguration config;

    private Process serverProcess;
    private HttpClient llamaClient;

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
            "--ctx-size", config.ContextSize.ToString(),
            "--batch-size", config.BatchSize.ToString(),
            "--n-gpu-layers", config.GpuLayerCount.ToString(),
            "--no-webui",
            "--use-mmap",
            "--mlock", config.UseMemoryLock.ToString().ToLower()
        };

        // Start server
        serverProcess = new Process
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

        serverProcess.Start();

        // log server output in background (non-blocking)
        _ = Task.Run(async () =>
        {
            while (!serverProcess.HasExited)
            {
                var line = await serverProcess.StandardError
                    .ReadLineAsync()
                    .ConfigureAwait(false);

                if (line is null)
                    break;

                Console.Error.WriteLine($"[llama-server] {line}");
            }
        });

        return ValueTask.FromResult(serverProcess);
    }

    public void Dispose()
    {
        try { llamaClient?.Dispose(); } catch { /* ignore */ }
        try
        {
            if (!serverProcess.HasExited)
                serverProcess.Kill(entireProcessTree: true);
        }
        catch { /* ignore */ }
        finally
        {
            serverProcess?.Dispose();
        }
    }

    
}