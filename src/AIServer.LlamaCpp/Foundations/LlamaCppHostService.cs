using System.Diagnostics;
using System.Threading.Channels;
using AIServer.LlamaCpp.Brokers;

namespace AIServer.LlamaCpp.Foundations;

internal class LlamaCppHostService : ILlamaCppHostService
{
    private readonly ILlamaCppHostBroker llamaCppHostBroker;

    public LlamaCppHostService(ILlamaCppHostBroker llamaCppHostBroker) =>
        this.llamaCppHostBroker = llamaCppHostBroker;

    public async IAsyncEnumerable<string> StartAsync(string modelName)
    {
        bool alreadyRunning = await llamaCppHostBroker.IsHostProcessRunningAsync();

        if (alreadyRunning)
            throw new InvalidOperationException("AI Host Process is already running!");


        Process llamaServerProcess = await llamaCppHostBroker
            .StartAsync(modelName);

        IAsyncEnumerable<string> processOutput = 
            HandleProcessOutput(llamaServerProcess);

        await foreach (string processOutputLine in processOutput)
            yield return processOutputLine;

        yield return "Host is ready!";
    }

    public ValueTask StopAsync()
    {
        llamaCppHostBroker.Dispose();
        return ValueTask.CompletedTask;
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

            channel.Writer.TryWrite($"ERROR {e.Data}");
        };

        DataReceivedEventHandler errHandler = (_, e) =>
        {
            if (e.Data is null)
            {
                stderrClosed.TrySetResult();
                return;
            }

            channel.Writer.TryWrite($"INFO {e.Data}");
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
}