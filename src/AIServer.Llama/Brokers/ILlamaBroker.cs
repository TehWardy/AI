using AIServer.Llama.Models;
using LLama.Common;

namespace AIServer.Llama.Brokers;

internal interface ILlamaBroker
{
    IAsyncEnumerable<string> SendPromptAsync(LlamaChatPrompt prompt);
    void LoadModel(string modelName);
    string GetCurrentModelName();
}