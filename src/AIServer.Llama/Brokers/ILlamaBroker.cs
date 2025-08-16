using AIServer.Llama.Models;
using LLama.Common;
using System.Threading.Tasks;

namespace AIServer.Llama.Brokers;

internal interface ILlamaBroker
{
    IAsyncEnumerable<string> SendPromptAsync(LlamaChatPrompt prompt);
    ValueTask InitializeChatSession(string modelName, string systemPrompt);
}
