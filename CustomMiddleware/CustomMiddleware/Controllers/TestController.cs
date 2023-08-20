using CustomMiddleware.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CustomMiddleware.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [MyCustomAllowAnonymous]
    [HttpGet("user")]
    public IActionResult GetUser()
    {
        return Content("You accessed on the user page.", "text/plain");
    }
    
    [NeedRole("Admin")]
    [HttpGet("admin")]
    public IActionResult GetAdmin()
    {
        return Content("You accessed on the admin page.", "text/plain");
    }
}