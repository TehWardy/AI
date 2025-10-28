using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp.Foundations;
internal interface ILlamaCppService
{
    IAsyncEnumerable<string> SendPromptAsync(List<Message> conversationHistory);
}