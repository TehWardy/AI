using AIServer.MCPTools.IG;
using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers.Api.IG;

[Route("api/pricing/")]
public class PricingController(IGClient igClient)
    : Controller
{
    [HttpGet("{epic}")]
    public async Task<IActionResult> Post(string epic)
    {
        int setSize = 20;

        var result = new
        {
            Day    = await igClient.GetPricingData(epic, "DAY", setSize),
            Hour   = await igClient.GetPricingData(epic, "HOUR", setSize),
            Minute = await igClient.GetPricingData(epic, "MINUTE", setSize),
            Second = await igClient.GetPricingData(epic, "SECOND", setSize)
        };

        return Ok(result);
    }
}
