using AIServer.IG;
using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers.Api.IG;

[Route("api/markets/")]
public class MarketController : Controller
{
    private readonly ILogger<MarketController> _logger;
    private readonly IGClient _igClient;

    public MarketController(ILogger<MarketController> logger, IGClient igClient)
    {
        _logger = logger;
        _igClient = igClient;
    }

    [HttpGet("")]
    public async Task<IActionResult> Get()
    {
        var root = await _igClient.GetMarketHierarchyAsync();
        return Ok(new[] { root }); // Wrap in array for tree
    }

    [HttpGet("{nodeId}")]
    public async Task<IActionResult> Get(string nodeId)
    {
        var node = await _igClient.GetMarketHierarchyAsync(nodeId);
        return Ok(node);
    }
}