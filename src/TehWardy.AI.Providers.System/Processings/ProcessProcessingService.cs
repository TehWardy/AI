using System.Diagnostics;
using System.Threading.Channels;
using TehWardy.AI.Providers.Foundations;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.System.Processings;

internal class ProcessProcessingService(IProcessService processService)
    : IProcessProcessingService
{
    public async IAsyncEnumerable<ProcessToken> ExecuteProcessAsync(
        string executablePath,
        string workingDirectory,
        string arguments)
    {
        using Process process = processService
            .CreateProcess(executablePath, workingDirectory, arguments);

        var channelOptions = new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        };

        var channel = Channel.CreateUnbounded<ProcessToken>(channelOptions);
        process.Start();

        // Kick off two pumps.
        Task pumpOut = PumpLinesAsync(
            reader: process.StandardOutput,
            sourceStream: ProcessStreamSource.StdOut,
            writer: channel.Writer);

        Task pumpErr = PumpLinesAsync(
            reader: process.StandardError,
            sourceStream: ProcessStreamSource.StdErr,
            writer: channel.Writer);

        // Wait for exit (and both pumps to finish), then complete channel and emit final.
        Task exitTask = process.WaitForExitAsync();

        _ = Task.Run(async () =>
        {
            try
            {
                await exitTask.ConfigureAwait(false);
                await Task.WhenAll(pumpOut, pumpErr).ConfigureAwait(false);

                channel.Writer.TryWrite(new ProcessToken
                {
                    StreamSource = ProcessStreamSource.StdOut,
                    Value = null,
                    IsFinalToken = true,
                    ExitCode = process.ExitCode
                });
            }
            catch (Exception ex)
            {
                channel.Writer.TryWrite(new ProcessToken
                {
                    StreamSource = ProcessStreamSource.StdErr,
                    Value = ex.Message,
                    IsFinalToken = true,
                    ExitCode = -1
                });
            }
            finally
            {
                channel.Writer.TryComplete();
            }
        });

        await foreach (ProcessToken token in channel.Reader.ReadAllAsync().ConfigureAwait(false))
            yield return token;
    }

    private static async Task PumpLinesAsync(
        StreamReader reader,
        ProcessStreamSource sourceStream,
        ChannelWriter<ProcessToken> writer)
    {
        while (true)
        {
            string line = await reader.ReadLineAsync().ConfigureAwait(false);

            if (line is null)
                break;

            writer.TryWrite(new ProcessToken
            {
                StreamSource = ProcessStreamSource.StdErr,
                Value = line,
                IsFinalToken = false,
                ExitCode = 0
            });
        }
    }
}