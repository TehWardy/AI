using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp.Brokers;
internal interface ILlamaCppBroker
{
    ValueTask<StreamReader> SendCompletionRequestAsync(LlamaServerCompletionRequest request);
}