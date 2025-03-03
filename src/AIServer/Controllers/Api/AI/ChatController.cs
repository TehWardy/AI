using AIServer.AI;
using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers.Api.AI;

[Route("api/chat")]
public class ChatController(ILogger<HomeController> logger, AIChatClient chatClient)
    : Controller
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody]string message) =>
        Ok(await chatClient.SendMessageAsync(message));
}