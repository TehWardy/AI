using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers;

public class HomeController() 
    : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("Markets")]
    public IActionResult Markets() => View();
}
