using TehWardy.AI.Providers.AI.Foundations;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers.AI.Orchestrations;

internal class ConversationRouterOrchestrationService(
    ILargeLanguageModelService largeLanguageModelService,
    IRoutingService routingService)
        : IConversationRouterOrchestrationService
{
    public async ValueTask<Token> InferRouteAsync<T>(
        InferrenceRequest inferrenceRequest)
    {
        string routingInformation =
            await routingService.RetrieveAllRoutingInformationAsync<T>();

        AddContextToInferrenceRequest(inferrenceRequest, routingInformation);

        return await largeLanguageModelService
            .InferAsync(inferrenceRequest);
    }

    void AddContextToInferrenceRequest(
        InferrenceRequest inferrenceRequest,
        string routingInformation)
    {
        ChatMessage systemContextMessage = new()
        {
            Role = "system",
            Message = """
You are a routing agent.
            
You will be given:
 1) A JSON array named ROUTES.
 2) A user request.

Pick exactly ONE element from ROUTES and return ONLY that element as a JSON object.

Rules:
 - Output must be valid JSON.
 - Output must start with '{' and end with '}'.
 - Do not wrap in markdown or code fences.
 - Do not output any additional keys or text.
 - Return the selected object exactly as it appears in ROUTES(same properties / values).

ROUTES:
""" + routingInformation
        };

        inferrenceRequest.Context = inferrenceRequest.Context
            .Prepend(systemContextMessage)
            .ToList();
    }
}