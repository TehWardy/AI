using AIServer.Ollama.Models;

namespace AIServer.Ollama.Brokers;
internal interface IOllamaConversationBroker
{
    ValueTask<Stream> SendPromptAsync(ChatPrompt prompt);
}