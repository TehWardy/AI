using System.Diagnostics;
using System.Threading.Channels;
using AIServer.Ollama.Brokers;

namespace AIServer.Ollama.Foundations;

internal class OllamaModelHostService : IOllamaModelHostService
{
    private readonly IOllamaModelHostBroker hostBroker;

    public OllamaModelHostService(IOllamaModelHostBroker hostBroker) =>
        this.hostBroker = hostBroker;

    public async IAsyncEnumerable<string> DownloadedModelAsync(string model)
    {
        var process = await hostBroker
            .CreateOllamaModelDownloadProcessAsync(model);

        yield return $"[Model Prep Output] Downloading model {model} ...";

        IAsyncEnumerable<string> processOutput =
            HandleProcessOutput(process);

        await foreach (string processOutputLine in processOutput)
            yield return processOutputLine;

        yield return $"Downloading model '{model}' finished!";
    }

    public async IAsyncEnumerable<string> StartOllamaHostProcess()
    {
        bool alreadyRunning = await hostBroker.IsHostProcessRunningAsync();

        if (alreadyRunning)
            throw new InvalidOperationException("AI Host Process is already running!");

        yield return "Starting up host ...";

        Process ollamaProcess = await hostBroker
            .CreateOllamaHostProcessAsync();

        IAsyncEnumerable<string> processOutput =
            HandleProcessOutput(ollamaProcess);

        await foreach (string processOutputLine in processOutput)
            yield return processOutputLine;

        yield return "Host is ready!";
    }

    async IAsyncEnumerable<string> HandleProcessOutput(Process process)
    {
        // Bridge: writers (event handlers) -> reader (iterator)
        var channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        var stdoutClosed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var stderrClosed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        DataReceivedEventHandler outHandler = (_, e) =>
        {
            if (e.Data is null)
            {
                stdoutClosed.TrySetResult();
                return;
            }

            channel.Writer.TryWrite($"INFO {e.Data}");
        };

        DataReceivedEventHandler errHandler = (_, e) =>
        {
            if (e.Data is null)
            {
                stderrClosed.TrySetResult();
                return;
            }

            channel.Writer.TryWrite($"ERROR {e.Data}");
        };

        process.OutputDataReceived += outHandler;
        process.ErrorDataReceived += errHandler;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // When the process and both streams finish, complete the channel
        var completionTask = Task.Run(async () =>
        {
            try
            {
                await process.WaitForExitAsync();
                await Task.WhenAll(stdoutClosed.Task, stderrClosed.Task);
                channel.Writer.TryComplete();
            }
            catch (Exception ex)
            {
                channel.Writer.TryComplete(ex);
            }
        });

        await foreach (var line in channel.Reader.ReadAllAsync())
            yield return line;

        await completionTask;
    }

    public async ValueTask StopOllamaHostProcessAsync() =>
        await hostBroker.StopOllamaHostProcessAsync();
}