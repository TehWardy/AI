using Microsoft.AspNetCore.Mvc;

namespace TehWardy.AI.API.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet(Name = "")]
    public IActionResult Get() => Ok("Hello World!");
}