using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp.Foundations;
public interface ILlamaCppService
{
    IAsyncEnumerable<string> SendPromptAsync(LlamaCppPrompt prompt);
}