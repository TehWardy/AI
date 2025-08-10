using AIServer.MCPTools.IG;
using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers.Api.IG;

[Route("api/markets/")]
public class MarketController : Controller
{
    private readonly IGClient igClient;

    public MarketController(IGClient igClient) =>
        this.igClient = igClient;

    [HttpGet("")]
    public async Task<IActionResult> Get()
    {
        var root = await igClient.GetMarketHierarchyAsync();
        return Ok(root); 
    }

    [HttpGet("{nodeId}")]
    public async Task<IActionResult> Get(string nodeId)
    {
        var node = await igClient.GetMarketHierarchyAsync(nodeId);
        return Ok(node);
    }
}