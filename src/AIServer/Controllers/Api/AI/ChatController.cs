using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers.Api.AI;

public class ChatController : Controller
{
    internal async ValueTask Respond(IAsyncEnumerable<string> tokenStream)
    {
        Response.ContentType = "plain/text";
        HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
        HttpContext.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        await foreach (string token in tokenStream)
        {
            await Response.WriteAsync(token);
            await Response.Body.FlushAsync();
        }
    }
}