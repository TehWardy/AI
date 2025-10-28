using System.Text.Json;
using AIServer.Ollama;
using AIServer.Ollama.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers.Api.AI;

[Route("api/chat/ollama/")]
public class OllamaChatController(
    IOllamaChatClient chatClient) 
        : ChatController
{
    [HttpPost("{modelId}/{id}")]
    public async Task PostAsync(
        [FromRoute] string modelId,
        [FromRoute] string id,
        [FromBody] string message)
    {
        var api = new HttpClient()
        {
            BaseAddress = new Uri("https://localhost:7181/api/tools/")
        };

        chatClient.ModelId = modelId;

        //chatClient.Tools = await api
        //    .GetFromJsonAsync<IEnumerable<IDictionary<string, object>>>(
        //        "from-swagger-spec?websiteRootUri=https://localhost:7181/&fromPath=/api/tools");

        await Respond(chatClient.SendAsync(message));
    }

    internal async ValueTask Respond(IAsyncEnumerable<ResponseToken> tokenStream)
    {
        Response.ContentType = "plain/text";
        HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
        HttpContext.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        await foreach (ResponseToken token in tokenStream)
        {
            await Response.WriteAsync(JsonSerializer.Serialize(token.Message));
            await Response.Body.FlushAsync();
        }
    }
}