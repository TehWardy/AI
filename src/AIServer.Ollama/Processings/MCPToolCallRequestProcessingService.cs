using System.Text;
using System.Text.Json;
using AIServer.Ollama.Foundations;
using AIServer.Ollama.Models;

namespace AIServer.Ollama.Processings;
internal class MCPToolCallRequestProcessingService : IMCPToolCallRequestProcessingService
{
    private readonly IMCPToolHttpCallRequestService requestService;

    public MCPToolCallRequestProcessingService(IMCPToolHttpCallRequestService requestService) =>
        this.requestService = requestService;

    public async ValueTask<ResponseToken> ExecuteToolCallAsync(string model, ToolFunctionDetails callDetails)
    {
        try
        {
            var requestMessage = new HttpRequestMessage();

            if (callDetails.Arguments.ContainsKey("endpoint"))
            {
                requestMessage.RequestUri =
                    new Uri(callDetails.Arguments["endpoint"].ToString());
            }

            if (callDetails.Arguments.ContainsKey("verb"))
            {
                requestMessage.Method =
                    new HttpMethod(callDetails.Arguments["verb"].ToString());
            }

            if (callDetails.Arguments.ContainsKey("body"))
            {
                requestMessage.Content = new StringContent(
                    JsonSerializer.Serialize(callDetails.Arguments["body"]),
                    Encoding.UTF8,
                    "application/json");
            }

            if (callDetails.Arguments.ContainsKey("headers"))
            {
                var element = (JsonElement)callDetails.Arguments["headers"];

                bool empty = element.ValueKind == JsonValueKind.Object &&
                    !element.EnumerateObject().Any();

                if (!empty)
                {
                    IDictionary<string, object> headers =
                        (IDictionary<string, object>)callDetails.Arguments["headers"];

                    foreach (var header in headers)
                        requestMessage.Headers.Add(header.Key.ToString(), header.Value.ToString());
                }
            }

            var response = await requestService.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            return CreateResponseToken(
                model,
                callDetails.Name,
                await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return CreateResponseToken(
                model,
                callDetails.Name,
                $"Failed to make the requested http request due to the following exception ...\n{ex.Message}");
        }
    }

    private ResponseToken CreateResponseToken(string model, string toolName, string toolCallResponseString)
    {
        return new ResponseToken()
        {
            Done = false,
            Message = BuildToolMessageData(toolName, toolCallResponseString),
            Model = model
        };
    }

    MessageData BuildToolMessageData(string toolName, string content)
    {
        return new MessageData
        {
            Role = "tool",
            ToolName = toolName,
            Content = content
        };
    }
}
