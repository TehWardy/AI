using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TehWardy.AI.Models;

namespace TehWardy.AI.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ConversationController(IAgenticConversation conversation)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<List<Conversation>> GetAllAsync() =>
        await conversation.RetrieveAllConversationsAsync();

    [HttpGet("{conversationId}")]
    public async Task<Conversation> GetAsync([FromRoute] Guid conversationId) =>
        await conversation.RetrieveConversationByIdAsync(conversationId);

    [HttpPost("create")]
    public async Task<Conversation> CreateAsync([FromBody] Prompt prompt) =>
        await conversation.CreateConversationAsync(prompt);

    [HttpPost("")]
    public async Task PostAsync([FromBody] Prompt prompt)
    {
        Response.StatusCode = StatusCodes.Status200OK;
        Response.ContentType = "application/x-ndjson";

        await foreach (var token in conversation.InferStreamingAsync(prompt))
        {
            string json = JsonSerializer.Serialize(token);
            Console.WriteLine(json);

            await Response.WriteAsync(json);
            await Response.WriteAsync("\n");

            await Response.Body.FlushAsync();
        }
    }
}