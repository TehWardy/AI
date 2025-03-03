using AIServer.IG;
using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers.Api.IG;

[Route("api/pricing")]
public class PricingController(ILogger<PricingController> logger, IGClient igClient)
    : Controller
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string epic)
    {
        var result = new
        {
            Day    = await igClient.GetPricingData(epic, "DAY", 100),
            Hour   = await igClient.GetPricingData(epic, "HOUR", 100),
            Minute = await igClient.GetPricingData(epic, "MINUTE", 100),
            Second = await igClient.GetPricingData(epic, "SECOND", 100)
        };

        return Ok(result);
    }
}
