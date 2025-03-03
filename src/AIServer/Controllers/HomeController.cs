using AIServer.AI;
using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers;

public class HomeController(ILogger<HomeController> logger, AIChatClient chatClient) 
    : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("Markets")]
    public IActionResult Markets() => View();
}
