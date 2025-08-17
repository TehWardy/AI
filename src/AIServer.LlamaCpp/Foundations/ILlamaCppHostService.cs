using System.Diagnostics;

namespace AIServer.LlamaCpp.Foundations;
internal interface ILlamaCppHostService
{
    ValueTask<Process> StartAsync(string modelName);
}