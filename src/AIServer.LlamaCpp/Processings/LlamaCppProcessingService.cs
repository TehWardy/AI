using AIServer.LlamaCpp.Foundations;
using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp.Processings;

internal class LlamaCppProcessingService : ILlamaCppProcessingService
{
    private readonly ILlamaCppService llamaCppService;

    public LlamaCppProcessingService(ILlamaCppService llamaCppService) =>
        this.llamaCppService = llamaCppService;

    public async IAsyncEnumerable<ResponseToken> SendPromptAsync(List<Message> conversationHistory)
    {
        string channel = null;

        await foreach (string rawToken in llamaCppService.SendPromptAsync(conversationHistory))
        {
            if (channel is null || channel == "<|channel|>")
            {
                channel = rawToken;
                continue;
            }

            if (channel == "analysis")
            {
                yield return  new ResponseToken
                {
                    Message = new ResponseMessageData 
                    {
                        Role = "assistant",
                        Thought = rawToken
                    }
                };
            }
            else
            {
                yield return new ResponseToken
                {
                    Message = new ResponseMessageData
                    {
                        Role = "assistant",
                        Content = rawToken
                    }
                };
            }
        }

        if (channel == "analysis")
        { 
            
        }
    }
}