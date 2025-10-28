namespace AIServer.LlamaCpp.Foundations;

internal interface ILlamaCppHostService
{
    IAsyncEnumerable<string> StartAsync(string modelName);
    ValueTask StopAsync();
}