using System.Diagnostics;
using System.Text.Json;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Providers.Ollama.Brokers;
using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Foundations;

internal class OllamaConversationService(IOllamaBroker ollamaConversationBroker)
    : IOllamaConversationService
{
    public async IAsyncEnumerable<Token> InferStreamingAsync(InferrenceRequest inferrenceRequest)
    {
        OllamaPrompt prompt = CreatePrompt(inferrenceRequest);

        IAsyncEnumerable<OllamaToken> response =
            InferStreamingAsync(prompt);

        await foreach (OllamaToken ollamaToken in response)
            yield return MapToToken(ollamaToken);
    }

    public async ValueTask<Token> InferAsync(InferrenceRequest inferrenceRequest)
    {
        OllamaPrompt prompt = CreatePrompt(inferrenceRequest);
        prompt.Stream = false;

        var ollamaToken = await ollamaConversationBroker.SendPromptAsync(prompt);

        return MapToToken(ollamaToken);
    }

    async IAsyncEnumerable<OllamaToken> InferStreamingAsync(OllamaPrompt prompt)
    {
        using Stream responseStream = await ollamaConversationBroker
            .SendPromptForStreamingAsync(prompt);

        var reader = new StreamReader(responseStream);

        while ((await reader.ReadLineAsync()) is string line)
        {
            OllamaToken token = JsonSerializer
                .Deserialize<OllamaToken>(line);

            if (token.Message?.ToolCalls is not null)
                Debug.WriteLine($"[ollama:toolcalls]\n{JsonSerializer.Serialize(token.Message.ToolCalls)}");

            yield return token;

            if (token.Done)
                yield break;
        }
    }

    static Token MapToToken(OllamaToken ollamaToken) => new()
    {
        Content = ollamaToken.Message?.Content,
        Thought = ollamaToken.Message?.Thought,
        ToolCalls = ollamaToken.Message?.ToolCalls is not null
                ? MapToolCalls(ollamaToken.Message.ToolCalls)
                : null
    };

    static ToolCall[] MapToolCalls(ToolCallDetails[] toolCalls) => toolCalls.Select(toolCall =>
    {
        string[] toolAndFunctionNames = toolCall.Function.Name.Split('.');

        return new ToolCall()
        {
            ToolName = toolAndFunctionNames.Length > 0 ? toolAndFunctionNames[0] : null,
            FunctionName = toolAndFunctionNames.Length > 1 ? toolAndFunctionNames[1] : null,
            Arguments = toolCall.Function.Arguments
        };
    }).ToArray();

    static OllamaPrompt CreatePrompt(InferrenceRequest inferrenceRequest) => new()
    {
        Stream = true,
        Model = inferrenceRequest.LLMModelName,

        Messages = inferrenceRequest.Context.Select(message => new OllamaMessageData
        {
            Role = message.Role,
            Content = message.Message
        }).ToList(),

        Options = new OllamaPromptOptions
        {
            Temperature = 1.0,
            ContextLength = inferrenceRequest.ContextLength
        },

        Tools = MapToolArray(inferrenceRequest.Tools ?? [])
    };

    static OllamaTool[] MapToolArray(IList<Tool> allowedTools) =>
        allowedTools.SelectMany(tool => MapTool(tool))
        .ToArray();

    static IEnumerable<OllamaTool> MapTool(Tool tool) =>
        tool.ToolFunctions.Select(function => MapToolFunction(tool, function));

    static OllamaTool MapToolFunction(Tool tool, ToolFunction toolFunction) => new()
    {
        Function = new OllamaToolFunction
        {
            Name = $"{tool.Name}.{toolFunction.Name}",
            Description = toolFunction.Description,
            Parameters = MapToSchema(toolFunction.Parameters)
        }
    };

    static OllamaSchema MapToSchema(ToolParameter[] parameters) => new()
    {
        Type = "object",
        Properties = MapToolParameters(parameters),
        Required = parameters.Where(parameter => parameter.Required)
            .Select(parameter => parameter.Name)
            .ToArray()
    };

    static Dictionary<string, OllamaSchemaProperty> MapToolParameters(ToolParameter[] parameters)
    {
        if (parameters is null)
            return null;

        IList<KeyValuePair<string, OllamaSchemaProperty>> properties = parameters
            .Select(parameter => new KeyValuePair<string, OllamaSchemaProperty>(
                parameter.Name,
                MapParameter(parameter)))
            .ToList();

        return new Dictionary<string, OllamaSchemaProperty>(properties);
    }

    static OllamaSchemaProperty MapParameter(ToolParameter parameter) => new()
    {
        Type = parameter.Type,
        Description = parameter.Description,

        Properties = parameter.Type == "object"
            ? MapToolParameters(parameter.Properties)
            : null,

        Required = parameter.Type == "object"
            ? parameter.Properties?.Where(x => x.Required).Select(x => x.Name).ToArray()
            : null
    };
}