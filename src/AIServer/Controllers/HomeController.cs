using AIServer.AI;
using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers;

public class HomeController(ILogger<HomeController> logger, AIChatClient chatClient) 
    : Controller
{
    public IActionResult Index() => View();
}
