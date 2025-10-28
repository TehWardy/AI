using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp.Brokers;
internal interface ILlamaCppBroker
{
    ValueTask<HttpResponseMessage> SendCompletionRequestAsync(LlamaServerCompletionRequest request);
}