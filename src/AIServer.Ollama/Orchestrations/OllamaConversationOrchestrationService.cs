using AIServer.Ollama.Models;
using AIServer.Ollama.Processings;

namespace AIServer.Ollama.Orchestrations;

internal class OllamaConversationOrchestrationService : IOllamaConversationOrchestrationService
{
    private readonly IOllamaConversationProcessingService conversationProcessingService;
    private readonly IMCPToolCallRequestProcessingService toolRequestProcessingService;

    public OllamaConversationOrchestrationService(
        IOllamaConversationProcessingService conversationProcessingService,
        IMCPToolCallRequestProcessingService toolRequestProcessingService)
    {
        this.conversationProcessingService = conversationProcessingService;
        this.toolRequestProcessingService = toolRequestProcessingService;
    }

    public async IAsyncEnumerable<ResponseToken> SendPromptAsync(ChatPrompt prompt)
    {
        await foreach (var parsedToken in conversationProcessingService.SendPromptAsync(prompt))
        {
            yield return parsedToken;

            if (TokenContainsToolCalls(parsedToken))
            {
                await foreach (var toolResponse in ProcessToolCalls(prompt, parsedToken))
                    yield return toolResponse;
            }
        }
    }

    private bool TokenContainsToolCalls(ResponseToken token) =>
        (token.Message.ToolCalls ?? []).Any();

    private async IAsyncEnumerable<ResponseToken> ProcessToolCalls(ChatPrompt sourcePrompt, ResponseToken token)
    {
        foreach (var toolCall in token.Message.ToolCalls)
        {
            ResponseToken toolResponse = await toolRequestProcessingService
                .ExecuteToolCallAsync(sourcePrompt.Model, toolCall.Function);

            sourcePrompt.Messages.Add(toolResponse.Message);

            IAsyncEnumerable<ResponseToken> tollCallResponse =
                conversationProcessingService.SendPromptAsync(sourcePrompt);

            await foreach (ResponseToken toolCallResponseToken in tollCallResponse)
                yield return toolCallResponseToken;
        }
    }
}