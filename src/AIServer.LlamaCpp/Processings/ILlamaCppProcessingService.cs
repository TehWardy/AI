using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp.Processings;
public interface ILlamaCppProcessingService
{
    IAsyncEnumerable<ResponseToken> SendPromptAsync(List<Message> conversationHistory);
}