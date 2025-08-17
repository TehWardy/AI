using System.Diagnostics;

namespace AIServer.LlamaCpp.Brokers;
internal interface ILlamaCppHostBroker : IDisposable
{
    ValueTask<Process> StartAsync(string modelName);
}